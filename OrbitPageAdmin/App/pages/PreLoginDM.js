/**
 * Dashboard # Single Page Application [SPA] Dependency Manager Configurator to be resolved via Require JS library.
 * @class PreLoginDM
 * @module PreLogin
 */
appRequire = require
    .config({
        waitSeconds: 200,
        shim: {            
            underscore: {
                exports: "_"
            },
            angular: {
                exports: "angular",
                deps: ["jquery"]
            },
            //moment: {
            //    deps: ["jquery"]
            //},            
            bootstrap: {
                deps: ["jquery"]
            },            
            jquery: {
                exports: "$"
            },
            restangular: {
                deps: ["angular", "underscore"]
            },
            jquery_cookie: {
                deps: ["jquery"]
            },
            angular_cookies: {
                deps: ["angular"]
            },
            angular_route: {
                deps: ["angular", "jquery"]
            },
            angular_animate: {
                deps: ["angular", "jquery", "angular_route"]
            },
            sanitize: {
                deps: ["angular", "jquery"]
            },
            jquery_toastmessage: {
                deps: ["jquery"]
            },
            toastMessage: {
                deps: ["jquery_toastmessage"]
            },            
            jquery_blockUI: {
                deps: ["jquery"]
            },            
            configureBlockUI: {
                deps: ["jquery_blockUI", "underscore"]
            },
            jquery_ui_min: {
                deps: ["jquery"]
            },
            jquery_ui_touch_punch_min: {
                deps: ["jquery", "jquery_ui_min"]
            },               
            beforeLoginCookieService: {
                deps: ["jquery", "angular", "configureBlockUI", "jquery_blockUI", "toastMessage"]
            },                                   
            fancybox: {//new
                deps: ["jquery"]
            },            
            domReady: {//new
                deps: ["jquery"]
            },            
            ngAutocomplete: {
                deps: ["jquery", "angular"]
            },
            angucomplete_alt_min: {
                deps: ["jquery", "angular"]
            },
            angular_input_stars: {
                deps: ["jquery", "angular"]
            },
            bootstrap_ui: {
                deps: ["jquery", "angular"]
            },
            prettify: { //used
                deps: ["jquery"]
            },
            jquery_sidr_min: {
                deps: ["jquery"]
            },
            angular_resource: {
                deps: ["jquery", "angular"]
            },
            urNoticeScript: {
                deps: ["jquery"]
            },
            beforeLoginSolrService: {
                deps: ["jquery", "angular", "configureBlockUI", "jquery_blockUI", "toastMessage", "angular_resource"]
            },
            beforeLoginApp: {
                deps: ["jquery", "angular", "configureBlockUI", "toastMessage", "jquery_sidr_min", "bootstrap_ui", "urNoticeScript"]
            },
            beforeLoginIndex: {
                deps: ["jquery", "restangular", "angular", "configureBlockUI", "toastMessage", "jquery_sidr_min", "bootstrap_ui", "urNoticeScript", "beforeLoginSolrService", "angular_animate"]
            },
            AllUsersPageController: {
                deps: ["jquery", "angular", "restangular", "configureBlockUI", "toastMessage"]
            },
            AllCompaniesController: {
                deps: ["jquery", "angular", "restangular", "configureBlockUI", "toastMessage"]
            },
            validateEmail: {
                deps: ["jquery", "angular", "restangular", "configureBlockUI", "toastMessage"]
            },
            showMessageTemplate: {
                deps: ["jquery", "angular", "restangular", "configureBlockUI", "toastMessage"]
            },
            beforeLoginForgetPassword: {
                deps: ["jquery", "angular", "restangular", "configureBlockUI", "toastMessage"]
            },
            beforeLoginResetPassword: {
                deps: ["jquery", "angular", "restangular", "configureBlockUI", "toastMessage"]
            },
            beforeLoginEditPage: {
                deps: ["jquery", "angular", "restangular", "configureBlockUI", "toastMessage", "bootstrap_ui"]
            },
            beforeLoginCompanyDetails: {
                deps: ["jquery", "angular", "restangular", "configureBlockUI", "toastMessage", "angular_input_stars", "bootstrap_ui"]
            },
            beforeLoginUserDetails: {
                deps: ["jquery", "angular", "restangular", "configureBlockUI", "toastMessage", "angular_input_stars", "bootstrap_ui"]
            },
            beforeLoginSearch: {
                deps: ["jquery", "angular", "restangular", "configureBlockUI", "toastMessage", "angular_input_stars", "bootstrap_ui"]
            },
        },
        paths: {
            //==============================================================================================================
            // 3rd Party JavaScript Libraries
            //==============================================================================================================            
            underscore: "../../App/js/underscore-min",
            jquery: "../../App/js/jquery.min",
            jquery_ui_min: "../../App/js/jquery-ui.min",
            //hammer_min: "../../App/js/hammer.min",
            angular: "../../App/js/angular.1.2.13",
            //m2ei18n: "../../App/js/m2ei18n",
            jquery_toastmessage: "../../App/third-Party/toastmessage/js/jquery.toastmessage",
            toastMessage: "../../App/js/toastMessage",
            jquery_cookie: "../../App/js/jquery.cookie",
            jquery_blockUI: "../../App/js/jquery.blockUI",                                       
            //moment: "../../App/js/moment.min",            
            bootstrap: "../../App/third-Party/wysihtml5/lib/js/bootstrap.min",
            bootstrap_ui: "../../App/js/angular/ui-bootstrap-tpls-0.14.3",
            angular_cookies: "../../App/js/angular-cookies",
            restangular: "../../App/js/restangular.min",
            angular_animate: "../../App/js/angular-animate",

            configureBlockUI: "../../App/js/configureBlockUI",
            angular_route: "../../App/js/angular-route",
            angular_resource: "../../App/js/angular-resource.min",
            sanitize: "../../App/js/angular/ngSanitize/sanitize",
            jquery_nivo_slider: "../../App/js/jquery.nivo.slider",
            ngAutocomplete: "../../App/js/ngAutocomplete",
            angucomplete_alt_min: "../../App/js/angular/angucomplete-alt.min",            
            jquery_ui_touch_punch_min: "../../App/js/jquery.ui.touch-punch.min",
            jquery_sidr_min: "../../App/third-Party/sidr-package/jquery.sidr.min",            
            domReady: "../../App/js/domReady",            
            fancybox: "../../App/third-Party/fancybox/source/jquery.fancybox.js?v=2.1.5",//new                                                
            prettify: "../../App/third-Party/wysihtml5/lib/js/prettify",            
            angular_input_stars: "../../App/third-Party/angular-input-stars-master/angular-input-stars",
            urNoticeScript: "../../App/js/urNoticeScript",
            //==============================================================================================================
            // Application Related JS
            //==============================================================================================================
            beforeLoginCookieService: "../../../../App/pages/beforeLogin/controller/common/CookieService",
            beforeLoginApp: ".././../App/pages/beforeLogin/controller/beforeLoginApp",
            beforeLoginIndex: "../../App/pages/beforeLogin/Index/index",
            beforeLoginSolrService: "../../App/pages/beforeLogin/controller/common/SolrService",
            AllUsersPageController: "../../App/pages/beforeLogin/AllUsers/AllUsers",
            AllCompaniesController: "../../App/pages/beforeLogin/AllCompanies/AllCompanies",
            validateEmail: "../../App/pages/beforeLogin/ValidateEmail/validateEmail",
            showMessageTemplate: "../../App/pages/beforeLogin/ShowMessage/showMessageTemplate",
            beforeLoginForgetPassword: "../../App/pages/beforeLogin/ForgetPassword/ForgetPassword",
            beforeLoginResetPassword: "../../App/pages/beforeLogin/ResetPassword/resetpasswordTemplate",
            beforeLoginEditPage: "../../App/pages/beforeLogin/EditPage/editPage",
            beforeLoginCompanyDetails: "../../App/pages/beforeLogin/CompanyDetails/CompanyDetails",
            beforeLoginUserDetails: "../../App/pages/beforeLogin/UserDetails/UserDetails",
            beforeLoginSearch: "../../App/pages/beforeLogin/Search/Search",

            //TweenMax_min: "http://cdnjs.cloudflare.com/ajax/libs/gsap/1.9.7/TweenMax.min",
            
        },
        urlArgs: ""
    });

appRequire(["jquery", "angular", "jquery_toastmessage", "toastMessage","sanitize","jquery_cookie",
    "jquery_blockUI", "angular_route", "beforeLoginCookieService","restangular",
    "beforeLoginApp", "jquery_sidr_min", "beforeLoginIndex",
    "prettify", "bootstrap","urNoticeScript","angular_animate",
    "domReady", "fancybox", "ngAutocomplete", "angucomplete_alt_min",
    "angular_input_stars", "bootstrap_ui", "AllUsersPageController", "AllCompaniesController", "validateEmail",
    "showMessageTemplate", "beforeLoginForgetPassword", "beforeLoginResetPassword", "beforeLoginEditPage",
    "beforeLoginCompanyDetails", "beforeLoginSearch", "beforeLoginUserDetails"
], function() {
    angular.bootstrap(document.getElementById("main"), ["beforeLoginApp"]);
});
