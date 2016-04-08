'use strict';
define([appLocation.preLogin], function (app) {
    app.controller('beforeLoginWorkgraphy', function ($scope, $interval, $http, $routeParams, $rootScope, $location, Restangular, CookieUtil, SolrServiceUtil) {

        $('title').html(window.madetoearn.i18n.beforeLoginOrbitPageCompanyTitle);
        
        //detectIfUserLoggedIn();
        /*$rootScope.sitehosturl = $location.host();//"localhost:40287";
        if ($location.host() == "localhost") {
            $rootScope.sitehosturl = "localhost:28308";
        }*/
        $rootScope.sitehosturl = "www.orbitpage.com/searchapi";

        $scope.searchBoxText = window.madetoearn.i18n.beforeLoginIndexSearchBoxText;
        $scope.beforeLoginIndexLatestWorkgraphy = window.madetoearn.i18n.beforeLoginIndexLatestWorkgraphy;
        $scope.beforeLoginIndexTopCompanies = window.madetoearn.i18n.beforeLoginIndexTopCompanies;
        
        
        getLatestWorkgraphy();

        function getLatestWorkgraphy() {

            $scope.currentPage = 0;
            $scope.perpage = 10;
            $scope.totalMatch = 10;
            var url = ServerContextPath.solrServer + '/Search/GetLatestWorkgraphy?page=' + $scope.currentPage + '&perpage=' + $scope.perpage + '&totalMatch=' + $scope.totalMatch;
            var headers = {
                'Content-Type': 'application/json',
                'UTMZT': $.cookie('utmzt'),
                'UTMZK': $.cookie('utmzk'),
                'UTMZV': $.cookie('utmzv'),
            };
            startBlockUI('wait..', 3);
            $.ajax({
                url: url,
                method: "GET",
                headers: headers
            }).done(function (data, status) {
                stopBlockUI();
                console.log(data);
                if (data.Status == "200") {
                    //showToastMessage("Success", data.Message);
                    
                    $scope.totalMatch = data.Message;
                    //$scope.queryParam.totalNumberOfPages = Math.ceil((data.Payload.count / $scope.queryParam.perpage));
                    $scope.LatestWorkGraphyList = data.Payload;
                    //$.each(data.Payload, function (i, val) {
                    //    $scope.searchResult[i].companyname = data.Payload.result[i].companyname;
                    //    $scope.searchResult[i].website = data.Payload.result[i].website;

                    //    if ($scope.searchResult[i].logourl == 'tps://s3-ap-southeast-1.amazonaws.com/urnotice/company/small/LogoUploadEmpty.png')
                    //        $scope.searchResult[i].logourl = "http://placehold.it/50x50";

                    //    $scope.searchResult[i].linkurl = "/#companydetails/" + $scope.searchResult[i].companyname.replace(/ /g, "_").replace(/\//g, "_OR_") + "/" + $scope.searchResult[i].guid;
                    //    $scope.pagination.show = true;
                    //});

                    if ($scope.$root.$$phase != '$apply' && $scope.$root.$$phase != '$digest') {
                        $scope.$apply();
                    }
                    //$scope.$apply();
                    //console.log($scope.competitorDetails);
                }
                else {
                    showToastMessage("Warning", data.Message);
                }
            });
            
            
        }


        

        $scope.openFacebookAuthWindow = function() {
            var url = '/SocialAuth/FBLoginGetRedirectUri';
            startBlockUI('wait..', 3);
            $http({
                url: url,
                method: "GET",
                headers: { 'Content-Type': 'application/json' }
            }).success(function(data, status, headers, config) {
                //$scope.persons = data; // assign  $scope.persons here as promise is resolved here
                stopBlockUI();
                if (data.Status == "199") {
                    location.href = data.Message;
                } else {
                    alert("some error occured");
                }

            }).error(function(data, status, headers, config) {
                alert("internal server error occured");
            });
            //            var win = window.open("/SocialAuth/FBLogin/facebook", "Ratting", "width=" + popWindow.width + ",height=" + popWindow.height + ",0,status=0,scrollbars=1");
            //            win.onunload = onun;

            //            function onun() {
            //                if (win.location != "about:blank") // This is so that the function 
            //                // doesn't do anything when the 
            //                // window is first opened.
            //                {
            //                    //$route.reload();
            //                    //alert("working");
            //                    //location.reload();
            //                    //alert("closed");
            //                }
            //            }
        };

        $scope.openLinkedinAuthWindow = function() {
            var url = '/SocialAuth/LinkedinLoginGetRedirectUri';
            startBlockUI('wait..', 3);
            $http({
                url: url,
                method: "GET",
                headers: { 'Content-Type': 'application/json' }
            }).success(function(data, status, headers, config) {
                //$scope.persons = data; // assign  $scope.persons here as promise is resolved here
                stopBlockUI();
                if (data.Status == "199") {
                    location.href = data.Message;
                } else {
                    alert("some error occured");
                }

            }).error(function(data, status, headers, config) {
                alert("internal server error occured");
            });
            //            var win = window.open("/SocialAuth/LinkedinLogin", "Ratting", "width=" + popWindow.width + ",height=" + popWindow.height + ",0,status=0,scrollbars=1");
            //            win.onunload = onun;

            //            function onun() {
            //                if (win.location != "about:blank") // This is so that the function 
            //                // doesn't do anything when the 
            //                // window is first opened.
            //                {
            //                    //$route.reload();
            //                    //alert("working");
            //                    //location.reload();
            //                    //alert("closed");
            //                }
            //            }
        };

        $scope.openGoogleAuthWindow = function() {
            var url = '/SocialAuth/GoogleLoginGetRedirectUri';
            startBlockUI('wait..', 3);
            $http({
                url: url,
                method: "GET",
                headers: { 'Content-Type': 'application/json' }
            }).success(function(data, status, headers, config) {
                //$scope.persons = data; // assign  $scope.persons here as promise is resolved here
                stopBlockUI();
                if (data.Status == "199") {
                    location.href = data.Message;
                } else {
                    alert("some error occured");
                }

            }).error(function(data, status, headers, config) {
                alert("internal server error occured");
            });
            //            var win = window.open("/SocialAuth/GoogleLogin/", "Ratting", "width=" + popWindow.width + ",height=" + popWindow.height + ",0,status=0,scrollbars=1");
            //            win.onunload = onun;

            //            function onun() {
            //                if (win.location != "about:blank") // This is so that the function 
            //                // doesn't do anything when the 
            //                // window is first opened.
            //                {
            //                    //$route.reload();
            //                    //alert("working");
            //                    //location.reload();
            //                    //alert("closed");
            //                }
            //            }
        };
    });

    function isValidEmailAddress(emailAddress) {
        var pattern = new RegExp(/^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$/i);
        return pattern.test(emailAddress);
    };
});


			

