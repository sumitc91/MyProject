'use strict';
define([appLocation.adminPostLogin], function (app) {

    app.controller('AdminAfterLoginValidateDesignation', function ($scope, $http, $route, $rootScope, $routeParams, CookieUtil) {
        $scope.logoImage = { url: logoImage };
        //showToastMessage("Error", "Title of the Template cann't be empty");
        $('title').html("index"); //TODO: change the title so cann't be tracked in log

        $scope.user = {
            name: 'awesome user'
        };

        $scope.NewDesignation = {
            designationName:"",
            description:""
        };
        $scope.designationName = "";
        $scope.addDesignation = function (selected) {
            console.log(selected);
        };

        $scope.SubmitNewDesignationToServer = function() {

            console.log("$scope.designationName : " + $scope.designationName);
            var newDesignationData = {
                designationName: $('#designationName_value').val(),
                description: $scope.NewDesignation.description
            };


            var url = ServerContextPath.empty + '/Admin/AddNewDesignation';
            var headers = {
                'Content-Type': 'application/json',
                'UTMZT': CookieUtil.getUTMZT(),
                'UTMZK': CookieUtil.getUTMZK(),
                'UTMZV': CookieUtil.getUTMZV()
            };
            startBlockUI('wait..', 3);
            $http({
                url: url,
                method: "POST",
                data: newDesignationData,
                headers: headers
            }).success(function(data, status, headers, config) {
                //$scope.persons = data; // assign  $scope.persons here as promise is resolved here
                stopBlockUI();
                if (data.Status == "200") {
                    showToastMessage("Success", data.Message);

                } else if (data.Status == "202") {
                    showToastMessage("Warning", data.Message);

                } else if (data.Status == "500") {
                    showToastMessage("Error", data.Message);

                }
            }).error(function(data, status, headers, config) {

            });


        };

        $scope.UpdateDesignationSolr = function () {

            console.log("UpdateDesignationSolr");
            var url = ServerContextPath.empty + '/Admin/LoadDesignationDataFromMySqlToSolr';
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
                //$scope.persons = data; // assign  $scope.persons here as promise is resolved here
                stopBlockUI();
                if (data.Status == "200") {
                    showToastMessage("Success", data.Message);
                } else if (data.Status == "202") {
                    showToastMessage("Warning", data.Message);

                } else if (data.Status == "500") {
                    showToastMessage("Error", data.Message);

                }
            }).error(function (data, status, headers, config) {

            });


        };
    });

});

