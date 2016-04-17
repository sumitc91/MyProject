var logoImage = "../../Template/AdminLTE-master/img/m2eV3.png";
var googleAnalyticsAppID = "UA-72255297-1";
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
    empty: "",
    
    authServer: "http://www.orbitpage.com/authapi",
    solrServer: "http://www.orbitpage.com/searchapi",
    userServer: "http://www.orbitpage.com/userapi",
    cookieDomain: ".orbitpage.com"


    //authServer: "http://localhost:31959",
    //solrServer: "http://localhost:28308",
    //userServer: "http://localhost:6368",
    //cookieDomain: "localhost",
};

var appLocation = {
    'common': '../../App/CommonInit',
    'preLogin': '../../App/Pages/PreLoginInit',
    'postLogin': '../../App/Pages/PostLoginInit',
    'userPostLogin': '../../App/Pages/UserPostLoginInit',
    'adminPostLogin': '../../App/Pages/AdminPostLoginInit'
};

var UserReaction = {
    Like:"Like"
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

function getParameterByName(name) {
    var match = RegExp('[?&]' + name + '=([^&]*)').exec(window.location.search);
    return match && decodeURIComponent(match[1].replace(/\+/g, ' '));
}

function updateMe( data ) {
    if (data == 'login') {
        //location.href = "/" + $.cookie('loginType');
        redirectAfterSocialLogin();
    }        
    else if(data == 'fblikepage')
        location.href="/user#/facebookLikePage";
    else if(data == 'error')
        alert("Internal Server Error Occured !! Try Again");
}

function setReturnUrlInCookie(data) {
    $.cookie('returnUrl', data, { expires: 365, path: '/' });
}

function redirectAfterSocialLogin() {
    var returnUrl = "";
    if ($.cookie('returnUrl') != null && $.cookie('returnUrl') != "null")
        returnUrl = $.cookie('returnUrl');

    //console.log(returnUrl + " : " + returnUrl.toLowerCase().indexOf("login"));
    if (returnUrl.toLowerCase().indexOf("login") >= 0 || returnUrl.toLowerCase().indexOf("resetpassword") >= 0) {
        console.log("/#");
        window.location.href = "/#";

        //window.location = '/#/';
        //window.location.reload();
    } else {
        console.log("/#" + returnUrl);
        window.location.href = "/#" + returnUrl;

        //window.location = '/#' + returnUrl;
        //window.location.reload();
    }

}

function redirectAfterLogin() {
    var returnUrl = "";
    if ($.cookie('returnUrl') != null && $.cookie('returnUrl') != "null")
        returnUrl = $.cookie('returnUrl');

    //console.log(returnUrl + " : " + returnUrl.toLowerCase().indexOf("login"));
    if (returnUrl.toLowerCase().indexOf("login") >= 0 || returnUrl.toLowerCase().indexOf("resetpassword") >= 0) {
        console.log("/#");
        //window.location.href = "/#";

        window.location = '/#/';
        window.location.reload();
    } else {
        console.log("/#" + returnUrl);
        //window.location.href = "/#" + returnUrl;

        window.location = '/#' + returnUrl;
        window.location.reload();
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
             var url = ServerContextPath.authServer + '/Auth/IsValidSession';
                 $.ajax({
						url: url,
						type: "POST",
                        headers: headers
						}).done(function(data,status) {
							console.log(data);
                     if (data == true) {
                         //location.href = "/" + $.cookie('loginType');
                     } else {
                         removeAllCookies(ServerContextPath.cookieDomain);
                         // will first fade out the loading animation
                         jQuery("#status").fadeOut();
                         // will fade out the whole DIV that covers the website.
                         jQuery("#preloader").delay(1000).fadeOut("medium");
                     }
                 });
         }
         else
         {
                removeAllCookies(ServerContextPath.cookieDomain);
                // will first fade out the loading animation
                jQuery("#status").fadeOut();
                // will fade out the whole DIV that covers the website.
                jQuery("#preloader").delay(1000).fadeOut("medium");
         }
					
}

function removeAllCookies(cookieDomain) {
    $.removeCookie('utmzt', { path: '/', domain: cookieDomain });
    $.removeCookie('utmzk', { path: '/', domain: cookieDomain });
    $.removeCookie('utmzv', { path: '/', domain: cookieDomain });
    $.removeCookie('utime', { path: '/', domain: cookieDomain });
    $.removeCookie('kmsi', { path: '/', domain: cookieDomain });
    $.removeCookie('uservertexid', { path: '/', domain: cookieDomain });
    $.removeCookie('userName', { path: '/', domain: cookieDomain });
    $.removeCookie('userImageUrl', { path: '/', domain: cookieDomain });
}

function removeRefCookies(cookieDomain) {
    $.removeCookie('refKey', { path: '/', domain: cookieDomain });
}

function validateAndReplaceToDateFormat(val) {
    return val.replace("\/Date(", "").replace(")\/", "");
}
//removeRefCookies(ServerContextPath.cookieDomain);

function logout(){
    var headers = {
                        'Content-Type': 'application/json',
						'UTMZT': $.cookie('utmzt'),
						'UTMZK': $.cookie('utmzk'),
						'UTMZV': $.cookie('utmzv')                       
                    };
         if($.cookie('utmzt') != null && $.cookie('utmzt') != "")
         {
             var url = ServerContextPath.authServer + '/Auth/Logout';
//                 $.ajax({
//						url: url,
//						type: "POST",
//                        headers: headers
//						}).done(function(data,status) {							                                                                              
//                           					
//						});
//                
             startBlockUI('wait..', 3);
                    $.ajax({
                       type: "POST",
                       url: url,
                       headers: headers,                       
                       success: function (result) {
                           stopBlockUI();
                           removeAllCookies(ServerContextPath.cookieDomain);
                           location.href = "/";
                           //redirectAfterLogin();
                       },
                       error: function (request, status, errorThrown) {
                           stopBlockUI();
                           removeAllCookies(ServerContextPath.cookieDomain);                            
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

function initializeHoverCard() {
    console.log("initializeHoverCard");
    $('.show_hovercard').on({
        'mouseenter': function (e) {
            var $current_node = $(this);

            var target = $(e.target);
            var user_name = target.attr('user_name');

            console.log(user_name);
            var $parent = $current_node.parent();
            var markup = "<div class='hovercard'>" + user_name + "'s hovercard</div>";
            $parent.append(markup);
        }
    });

    $('.hover_div').on({
        'mouseleave': function (e) {
            var $hovercard = $(this).find('.hovercard');
            $hovercard.remove();
        }
    });
}