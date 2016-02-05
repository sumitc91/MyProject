
'use strict';
define([appLocation.preLogin], function (app) {

    app.config(function ($routeProvider, $httpProvider) {

        $routeProvider.when("/", { templateUrl : "../../App/pages/beforeLogin/Index/Index.html" }).
                       when("/signup/user/:ref", { templateUrl: "../../App/Pages/BeforeLogin/SignUpUser/SignUpUser.html" }).
                       when("/signup/client/:ref", { templateUrl: "../../App/Pages/BeforeLogin/SignUpClient/SignUpClient.html" }).
                       when("/signup/user", { templateUrl: "../../App/pages/beforeLogin/SignUpUser/SignUpUser.html" }).
                       when("/signup/client", { templateUrl: "../../App/Pages/BeforeLogin/SignUpClient/SignUpClient.html" }).
                       when("/login", { templateUrl: "../../App/Pages/BeforeLogin/Login/Login.html" }).
                       when("/login/:code", { templateUrl: "../../App/Pages/BeforeLogin/Login/Login.html" }).
                       when("/faq", { templateUrl: "../../App/pages/beforeLogin/FAQ/FAQ.html" }).
                       when("/facebookLogin/:userType", { templateUrl: "../../Resource/templates/beforeLogin/contentView/facebookLogin.html" }).
                       when("/facebookLogin", { templateUrl: "../../Resource/templates/beforeLogin/contentView/facebookLogin.html" }).
                       when("/googleLogin/:userType", { templateUrl: "../../Resource/templates/beforeLogin/contentView/googleLogin.html" }).
                       when("/googleLogin", { templateUrl: "../../Resource/templates/beforeLogin/contentView/googleLogin.html" }).
                       when("/linkedinLogin/:userType", { templateUrl: "../../Resource/templates/beforeLogin/contentView/linkedinLogin.html" }).
                       when("/linkedinLogin", { templateUrl: "../../Resource/templates/beforeLogin/contentView/linkedinLogin.html" }).
                       when("/validate/:userName/:guid", { templateUrl: "../../App/Pages/BeforeLogin/validateEmail/validateEmail.html" }).
                       when("/tnc", { templateUrl: "../../App/pages/beforeLogin/TnC/TnC.html" }).
                       when("/privacy", { templateUrl: "../../App/pages/beforeLogin/Privacy/Privacy.html" }).
                       when("/stories", { templateUrl: "../../App/pages/beforeLogin/Blog/Blog.html" }).
                       when("/poststory", { templateUrl: "../../App/pages/beforeLogin/PostStory/PostStory.html" }).
                       when("/addnewnotice", { templateUrl: "../../App/pages/beforeLogin/AddNewNotice/AddNewNotice.html" }).
                       when("/story/:storyid", { templateUrl: "../../App/pages/beforeLogin/SingleBlog/SingleBlog.html" }).
                       when("/search", { templateUrl: "../../App/pages/beforeLogin/Search/Search.html" }).
                       when("/search/:q/:page/:perpage", { templateUrl: "../../App/pages/beforeLogin/Search/Search.html" }).
                       when("/404", { templateUrl: "../../App/pages/beforeLogin/404/404.html" }).
                       when("/aboutus", { templateUrl: "../../App/pages/beforeLogin/AboutUs/AboutUs.html" }).
                       when("/contactus", { templateUrl: "../../App/pages/beforeLogin/ContactUs/contactus.html" }).
                       when("/showmessage/:code", { templateUrl: "../../App/Pages/BeforeLogin/ShowMessage/showmessage.html" }).
                       when("/forgetpassword", { templateUrl: "../../App/pages/beforeLogin/ForgetPassword/ForgetPassword.html" }).
                       when("/resetpassword/:userName/:guid", { templateUrl: "../../App/pages/beforeLogin/ResetPassword/resetpassword.html" }).
                       when("/companydetails/:companyName/:companyid/", { templateUrl: "../../App/pages/beforeLogin/CompanyDetails/CompanyDetails.html" }).
                       when("/AccepterDetails", { templateUrl: "../../App/Pages/BeforeLogin/UserMoreInfo/UserMoreInfo.html" }).
                       when("/RequesterDetails", { templateUrl: "../../App/Pages/BeforeLogin/ClientMoreInfo/ClientMoreInfo.html" }).
                       when("/career", { templateUrl: "../../App/pages/beforeLogin/Career/Career.html" }).
                       when("/editpage", { templateUrl: "../../App/pages/beforeLogin/EditPage/EditPage.html" }).
                       when("/userprofile", { templateUrl: "../../App/pages/beforeLogin/UserProfile/UserProfile.html" }).
                       otherwise({ templateUrl: "../../App/pages/beforeLogin/404/404.html" });


        //Enable cross domain calls
        $httpProvider.defaults.useXDomain = true;

        //Remove the header used to identify ajax call  that would prevent CORS from working
        delete $httpProvider.defaults.headers.common['X-Requested-With'];
    });

    app.run(function ($rootScope, $location, $window) { //Insert in the function definition the dependencies you need.

        $rootScope.$on("$locationChangeStart", function (event, next, current) {

            //detectIfUserLoggedIn();

            //gaWeb("BeforeLogin-Page Visited", "Page Visited", next);
            var path = next.split('#');
            var contextPath = path[1];
            //gaPageView(path, 'title');
            //console.log(contextPath);
            if (contextPath != "/login" && contextPath != "/login/Password200" && contextPath != "/login/ConatctUs200")
            {                
                setReturnUrlInCookie(contextPath);
            }
            $window.scrollTo(0, 0);
        });
    });

    /*app.controller('ModalInstanceCtrl', function ($scope, $uibModalInstance, items) {

        $scope.items = items;
        $scope.selected = {
            item: $scope.items[0]
        };

        $scope.ok = function () {
            $uibModalInstance.close($scope.selected.item);
        };

        $scope.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };
    });*/

    app.filter('cut', function () {
        return function (value, wordwise, max, tail) {
            if (!value) return '';

            max = parseInt(max, 10);
            if (!max) return value;
            if (value.length <= max) return value;

            value = value.substr(0, max);
            if (wordwise) {
                var lastspace = value.lastIndexOf(' ');
                if (lastspace != -1) {
                    value = value.substr(0, lastspace);
                }
            }

            return value + (tail || ' …');
        };
    });

    app.directive('autoComplete', function ($timeout) {
        return function (scope, iElement, iAttrs) {
            iElement.autocomplete({
                source: scope[iAttrs.uiItems],
                select: function () {
                    $timeout(function () {
                        iElement.trigger('input');
                    }, 0);
                }
            });
        };
    });

    app.controller('beforeLoginMasterPageController', function ($scope, $location, $http, $rootScope,$upload, CookieUtil) {

        _.defer(function () { $scope.$apply(); });
        $rootScope.IsMobileDevice = (mobileDevice || isAndroidDevice) ? true : false;
        $rootScope.logoImage = { url: logoImage };
        $rootScope.isUserLoggedIn = false;
        $rootScope.profileDropDownCss = "hideFromCss";
        $rootScope.sideBarMenuClass = "hide";

        //$('title').html("index"); //TODO: change the title so cann't be tracked in log

        if (CookieUtil.getUTMZT() != null && CookieUtil.getUTMZT() != '' && CookieUtil.getUTMZT() != "")
            loadClientDetails();

        function loadClientDetails() {
            var url = ServerContextPath.empty + '/User/GetDetails?userType=user';
            var headers = {
                'Content-Type': 'application/json',
                'UTMZT': CookieUtil.getUTMZT(),
                'UTMZK': CookieUtil.getUTMZK(),
                'UTMZV': CookieUtil.getUTMZV()
            };
            //startBlockUI('wait..', 3);
            $http({
                url: url,
                method: "POST",
                headers: headers
            }).success(function (data, status, headers, config) {
                //$scope.persons = data; // assign  $scope.persons here as promise is resolved here
                //stopBlockUI();
                if (data.Status == "200") {
                    $rootScope.clientDetailResponse = data.Payload;
                    //$scope.UserNotificationsList.Messages = data.Payload.Messages;
                    //$scope.UserNotificationsList.Notifications = data.Payload.Notifications;
                    CookieUtil.setUserName(data.Payload.FirstName + ' ' + data.Payload.LastName, userSession.keepMeSignedIn);
                    CookieUtil.setUserImageUrl(data.Payload.imageUrl, userSession.keepMeSignedIn);
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
                    //location.href = "/?type=info&mssg=your session is expired/#/login";
                    //showToastMessage("Notice", "Session Expired.");
                    $rootScope.isUserLoggedIn = false;
                }
            }).error(function (data, status, headers, config) {
                //stopBlockUI();
                showToastMessage("Error", "Internal Server Error Occured.");
            });
        }

        $scope.signOut = function () {
            logout();
            //$rootScope.isUserLoggedIn = false;
            //showToastMessage("Success", "Logged Out");
        };

        $scope.socialButtonClicked = function (name) {
            console.log("functionclicked");
            $().toastmessage('showSuccessToast', "message");
            //showToastMessage("Success","Message");
        };

        
        $scope.toggleProfileDropDownCss = function () {
            
            if ($rootScope.profileDropDownCss == 'hideFromCss') {
                $rootScope.profileDropDownCss = 'displayFromCss';                
            } else {
                $rootScope.profileDropDownCss = 'hideFromCss';                
            }
        };

        $rootScope.wysiHTML5InputImageTextBoxId = "http://";
        $scope.onFileSelect = function ($files) {

            startBlockUI('wait..', 3);
            //$files: an array of files selected, each file has name, size, and type.
            for (var i = 0; i < $files.length; i++) {
                var file = $files[i];
                $scope.upload = $upload.upload({
                    url: '/Upload/UploadAngularFileOnImgUr', //UploadAngularFileOnImgUr
                    //method: 'POST' or 'PUT',
                    //headers: {'header-key': 'header-value'},
                    //withCredentials: true,
                    data: { myObj: $scope.myModelObj },
                    file: file, // or list of files ($files) for html5 only
                    //fileName: 'doc.jpg' or ['1.jpg', '2.jpg', ...] // to modify the name of the file(s)
                    // customize file formData name ('Content-Desposition'), server side file variable name. 
                    //fileFormDataName: myFile, //or a list of names for multiple files (html5). Default is 'file' 
                    // customize how data is added to formData. See #40#issuecomment-28612000 for sample code
                    //formDataAppender: function(formData, key, val){}
                }).progress(function (evt) {
                    console.log('percent: ' + parseInt(100.0 * evt.loaded / evt.total));
                }).success(function (data, status, headers, config) {

                    stopBlockUI();

                    userSession.wysiHtml5UploadedInstructionsImageUrlLink.push(data.data);


                    $(".bootstrap-wysihtml5-insert-image-url").val(data.data.link_s);
                });
                //.error(...)
                //.then(success, error, progress); 
                // access or attach event listeners to the underlying XMLHttpRequest.
                //.xhr(function(xhr){xhr.upload.addEventListener(...)})
            }
            /* alternative way of uploading, send the file binary with the file's content-type.
               Could be used to upload files to CouchDB, imgur, etc... html5 FileReader is needed. 
               It could also be used to monitor the progress of a normal http post/put request with large data*/
            // $scope.upload = $upload.http({...})  see 88#issuecomment-31366487 for sample code.
        };

        $scope.onS3FileSelect = function ($files) {

            startBlockUI('wait..', 3);
            //$files: an array of files selected, each file has name, size, and type.
            for (var i = 0; i < $files.length; i++) {
                var file = $files[i];
                $scope.upload = $upload.upload({
                    url: '/S3/UploadToS3', //UploadAngularFileOnImgUr
                    //method: 'POST' or 'PUT',
                    //headers: {'header-key': 'header-value'},
                    //withCredentials: true,
                    data: { myObj: $scope.myModelObj },
                    file: file, // or list of files ($files) for html5 only
                    //fileName: 'doc.jpg' or ['1.jpg', '2.jpg', ...] // to modify the name of the file(s)
                    // customize file formData name ('Content-Desposition'), server side file variable name. 
                    //fileFormDataName: myFile, //or a list of names for multiple files (html5). Default is 'file' 
                    // customize how data is added to formData. See #40#issuecomment-28612000 for sample code
                    //formDataAppender: function(formData, key, val){}
                }).progress(function (evt) {
                    console.log('percent: ' + parseInt(100.0 * evt.loaded / evt.total));
                }).success(function (data, status, headers, config) {

                    stopBlockUI();

                    showToastMessage("Success", "Successfully Uploaded.");
                });
                //.error(...)
                //.then(success, error, progress); 
                // access or attach event listeners to the underlying XMLHttpRequest.
                //.xhr(function(xhr){xhr.upload.addEventListener(...)})
            }
            /* alternative way of uploading, send the file binary with the file's content-type.
               Could be used to upload files to CouchDB, imgur, etc... html5 FileReader is needed. 
               It could also be used to monitor the progress of a normal http post/put request with large data*/
            // $scope.upload = $upload.http({...})  see 88#issuecomment-31366487 for sample code.
        };

        $rootScope.beforeLoginFooterInfo = {
            requester: "Crowd Automation Requester",
            accepter: "Crowd Automation Accepter",
            knowMore: "Learn more about",
            impLinks: window.madetoearn.i18n.beforeLoginMasterPageFooterImportantLinks,
            FAQ: window.madetoearn.i18n.beforeLoginMasterPageFAQ,
            contactUs: window.madetoearn.i18n.beforeLoginMasterPageContactUs,
            TnC: window.madetoearn.i18n.beforeLoginMasterPageTnC,
            developers:"Developers Section",
            aboutus: window.madetoearn.i18n.beforeLoginMasterPageAboutUs,
            home: window.madetoearn.i18n.beforeLoginMasterPageHome,
            footerMost: window.madetoearn.i18n.beforeLoginMasterPageFooterMost
        };

        $('#responsive-menu-button').sidr({
            name: 'sidr-main',
            side: 'right',
            source: '#nav_mobi'
        });
        $("body").click(function () {
            $.sidr('close', 'sidr-main');
        });

        if (getParameterByName("lang") == "hi_in") {
            
        } else {
            
        }

    });

    function loadjscssfile(filename, filetype) {
        var fileref = "";
        if (filetype == "js") { //if filename is a external JavaScript file
            fileref = document.createElement('script');
            fileref.setAttribute("type", "text/javascript");
            fileref.setAttribute("src", filename);
        }
        else if (filetype == "css") { //if filename is an external CSS file
            fileref = document.createElement("link");
            fileref.setAttribute("rel", "stylesheet");
            fileref.setAttribute("type", "text/css");
            fileref.setAttribute("href", filename);
        }
        if (typeof fileref != "undefined")
            document.getElementsByTagName("head")[0].appendChild(fileref);
    }

    //loadjscssfile("../../App/Pages/BeforeLogin/SignUpClient/signUpClientController.js", "js"); //dynamically load and add this .js file
    //loadjscssfile("../../App/Pages/BeforeLogin/Controller/common/CookieService.js", "js"); 

});
