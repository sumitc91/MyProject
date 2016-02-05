'use strict';
define([appLocation.preLogin], function (app) {
    app.controller('beforeLoginSignUpUser', function ($scope, $http, $routeParams, CookieUtil) {
        $('title').html("index"); //TODO: change the title so cann't be tracked in log
        
        //detectIfUserLoggedIn();

        $scope.UserFormData = {
            FirstName: "",
            LastName: "",
            EmailId: "",
            Password: "",
            ConfirmPassword: "",
            tnc:false
        };
        $scope.showErrors = false;
        $scope.EmailIdAlert = {
            visible: false,
            message: ''
        };
        $scope.PasswordAlert = {
            visible: false,
            message: ''
        };
        $scope.FirstNameAlert = {
            visible: false,
            message: ''
        };
        $scope.LastNameAlert = {
            visible: false,
            message: ''
        };
        
        CookieUtil.setLoginType("user", $scope.KeepMeSignedInCheckBox); // by default set type as user..
        if($routeParams.ref != null || $routeParams.ref != "")
            CookieUtil.setRefKey($routeParams.ref, true); // set the referral key

        $scope.UserSignUp = function () {
            var userSignUpData = {
                FirstName: $scope.UserFormData.FirstName,
                LastName: $scope.UserFormData.LastName,
                Username: $scope.UserFormData.EmailId,
                Password: $scope.UserFormData.Password,                
                Source: 'web',
                Referral: 'NA' // TODO: need to update with referral id.
            }
            var url = ServerContextPath.authServer + '/Auth/CreateAccount';
            var validateEmail = false;
            var validatePassword = false;
            var validateFirstName = false;
            var validateLastName = false;
            $scope.showErrors = false;

            if ($scope.UserFormData.FirstName != "") {
                validateFirstName = true;
                $scope.FirstNameAlert.visible = false;
                $scope.FirstNameAlert.message = "";
            }
            else {
                $scope.FirstNameAlert.visible = true;
                $scope.FirstNameAlert.message = "First Name Cannot be Empty !!!";
            }

            if ($scope.UserFormData.LastName != "") {
                validateLastName = true;
                $scope.LastNameAlert.visible = false;
                $scope.LastNameAlert.message = "";
            }
            else {
                $scope.LastNameAlert.visible = true;
                $scope.LastNameAlert.message = "Last Name Cannot be Empty !!!";
            }

            if (isValidEmailAddress($scope.UserFormData.EmailId) && $scope.UserFormData.EmailId != "") {
                validateEmail = true;
                $scope.EmailIdAlert.visible = false;
                $scope.EmailIdAlert.message = "";
            }
            else {
                $scope.EmailIdAlert.visible = true;
                $scope.EmailIdAlert.message = "Incorrect Email Id !!!";
            }

            if ($scope.UserFormData.Password == $scope.UserFormData.ConfirmPassword) {
                if ($scope.UserFormData.Password != "") {
                    validatePassword = true;
                    $scope.PasswordAlert.visible = false;
                    $scope.PasswordAlert.message = "";
                }
                else {
                    $scope.PasswordAlert.visible = true;
                    $scope.PasswordAlert.message = "Your Password Cannot be Empty !!!";
                }

            }
            else {
                $scope.PasswordAlert.visible = true;
                $scope.PasswordAlert.message = "Password didn't match !!!";
            }

            if (validateEmail && validatePassword && validateFirstName && validateLastName) {
                startBlockUI('wait..', 3);

                var headers = {
                    'Content-Type': 'application/json',                   
                    '_ga': $.cookie('_ga')
                };

                if(CookieUtil.getRefKey("refKey") != null && CookieUtil.getRefKey("refKey") != "")
                    userSignUpData.Referral = CookieUtil.getRefKey("refKey");
                else
                    userSignUpData.Referral = "NA";                            

                $http({
                    url: url,
                    method: "POST",
                    data: userSignUpData,
                    headers: headers
                }).success(function (data, status, headers, config) {
                    //$scope.persons = data; // assign  $scope.persons here as promise is resolved here
                    stopBlockUI();
                    if (data.Status == "409")
                        showToastMessage("Warning", "Username already registered !");
                    else if (data.Status == "500")
                        showToastMessage("Error", "Internal Server Error Occured !");
                    else if (data.Status == "200")
                    {
                        showToastMessage("Success", "Account successfully created ! check your email for validation.");
                        
                        $.removeCookie('refKey', { path: '/' });
                        
                        location.href = "/?email=" + $scope.UserFormData.EmailId + "#/showmessage/1/";
                    }
                }).error(function (data, status, headers, config) {

                });
            }
            else {
                $scope.showErrors = true;
                showToastMessage("Error", "Some Fields are Invalid !!!");
            }

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

    });

    function isValidEmailAddress(emailAddress) {
        var pattern = new RegExp(/^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$/i);
        return pattern.test(emailAddress);
    };
        
   
});



			

