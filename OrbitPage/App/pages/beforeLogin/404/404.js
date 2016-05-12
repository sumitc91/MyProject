'use strict';
define([appLocation.preLogin], function (app) {
    app.controller('beforeLogin404', function ($scope, $http, $rootScope, Restangular, CookieUtil) {
        $('title').html("index"); //TODO: change the title so cann't be tracked in log
        $rootScope.userOrbitFeedList.show = true;
        //detectIfUserLoggedIn();

        $scope.projectDetailsDivShow = false;
        $scope.totalProjects = "124";
        $scope.successRate = "91";
        $scope.totalUsers = "3423";
        $scope.projectCategories = "25";
        getBeforeLoginUserProjectDetails();
        function getBeforeLoginUserProjectDetails() {

            Restangular.one('Home/BeforeLoginUserProjectDetailsService').get().then(
                function (success) {

                    if (success.Status == "200") {
                        $scope.totalProjects = success.Payload.TotalProjects;
                        $scope.successRate = success.Payload.SuccessRate;
                        $scope.totalUsers = success.Payload.TotalUsers;
                        $scope.projectCategories = success.Payload.ProjectCategories;
                        $scope.projectDetailsDivShow = true;
                    }

                }, function (failure) {

                });
            $scope.$$phase || $scope.$apply();
        };

        $scope.updateBeforeLoginUserProjectDetailsDiv = function (totalProjects, successRate, totalUsers, projectCategories) {
            //alert("inside angular js function updateBeforeLoginUserProjectDetailsDiv");
            if (totalProjects != "")
                $scope.totalProjects = totalProjects;
            if (successRate != "")
                $scope.successRate = successRate;
            if (totalUsers != "")
                $scope.totalUsers = totalUsers;
            if (projectCategories != "")
                $scope.projectCategories = projectCategories;

        }
    });
});



			

