'use strict';
define([appLocation.preLogin], function (app) {

    app.controller('beforeLoginEditPage', function ($scope, $http, $rootScope, CookieUtil) {
        $('title').html("edit page"); //TODO: change the title so cann't be tracked in log
        

        $scope.openFacebookAuthWindow = function () {
            var win = window.open("/SocialAuth/FBLogin/facebook", "Ratting", "width=800,height=480,0,status=0,scrollbars=1");
            win.onunload = onun;

            function onun() {
                if (win.location != "about:blank") // This is so that the function 
                // doesn't do anything when the 
                // window is first opened.
                {
                    $route.reload();
                    //alert("closed");
                }
            }
        }
    });

});

