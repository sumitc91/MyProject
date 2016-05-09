'use strict';
define([appLocation.preLogin], function (app) {
    app.controller('ModalInstanceCtrl', function ($scope, $uibModalInstance, items) {

        $scope.items = items;
        $scope.averageRatingInDoubleFormat = 0.0;
        $scope.userRatingData = {
            overAllRating:0
    };
        //$scope.showUserInputRatingStar = false;
        $scope.selected = {
            item: $scope.items[0]
        };

        $scope.ok = function () {
            $uibModalInstance.close($scope.selected.item);
        };

        $scope.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };
    });

    app.controller('beforeLoginCompanyDetails', function ($scope, $http, $route,$uibModal,$log, $rootScope, $routeParams, $location, $timeout, CookieUtil) {
        $('title').html("indexcd"); //TODO: change the title so cann't be tracked in log
        
        $scope.ratingCount = 0;
        $scope.companyid = $routeParams.companyid;
        $scope.companyDetails = {
            
        };
        $scope.competitorDetails = [{}];
        CompanyDetailsById();

        function CompanyDetailsById() {
            var url = ServerContextPath.empty + '/Solr/CompanyDetailsById?cid=' + $scope.companyid;
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
                console.log(data);
                if (data.Status == "200") {
                    //showToastMessage("Success", data.Message);
                    $scope.companyDetails = data.Payload[0];
                    if ($scope.companyDetails.logourl == 'tps://s3-ap-southeast-1.amazonaws.com/urnotice/company/small/LogoUploadEmpty.png')
                        $scope.companyDetails.logourl = "http://placehold.it/350x150";
                    $scope.averageRatingInDoubleFormat = $scope.companyDetails.averagerating;
                    $scope.companyDetails.averagerating = Math.round($scope.companyDetails.averagerating);
                    $scope.$apply();
                    console.log($scope.companyDetails);
                    getCompanyCompetitorsDetail($scope.companyDetails.size, $scope.companyDetails.rating, $scope.companyDetails.speciality);
                }
                else {
                    showToastMessage("Warning", data.Message);
                }
            });

        }
        
        
        getUserRatingStatus();

        function getCompanyCompetitorsDetail(size, rating, speciality) {
            var url = ServerContextPath.empty + '/Solr/GetCompanyCompetitorsDetail?size=' + size + '&rating=' + rating + '&speciality=' + speciality;
            var headers = {
                'Content-Type': 'application/json',
                'UTMZT': $.cookie('utmzt'),
                'UTMZK': $.cookie('utmzk'),
                'UTMZV': $.cookie('utmzv'),                
            };

            $.ajax({
                url: url,
                method: "GET",
                headers: headers
            }).done(function (data, status) {
                console.log(data);
                if (data.Status == "200") {
                    //showToastMessage("Success", data.Message);
                    $scope.competitorDetails = data.Payload;
                    /*if ($scope.companyDetails.logourl == 'tps://s3-ap-southeast-1.amazonaws.com/urnotice/company/small/LogoUploadEmpty.png')
                        $scope.companyDetails.logourl = "http://placehold.it/350x150";*/

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
                    //console.log($scope.competitorDetails);
                }
                else {
                    showToastMessage("Warning", data.Message);
                }
            });
        }

        $scope.myFunct = function (keyEvent) {
            if (keyEvent.which === 13) {
                location.href = "/#search/?q=" + $("#companyName_value").val() + "&page=1&perpage=10";
            }
        }

        $scope.searchCompany = function () {
            location.href = "/#search/?q=" + $("#companyName_value").val() + "&page=1&perpage=10";
        }

        $scope.selectCompany = function (selected) {
            console.log(selected);
            location.href = "/#companydetails/" + selected.originalObject.companyname.replace(/ /g, "_").replace(/\//g, "_OR_") + "/" + selected.originalObject.guid;
        };

        $scope.items = ['item1', 'item2', 'item3'];

        $scope.animationsEnabled = true;

        $scope.open = function (size) {

            var modalInstance = $uibModal.open({
                animation: $scope.animationsEnabled,
                templateUrl: 'myModalContent.html',
                controller: 'ModalInstanceCtrl',
                size: size,
                resolve: {
                    items: function () {
                        return $scope.items;
                    }
                }
            });

            modalInstance.result.then(function (selectedItem) {
                $scope.selected = selectedItem;
            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });
        };

        $scope.toggleAnimation = function () {
            $scope.animationsEnabled = !$scope.animationsEnabled;
        };

        $scope.submitRating = function () {
            //console.log("userSession.selectedStars " + userSession.selectedStars);
            /*var size = 'lg';
            var modalInstance = $uibModal.open({
                animation: $scope.animationsEnabled,
                templateUrl: 'myModalContent.html',
                controller: 'ModalInstanceCtrl',
                size: size,
                resolve: {
                    items: function () {
                        return $scope.items;
                    }
                }
            });

            modalInstance.result.then(function (selectedItem) {
                $scope.selected = selectedItem;
            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });*/

            var url = ServerContextPath.empty + '/Rating/SubmitCompanyRating?cid=' + $scope.companyid + '&rating=' + userSession.selectedStars;
            var headers = {
                'Content-Type': 'application/json',
                'UTMZT': $.cookie('utmzt'),
                'UTMZK': $.cookie('utmzk'),
                'UTMZV': $.cookie('utmzv'),
                '_ga': $.cookie('_ga')
            };

            $.ajax({
                url: url,
                method: "GET",
                headers: headers
            }).done(function (data, status) {
                console.log(data);
                if (data.Status == "200") {
                    //showToastMessage("Success", "Thanks for submitting valuable rating.");
                    if (data.Payload != 0) {
                        $scope.companyDetails.averagerating = Math.round($scope.companyDetails.averagerating);
                        if ($scope.$root.$$phase != '$apply' && $scope.$root.$$phase != '$digest') {
                            $scope.$apply();
                        }
                        showToastMessage("Success", "Your Rating is Submitted.");
                    }
                    
                }
                else if(data.Status == "201" ) {
                    showToastMessage("Warning", "Your Rating is already saved.");
                }
                else if (data.Status == "401") {
                    showToastMessage("Error", "Can be changed only within 2 days of submission.");
                }
            });
        };

        function getUserRatingStatus() {
            var url = ServerContextPath.empty + '/Rating/GetUserRatingStatus?cid=' + $scope.companyid;
            var headers = {
                'Content-Type': 'application/json',
                'UTMZT': $.cookie('utmzt'),
                'UTMZK': $.cookie('utmzk'),
                'UTMZV': $.cookie('utmzv'),
                '_ga': $.cookie('_ga')
            };

            $.ajax({
                url: url,
                method: "GET",
                headers: headers
            }).done(function (data, status) {
                console.log(data);
                //$scope.showUserInputRatingStar = true;
                if (data.Status == "200") {
                    //showToastMessage("Success", data.Message);
                    
                    $scope.userRatingData = data.Payload;                    
                    //console.log(data.Paylod);
                }
                

                if ($scope.$root.$$phase != '$apply' && $scope.$root.$$phase != '$digest') {
                    $scope.$apply();
                }
            });
        }

    });

    function isValidEmailAddress(emailAddress) {
        var pattern = new RegExp(/^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$/i);
        return pattern.test(emailAddress);
    };

    function isValidFormField(emailAddress) {
        var pattern = new RegExp(/^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$/i);
        return pattern.test(emailAddress);
    };
    
});



			

