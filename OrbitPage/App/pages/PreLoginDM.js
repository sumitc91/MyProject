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
            ng_infinite_scroll: {
                deps: ["jquery", "angular"]
            },
            rzslider: {
                deps: ["jquery", "angular"]
            },
            mentio: {
                deps: ["jquery", "angular"]
            },
            prettify: { //used 
                deps: ["jquery"]
            },
            jquery_sidr_min: {
                deps: ["jquery"]
            },
            ngtimeago: {
                deps: ["jquery", "angular"]
            },
            wysihtml5: {
                deps: ["jquery"]
            },
            bootstrap_wysihtml5: { //used
                deps: ["jquery", "wysihtml5"]
            },
            filedrop: {//new
                deps: ["jquery"]
            },
            fileDropScript: {//new
                deps: ["jquery", "filedrop", "domReady"]
            },
            angular_resource: {
                deps: ["jquery", "angular"]
            },
            urNoticeScript: {
                deps: ["jquery"]
            },
            motionCaptcha: {
                deps: ["jquery"]
            },
            googleChartLoader: {
                deps: ["jquery"]
            },
            jquery_totemticker: {
                deps: ["jquery"]
            },
            angularjs_fileUpload_shim: {//new
                deps: ["angular", "jquery"]
            },
            angularjs_fileUpload: {//new
                deps: ["angular", "jquery", "angularjs_fileUpload_shim"]
            },
            AngularFileUploadController: { //new 
                deps: ["jquery", "angular", "restangular", "configureBlockUI", "toastMessage", "beforeLoginCookieService", "fancybox", "angularjs_fileUpload", "angularjs_fileUpload_shim"]
            },
            beforeLoginSolrService: {
                deps: ["jquery", "angular", "configureBlockUI", "jquery_blockUI", "toastMessage", "angular_resource"]
            },
            beforeLoginApp: {
                deps: ["jquery", "angular", "configureBlockUI", "toastMessage", "jquery_sidr_min", "bootstrap_ui", "urNoticeScript", "ngtimeago"]
            },
            beforeLoginIndex: {
                deps: ["jquery", "beforeLoginApp", "restangular", "angular", "configureBlockUI", "toastMessage", "jquery_sidr_min", "bootstrap_ui", "urNoticeScript", "beforeLoginSolrService", "angular_animate"]
            },
            beforeLoginLoginPage: {
                deps: ["jquery","beforeLoginApp", "angular", "restangular", "configureBlockUI", "toastMessage"]
            },
            beforeLoginSignUpUser: {
                deps: ["jquery", "beforeLoginApp", "angular", "restangular", "configureBlockUI", "toastMessage", "motionCaptcha"]
            },
            validateEmail: {
                deps: ["jquery", "beforeLoginApp", "angular", "restangular", "configureBlockUI", "toastMessage"]
            },
            showMessageTemplate: {
                deps: ["jquery", "beforeLoginApp", "angular", "restangular", "configureBlockUI", "toastMessage"]
            },
            beforeLoginForgetPassword: {
                deps: ["jquery", "beforeLoginApp", "angular", "restangular", "configureBlockUI", "toastMessage"]
            },
            beforeLoginResetPassword: {
                deps: ["jquery", "beforeLoginApp", "angular", "restangular", "configureBlockUI", "toastMessage"]
            },
            beforeLoginEditPage: {
                deps: ["jquery", "beforeLoginApp", "angular", "restangular", "configureBlockUI", "toastMessage", "bootstrap_ui"]
            },
            beforeLoginCompanyDetails: {
                deps: ["jquery", "beforeLoginApp", "angular", "restangular", "configureBlockUI", "toastMessage", "angular_input_stars", "bootstrap_ui", "googleChartLoader"]
            },
            beforeLoginUserDetails: {
                deps: ["jquery", "beforeLoginApp", "angular", "restangular", "configureBlockUI", "toastMessage", "angular_input_stars", "bootstrap_ui"]
            },
            beforeLoginSearch: {
                deps: ["jquery", "beforeLoginApp", "angular", "restangular", "configureBlockUI", "toastMessage", "angular_input_stars", "bootstrap_ui"]
            },
            beforeLoginUserProfile: {
                deps: ["jquery", "beforeLoginApp", "angular", "restangular", "configureBlockUI", "toastMessage", "angular_input_stars", "bootstrap_ui", "AngularFileUploadController", "ngtimeago", "ng_infinite_scroll", "jquery_totemticker", "mentio"]
            },
            beforeLoginOrbitFeed: {
                deps: ["jquery", "beforeLoginApp", "angular", "restangular", "configureBlockUI", "toastMessage", "angular_input_stars", "bootstrap_ui", "AngularFileUploadController", "ngtimeago", "ng_infinite_scroll", "jquery_totemticker"]
            },
            beforeLoginViewPostDetail: {
                deps: ["jquery", "beforeLoginApp", "angular", "restangular", "configureBlockUI", "toastMessage", "angular_input_stars", "bootstrap_ui", "AngularFileUploadController", "ngtimeago"]
            },
            beforeLoginPostStory: {
                deps: ["jquery", "beforeLoginApp", "angular", "restangular", "configureBlockUI", "toastMessage", "wysihtml5", "bootstrap_wysihtml5", "prettify", "bootstrap", "filedrop", "fileDropScript", "domReady", "fancybox", "AngularFileUploadController", "ngAutocomplete", "angucomplete_alt_min"]
            },
            beforeLoginWorkgraphy: {
                deps: ["jquery", "beforeLoginApp", "restangular", "angular", "configureBlockUI", "toastMessage", "bootstrap_ui", "angular_animate", "ngtimeago"]
            },
            beforeLoginAllBlogs: {
                deps: ["jquery", "beforeLoginApp", "restangular", "angular", "configureBlockUI", "toastMessage", "bootstrap_ui", "angular_animate", "ngtimeago"]
            },
            beforeLoginSingleBlog: {
                deps: ["jquery", "beforeLoginApp", "angular", "restangular", "configureBlockUI", "toastMessage", "bootstrap_ui", "ngtimeago"]
            },
            beforeLoginSingleWorkgraphy: {
                deps: ["jquery", "beforeLoginApp", "angular", "restangular", "configureBlockUI", "toastMessage", "bootstrap_ui", "ngtimeago"]
            },
            beforeLoginUrnotice: {
                deps: ["jquery", "beforeLoginApp", "angular", "restangular", "configureBlockUI", "toastMessage", "bootstrap_ui", "ngtimeago"]
            },
            beforeLoginPostYourNotice: {
                deps: ["jquery", "beforeLoginApp", "angular", "restangular", "configureBlockUI", "toastMessage", "angular_input_stars", "bootstrap_ui", "rzslider"]
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

            angularjs_fileUpload_shim: "../../App/third-Party/angular-file-upload-master/dist/angular-file-upload-shim.min",
            angularjs_fileUpload: "../../App/third-Party/angular-file-upload-master/dist/angular-file-upload.min",
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
            motionCaptcha: "../../App/js/jquery.motionCaptcha.0.2",
            //jquery_particleground: "../../App/js/jquery.particleground",
            ngtimeago: "../../App/js/angular/ngtimeago",
            rzslider: "../../App/js/angular/rzslider",
            mentio: "../../App/js/angular/mentio",
            ng_infinite_scroll: "../../App/js/angular/ng-infinite-scroll.min",
            wysihtml5: "../../App/third-Party/wysihtml5/lib/js/wysihtml5-0.3.0",
            bootstrap_wysihtml5: "../../App/third-Party/wysihtml5/lib/js/bootstrap3-wysihtml5.all.min",
            filedrop: "../../App/third-Party/html5-file-upload/assets/js/jquery.filedrop",
            fileDropScript: "../../App/third-Party/html5-file-upload/assets/js/script",
            googleChartLoader: "https://www.gstatic.com/charts/loader",
            jquery_totemticker: "../../App/js/jquery.totemticker",
            //==============================================================================================================
            // Application Related JS
            //==============================================================================================================
            beforeLoginCookieService: "../../../../App/pages/beforeLogin/controller/common/CookieService",
            AngularFileUploadController: "../../App/Pages/beforeLogin/controller/common/AngularFileUploadController",//new
            beforeLoginApp: ".././../App/pages/beforeLogin/controller/beforeLoginApp",
            beforeLoginIndex: "../../App/pages/beforeLogin/Index/index",
            beforeLoginSolrService: "../../App/pages/beforeLogin/controller/common/SolrService",
            beforeLoginLoginPage: "../../App/pages/beforeLogin/Login/Login",
            beforeLoginSignUpUser: "../../App/pages/beforeLogin/SignUpUser/SignUpUser",
            validateEmail: "../../App/pages/beforeLogin/ValidateEmail/validateEmail",
            showMessageTemplate: "../../App/pages/beforeLogin/ShowMessage/showMessageTemplate",
            beforeLoginForgetPassword: "../../App/pages/beforeLogin/ForgetPassword/ForgetPassword",
            beforeLoginResetPassword: "../../App/pages/beforeLogin/ResetPassword/resetpasswordTemplate",
            beforeLoginEditPage: "../../App/pages/beforeLogin/EditPage/editPage",
            beforeLoginCompanyDetails: "../../App/pages/beforeLogin/CompanyDetails/CompanyDetails",
            beforeLoginUserDetails: "../../App/pages/beforeLogin/UserDetails/UserDetails",
            beforeLoginSearch: "../../App/pages/beforeLogin/Search/Search",
            beforeLoginUserProfile: "../../App/pages/beforeLogin/UserProfile/UserProfile",
            beforeLoginOrbitFeed: "../../App/pages/beforeLogin/OrbitFeed/OrbitFeed",
            beforeLoginViewPostDetail: "../../App/pages/beforeLogin/ViewPostDetail/ViewPostDetail",
            beforeLoginPostStory: "../../App/pages/beforeLogin/PostStory/PostStory",
            beforeLoginPostBlog: "../../App/pages/beforeLogin/PostBlog/PostBlog",
            beforeLoginWorkgraphy: "../../App/pages/beforeLogin/Workgraphy/Workgraphy",
            beforeLoginAllBlogs: "../../App/pages/beforeLogin/AllBlogs/AllBlogs",
            beforeLoginSingleBlog: "../../App/pages/beforeLogin/SingleBlog/SingleBlog",
            beforeLoginSingleWorkgraphy: "../../App/pages/beforeLogin/SingleWorkgraphy/SingleWorkgraphy",
            beforeLoginUrnotice: "../../App/pages/beforeLogin/Urnotice/Urnotice",
            beforeLoginPostYourNotice: "../../App/pages/beforeLogin/PostYourNotice/PostYourNotice",
            //TweenMax_min: "http://cdnjs.cloudflare.com/ajax/libs/gsap/1.9.7/TweenMax.min",
            
        },
        urlArgs: ""
    });

appRequire(["jquery", "angular", "jquery_toastmessage", "toastMessage","sanitize","jquery_cookie",
    "jquery_blockUI", "angular_route", "beforeLoginCookieService","restangular",
    "beforeLoginApp", "jquery_sidr_min", "beforeLoginIndex",
    "prettify", "bootstrap","urNoticeScript","angular_animate",
    "domReady", "fancybox", "ngAutocomplete", "angucomplete_alt_min",
    "angular_input_stars", "bootstrap_ui", "beforeLoginLoginPage", "beforeLoginSignUpUser", "validateEmail",
    "showMessageTemplate", "beforeLoginForgetPassword", "beforeLoginResetPassword", "beforeLoginEditPage",
    "beforeLoginCompanyDetails", "beforeLoginSearch", "beforeLoginUserDetails", "beforeLoginUserProfile",
    "angularjs_fileUpload_shim", "angularjs_fileUpload", "AngularFileUploadController", "motionCaptcha", "ngtimeago",
    "beforeLoginViewPostDetail", "ng_infinite_scroll", "filedrop", "fileDropScript", "beforeLoginPostStory", "beforeLoginWorkgraphy",
    "beforeLoginSingleBlog", "beforeLoginUrnotice", "beforeLoginPostYourNotice", "beforeLoginPostBlog", "beforeLoginAllBlogs",
    "beforeLoginSingleWorkgraphy", "jquery_totemticker", "beforeLoginOrbitFeed", "rzslider", "mentio"
], function() {
    angular.bootstrap(document.getElementById("main"), ["beforeLoginApp"]);
});
