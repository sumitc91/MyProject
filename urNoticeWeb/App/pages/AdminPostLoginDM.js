/**
 * Dashboard # Single Page Application [SPA] Dependency Manager Configurator to be resolved via Require JS library.
 * @class PreLoginDM
 * @module PreLogin
 */
appRequire = require
    .config({
        waitSeconds: 200,
        shim: {
            underscore: { //used
                exports: "_"
            },
            angular: {//used
                exports: "angular",
                deps: ["jquery"]
            },
            moment: {//used
                deps: ["jquery"]
            },            
            bootstrap: {//used
                deps: ["jquery"]
            },
            bootstrap_switch: { //used
                deps: ["jquery"]
            },
            jquery: {//used
                exports: "$"
            },//         
            jquery_cookie: {//used
                deps: ["jquery"]
            },
            angularjs_fileUpload_shim: {//new
                deps: ["angular", "jquery"]
            },
            angularjs_fileUpload: {//new
                deps: ["angular", "jquery", "angularjs_fileUpload_shim"]
            },
            angular_route: {//new
                deps: ["angular", "jquery"]
            },
            angular_animate: {
                deps: ["angular", "jquery"]
            },
            sanitize: {
                deps: ["angular", "jquery"]
            },            
            //m2ei18n: {
            //    deps: ["jquery"]
            //},
            restangular: {//used
                deps: ["angular", "underscore"]
            },
            angular_cookies: {//used
                deps: ["angular"]
            },          
            jquery_toastmessage: { //used
                deps: ["jquery"]
            },
            wysihtml5: { //used
                deps: ["jquery"]
            },
            toastMessage: {
                deps: ["jquery_toastmessage"]
            },            
            jquery_blockUI: {//used
                deps: ["jquery"]
            },
            configureBlockUI: {//used
                deps: ["jquery_blockUI"]
            },            
            jquery_slimscroll: {//used
                deps: ["jquery"]
            },            
            beforeLoginAdminLTETree: {//used
                deps: ["jquery"]
            },
            iCheck: {//used
                deps: ["jquery"]
            },
            filedrop: {//new
                deps: ["jquery"]
            },
            domReady: {//new
                deps: ["jquery"]
            },
            fileDropScript: {//new
                deps: ["jquery", "filedrop", "domReady"]
            },
            ngAutocomplete: {
                deps: ["jquery", "angular"]
            },
            fancybox: {//new
                deps: ["jquery"]
            },
            hamster: {//new
                deps: ["jquery","angular"]
            },
            mousewheel: {//new
                deps: ["jquery","angular"]
            },
//            jquery_panzoom: {//new
//                deps: ["jquery"]
//            },
            panzoom: {//new
                deps: ["jquery","angular"]
            },
//            dragend: {//new
//                deps: ["jquery"]
//            },
            idangerous_swiper_2_1_min: {//new
                deps: ["jquery"]
            },
            PanZoomService: {
                deps: ["jquery", "panzoom","mousewheel","hamster","angular"]
            },
            panzoomwidget: {
                deps: ["jquery", "panzoom", "mousewheel", "hamster", "angular"]
            },
            angucomplete_alt_min: {
                deps: ["jquery", "angular"]
            },
            xeditable: {
                deps: ["jquery", "angular"]
            },
            userAfterLoginCookieService: {
                deps: ["jquery", "jquery_cookie"]
            },
            beforeLoginAdminLTEApp: {//used
                deps: ["jquery", "jquery_slimscroll", "bootstrap", "bootstrap_switch", "beforeLoginAdminLTETree", "iCheck"]
            },            
            userAfterLoginApp: { //new
                deps: ["jquery", "angular", "restangular", "configureBlockUI", "toastMessage", "userAfterLoginCookieService", "fancybox"]
            },
            AngularFileUploadController: { //new
                deps: ["jquery", "angular", "restangular", "configureBlockUI", "toastMessage", "userAfterLoginCookieService", "fancybox", "angularjs_fileUpload", "angularjs_fileUpload_shim"]
            },
            SessionManagement: { //used
                deps: ["jquery", "angular", "restangular", "configureBlockUI", "toastMessage", "userAfterLoginCookieService", "fancybox"]
            },
            userAfterLoginIndex: { //used
                deps: ["jquery", "angular", "restangular", "configureBlockUI", "toastMessage", "userAfterLoginCookieService", "fancybox"]
            },
            UserAfterLoginReputationHistory: { //used
                deps: ["jquery", "angular", "restangular", "configureBlockUI", "toastMessage", "userAfterLoginCookieService", "fancybox"]
            },
            showAfterLoginShowTemplate: { //used
                deps: ["jquery", "angular", "restangular", "configureBlockUI", "toastMessage", "userAfterLoginCookieService", "fancybox"]
            },
            userAfterLoginShowTemplateDetail: { //used
                deps: ["jquery", "angular", "restangular", "configureBlockUI", "toastMessage", "userAfterLoginCookieService", "fancybox"]
            },                       
            userAfterLoginEditPage: {
                deps: ["jquery", "angular", "restangular", "configureBlockUI", "toastMessage", "userAfterLoginCookieService", "fancybox"]
            },
            userAfterLoginTemplateSample: {
                deps: ["jquery", "angular", "restangular", "configureBlockUI", "toastMessage", "userAfterLoginCookieService", "fancybox"]
            },
             userAfterLoginActiveThreads: {
                deps: ["jquery", "angular", "restangular", "configureBlockUI", "toastMessage", "userAfterLoginCookieService", "fancybox"]
            },
             AdminAfterLoginValidateDesignation: {
                 deps: ["jquery", "angular", "restangular", "configureBlockUI", "toastMessage", "userAfterLoginCookieService", "fancybox", "angucomplete_alt_min", "xeditable"]
             },
             AdminAfterLoginValidateCompany: {
                 deps: ["jquery", "angular", "restangular", "configureBlockUI", "toastMessage", "userAfterLoginCookieService", "fancybox", "angucomplete_alt_min", "ngAutocomplete", "AngularFileUploadController"]
             },
            UserAfterLoginFacebookLike: {
                deps: ["jquery", "angular", "restangular", "configureBlockUI", "toastMessage", "userAfterLoginCookieService", "fancybox"]
            },
            UserAfterLoginReferralPage: {
                deps: ["jquery", "angular", "restangular", "configureBlockUI", "toastMessage", "userAfterLoginCookieService", "fancybox"]
            },
            UserAfterLoginSubmitIdProofPage: {
                deps: ["jquery", "angular", "restangular", "configureBlockUI", "toastMessage", "userAfterLoginCookieService", "fancybox", "AngularFileUploadController"]
            },            
            UserAfterLoginEarningHistory: {
                deps: ["jquery", "angular", "restangular", "configureBlockUI", "toastMessage", "userAfterLoginCookieService"]
            },        
            UserAfterLoginAllMessages: {
                deps: ["jquery", "angular", "restangular", "configureBlockUI", "toastMessage", "userAfterLoginCookieService"]
            },
            UserAfterLoginAllNotifications: {
                deps: ["jquery", "angular", "restangular", "configureBlockUI", "toastMessage", "userAfterLoginCookieService"]
            },
            adminAfterLoginCompanyDetails: {
                deps: ["jquery", "angular", "restangular", "configureBlockUI", "toastMessage", "userAfterLoginCookieService", "angucomplete_alt_min", "xeditable"]
            },
                        
        },
        paths: {
            //==============================================================================================================
            // 3rd Party JavaScript Libraries
            //==============================================================================================================            
            underscore: "../../App/js/underscore-min",
            jquery: "../../App/js/jquery.min",//used..
            angular: "../../App/js/angular.1.2.13",//used..
            //m2ei18n: "../../App/js/m2ei18n",
            jquery_toastmessage: "../../App/third-Party/toastmessage/js/jquery.toastmessage",//used
            toastMessage: "../../App/js/toastMessage",//used
            jquery_cookie: "../../App/js/jquery.cookie",//used..
            jquery_blockUI: "../../App/js/jquery.blockUI",//used                 
            restangular: "../../App/js/restangular.min",           
            moment: "../../App/js/moment.min",            
            bootstrap: "../../Template/AdminLTE-master/js/bootstrap.min",//used
            bootstrap_switch: "../../Template/AdminLTE-master/js/bootstrap-switch",
            beforeLoginAdminLTEApp: "../../Template/AdminLTE-master/js/AdminLTE/app",//used
            beforeLoginAdminLTETree: "../../Template/AdminLTE-master/js/AdminLTE/tree",//used
            jquery_slimscroll: "../../Template/AdminLTE-master/js/plugins/slimScroll/jquery.slimscroll",
            iCheck: "../../Template/AdminLTE-master/js/plugins/iCheck/icheck.min",
            angular_cookies: "../../App/js/angular-cookies",//used..
            configureBlockUI: "../../App/js/configureBlockUI",//used..
            fancybox: "../../App/third-Party/fancybox/source/jquery.fancybox.js?v=2.1.5",//new
            filedrop: "../../App/third-Party/html5-file-upload/assets/js/jquery.filedrop",
            domReady: "../../App/js/domReady",
            fileDropScript: "../../App/third-Party/html5-file-upload/assets/js/script",
            xeditable: "../../App/third-Party/angular-xeditable-0.1.8/js/xeditable",
            wysihtml5: "../../Template/AdminLTE-master/js/plugins/bootstrap-wysihtml5/bootstrap3-wysihtml5.all.min",
            //dragend: "../../App/js/dragend",//used..
            idangerous_swiper_2_1_min: "../../App/third-Party/Swiper-master/demos/js/idangerous.swiper-2.1.min",//used..
            //jquery_panzoom: "../../App/js/jquery.panzoom",//used..
            hamster: "../../App/Pages/UserAfterLogin/Controller/panzoom/hamster",//used..
            mousewheel: "../../App/Pages/UserAfterLogin/Controller/panzoom/mousewheel",//used..
            angularjs_fileUpload_shim: "../../App/third-Party/angular-file-upload-master/dist/angular-file-upload-shim.min",
            angularjs_fileUpload: "../../App/third-Party/angular-file-upload-master/dist/angular-file-upload.min",
            ngAutocomplete: "../../App/js/ngAutocomplete",
            angucomplete_alt_min: "../../App/js/angular/angucomplete-alt.min",
            panzoom: "../../App/Pages/UserAfterLogin/Controller/panzoom/directives/panzoom",//used..
            PanZoomService: "../../App/Pages/UserAfterLogin/Controller/panzoom/services/PanZoomService",//used..
            panzoomwidget: "../../App/Pages/UserAfterLogin/Controller/panzoom/directives/panzoomwidget",//used..
            angular_route: "../../App/js/angular-route",
            angular_animate: "../../App/js/angular-animate",
            sanitize: "../../App/js/angular/ngSanitize/sanitize",

            //==============================================================================================================
            // Application Related JS
            //==============================================================================================================
            userAfterLoginApp: ".././../App/Pages/AdminAfterLogin/Controller/adminAfterLoginApp",//changed..
            SessionManagement: "../../App/Pages/AdminAfterLogin/Controller/common/SessionManagement",//new
            AngularFileUploadController: "../../App/Pages/AdminAfterLogin/Controller/common/AngularFileUploadController",//new
            userAfterLoginIndex: "../../App/Pages/AdminAfterLogin/index/index",//new
            userAfterLoginShowTemplate: "../../App/Pages/AdminAfterLogin/ShowTemplate/ShowTemplate",//new
            userAfterLoginShowTemplateDetail: "../../App/Pages/AdminAfterLogin/ShowTemplateDetail/ShowTemplateDetail",//new
            userAfterLoginCookieService: "../../../../App/Pages/AdminAfterLogin/Controller/common/CookieServiceAdminView",//used            
            userAfterLoginEditPage: "../../App/Pages/AdminAfterLogin/EditPage/editPage",//used
            userAfterLoginTemplateSample: "../../App/Pages/AdminAfterLogin/TemplateSample/TemplateSample",//used
            AdminAfterLoginValidateDesignation: "../../App/Pages/AdminAfterLogin/ValidateDesignation/ValidateDesignation",//used
            AdminAfterLoginValidateCompany: "../../App/Pages/AdminAfterLogin/ValidateCompany/ValidateCompany",//used
            userAfterLoginModeration: "../../App/Pages/AdminAfterLogin/Survey/Survey",//used
            userAfterLoginActiveThreads: "../../App/Pages/AdminAfterLogin/UserActiveThreads/UserActiveThreads",//used                        
            UserAfterLoginReferralPage: "../../App/Pages/AdminAfterLogin/Referrals/Referrals", //used
            UserAfterLoginFacebookLike: "../../App/Pages/AdminAfterLogin/Ads/facebookLike/facebookLike",//used
            UserAfterLoginReputationHistory: "../../App/Pages/AdminAfterLogin/UserReputationHistory/UserReputationHistory",//used
            UserAfterLoginSubmitIdProofPage: "../../App/Pages/AdminAfterLogin/SubmitIdProof/SubmitIdProof",//used
            UserAfterLoginEarningHistory: "../../App/Pages/AdminAfterLogin/UserEarningHistory/UserEarningHistory",//used
            UserAfterLoginAllMessages: "../../App/Pages/AdminAfterLogin/ShowAllMessages/ShowAllMessages",//used
            UserAfterLoginAllNotifications: "../../App/Pages/AdminAfterLogin/ShowAllNotifications/ShowAllNotifications",//used
            adminAfterLoginCompanyDetails: "../../App/pages/AdminAfterLogin/CompanyDetails/CompanyDetails",
        },
        urlArgs: ""
    });

appRequire(["underscore", "jquery", "angular", "jquery_toastmessage", "toastMessage", "jquery_cookie",
    "jquery_blockUI", "restangular", "moment", "bootstrap", "bootstrap_switch", "beforeLoginAdminLTEApp","beforeLoginAdminLTETree",
    "jquery_slimscroll", "iCheck", "angular_cookies", "configureBlockUI", "fancybox", "userAfterLoginApp", "SessionManagement",
    "userAfterLoginIndex", "userAfterLoginShowTemplate", "userAfterLoginCookieService", "userAfterLoginEditPage", "fancybox", "filedrop","wysihtml5",
    "fileDropScript", "domReady", "userAfterLoginTemplateSample", "idangerous_swiper_2_1_min", "userAfterLoginShowTemplateDetail", "AdminAfterLoginValidateDesignation",
    "userAfterLoginActiveThreads","hamster","mousewheel",
    "panzoom", "PanZoomService", "panzoomwidget", "angular_route", "sanitize", "angular_animate", "UserAfterLoginFacebookLike",
    "UserAfterLoginReferralPage", "UserAfterLoginEarningHistory", "UserAfterLoginReputationHistory", "angularjs_fileUpload_shim", "angularjs_fileUpload",
    "UserAfterLoginSubmitIdProofPage", "UserAfterLoginAllMessages", "UserAfterLoginAllNotifications", "angucomplete_alt_min", "AdminAfterLoginValidateCompany",
    "ngAutocomplete", "AngularFileUploadController", "xeditable", "adminAfterLoginCompanyDetails"
], function() {
    angular.bootstrap(document.getElementById("mainUser"), ["afterLoginAdminApp"]);
});
