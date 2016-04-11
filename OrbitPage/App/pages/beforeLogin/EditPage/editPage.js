'use strict';
define([appLocation.preLogin], function (app) {

    app.controller('beforeLoginEditPage', function ($scope, $http, $rootScope, CookieUtil) {
        $('title').html("edit page"); //TODO: change the title so cann't be tracked in log
        

        $scope.loadingUserDetails = false;
        $rootScope.clientDetailResponse = {};
        $scope.disabledEmailModel = "";
        //$('title').html("index"); //TODO: change the title so cann't be tracked in log

        if (CookieUtil.getUTMZT() != null && CookieUtil.getUTMZT() != '' && CookieUtil.getUTMZT() != "") {
            console.log("cookie available.");
            loadClientDetails();
        } else {
            console.log("cookie not available.");
        };


        function loadClientDetails() {
            var url = ServerContextPath.solrServer + '/Search/GetDetails?userType=user';
            //var url = ServerContextPath.userServer + '/User/GetDetails?userType=user';
            var headers = {
                'Content-Type': 'application/json',
                'UTMZT': CookieUtil.getUTMZT(),
                'UTMZK': CookieUtil.getUTMZK(),
                'UTMZV': CookieUtil.getUTMZV()
            };
            startBlockUI('wait..', 3);
            $scope.loadingUserDetails = true;
            $http({
                url: url,
                method: "GET",
                headers: headers
            }).success(function (data, status, headers, config) {
                //$scope.persons = data; // assign  $scope.persons here as promise is resolved here
                stopBlockUI();
                $scope.loadingUserDetails = false;
                if (data.Status == "200") {
                    $rootScope.clientDetailResponse = data.Payload;
                    $scope.disabledEmailModel = $rootScope.clientDetailResponse.Email;
                    //$scope.UserNotificationsList.Messages = data.Payload.Messages;
                    //$scope.UserNotificationsList.Notifications = data.Payload.Notifications;
                    CookieUtil.setUserName(data.Payload.Firstname + ' ' + data.Payload.LastName, userSession.keepMeSignedIn);
                    CookieUtil.setUserImageUrl(data.Payload.Profilepic, userSession.keepMeSignedIn);
                    $rootScope.isUserLoggedIn = true;
                    if (data.Payload.isLocked == "true") {
                        location.href = "/Auth/LockAccount?status=true";
                    }
                }
                else if (data.Status == "404") {

                    alert("This template is not present in database");
                }
                else if (data.Status == "500") {

                    alert("Internal Server Error Occured");
                }
                else if (data.Status == "401") {
                    $rootScope.isUserLoggedIn = false;
                }
            }).error(function (data, status, headers, config) {
                //stopBlockUI();
                //showToastMessage("Error", "Internal Server Error Occured.");                
            });
        };

        $scope.submitEditProfile = function() {

            var editPersonRequest = {
                FirstName: $rootScope.clientDetailResponse.Firstname,
                LastName: $rootScope.clientDetailResponse.Lastname,
                ImageUrl: $rootScope.clientDetailResponse.Profilepic,
                CoverPic: $rootScope.clientDetailResponse.Coverpic,
                Email: $rootScope.clientDetailResponse.Email
            };

            var url = ServerContextPath.empty + '/User/EditPersonDetails';
            var headers = {
                'Content-Type': 'application/json',
                'UTMZT': CookieUtil.getUTMZT(),
                'UTMZK': CookieUtil.getUTMZK(),
                'UTMZV': CookieUtil.getUTMZV(),
                '_ga': $.cookie('_ga')
            };
                
            startBlockUI('wait..', 3);
            $http({
                url: url,
                method: "POST",
                data: editPersonRequest,
                headers: headers
            }).success(function(data, status, headers, config) {
                //$scope.persons = data; // assign  $scope.persons here as promise is resolved here
                stopBlockUI();                
                showToastMessage("Success", "Successfully Edited");
            }).error(function(data, status, headers, config) {

            });
                
            
        };

        $scope.openFacebookAuthWindow = function () {
            var win = window.open("/SocialAuth/FBLogin/facebook", "Ratting", "width=800,height=480,0,status=0,scrollbars=1");
            win.onunload = onun;

            function onun() {
                if (win.location != "about:blank") // This is so that the function 
                // doesn't do anything when the 
                // window is first opened.
                {
                    $route.reload();
                    //alert("closed");
                }
            }
        }
    });

});

