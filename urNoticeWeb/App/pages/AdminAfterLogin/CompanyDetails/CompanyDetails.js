'use strict';
define([appLocation.adminPostLogin], function (app) {
    app.controller('adminAfterLoginCompanyDetails', function ($scope, $http, $route, $rootScope, $routeParams, $location, $timeout, CookieUtil) {
        $('title').html("indexcd"); //TODO: change the title so cann't be tracked in log
        
        $scope.isSolrUpdated = true;
        $scope.companyid = $routeParams.companyid;
        $scope.companyDetails = {
            
        };
        $scope.competitorDetails = [{}];
        if ($routeParams.companyid != null && $routeParams.companyid != undefined) {
            getCompanyDetails();
            checkIfSolrUpdated();
        }
            


        function getCompanyDetails() {
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
                    //$scope.$apply();
                    if ($scope.$root.$$phase != '$apply' && $scope.$root.$$phase != '$digest') {
                        $scope.$apply();
                    }
                    console.log($scope.companyDetails);
                    //getCompanyCompetitorsDetail($scope.companyDetails.size,$scope.companyDetails.rating,$scope.companyDetails.speciality);
                }
                else {
                    showToastMessage("Warning", data.Message);
                }
            });
        }

        $scope.UpdateCompanyData = function () {

            console.log("UpdateCompanyData");
            /*var updateCompanyDataObj = {
                updatedData: $scope.companyDetails
            };*/
            var url = ServerContextPath.empty + '/Admin/UpdateCompanyData';
            var headers = {
                'Content-Type': 'application/json',
                'UTMZT': CookieUtil.getUTMZT(),
                'UTMZK': CookieUtil.getUTMZK(),
                'UTMZV': CookieUtil.getUTMZV()
            };
            startBlockUI('wait..', 3);
            $http({
                url: url,
                method: "POST",
                data: $scope.companyDetails,
                headers: headers
            }).success(function (data, status, headers, config) {
                //$scope.persons = data; // assign  $scope.persons here as promise is resolved here
                stopBlockUI();
                if (data.Status == "200") {
                    showToastMessage("Success", data.Message);
                } else if (data.Status == "202") {
                    showToastMessage("Warning", data.Message);

                } else if (data.Status == "500") {
                    showToastMessage("Error", data.Message);

                }
            }).error(function (data, status, headers, config) {

            });


        };

        function checkIfSolrUpdated() {
            var url = ServerContextPath.empty + '/Admin/IsSolrUpdatedForCompanyId?cid=' + $scope.companyid;
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
                    $scope.isSolrUpdated = data.Payload;
                    //$scope.$apply();
                    if ($scope.$root.$$phase != '$apply' && $scope.$root.$$phase != '$digest') {
                        $scope.$apply();
                    }
                }
                else {
                    showToastMessage("Warning", data.Message);
                }
            });
        }

        $scope.selectCompany = function (selected) {
            console.log(selected);
            location.href = "#companydetails/" + selected.originalObject.companyname.replace(/ /g, "_").replace(/\//g, "_OR_") + "/" + selected.originalObject.guid;

        };
      
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



			

