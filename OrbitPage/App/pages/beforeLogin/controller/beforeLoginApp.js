
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
                       when("/userdetails/:userid/:source", { templateUrl: "../../App/pages/beforeLogin/UserDetails/UserDetails.html" }).
                       when("/AccepterDetails", { templateUrl: "../../App/Pages/BeforeLogin/UserMoreInfo/UserMoreInfo.html" }).
                       when("/RequesterDetails", { templateUrl: "../../App/Pages/BeforeLogin/ClientMoreInfo/ClientMoreInfo.html" }).
                       when("/career", { templateUrl: "../../App/pages/beforeLogin/Career/Career.html" }).
                       when("/editpage", { templateUrl: "../../App/pages/beforeLogin/EditPage/EditPage.html" }).
                       when("/userprofile", { templateUrl: "../../App/pages/beforeLogin/UserProfile/UserProfile.html" }).
                       when("/userprofile/:vertexId", { templateUrl: "../../App/pages/beforeLogin/UserProfile/UserProfile.html" }).
                       when("/viewpostdetail/:vertexId", { templateUrl: "../../App/pages/beforeLogin/ViewPostDetail/ViewPostDetail.html" }).
                       otherwise({ templateUrl: "../../App/pages/beforeLogin/404/404.html" });


        //Enable cross domain calls
        $httpProvider.defaults.useXDomain = true;

        //Remove the header used to identify ajax call  that would prevent CORS from working
        delete $httpProvider.defaults.headers.common['X-Requested-With'];
    });

    app.run(function ($rootScope, $location, $window) { //Insert in the function definition the dependencies you need.

        $rootScope.$on("$locationChangeStart", function (event, next, current) {

            //detectIfUserLoggedIn();

            gaWeb("BeforeLogin-Page Visited", "Page Visited", next);
            var path = next.split('#');            
            var contextPath = path[1];
            if (contextPath == null || contextPath == "null" || contextPath == "undefined")
                contextPath = "/";
            
            gaPageView(path, 'title');
            //console.log(contextPath);
            if (
                !contextPath.match("/login")
                && !contextPath.match("/validate")
                && !contextPath.match("/signup")
                && !contextPath.match("/forgetpassword")
                && !contextPath.match("/showmessage")
                )
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

    app.filter('reverse', function () {
        return function (items) {
            return items.slice().reverse();
        };
    });

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

    app.controller('beforeLoginMasterPageController', function ($scope, $location, $http, $rootScope, CookieUtil) {

        _.defer(function () { $scope.$apply(); });
        $rootScope.IsMobileDevice = (mobileDevice || isAndroidDevice) ? true : false;
        $rootScope.logoImage = { url: logoImage };
        $rootScope.isUserLoggedIn = false;
        $rootScope.profileDropDownCss = "hideFromCss";

        $rootScope.logourl = "https://s3-ap-southeast-1.amazonaws.com/urnotice/landing_page_logo/logo_final_with_text_732x12.png";
        //$rootScope.logourl = "https://s3-ap-southeast-1.amazonaws.com/urnotice/orbitpage/LandingPageLogo/orbitPagelogo_indian_flag.png";
        //$rootScope.landingPageLogourl = "https://s3-ap-southeast-1.amazonaws.com/urnotice/orbitpage/LandingPageLogo/orbitPagelogo_indian_flag.png";
        $rootScope.landingPageLogourl = "https://s3-ap-southeast-1.amazonaws.com/urnotice/landing_page_logo/logo_final_with_text_732x12.png";

        $scope.searchBoxText = window.madetoearn.i18n.beforeLoginIndexSearchBoxText;
        $scope.loadingUserDetails = false;
        $rootScope.clientDetailResponse = {};
        //$('title').html("index"); //TODO: change the title so cann't be tracked in log

        if (CookieUtil.getUTMZT() != null && CookieUtil.getUTMZT() != '' && CookieUtil.getUTMZT() != "") {
            //console.log("cookie available. : " + CookieUtil.getUserName() + "   &  " + CookieUtil.getUserImageUrl());

            if (CookieUtil.getUserName() != null && CookieUtil.getUserName() != '' && CookieUtil.getUserName() != "") {
                $rootScope.clientDetailResponse.FirstName = CookieUtil.getUserName();
                $rootScope.clientDetailResponse.imageUrl = CookieUtil.getUserImageUrl();
                $rootScope.isUserLoggedIn = true;
            }
            
            loadClientDetails();
            loadClientNotificationDetails();
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
            //startBlockUI('wait..', 3);
            $scope.loadingUserDetails = true;
            $http({
                url: url,
                method: "GET",
                headers: headers
            }).success(function (data, status, headers, config) {
                //$scope.persons = data; // assign  $scope.persons here as promise is resolved here
                //stopBlockUI();
                $scope.loadingUserDetails = false;
                if (data.Status == "200") {
                    $rootScope.clientDetailResponse = data.Payload;
                    //$scope.UserNotificationsList.Messages = data.Payload.Messages;
                    //$scope.UserNotificationsList.Notifications = data.Payload.Notifications;
                    CookieUtil.setUserName(data.Payload.FirstName, userSession.keepMeSignedIn);
                    CookieUtil.setUserImageUrl(data.Payload.imageUrl, userSession.keepMeSignedIn);
                    $rootScope.isUserLoggedIn = true;

                    initSidr(true);

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
                    removeAllCookies(ServerContextPath.cookieDomain);
                }
            }).error(function (data, status, headers, config) {
                //stopBlockUI();
                //showToastMessage("Error", "Internal Server Error Occured.");                
            });
        }

        function loadClientNotificationDetails() {
            var url = ServerContextPath.userServer + '/User/GetNotificationDetails?from=0&to=10';
            //var url = ServerContextPath.userServer + '/User/GetDetails?userType=user';
            var headers = {
                'Content-Type': 'application/json',
                'UTMZT': CookieUtil.getUTMZT(),
                'UTMZK': CookieUtil.getUTMZK(),
                'UTMZV': CookieUtil.getUTMZV()
            };
            //startBlockUI('wait..', 3);
            //$scope.loadingUserDetails = true;
            $http({
                url: url,
                method: "GET",
                headers: headers
            }).success(function (data, status, headers, config) {
                //$scope.persons = data; // assign  $scope.persons here as promise is resolved here
                //stopBlockUI();
                $scope.loadingUserDetails = false;
                
                $rootScope.clientNotificationDetailResponse = data.results;
                $rootScope.clientNotificationDetailResponse.count = data.results.length;

                console.log($rootScope.clientNotificationDetailResponse);
                if (data.Status == "500") {

                    alert("Internal Server Error Occured");
                }
                else if (data.Status == "401") {
                    $rootScope.isUserLoggedIn = false;
                    removeAllCookies(ServerContextPath.cookieDomain);
                }
            }).error(function (data, status, headers, config) {
                //stopBlockUI();
                //showToastMessage("Error", "Internal Server Error Occured.");                
            });
        }

        function initSidr(latestUserInfoAvailable) {
            var sidrMenu = "";
            sidrMenu += '<h1>' + 'OrbitPage' + '</h1>';
            sidrMenu += '<ul>';
            if ($rootScope.isUserLoggedIn && latestUserInfoAvailable) {
                sidrMenu += '<li><img src=\"' + $rootScope.clientDetailResponse.imageUrl + '\" height=\"30px\" widht=\"30px\" />' + $rootScope.clientDetailResponse.FirstName + ' ' + $rootScope.clientDetailResponse.LastName + '</li>';
                sidrMenu += '<li><a href="#/userprofile/' + $rootScope.clientDetailResponse.vertexId + '">My Profile</a></li>';
                sidrMenu += '<li role="menuitem"><a href="#/editpage">Edit Profile</a></li>';
            } else {
                sidrMenu += '<li><a href=\"#/login\">Login</a></li>';
                sidrMenu += '<li><a href=\"#\">Register</a></li>';
            }

            sidrMenu += '<li><hr/></li>';
            sidrMenu += '<li><a href=\"#\">Worgraphy</a></li>';
            sidrMenu += '<li><a href=\"#\">Urnotice</a></li>';

            sidrMenu += '</ul>';

            $('#responsive-menu-button').sidr({
                name: 'sidr-callback',
                source: function () {
                    return sidrMenu;
                }
            });

            $("body").click(function () {
                $.sidr('close', 'sidr-callback');
            });
        }

        $scope.signOut = function () {
            logout();
            //$rootScope.isUserLoggedIn = false;
            //showToastMessage("Success", "Logged Out");
        };

        $scope.openPost = function (postUrl) {
            location.href = postUrl;
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

        
        if (getParameterByName("lang") == "hi_in") {
            
        } else {
            
        }
        initSidr(false);
        //$('#responsive-menu-button').sidr({
        //    name: 'sidr-main',
        //    side: 'right',
        //    source: '#nav_mobi'
        //});

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
