﻿'use strict';
define([appLocation.userPostLogin], function (app) {

    app.controller('UserAfterLoginFacebookLike', function ($scope, $http, $route, $rootScope, CookieUtil, $sce) {
        $scope.logoImage = { url: logoImage };
        $('title').html("index"); //TODO: change the title so cann't be tracked in log

        $scope.facebookLikeIframe = "";
        $scope.facebookLikePageUrl = "";
        $scope.facebookLikePageId = "";
        $scope.isUserConnectedToFacebook = true;
        $scope.isFacebookAuthCookieExpired = false;

        $scope.showFacebookDetailDiv = false;
        $scope.facebookData = {};
        $scope.FacebookLikeList = [];
        var url = ServerContextPath.empty + '/User/GetAllFacebookLikeTemplateInformation';
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
            data: "",
            headers: headers
        }).success(function (data, status, headers, config) {
            //$scope.persons = data; // assign  $scope.persons here as promise is resolved here
            stopBlockUI();
            if (data.Status == "200") {
                var i = 0;
                //console.log(data.Payload);
                $.each(data.Payload, function (key, value) {
                    $scope.FacebookLikeList[i] = this;
                    $scope.FacebookLikeList[i].pageUrl = $sce.trustAsHtml(this.pageUrl);
                    i++;
                });
                //console.log($scope.FacebookLikeList);
            }
            else if (data.Status == "401") {
                location.href = "/?type=info&mssg=your session is expired/#/login";
            }
            else if (data.Status == "205") {
                $scope.isUserConnectedToFacebook = false;
            }
            else if (data.Status == "206") {
                $scope.isUserConnectedToFacebook = false;
                $scope.isFacebookAuthCookieExpired = true;
            }
        }).error(function (data, status, headers, config) {

        });

        $scope.validateFacebookLike = function (id) {
            //console.log(id);
            //alert($scope.FacebookLikeList[id].refKey);
            validateFacebookLikeFunction($scope.FacebookLikeList[id].refKey);

        }

        function validateFacebookLikeFunction(refKey) {
            var url = ServerContextPath.empty + '/User/ValidateFacebookLike';
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
                data: JSON.stringify({ refKey: refKey }),
                headers: headers
            }).success(function (data, status, headers, config) {
                //$scope.persons = data; // assign  $scope.persons here as promise is resolved here
                stopBlockUI();
                if (data.Status == "200") {
                    showToastMessage("Success", "Congrats !! you are credited for you job.");		
                }
                else if (data.Status == "401") {
                    showToastMessage("Error", "Username or Password is incorrect !!!");		
                }
                else if (data.Status == "205") {
                    $scope.isUserConnectedToFacebook = false;
                }
                else if (data.Status == "206") {
                    $scope.isUserConnectedToFacebook = false;
                    $scope.isFacebookAuthCookieExpired = true;
                }
                else if (data.Status == "207") {
                    showToastMessage("Error", "You haven't liked this page yet. !!!");
                }
                else if (data.Status == "208") {
                    showToastMessage("Error", "You have already earned from this page like !!!");
                }
            }).error(function (data, status, headers, config) {

            });
        }
        $scope.toggleActiveThreads = function () {
            console.log($scope.showUserActiveThreads);
            if ($scope.showUserActiveThreads == true)
                $scope.showUserActiveThreads = false;
            else
                $scope.showUserActiveThreads = true;

        }

        $scope.openFacebookAuthWindow = function () {

            var url = '/SocialAuth/FBLoginGetRedirectUri';
            startBlockUI('wait..', 3);
            $http({
                url: url,
                method: "GET",
                headers: { 'Content-Type': 'application/json' }
            }).success(function (data, status, headers, config) {
                //$scope.persons = data; // assign  $scope.persons here as promise is resolved here
                stopBlockUI();
                if (data.Status == "199") {
                    location.href = data.Message;
                }
                else {
                    alert("some error occured");
                }

            }).error(function (data, status, headers, config) {
                alert("internal server error occured");
            });

            //            var win = window.open("/SocialAuth/FBLogin/facebook", "Ratting", "width=800,height=480,0,status=0,scrollbars=1");
            //            win.onunload = onun;

            //            function onun() {
            //                if (win.location != "about:blank") // This is so that the function 
            //                // doesn't do anything when the 
            //                // window is first opened.
            //                {
            //                    //$route.reload();
            //                    location.reload();
            //                    //alert("closed");
            //                }
            //            }
        }

    });

});
