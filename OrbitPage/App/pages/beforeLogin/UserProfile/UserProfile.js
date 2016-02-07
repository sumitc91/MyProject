'use strict';
define([appLocation.preLogin], function (app) {

    app.controller('beforeLoginUserProfile', function ($scope, $http, $rootScope, CookieUtil) {
        $('title').html("edit page"); //TODO: change the title so cann't be tracked in log

        $scope.name = "Sumit Chourasia";
        $scope.UserPostList = [];
        getUserPost();

        $scope.createNewUserPost = function () {
            createNewUserPost();
        };

        function createNewUserPost() {

            var url = ServerContextPath.userServer + '/User/UserPost?message=' + $scope.UserPostMessage;
            var headers = {
                'Content-Type': 'application/json',
                'UTMZT': $.cookie('utmzt'),
                'UTMZK': $.cookie('utmzk'),
                'UTMZV': $.cookie('utmzv'),
            };
            startBlockUI('wait..', 3);
            $.ajax({
                url: url,
                method: "GET",
                headers: headers
            }).done(function (data, status) {
                stopBlockUI();
                //console.log(data);
                getUserPost();

            });
        };

        function getUserPost() {
            var url = ServerContextPath.userServer + '/User/GetUserPost?from=0&to=10';
            var headers = {
                'Content-Type': 'application/json',
                'UTMZT': $.cookie('utmzt'),
                'UTMZK': $.cookie('utmzk'),
                'UTMZV': $.cookie('utmzv'),
            };
            startBlockUI('wait..', 3);
            $.ajax({
                url: url,
                method: "GET",
                headers: headers
            }).done(function (data, status) {
                stopBlockUI();
                $scope.$apply(function () {
                    $scope.UserPostList = data.results;
                    console.log($scope.UserPostList);
                });
                
            });
        };

    });

});

