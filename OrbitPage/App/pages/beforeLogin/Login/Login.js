'use strict';
define([appLocation.preLogin], function (app) {
    app.controller('beforeLoginLoginPage', function ($scope, $http, $route, $rootScope, $routeParams,$location, $timeout, CookieUtil) {
        $('title').html("index"); //TODO: change the title so cann't be tracked in log
        
        $scope.mobileDevice = mobileDevice != null ? true : false;
        $scope.EmailId = "";
        $scope.Password = "";
        $scope.KeepMeSignedInCheckBox = true;
        $scope.showHeaderErrors = false;
        $scope.showFooterErrors = false;

        if (getParameterByName('returnUrl') != null && getParameterByName('returnUrl') != "null") {
            CookieUtil.setReturnUrl(getParameterByName('returnUrl'), userSession.keepMeSignedIn);
        } else {
            //setReturnUrlInCookie($location.path());
        }
           
        
        //showToastMessage("Error", CookieUtil.getReturnUrl());

        $scope.EmailIdAlert = {
            visible: false,
            message: ''
        };
        $scope.PasswordAlert = {
            visible: false,
            message: ''
        };
        $scope.HeaderAlert = {
            visible: false,
            message: '',
            classType: ''
        };
        $scope.ForgetPasswordAlert = {
            visible: false,
            message: ''
        }
        $scope.userConstants = userConstants;
        $scope.clientConstants = clientConstants;

        if (getParameterByName("type") == "info") {
            $scope.showHeaderErrors = true;
            $scope.HeaderAlert.visible = true;
            $scope.HeaderAlert.classType = "warning";
            $scope.HeaderAlert.message = getParameterByName("mssg");
        }
        if ($routeParams.code == "Password200") {
            showToastMessage("Success", "Password has been successfully changed.");
            $scope.showHeaderErrors = true;
            $scope.HeaderAlert.visible = true;
            $scope.HeaderAlert.classType = "success";
            $scope.HeaderAlert.message = "Your Password has been successfully changed. To continue, please login.";
        }
        if ($routeParams.code == "ConatctUs200") {
            showToastMessage("Success", "Your message has been successfully submitted.");
            $scope.showHeaderErrors = true;
            $scope.HeaderAlert.visible = true;
            $scope.HeaderAlert.classType = "success";
            $scope.HeaderAlert.message = "Your Message has been successfully submitted. To continue, please login.";
        }
        if (CookieUtil.getLoginType() == null || CookieUtil.getLoginType() == "") {
            CookieUtil.setLoginType("user", $scope.KeepMeSignedInCheckBox); // by default set type as user..       
            $('#loginUserTypeRadioButtonId').attr('checked', true);
            $('.userTypeId').html(userConstants.name_abb);
            $scope.userType = userConstants.name_abb;
            $scope.isUser = true;            
        }           
        else {
            if (CookieUtil.getLoginType() == "user") {
                $('#loginUserTypeRadioButtonId').attr('checked', true);
                $('.userTypeId').html(userConstants.name_abb);
                $scope.userType = userConstants.name_abb;
                //$scope.isUser = true;                
                $scope.isUser = true;               
                //console.log(userConstants.name_abb);
            }
            else {
                $('#loginClientTypeRadioButtonId').attr('checked', true);
                $('.userTypeId').html(clientConstants.name_abb);
                $scope.userType = clientConstants.name_abb;
                //$scope.isUser = false;                
                $scope.isUser = false;
                
            }
        }

        $scope.Login = function() {
            $scope.showFooterErrors = false;


            if ($scope.EmailId == null || $scope.EmailId == "") {
                if ($('#LoginEmailId').val() != "" || $('#LoginEmailId').val() != null)
                    $scope.EmailId = $('#LoginEmailId').val();
            }

            if ($scope.Password == null || $scope.Password == "") {
                if ($('#LoginPasswordId').val() != "" || $('#LoginPasswordId').val() != null)
                    $scope.Password = $('#LoginPasswordId').val();
            }

            var userLoginData = {
                Username: $scope.EmailId,
                Password: $scope.Password,
                Type: 'web',
                KeepMeSignedInCheckBox: $scope.KeepMeSignedInCheckBox
            }

            userSession.keepMeSignedIn = $scope.KeepMeSignedInCheckBox;

            var url = ServerContextPath.empty + '/Auth/Login';
            var validatePassword = false;
            var validateUsername = false;

            if ($scope.EmailId != "") {
                validateUsername = true;
                $scope.EmailIdAlert.visible = false;
                $scope.EmailIdAlert.message = "";
            } else {
                $scope.EmailIdAlert.visible = true;
                $scope.EmailIdAlert.message = "Enter UserId.";
            }
            if ($scope.Password != "") {
                validatePassword = true;
                $scope.PasswordAlert.visible = false;
                $scope.PasswordAlert.message = "";
            } else {
                $scope.PasswordAlert.visible = true;
                $scope.PasswordAlert.message = "Password cannot be empty.";
            }

            if (validateUsername && validatePassword) {
                startBlockUI('wait..', 3);
                $http({
                    url: url,
                    method: "POST",
                    data: userLoginData,
                    headers: { 'Content-Type': 'application/json' }
                }).success(function(data, status, headers, config) {
                    //$scope.persons = data; // assign  $scope.persons here as promise is resolved here
                    stopBlockUI();
                    if (data.Status == "401") {
                        showToastMessage("Notice", "The username/password combination is incorrect !");
                        $scope.showHeaderErrors = true;
                        $scope.HeaderAlert.visible = true;
                        $scope.HeaderAlert.classType = "danger";
                        $scope.HeaderAlert.message = "The username/password combination you entered is incorrect. Please try again(make sure your caps lock is off).";
                        $scope.ForgetPasswordAlert.visible = true;
                        $scope.ForgetPasswordAlert.message = "Forgot your password?";
                    } else if (data.Status == "500") {
                        $scope.showHeaderErrors = true;
                        $scope.HeaderAlert.visible = true;
                        $scope.HeaderAlert.classType = "danger";
                        $scope.HeaderAlert.message = "Internal server error occured. Please try again.";
                        showToastMessage("Error", "Internal Server Error Occured !");
                    } else if (data.Status == "403") {
                        showToastMessage("Warning", "Your Account is not verified. Please check your mail !");
                        $scope.showHeaderErrors = true;
                        $scope.HeaderAlert.visible = true;
                        $scope.HeaderAlert.classType = "danger";
                        $scope.HeaderAlert.message = "Your Account is not verified yet. Please check your mail and verfiy your account.";
                    } else if (data.Status == "200") {
                        //showToastMessage("Success", "Successfully Logged in !");                    
                        //console.log("data : " + data);
                        //alert("auth token : "+data.Payload.AuthToken);
                        CookieUtil.setUTMZT(data.Payload.UTMZT, userSession.keepMeSignedIn);
                        CookieUtil.setUTMZK(data.Payload.UTMZK, userSession.keepMeSignedIn);
                        CookieUtil.setUTMZV(data.Payload.UTMZV, userSession.keepMeSignedIn);
                        CookieUtil.setUTIME(data.Payload.TimeStamp, userSession.keepMeSignedIn);

                        $.cookie('uservertexid', data.Payload.VertexId, { expires: 365, path: '/', domain: ServerContextPath.cookieDomain });
                        $.cookie('userName', data.Payload.FirstName, { expires: 365, path: '/', domain: ServerContextPath.cookieDomain });
                        $.cookie('userImageUrl', data.Payload.imageUrl, { expires: 365, path: '/', domain: ServerContextPath.cookieDomain });

                        CookieUtil.setKMSI(userSession.keepMeSignedIn, true); // to store KMSI value for maximum possible time.


                        $rootScope.clientDetailResponse.FirstName = data.Payload.FirstName;
                        $rootScope.clientDetailResponse.LastName = data.Payload.LastName;
                        $rootScope.clientDetailResponse.Username = data.Payload.Username;
                        $rootScope.clientDetailResponse.imageUrl = data.Payload.imageUrl;
                        $rootScope.isUserLoggedIn = true;
                        
                        //$window.location.reload();
                        redirectAfterLogin();
                    }

                }).error(function(data, status, headers, config) {

                });
            } else {
                $scope.showFooterErrors = true;
                showToastMessage("Error", "Some Fields are Invalid.");
            }

        };

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
            //            var win = window.open("/SocialAuth/FBLogin/facebook", "Ratting", "width=" + popWindow.width + ",height=" + popWindow.height + ",0,status=0,scrollbars=1");
            //            win.onunload = onun;

            //            function onun() {
            //                if (win.location != "about:blank") // This is so that the function 
            //                // doesn't do anything when the 
            //                // window is first opened.
            //                {
            //                    //$route.reload();
            //                    //alert("working");
            //                    //location.reload();
            //                    //alert("closed");
            //                }
            //            }
        }

        $scope.openLinkedinAuthWindow = function () {
            var url = '/SocialAuth/LinkedinLoginGetRedirectUri';
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
            //            var win = window.open("/SocialAuth/LinkedinLogin", "Ratting", "width=" + popWindow.width + ",height=" + popWindow.height + ",0,status=0,scrollbars=1");
            //            win.onunload = onun;

            //            function onun() {
            //                if (win.location != "about:blank") // This is so that the function 
            //                // doesn't do anything when the 
            //                // window is first opened.
            //                {
            //                    //$route.reload();
            //                    //alert("working");
            //                    //location.reload();
            //                    //alert("closed");
            //                }
            //            }
        }

        $scope.openGoogleAuthWindow = function () {
            var url = '/SocialAuth/GoogleLoginGetRedirectUri';
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
            //            var win = window.open("/SocialAuth/GoogleLogin/", "Ratting", "width=" + popWindow.width + ",height=" + popWindow.height + ",0,status=0,scrollbars=1");
            //            win.onunload = onun;

            //            function onun() {
            //                if (win.location != "about:blank") // This is so that the function 
            //                // doesn't do anything when the 
            //                // window is first opened.
            //                {
            //                    //$route.reload();
            //                    //alert("working");
            //                    //location.reload();
            //                    //alert("closed");
            //                }
            //            }
        }

        $('.TextBoxBeforeLoginFormSubmitButtonClass').keypress(function (e) {
            if (e.keyCode == 13)
                $('#LoginFormSubmitButtonId').click();
        });

        //radiobutton
        $('.loginUserTypeRadioButton').on('change', function () {
            CookieUtil.setLoginType(this.value, userSession.keepMeSignedIn);
            //var data = this.value;
            if (this.value == "user") {
                $scope.userType = userConstants.name_abb;
                $scope.$apply(function () {
                    $scope.isUser = true;
                });
                //$scope.isUser = true;
                $('.userTypeId').html(userConstants.name_abb);

            }
            else {
                $('.userTypeId').html(clientConstants.name_abb);
                $scope.userType = clientConstants.name_abb;
                $scope.$apply(function () {
                    $scope.isUser = false;
                });
                //$scope.isUser = false;
            }
            //console.log(this.value);
        });

        //        $('.loginUserTypeRadioButton').on('ifChecked', function (event) {
        //            //var a = "ifChecked "+ $(this).val();
        //            //console.log(a + "---" + this.value);
        //            var data = this.value;
        //            console.log("2 " + data);
        //        });
    });

    function isValidEmailAddress(emailAddress) {
        var pattern = new RegExp(/^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$/i);
        return pattern.test(emailAddress);
    };

    function isValidFormField(emailAddress) {
        var pattern = new RegExp(/^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$/i);
        return pattern.test(emailAddress);
    };
    
});



			

