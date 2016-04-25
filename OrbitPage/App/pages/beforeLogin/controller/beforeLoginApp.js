
'use strict';
define([appLocation.preLogin], function (app) {

    app.config(function ($routeProvider, $httpProvider) {

        $routeProvider.when("/", { templateUrl: "../../App/pages/beforeLogin/Index/Index.html" }).
                       when("/index", { templateUrl: "../../App/pages/beforeLogin/Index/Index.html" }).
                       when("/signup/user/:ref", { templateUrl: "../../App/Pages/BeforeLogin/SignUpUser/SignUpUser.html" }).
                       when("/signup/client/:ref", { templateUrl: "../../App/Pages/BeforeLogin/SignUpClient/SignUpClient.html" }).
                       when("/signup/user", { templateUrl: "../../App/pages/beforeLogin/SignUpUser/SignUpUser.html" }).
                       when("/signup/client", { templateUrl: "../../App/Pages/BeforeLogin/SignUpClient/SignUpClient.html" }).
                       when("/login", { templateUrl: "../../App/Pages/BeforeLogin/Login/Login.html" }).
                       when("/login2", { templateUrl: "../../App/Pages/BeforeLogin/Login/Login2.html" }).
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
                       when("/postblog", { templateUrl: "../../App/pages/beforeLogin/PostBlog/PostBlog.html" }).
                       when("/addnewnotice", { templateUrl: "../../App/pages/beforeLogin/AddNewNotice/AddNewNotice.html" }).
                       when("/workgraphy", { templateUrl: "../../App/pages/beforeLogin/Workgraphy/Workgraphy.html" }).
                       when("/allblogs", { templateUrl: "../../App/pages/beforeLogin/AllBlogs/AllBlogs.html" }).
                       when("/urnotice", { templateUrl: "../../App/pages/beforeLogin/Urnotice/Urnotice.html" }).
                       when("/postyournotice", { templateUrl: "../../App/pages/beforeLogin/PostYourNotice/PostYourNotice.html" }).
                       when("/story/:storyid", { templateUrl: "../../App/pages/beforeLogin/SingleWorkgraphy/SingleWorkgraphy.html" }).
                       when("/blog/:blogid", { templateUrl: "../../App/pages/beforeLogin/SingleBlog/SingleBlog.html" }).
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
                       when("/userprofile", { templateUrl: "../../App/pages/beforeLogin/UserProfile/UserProfile2.html" }).
                       when("/userprofile2", { templateUrl: "../../App/pages/beforeLogin/UserProfile/UserProfile2.html" }).
                       when("/userprofile/:vertexId", { templateUrl: "../../App/pages/beforeLogin/UserProfile/UserProfile2.html" }).
                       when("/userprofile2/:vertexId", { templateUrl: "../../App/pages/beforeLogin/UserProfile/UserProfile2.html" }).
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

    // html filter (render text as html)
    app.filter('html', [
        '$sce', function($sce) {
            return function(text) {
                return $sce.trustAsHtml(text);
            };
        }
    ]);

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

    app.directive('myContextmenu', function ($parse) {
        return {
            compile: function (tElem, tAttrs) {
                var fn = $parse(tAttrs.myContextmenu);
                return function (scope, elem, attrs) {
                    elem.on('contextmenu', function (evt) {
                        scope.$apply(function () {
                            fn(scope, { $event: evt });
                        });
                    });
                };
            }
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
        $rootScope.clientNotificationDetailResponseInfo = {
            busy: false,
            after: 0,
            itemPerPage:6
        };

        $rootScope.chatBox = {
            show: false
        };
        //$('title').html("index"); //TODO: change the title so cann't be tracked in log

        $scope.showChatBox = function () {
            //$('.list-text').show();
            
            $('#hangout').show();
            $rootScope.chatBox.show = true;
            
        };

        $scope.seenNotification = function () {
            var url = ServerContextPath.userServer + '/User/SeenNotification';
            var headers = {
                'Content-Type': 'application/json',
                'UTMZT': $.cookie('utmzt'),
                'UTMZK': $.cookie('utmzk'),
                'UTMZV': $.cookie('utmzv')
            };
            $rootScope.clientNotificationDetailResponseInfo.count = 0;
            $.ajax({
                url: url,
                method: "GET",
                headers: headers
            }).done(function (data, status) {
                //console.log(data);
                //$scope.CompanyNoticePeriodDetails = data.results;

            });
        };

        $scope.showPost = function (postId) {
            $location.url('/viewpostdetail/' + postId);
        };

        $scope.clientNotificationDetailResponseInfo.nextPage = function () {
            //alert("working");
            
            if ($rootScope.clientNotificationDetailResponseInfo.busy) return;
            $rootScope.clientNotificationDetailResponseInfo.busy = true;
            //console.log($rootScope.clientNotificationDetailResponseInfo.after);
            loadClientNotificationDetails($rootScope.clientNotificationDetailResponseInfo.after, $rootScope.clientNotificationDetailResponseInfo.after + $rootScope.clientNotificationDetailResponseInfo.itemPerPage,false);
            $rootScope.clientNotificationDetailResponseInfo.after = $rootScope.clientNotificationDetailResponseInfo.after + $rootScope.clientNotificationDetailResponseInfo.itemPerPage+1;
        };

        $scope.clientNotificationDetailResponseInfoUpdateFromPushNotification = function () {
            //alert("working");                             
            loadClientNotificationDetails(0, $rootScope.clientNotificationDetailResponseInfo.after+1,true);
            
        };

        if (CookieUtil.getUTMZT() != null && CookieUtil.getUTMZT() != '' && CookieUtil.getUTMZT() != "") {
            //console.log("cookie available. : " + CookieUtil.getUserName() + "   &  " + CookieUtil.getUserImageUrl());

            if (CookieUtil.getUserName() != null && CookieUtil.getUserName() != '' && CookieUtil.getUserName() != "") {
                $rootScope.clientDetailResponse.Firstname = CookieUtil.getUserName();
                $rootScope.clientDetailResponse.Profilepic = CookieUtil.getUserImageUrl();
                $rootScope.clientDetailResponse.VertexId = $.cookie('uservertexid');
                $rootScope.isUserLoggedIn = true;
            }
            
            loadClientDetails();
            $scope.clientNotificationDetailResponseInfo.nextPage();
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
                //console.log(data);
                if (data.Status == "200") {
                    $rootScope.clientDetailResponse = data.Payload;
                    //$scope.UserNotificationsList.Messages = data.Payload.Messages;
                    //$scope.UserNotificationsList.Notifications = data.Payload.Notifications;
                    CookieUtil.setUserName(data.Payload.Firstname, userSession.keepMeSignedIn);
                    CookieUtil.setUserImageUrl(data.Payload.Profilepic, userSession.keepMeSignedIn);

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

        

        function loadClientNotificationDetails(from, to, isFromPushNotification) {
            var url = ServerContextPath.userServer + '/User/GetNotificationDetails?from='+from+'&to='+to;
            //var url = ServerContextPath.userServer + '/User/GetDetails?userType=user';
            //console.log("loadClientNotificationDetails : "+from+" -- "+to);
            var headers = {
                'Content-Type': 'application/json',
                'UTMZT': CookieUtil.getUTMZT(),
                'UTMZK': CookieUtil.getUTMZK(),
                'UTMZV': CookieUtil.getUTMZV()
            };
            //startBlockUI('wait..', 3);
            //$scope.loadingUserDetails = true;
            $rootScope.clientNotificationDetailResponseInfo.busy = true;
            $scope.loadingNotificationDetails = true;
            //if ($scope.$root.$$phase != '$apply' && $scope.$root.$$phase != '$digest') {
            //    $scope.$apply();
            //}

            $http({
                url: url,
                method: "GET",
                headers: headers
            }).success(function (data, status, headers, config) {
                //$scope.persons = data; // assign  $scope.persons here as promise is resolved here
                //stopBlockUI();
                //console.log("loadClientNotificationDetails success : " + from + " -- " + to);
                $scope.loadingNotificationDetails = false;
                $rootScope.clientNotificationDetailResponseInfo.busy = false;
                $rootScope.clientNotificationDetailResponseInfo.count = data.unread;
                

                if (isFromPushNotification || from==0) {
                    $rootScope.clientNotificationDetailResponse = [];
                }

                if (data != null && data.results != null && data.results.length>0) {                    
                    for (var i = 0; i < data.results.length; i++) {                        
                        if ((from + i) < data.unread) {
                            data.results[i].class = "unread_notification";                            
                        }
                        $rootScope.clientNotificationDetailResponse.push(data.results[i]);                        
                    }
                }                   
                
                if (isFromPushNotification) {
                    var mssg = "";
                    if ($rootScope.clientNotificationDetailResponse[0].notificationInfo.Type == "WallPostNotification") {
                        mssg = $rootScope.clientNotificationDetailResponse[0].notificationByUser.FirstName + " " + $rootScope.clientNotificationDetailResponse[0].notificationByUser.LastName + "  Posted On your wall.";                        
                    } else if ($rootScope.clientNotificationDetailResponse[0].notificationInfo.Type == "CommentedOnPostNotification") {
                        mssg = $rootScope.clientNotificationDetailResponse[0].notificationByUser.FirstName + " " + $rootScope.clientNotificationDetailResponse[0].notificationByUser.LastName + "  Commented on one of your related post.";
                    } else if ($rootScope.clientNotificationDetailResponse[0].notificationInfo.Type == "UserReaction") {
                        mssg = $rootScope.clientNotificationDetailResponse[0].notificationByUser.FirstName + " " + $rootScope.clientNotificationDetailResponse[0].notificationByUser.LastName + "  Reacted on one of your related post.";
                    }
                    showToastMessage("Success", mssg);
                }

                //if ($scope.$root.$$phase != '$apply' && $scope.$root.$$phase != '$digest') {
                //    $scope.$apply();
                //}
                //console.log($rootScope.clientNotificationDetailResponse);
                if (data.Status == "500") {

                    alert("Internal Server Error Occured");
                }
                else if (data.Status == "401") {
                    $rootScope.isUserLoggedIn = false;
                    removeAllCookies(ServerContextPath.cookieDomain);
                }
            }).error(function (data, status, headers, config) {
                //stopBlockUI();
                $scope.loadingNotificationDetails = false;
                $rootScope.clientNotificationDetailResponseInfo.busy = false;
                showToastMessage("Error", "Internal Server Error Occured.");                
            });
        }

        function initSidr(latestUserInfoAvailable) {
            var sidrMenu = "";
            sidrMenu += '<h1>' + 'OrbitPage' + '</h1>';
            sidrMenu += '<ul>';
            if ($rootScope.isUserLoggedIn && latestUserInfoAvailable) {
                sidrMenu += '<li><img src=\"' + $rootScope.clientDetailResponse.Profilepic + '\" height=\"30px\" widht=\"30px\" />' + $rootScope.clientDetailResponse.Firstname + ' ' + $rootScope.clientDetailResponse.Lastname + '</li>';
                sidrMenu += '<li><a href="#/userprofile/' + $rootScope.clientDetailResponse.VertexId + '">My Profile</a></li>';
                sidrMenu += '<li role="menuitem"><a href="#/editpage">Edit Profile</a></li>';
                sidrMenu += '<li role="menuitem"><a ng-click="showChatBox()">Show Chatbox</a></li>';
                
            } else {
                sidrMenu += '<li><a href=\"#/login\">Login</a></li>';
                sidrMenu += '<li><a href=\"#\">Register</a></li>';
            }

            sidrMenu += '<li><hr/></li>';
            sidrMenu += '<li><a href=\"#/">Worgraphy</a></li>';
            sidrMenu += '<li><a href=\"#/urnotice">Urnotice</a></li>';
            sidrMenu += '<li><a href=\"#/workgraphy">Worgraphy</a></li>';
            
            sidrMenu += '</ul>';

            $('#responsive-menu-button').sidr({
                //name: 'sidr-callback',
                name: 'sidr-right',
                side: 'right',
                source: function () {
                    return sidrMenu;
                }
            });

            $("body").click(function () {
                //$.sidr('close', 'sidr-callback');
                $.sidr('close', 'sidr-right');
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
