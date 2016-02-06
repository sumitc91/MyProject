'use strict';
define([appLocation.preLogin], function (app) {
    app.factory('CookieUtil', function ($rootScope, $routeParams) {

        return {
            setUTMZT: function (UTMZT, keepMeSignedIn) {
                if (keepMeSignedIn) {
                    $.cookie('utmzt', UTMZT, { expires: 365, path: '/', domain: ServerContextPath.cookieDomain });
                }
                else {
                    $.cookie('utmzt', UTMZT, { path: '/', domain: ServerContextPath.cookieDomain });
                }
            },
            setUTMZK: function (UTMZK, keepMeSignedIn) {
                if (keepMeSignedIn) {
                    $.cookie('utmzk', UTMZK, { expires: 365, path: '/', domain: ServerContextPath.cookieDomain });
                }
                else {
                    $.cookie('utmzk', UTMZK, { path: '/', domain: ServerContextPath.cookieDomain });
                }
            },
            setUTMZV: function (UTMZV, keepMeSignedIn) {
                if (keepMeSignedIn) {
                    $.cookie('utmzv', UTMZV, { expires: 365, path: '/', domain: ServerContextPath.cookieDomain });
                }
                else {
                    $.cookie('utmzv', UTMZV, { path: '/', domain: ServerContextPath.cookieDomain });
                }
            },
            setUTIME: function (UTIME, keepMeSignedIn) {
                if (keepMeSignedIn) {
                    $.cookie('utime', UTIME, { expires: 365, path: '/', domain: ServerContextPath.cookieDomain });
                }
                else {
                    $.cookie('utime', UTIME, { path: '/', domain: ServerContextPath.cookieDomain });
                }
            },
            setKMSI: function (KMSI, keepMeSignedIn) {
                if (keepMeSignedIn) {
                    $.cookie('kmsi', KMSI, { expires: 365, path: '/', domain: ServerContextPath.cookieDomain });
                }
                else {
                    $.cookie('kmsi', KMSI, { path: '/', domain: ServerContextPath.cookieDomain });
                }
            },
            setLoginType: function (LoginType, keepMeSignedIn) {
                if (keepMeSignedIn) {
                    $.cookie('loginType', LoginType, { expires: 365, path: '/', domain: ServerContextPath.cookieDomain });
                }
                else {
                    $.cookie('loginType', LoginType, { path: '/', domain: ServerContextPath.cookieDomain });
                }
            },
            setUserName: function (userName, keepMeSignedIn) {
                if (keepMeSignedIn) {
                    $.cookie('userName', userName, { expires: 365, path: '/', domain: ServerContextPath.cookieDomain });
                }
                else {
                    $.cookie('userName', userName, { path: '/', domain: ServerContextPath.cookieDomain });
                }
            },
            setRefKey: function (RefKey, keepMeSignedIn) {
                if (keepMeSignedIn) {
                    $.cookie('refKey', RefKey, { expires: 365, path: '/', domain: ServerContextPath.cookieDomain });
                }
                else {
                    $.cookie('refKey', RefKey, { path: '/', domain: ServerContextPath.cookieDomain });
                }
            },
            setUserImageUrl: function (userImageUrl, keepMeSignedIn) {
                if (keepMeSignedIn) {
                    $.cookie('userImageUrl', userImageUrl, { expires: 365, path: '/', domain: ServerContextPath.cookieDomain });
                }
                else {
                    $.cookie('userImageUrl', userImageUrl, { path: '/', domain: ServerContextPath.cookieDomain });
                }
            },
            setReturnUrl: function (returnUrl, keepMeSignedIn) {
                if (keepMeSignedIn) {
                    $.cookie('returnUrl', returnUrl, { expires: 365, path: '/', domain: ServerContextPath.cookieDomain });
                }
                else {
                    $.cookie('returnUrl', returnUrl, { path: '/', domain: ServerContextPath.cookieDomain });
                }
            },
            getUTMZT: function () {
                return $.cookie('utmzt');
            },
            getUTMZK: function () {
                return $.cookie('utmzk');
            },
            getUTMZV: function () {
                return $.cookie('utmzv');
            },
            getUTIME: function () {
                return $.cookie('utime');
            },
            getKMSI: function () {
                return $.cookie('kmsi');
            },
            getRefKey: function () {
                return $.cookie('refKey');
            },
            getReturnUrl: function () {
                return $.cookie('returnUrl');
            },
            getLoginType: function () {
                return $.cookie('loginType');
            },
            getUserName: function () {
                return $.cookie('userName');
            },
            getUserImageUrl: function () {
                return $.cookie('userImageUrl');
            },
            removeUTMZT: function () {
                $.removeCookie('utmzt', { path: '/', domain: ServerContextPath.cookieDomain });
            },
            removeUTMZK: function () {
                $.removeCookie('utmzk', { path: '/', domain: ServerContextPath.cookieDomain });
            },
            removeUTMZV: function () {
                $.removeCookie('utmzv', { path: '/', domain: ServerContextPath.cookieDomain });
            },
            removeUTIME: function () {
                $.removeCookie('utime', { path: '/', domain: ServerContextPath.cookieDomain });
            },
            removeRefKey: function () {
                $.removeCookie('refKey', { path: '/', domain: ServerContextPath.cookieDomain });
            },
            removeKMSI: function () {
                $.removeCookie('kmsi', { path: '/', domain: ServerContextPath.cookieDomain });
            },
            removeReturnUrl: function () {
                $.removeCookie('returnUrl', { path: '/', domain: ServerContextPath.cookieDomain });
            }
        };

    });


});