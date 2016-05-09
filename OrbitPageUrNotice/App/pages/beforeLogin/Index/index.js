'use strict';
define([appLocation.preLogin], function (app) {
    app.controller('beforeLoginIndex', function ($scope,$interval, $http,$routeParams, $rootScope, $location, Restangular, CookieUtil, SolrServiceUtil) {

        $('title').html(window.madetoearn.i18n.beforeLoginOrbitPageCompanyTitle);
        
        $scope.searchBoxText = window.madetoearn.i18n.beforeLoginIndexSearchBoxText;
        $scope.beforeLoginIndexLatestWorkgraphy = window.madetoearn.i18n.beforeLoginIndexLatestWorkgraphy;
        $scope.beforeLoginIndexTopCompanies = window.madetoearn.i18n.beforeLoginIndexTopCompanies;
        $scope.isCollapsed = true;
        $scope.searchBackgroundImageList = [
            { url: "https://s3-ap-southeast-1.amazonaws.com/urnotice/App/img/indexPageSlider/test/motivational-hd-wallpaper1_1280x700.jpg", textColor: "blackColor" },
            { url: "https://s3-ap-southeast-1.amazonaws.com/urnotice/App/img/indexPageSlider/test/cropped-Amazing-Home-Interi.jpg", textColor: "whiteColor" },
            { url: "https://s3-ap-southeast-1.amazonaws.com/urnotice/App/img/indexPageSlider/test/Programmers+Wallpapers+HD+by+PCbots_1280x700.jpg", textColor: "whiteColor" },
            { url: "https://s3-ap-southeast-1.amazonaws.com/urnotice/App/img/indexPageSlider/test/d92f42382d1ac0ad61a2f772bf5f47aa_1280x700.jpg", textColor: "whiteColor" },
            { url: "https://s3-ap-southeast-1.amazonaws.com/urnotice/App/img/indexPageSlider/test/main_1280x700.jpg", textColor: "blackColor" },
            { url: "https://s3-ap-southeast-1.amazonaws.com/urnotice/App/img/indexPageSlider/test/big_thumb_c319bfe7d3002b9bf61c605e409963eb_1280x700.jpg", textColor: "whiteColor" },
            { url: "https://s3-ap-southeast-1.amazonaws.com/urnotice/App/img/indexPageSlider/test/Modern-Office-Building-In-P.jpg", textColor: "whiteColor" }
            
        ];
        $scope.searchBackgroundImage = "";

        $scope.searchBackgroundImage = $scope.searchBackgroundImageList[0].url;
        $scope.themeTextColor = $scope.searchBackgroundImageList[0].textColor;
        var counter = 0;
        $interval(function () {
            if (counter == $scope.searchBackgroundImageList.length-1)
                counter = 0;
            else
                counter++;
            $scope.searchBackgroundImage = $scope.searchBackgroundImageList[counter].url;
            $scope.themeTextColor = $scope.searchBackgroundImageList[counter].textColor;
        }, 30000, 0);

        $scope.companyDetails = {

        };
        getSolrServiceCompetitors();

        function getSolrServiceCompetitors() {

            SolrServiceUtil.get({ size: '10001', rating: '0', speciality: 'Management Consulting,Systems Integration and Technology,Business Process Outsourcing,Application and Infrastructure Outsourcing' }, function (data) {

                if (data.Status == 200) {
                    
                    //showToastMessage("Success", data.Message);
                    $scope.competitorDetails = data.Payload;
                    if ($scope.companyDetails.logourl == 'tps://s3-ap-southeast-1.amazonaws.com/urnotice/company/small/LogoUploadEmpty.png')
                        $scope.companyDetails.logourl = "http://placehold.it/350x150";

                    $.each(data.Payload, function (i, val) {
                        $scope.competitorDetails[i].companyname = data.Payload[i].companyname;
                        $scope.competitorDetails[i].website = data.Payload[i].website;

                        if ($scope.competitorDetails[i].logourl == 'tps://s3-ap-southeast-1.amazonaws.com/urnotice/company/small/LogoUploadEmpty.png')
                            $scope.competitorDetails[i].logourl = "http://placehold.it/50x50";

                        $scope.competitorDetails[i].linkurl = "/#companydetails/" + $scope.competitorDetails[i].companyname.replace(/ /g, "_").replace(/\//g, "_OR_") + "/" + $scope.competitorDetails[i].guid;

                    });

                    //$scope.$apply();
                    if ($scope.$root.$$phase != '$apply' && $scope.$root.$$phase != '$digest') {
                        $scope.$apply();
                    }
                }
            }, function (error) {
                // Error handler code
                showToastMessage("Error", "Internal Server Error Occured!");
            });

            
            
        }


        $scope.myFunct = function(keyEvent) {
            if (keyEvent.which === 13) {
                location.href = "/#search/?q=" + $("#companyName_value").val() + "&page=1&perpage=10";
            }
        };

        $scope.searchCompany = function() {
            location.href = "/#search/?q=" + $("#companyName_value").val() + "&page=1&perpage=10";
        };

        $scope.selectCompany = function (selected) {
            console.log(selected);
            location.href = "/#companydetails/" + selected.originalObject.companyname.replace(/ /g, "_").replace(/\//g, "_OR_") + "/" + selected.originalObject.guid;
            
        };

        $scope.imageIndex = 2;
        $scope.sliderImage = "https://s3-ap-southeast-1.amazonaws.com/urnotice/App/img/indexPageSlider/slider_final_low_3_" + $scope.imageIndex + ".jpg";

        $scope.prevSlide = function () {
            console.log('prevslide');
            if( $scope.imageIndex == 1)
                $scope.imageIndex = 3;
            else
                $scope.imageIndex = $scope.imageIndex - 1;

            $scope.sliderImage = "https://s3-ap-southeast-1.amazonaws.com/urnotice/App/img/indexPageSlider/slider_final_low_3_" + $scope.imageIndex + ".jpg";
            $scope.$$phase || $scope.$apply();
        };


        $scope.nextSlide = function () {
            console.log('nextslide');
            if( $scope.imageIndex ==3 )
                $scope.imageIndex = 1;
            else
                $scope.imageIndex = $scope.imageIndex + 1;

            $scope.sliderImage = "https://s3-ap-southeast-1.amazonaws.com/urnotice/App/img/indexPageSlider/slider_final_low_3_" + $scope.imageIndex + ".jpg";
            $scope.$$phase || $scope.$apply();
        };



        $scope.myInterval = 5000;
        $scope.noWrapSlides = false;
        var currIndex = 0;
        $scope.slides = [];
        var slides = $scope.slides;
        $scope.slides = [
        {
            image: 'https://s3-ap-southeast-1.amazonaws.com/urnotice/App/img/indexPageSlider/received_909588079090414.jpeg',
            text: ['Nice image1', 'Awesome photograph', 'That is so cool', 'I love that'][slides.length % 4],
            id: currIndex++
        },
        {
            image: 'https://s3-ap-southeast-1.amazonaws.com/urnotice/App/img/indexPageSlider/final_2.jpg',
            text: ['Nice image2', 'Awesome photograph', 'That is so cool', 'I love that'][slides.length % 4],
            id: currIndex++
        },
        {
            image: 'https://s3-ap-southeast-1.amazonaws.com/urnotice/App/img/indexPageSlider/final_3.jpg',
            text: ['Nice image3', 'Awesome photograph', 'That is so cool', 'I love that'][slides.length % 4],
            id: currIndex++
        }
        ];
        
        

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


			

