'use strict';
define([appLocation.preLogin], function (app) {
    app.controller('beforeLoginIndex', function ($scope, $http, $rootScope,$location, Restangular, CookieUtil) {
        $('title').html("index"); //TODO: change the title so cann't be tracked in log
        
        //detectIfUserLoggedIn();
        $rootScope.sitehosturl = "urnotice.com";//"localhost:40287";
        if ($location.host() == "localhost") {
            $rootScope.sitehosturl = "localhost:40287";
        }

        $scope.projectDetailsDivShow = false;
        $scope.totalProjects = "124";
        $scope.successRate = "91";
        $scope.totalUsers = "3423";
        $scope.projectCategories = "25";
        //getBeforeLoginUserProjectDetails();
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

        $scope.myFunct = function (keyEvent) {
            if (keyEvent.which === 13) {
                location.href = "/#search/?q=" + $("#companyName_value").val() + "&page=1&perpage=10";
            }
        }

        $scope.searchCompany = function() {
            location.href = "/#search/?q=" + $("#companyName_value").val() + "&page=1&perpage=10";
        };

        $scope.selectCompany = function (selected) {
            console.log(selected);
            location.href = "/#companydetails/" + selected.originalObject.companyname.replace(/ /g, "_").replace(/\//g, "_OR_") + "/" + selected.originalObject.guid;
            
        };

        $scope.imageIndex = 2;
        $scope.sliderImage = "https://s3-ap-southeast-1.amazonaws.com/urnotice/App/img/indexPageSlider/slider_final_low_3_" + $scope.imageIndex + ".jpg";

        $scope.prevSlide = function () {
            console.log('prevslide');
            if( $scope.imageIndex == 1)
                $scope.imageIndex = 3;
            else
                $scope.imageIndex = $scope.imageIndex - 1;

            $scope.sliderImage = "https://s3-ap-southeast-1.amazonaws.com/urnotice/App/img/indexPageSlider/slider_final_low_3_" + $scope.imageIndex + ".jpg";
            $scope.$$phase || $scope.$apply();
        };


        $scope.nextSlide = function () {
            console.log('nextslide');
            if( $scope.imageIndex ==3 )
                $scope.imageIndex = 1;
            else
                $scope.imageIndex = $scope.imageIndex + 1;

            $scope.sliderImage = "https://s3-ap-southeast-1.amazonaws.com/urnotice/App/img/indexPageSlider/slider_final_low_3_" + $scope.imageIndex + ".jpg";
            $scope.$$phase || $scope.$apply();
        };
    });

});


			

