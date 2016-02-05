'use strict';
define([appLocation.adminPostLogin], function (app) {

    app.controller('UserAfterLoginAllNotifications', function ($scope, $http, $rootScope, CookieUtil) {
        $('title').html("edit page"); //TODO: change the title so cann't be tracked in log
        $scope.referralKey = "init";
        var url = ServerContextPath.empty + '/Admin/GetNewCompanyLinkedInMapping';
        var headers = {
            'Content-Type': 'application/json',
            'UTMZT': $.cookie('utmzt'),
            'UTMZK': $.cookie('utmzk'),
            'UTMZV': $.cookie('utmzv')
        };
        $.ajax({
            url: url,
            method: "GET",
            headers: headers
        }).done(function (data, status) {
            console.log(data.Status == "200");
            if (data.Status == "200") {                
                $scope.$apply(function () {
                    $scope.UserAllNotificationsDetail = data.Payload;
                });
                
            }
            else {
                console.log("false");
            }
        });
    });

});

