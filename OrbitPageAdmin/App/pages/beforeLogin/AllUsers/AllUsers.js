'use strict';
define([appLocation.preLogin], function (app) {
    app.controller('AllUsersPageController', function ($scope, $http, $route, $rootScope,$uibModal, $log, $routeParams, $location, $timeout, CookieUtil) {
        $('title').html("index"); //TODO: change the title so cann't be tracked in log
        
        $scope.mobileDevice = mobileDevice != null ? true : false;
        $scope.allUsersQuery = "*";
        getUsers();

        $scope.fetchAllUsers = function() {
            getUsers();
        };

        function getUsers() {
            var url = ServerContextPath.solrServer + '/Search/GetUserDetailsAutocomplete?q=' + $scope.allUsersQuery;
            //var url = ServerContextPath.userServer + '/User/GetDetails?userType=user';
            var headers = {
                'Content-Type': 'application/json',
                'UTMZT': CookieUtil.getUTMZT(),
                'UTMZK': CookieUtil.getUTMZK(),
                'UTMZV': CookieUtil.getUTMZV()
            };
            //startBlockUI('wait..', 3);
            
            $http({
                url: url,
                method: "GET",
                headers: headers
            }).success(function (data, status, headers, config) {                
                //stopBlockUI();

                $scope.allUsers = data.Payload;
                //console.log($scope.allUsers);
            }).error(function (data, status, headers, config) {
                stopBlockUI();
                showToastMessage("Error", "Internal Server Error Occured.");                
            });
        };

        $scope.userFullDetail = {};

        $scope.getFullUserDetails = function (email) {
            getFullUserDetails(email);
        };

        function getFullUserDetails(email) {
            var url = ServerContextPath.userServer + '/User/GetFullUserDetails?email=' + email;            
            var headers = {
                'Content-Type': 'application/json',
                'UTMZT': CookieUtil.getUTMZT(),
                'UTMZK': CookieUtil.getUTMZK(),
                'UTMZV': CookieUtil.getUTMZV()
            };
            startBlockUI('wait..', 3);

            $http({
                url: url,
                method: "GET",
                headers: headers
            }).success(function (data, status, headers, config) {
                stopBlockUI();

                $scope.userFullDetail = data.Payload;
                //console.log($scope.userFullDetail);
                if ($scope.$root.$$phase != '$apply' && $scope.$root.$$phase != '$digest') {
                    $scope.$apply();
                }
            }).error(function (data, status, headers, config) {
                stopBlockUI();
                showToastMessage("Error", "Internal Server Error Occured.");
            });
        };

        
    });

    
    
});



			

