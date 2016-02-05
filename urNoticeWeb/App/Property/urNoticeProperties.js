var logoImage = "../../Template/AdminLTE-master/img/m2eV3.png";
var googleAnalyticsAppID = "UA-70627596-1";
var facebookAppId = "689268497785729";
var userSession = {
    username: "sumitchourasia91@gmail.com",
    guid: "",
    selectedPagePagination:"",
    selectedStars:0,
    listOfImgurImages:[],
    keepMeSignedIn:false,
    wysiHtml5UploadedInstructionsImageUrlLink: [],
    imgurImageTemplateModeratingPhotos :[],
    imgurImageTranscriptionTemplate: [] 
};

var companyName = "urNotice";
var userType = {
    requester: "requester",
    accepter: "accepter"
};

var companyConstants = {
    name: "urNotice",
    fullName: "Your Notice",
    supportEmail: "support@yournotice.com"
};

var clientConstants = {
    name: "",
    name_abb: "Requester"
};

var userConstants = {
    name: "",
    name_abb: "User",
    task: "",
    task_abb: "",
    batch: "batch",
    Batch: "Batch",
    Reputation: "Reputation",
    reputation: "reputation",
};

var ServerContextPath = {
    empty:"",
    authServer: "http://www.auth.urnotice.com"
};

var appLocation = {
    'common': '../../App/CommonInit',
    'preLogin': '../../App/Pages/PreLoginInit',
    'postLogin': '../../App/Pages/PostLoginInit',
    'userPostLogin': '../../App/Pages/UserPostLoginInit',
    'adminPostLogin': '../../App/Pages/AdminPostLoginInit'
};

var mobileDevice = detectmob();
var ipadDevice = detectipad();
var isAndroidDevice = detectAndroid();
function detectmob() {
    return (navigator.userAgent.match(/Android/i) || navigator.userAgent.match(/iPhone/i) || navigator.userAgent.match(/iPod/i) || navigator.userAgent.match(/BlackBerry/i) || navigator.userAgent.match(/Windows Phone/i));
}
function detectipad() {
    return (navigator.userAgent.match(/iPad/i) != null);
}
function detectAndroid() {
    return (navigator.userAgent.match(/Android/i) != null);
}

function updateMe(data) {
    if (data == 'login')
        location.href = "/" + $.cookie('loginType');
    else if (data == 'fblikepage')
        location.href = "/user#/facebookLikePage";
    else if (data == 'error')
        alert("Internal Server Error Occured !! Try Again");
}

function getParameterByName(name) {
    var match = RegExp('[?&]' + name + '=([^&]*)').exec(window.location.search);
    return match && decodeURIComponent(match[1].replace(/\+/g, ' '));
}

function updateMe( data ) {
    if (data == 'login') {
        //location.href = "/" + $.cookie('loginType');
        redirectAfterLogin();
    }        
    else if(data == 'fblikepage')
        location.href="/user#/facebookLikePage";
    else if(data == 'error')
        alert("Internal Server Error Occured !! Try Again");
}

function setReturnUrlInCookie(data) {
    $.cookie('returnUrl', data, { expires: 365, path: '/' });
}

function redirectAfterLogin() {
    var returnUrl = "";
    if ($.cookie('returnUrl') != null && $.cookie('returnUrl') != "null")
        returnUrl = $.cookie('returnUrl');

    console.log(returnUrl + " : " + returnUrl.toLowerCase().indexOf("login"));
    if (returnUrl.toLowerCase().indexOf("login") >= 0 || returnUrl.toLowerCase().indexOf("resetpassword") >= 0) {
        location.href = "/#";
    } else {
        location.href = "/#" + returnUrl;
    }
    
}

function detectIfUserLoggedIn(){
        var headers = {
                        'Content-Type': 'application/json',
						'UTMZT': $.cookie('utmzt'),
						'UTMZK': $.cookie('utmzk'),
						'UTMZV': $.cookie('utmzv')                       
                    };
         if($.cookie('utmzt') != null && $.cookie('utmzt') != "" && $.cookie('loginType') != null && $.cookie('loginType') != "")
         {
             var url = ServerContextPath.empty + '/Auth/IsValidSession';
                 $.ajax({
						url: url,
						type: "POST",
                        headers: headers
						}).done(function(data,status) {
							console.log(data);
                     if (data == true) {
                         //location.href = "/" + $.cookie('loginType');
                     } else {
                         $.removeCookie('utmzt', { path: '/' });
                         $.removeCookie('utmzk', { path: '/' });
                         $.removeCookie('utmzv', { path: '/' });
                         $.removeCookie('utime', { path: '/' });
                         $.removeCookie('kmsi', { path: '/' });
                         // will first fade out the loading animation
                         jQuery("#status").fadeOut();
                         // will fade out the whole DIV that covers the website.
                         jQuery("#preloader").delay(1000).fadeOut("medium");
                     }
                 });
         }
         else
         {
                 $.removeCookie('utmzt', { path: '/' });
                 $.removeCookie('utmzk', { path: '/' });
                 $.removeCookie('utmzv', { path: '/' });
                 $.removeCookie('utime', { path: '/' });
                 $.removeCookie('kmsi', { path: '/' });
                // will first fade out the loading animation
                jQuery("#status").fadeOut();
                // will fade out the whole DIV that covers the website.
                jQuery("#preloader").delay(1000).fadeOut("medium");
         }
					
}


function logout(){
    var headers = {
                        'Content-Type': 'application/json',
						'UTMZT': $.cookie('utmzt'),
						'UTMZK': $.cookie('utmzk'),
						'UTMZV': $.cookie('utmzv')                       
                    };
         if($.cookie('utmzt') != null && $.cookie('utmzt') != "")
         {
             var url = ServerContextPath.empty + '/Auth/Logout';
//                 $.ajax({
//						url: url,
//						type: "POST",
//                        headers: headers
//						}).done(function(data,status) {							                                                                              
//                           					
//						});
//                

                    $.ajax({
                       type: "POST",
                       url: url,
                       headers: headers,                       
                       success: function(result){
                            $.removeCookie('utmzt', { path: '/' });
                            $.removeCookie('utmzk', { path: '/' });
                            $.removeCookie('utmzv', { path: '/' });
                            $.removeCookie('utime', { path: '/' });
                            $.removeCookie('kmsi', { path: '/' });
                           location.href = "/";
                           //redirectAfterLogin();
                       },
                       error: function(request,status,errorThrown) {
                            $.removeCookie('utmzt', { path: '/' });
                            $.removeCookie('utmzk', { path: '/' });
                            $.removeCookie('utmzv', { path: '/' });
                            $.removeCookie('utime', { path: '/' });
                            $.removeCookie('kmsi', { path: '/' });
                           location.href = "/";
                           //redirectAfterLogin();
                       }
                     });


         }
         else
         {
             location.href = "/";
             //redirectAfterLogin();
         }
                     
}