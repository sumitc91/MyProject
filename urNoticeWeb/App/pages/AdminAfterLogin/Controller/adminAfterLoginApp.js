'use strict';
define([appLocation.adminPostLogin], function (app) {
    app.config(function ($routeProvider) {
        //(mobileDevice) ? "../../App/Pages/AdminAfterLogin/Index/MobileIndex.html"  : "../../App/Pages/AdminAfterLogin/Index/Index.html" }).
        $routeProvider.when("/", { templateUrl: "../../App/Pages/AdminAfterLogin/Index/Index.html" }).
                       when("/edit", { templateUrl: "../../App/Pages/AdminAfterLogin/EditPage/EditPage.html" }).
        //when("/showTemplateDetail/:refKey", { templateUrl: "../../App/Pages/AdminAfterLogin/ShowTemplateDetail/ShowTemplateDetail.html" }).
                       when("/showTemplateDetail/:type/:subType/:refKey", { templateUrl: "../../App/Pages/AdminAfterLogin/ShowTemplateDetail/ShowTemplateDetail.html" }).
                       when("/userThreads/:status", { templateUrl: "../../App/Pages/AdminAfterLogin/UserActiveThreads/UserActiveThreads.html" }).
                       when("/validateDesignation", { templateUrl: "../../App/Pages/AdminAfterLogin/ValidateDesignation/ValidateDesignation.html" }).
                       when("/validateCompany", { templateUrl: "../../App/Pages/AdminAfterLogin/ValidateCompany/ValidateCompany.html" }).
                       when("/companydetails", { templateUrl: "../../App/pages/AdminAfterLogin/CompanyDetails/CompanyDetails.html" }).
                       when("/companydetails/:companyName/:companyid/", { templateUrl: "../../App/pages/AdminAfterLogin/CompanyDetails/CompanyDetails.html" }).
                       when("/startTranscription/:refKey", { templateUrl: "../../App/Pages/AdminAfterLogin/DataEntry/TranscriptionTemplate/TranscriptionTemplate.html" }).
                       when("/startAngularTranscription/:refKey", { templateUrl: "../../App/Pages/AdminAfterLogin/DataEntry/TranscriptionTemplate/AngularTranscriptionTemplate.html" }).
                       when("/mobileModeration", { templateUrl: "../../App/Pages/AdminAfterLogin/Moderation/MobileModeration.html" }).
                       when("/webModeration", { templateUrl: "../../App/Pages/AdminAfterLogin/Moderation/WebModeration.html" }).
                       when("/imageModeration/:refKey", { templateUrl: "../../App/Pages/AdminAfterLogin/Moderation/WebModeration.html" }).
                       when("/showTemplate", { templateUrl: "../../App/Pages/AdminAfterLogin/ShowTemplate/ShowTemplate.html" }).
                       when("/editTemplate/:username/:templateid", { templateUrl: "../../App/Pages/AdminAfterLogin/EditTemplate/EditTemplate.html" }).
                       when("/templateSample/:type/:subType", { templateUrl: "../../App/Pages/AdminAfterLogin/TemplateSample/TemplateSample.html" }).
                       when("/mobileSlide", { templateUrl: "../../App/Pages/AdminAfterLogin/MobileSlide/MobileSlide.html" }).
                       when("/mobileSlide2", { templateUrl: "../../App/Pages/AdminAfterLogin/MobileSlide2/MobileSlide2.html" }).
                       when("/mobileSlide3", { templateUrl: "../../App/Pages/AdminAfterLogin/MobileSlide2/MobileSlide3.html" }).
                       when("/facebookLikePage", { templateUrl: "../../App/Pages/AdminAfterLogin/Ads/facebookLike/facebookLike.html" }).
                       when("/myReferrals", { templateUrl: "../../App/Pages/AdminAfterLogin/Referrals/Referrals.html" }).
                       when("/myEarningHistory", { templateUrl: "../../App/Pages/AdminAfterLogin/UserEarningHistory/UserEarningHistory.html" }).
                       when("/myReputationHistory", { templateUrl: "../../App/Pages/AdminAfterLogin/UserReputationHistory/UserReputationHistory.html" }).
                       when("/submitIdProof", { templateUrl: "../../App/Pages/AdminAfterLogin/SubmitIdProof/SubmitIdProof.html" }).
                       when("/userAllMessages", { templateUrl: "../../App/Pages/AdminAfterLogin/ShowAllMessages/ShowAllMessages.html" }).
                       when("/userAllNotifications", { templateUrl: "../../App/Pages/AdminAfterLogin/ShowAllNotifications/ShowAllNotifications.html" }).
                       otherwise({ templateUrl: "../../Resource/templates/beforeLogin/contentView/404.html" });

    });

    
    app.run(function ($rootScope, $location, CookieUtil, SessionManagementUtil, editableOptions) { //Insert in the function definition the dependencies you need.

        editableOptions.theme = 'bs3';

        $rootScope.$on("$locationChangeStart", function (event, next, current) {

            //            var headerSessionData = {
            //                UTMZT: CookieUtil.getUTMZT(),
            //                UTMZK: CookieUtil.getUTMZK(),
            //                UTMZV: CookieUtil.getUTMZV()
            //            }

            //SessionManagementUtil.isValidSession(headerSessionData);
            /* Sidebar tree view */
            $(".sidebar .treeview").tree();
            var htmlQRImage = "";
            htmlQRImage = "<img src=\"http://chart.apis.google.com/chart?cht=qr&chs=200x200&chl=" + next.replace("#", "%23") + "\"/>";
            $("#QRPageImageId").html(htmlQRImage);
            gaWeb("BeforeLogin-Page Visited", "Page Visited", next);
            var path = next.split('#');
            gaPageView(path, 'title');
        });
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

    app.controller('UserAfterMasterPage', function ($scope, $http, $rootScope,$location, CookieUtil) {

        _.defer(function () { $scope.$apply(); });
        $rootScope.IsMobileDevice = (mobileDevice || isAndroidDevice) ? true : false;
        
        $scope.signOut = function() {
            logout();
        };

        loadAdminDetails();

        function loadAdminDetails() {
            var url = ServerContextPath.empty + '/Admin/GetAdminDetails?userType=';
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
                headers: headers
            }).success(function (data, status, headers, config) {
                //$scope.persons = data; // assign  $scope.persons here as promise is resolved here
                stopBlockUI();
                if (data.Status == "200") {
                    $rootScope.clientDetailResponse = data.Payload;
                    $scope.UserNotificationsList.Messages = data.Payload.Messages;
                    $scope.UserNotificationsList.Notifications = data.Payload.Notifications;
                    CookieUtil.setUserName(data.Payload.FirstName + ' ' + data.Payload.LastName, userSession.keepMeSignedIn);
                    CookieUtil.setUserImageUrl(data.Payload.imageUrl, userSession.keepMeSignedIn);
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
                    location.href = "/?type=info&mssg=your session is expired/#/login";
                }
                else if (data.Status == "403") {
                    
                    $.removeCookie('utmzt', { path: '/' });
                    $.removeCookie('utmzk', { path: '/' });
                    $.removeCookie('utmzv', { path: '/' });
                    $.removeCookie('utime', { path: '/' });
                    $.removeCookie('kmsi', { path: '/' });

                    location.href = "/?type=info&mssg=You do not have admin priviledges/#/login";                    
                }
            }).error(function (data, status, headers, config) {
                stopBlockUI();
                showToastMessage("Error", "Internal Server Error Occured.");
            });
        }

        

        $scope.UserNotificationsList = {
            Messages: {
                UnreadMessages: "0",
                CountLabelType: "success",                
                MessageList: [
                //    {
                //        link: "#",
                //        imageUrl: "../../Template/AdminLTE-master/img/avatar3.png",
                //        messageTitle: "Support Team angular",
                //        MessagePostedInTimeAgo: "5 mins",
                //        messageContent: "Why not buy a new awesome theme?"
                //    },
                //    {
                //        link: "#",
                //        imageUrl: "../../Template/AdminLTE-master/img/avatar2.png",
                //        messageTitle: "AdminLTE Design Team angular",
                //        MessagePostedInTimeAgo: "2 hours",
                //        messageContent: "Why not buy a new awesome theme?"
                //    },
                //    {
                //        link: "#",
                //        imageUrl: "../../Template/AdminLTE-master/img/avatar.png",
                //        messageTitle: "Developers angular",
                //        MessagePostedInTimeAgo: "Today",
                //        messageContent: "Why not buy a new awesome theme?"
                //    },
                //    {
                //        link: "#",
                //        imageUrl: "../../Template/AdminLTE-master/img/avatar2.png",
                //        messageTitle: "AdminLTE Design Team angular",
                //        MessagePostedInTimeAgo: "2 hours",
                //        messageContent: "Why not buy a new awesome theme?"
                //    }
                ]
            
            },

            Notifications: {
                UnreadNotifications: "0",
                CountLabelType: "warning",
                NotificationList: [
                    //{
                    //    link: "#",
                    //    NotificationMessage: "5 new members joined today angular",
                    //    NotificationClass: "ion ion-ios7-people info",
                    //    NotificationPostedTimeAgo: "2 hours"
                    //},
                    //{
                    //    link: "#",
                    //    NotificationMessage: "Very long description here that may not fit into the page and may cause design problems",
                    //    NotificationClass: "fa fa-warning danger",
                    //    NotificationPostedTimeAgo: "3 hours"
                    //},
                    //{
                    //    link: "#",
                    //    NotificationMessage: "5 new members joined angular",
                    //    NotificationClass: "fa fa-users warning",
                    //    NotificationPostedTimeAgo: "5 hours"
                    //},
                    //{
                    //    link: "#",
                    //    NotificationMessage: "25 sales made",
                    //    NotificationClass: "ion ion-ios7-cart success",
                    //    NotificationPostedTimeAgo: "1 hours"
                    //},
                    //{
                    //    link: "#",
                    //    NotificationMessage: "You changed your username",
                    //    NotificationClass: "ion ion-ios7-person danger",
                    //    NotificationPostedTimeAgo: "8 hours"
                    //}
                ]
            },
            Tasks: {
                UnreadTasks: "0",
                CountLabelType: "danger",
                TaskList: [
                    //{
                    //    link: "#",
                    //    TaskDetail: "This is task Detail",
                    //    TotalCompleted:"4% completed of 1500 Threads"
                    //},
                    //{
                    //    link: "#",
                    //    TaskDetail: "This 2 is task Detail",
                    //    TotalCompleted: "8% completed of 1500 Threads"
                    //},
                    //{
                    //    link: "#",
                    //    TaskDetail: "This is task Detail",
                    //    TotalCompleted: "5% completed of 1500 Threads"
                    //}
                ]
            }
        };

        $scope.updateUserNotificationMessage = function(userType, newLink, newImageUrl, newMessageTitle, newMessagePostedInTimeAgo, newMessageContent) {
            //alert("inside angular js function updateBeforeLoginUserProjectDetailsDiv");
            var realTimeMessage = {
                link: newLink,
                imageUrl: newImageUrl,
                messageTitle: newMessageTitle,
                MessagePostedInTimeAgo: newMessagePostedInTimeAgo,
                messageContent: newMessageContent
            };
            $scope.UserNotificationsList.Messages.UnreadMessages = parseInt($scope.UserNotificationsList.Messages.UnreadMessages) + 1;
            $scope.UserNotificationsList.Messages.MessageList.push(realTimeMessage);
            showToastMessage("Success", newMessageTitle + "<br\>" + newMessageContent);
        };

        $scope.updateAllUserTaskNotification = function (newLink, newMessageTitle, newMessagePostedInTimeAgo, newMessageBody) {
            //alert("inside angular js function updateBeforeLoginUserProjectDetailsDiv");
            var realTimeTask = {
                link: newLink,
                TaskDetail: newMessageTitle,
                TotalCompleted: newMessagePostedInTimeAgo
            };
            $scope.UserNotificationsList.Tasks.UnreadTasks = parseInt($scope.UserNotificationsList.Tasks.UnreadTasks) + 1;
            $scope.UserNotificationsList.Tasks.TaskList.push(realTimeTask);
            showToastMessage("Success", newMessageTitle);

        };

        $scope.updateUserTaskNotification = function (userType, newLink, newMessageTitle, newMessagePostedInTimeAgo, newMessageBody) {
            //alert("inside angular js function updateBeforeLoginUserProjectDetailsDiv");
            var realTimeTask = {
                link: newLink,
                TaskDetail: newMessageTitle,
                TotalCompleted: newMessagePostedInTimeAgo
            };
            $scope.UserNotificationsList.Tasks.UnreadTasks = parseInt($scope.UserNotificationsList.Tasks.UnreadTasks) + 1;
            $scope.UserNotificationsList.Tasks.TaskList.push(realTimeTask);
            showToastMessage("Success", newMessageTitle);
        };

        $scope.updateUserNotification = function (userType, newLink, newImageUrl, newMessageTitle, newMessagePostedInTimeAgo) {
            //alert("inside angular js function updateBeforeLoginUserProjectDetailsDiv");
            var realTimeNotification = {
                link: newLink,
                NotificationMessage: newMessageTitle,
                NotificationClass: newImageUrl,
                NotificationPostedTimeAgo: newMessagePostedInTimeAgo
            };
            $scope.UserNotificationsList.Notifications.UnreadNotifications = parseInt($scope.UserNotificationsList.Notifications.UnreadNotifications) + 1;
            $scope.UserNotificationsList.Notifications.NotificationList.push(realTimeNotification);
            showToastMessage("Success", newMessageTitle);
        };

        $scope.ClientCategoryList = [
       {
           MainCategory: "Category",
           subCategoryList: [
           //{
           //    value: "Data entry", dropDownMenuShow: true, dropDownSubMenuClass: "dropdown-submenu", dropDownMenuClass: "dropdown-menu", dropDownSubMenuArrow: "dropdown", dropDownMenuList: [
           //    //{ value: "Verification & Duplication", link: "#/VerificationAndDuplicationSample" },
           //    { value: "Data Collection", link: "#" },
           //    { value: "Tagging of an Image", link: "#" },
           //    { value: "Search the web", link: "#" },
           //    { value: "Do Excel work", link: "#" },
           //    //{ value: "Find information", link: "#" },
           //    //{ value: "Post advertisements", link: "#" },
           //    { value: "Transcription", link: "#" },
           //    { value: "Transcription from A/V", link: "#" }
           //    ]
           //},
           //{
           //    value: "Content Writing", dropDownMenuShow: true, dropDownSubMenuClass: "dropdown-submenu", dropDownMenuClass: "dropdown-menu", dropDownSubMenuArrow: "dropdown", dropDownMenuList: [
           //      { value: "Article writing", link: "#" },
           //      { value: "Blog writing", link: "#" },
           //      { value: "Copy typing", link: "#" },
           //      { value: "Powerpoint", link: "#" },
           //      { value: "Short stories", link: "#" },
           //      { value: "Travel writing", link: "#" },
           //      { value: "Reviews", link: "#" },
           //      { value: "Product descriptions", link: "#" }
           //    ]
           //},
           //{
           //    value: "Survey", dropDownMenuShow: true, dropDownSubMenuClass: "dropdown-submenu", dropDownMenuClass: "dropdown-menu", dropDownSubMenuArrow: "dropdown", dropDownMenuList: [
           //      { value: "Product survey", link: "#/templateSample/test/okay" },
           //      { value: "User feedback survey", link: "#" },
           //      { value: "Pools", link: "#" },
           //      { value: "Survey Link", link: "#" }
           //    ]
           //},
           //{
           //    value: "Moderation", dropDownMenuShow: true, dropDownSubMenuClass: "dropdown-submenu", dropDownMenuClass: "dropdown-menu", dropDownSubMenuArrow: "dropdown", dropDownMenuList: [
           //      //{ value: "Moderating Ads", link: "#" },
           //      { value: "Moderating Photos", link: "#/mobileModeration" },
           //      { value: "Moderating Music", link: "#" },
           //      { value: "Moderating Video", link: "#" }
           //    ]
           //},
           {
               value: "Ads", dropDownMenuShow: true, dropDownSubMenuClass: "dropdown-submenu", dropDownMenuClass: "dropdown-menu", dropDownSubMenuArrow: "dropdown", dropDownMenuList: [
                 //{ value: "Facebook Views", link: "#" },
                 { value: "Facebook likes", link: "#/facebookLikePage" },
                 //{ value: "Video reviewing", link: "#" },
                 //{ value: "Comments on social media", link: "#" }
               ]
           }
           ]
       }
        ];
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

});
