'use strict';
define([appLocation.preLogin], function (app) {
    app.controller('beforeLoginSearch', function ($scope, $http, $rootScope,$routeParams,$location, Restangular, CookieUtil) {
        $('title').html("indexsea"); //TODO: change the title so cann't be tracked in log
        
        $scope.queryParam = {
            q: "",
            currentPage: "",
            totalMatch: "",
            perpage: "",
            totalNumberOfPages:""
        };

        $scope.pagination = {
            show: false,
            maxSize: 5,

        };
        /*$scope.queryParam.q = getParameterByName('q');
        $scope.queryParam.currentPage = getParameterByName('page');
        $scope.queryParam.totalMatch = getParameterByName('totalMatch');
        $scope.queryParam.perpage = getParameterByName('perpage');*/

        $scope.queryParam.q = $location.search().q;
        $scope.queryParam.currentPage = $location.search().page;
        $scope.queryParam.totalMatch = $location.search().totalMatch;
        $scope.queryParam.perpage = $location.search().perpage;
        
        console.log($scope.queryParam);

        $scope.maxSize = 5;
           
        if ($scope.queryParam.q != null)
            getCompanySearchDetail();

        function getCompanySearchDetail() {
            var url = ServerContextPath.empty + '/Solr/Search?q=' + $scope.queryParam.q + '&page=' + $scope.queryParam.currentPage + '&perpage=' + $scope.queryParam.perpage + '&totalMatch=' + $scope.queryParam.totalMatch;
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
                    $scope.searchResult = data.Payload.result;
                    $scope.queryParam.totalMatch = data.Payload.count;
                    $scope.queryParam.totalNumberOfPages = Math.ceil((data.Payload.count / $scope.queryParam.perpage));

                    $.each(data.Payload.result, function (i, val) {
                        $scope.searchResult[i].companyname = data.Payload.result[i].companyname;
                        $scope.searchResult[i].website = data.Payload.result[i].website;

                        if ($scope.searchResult[i].logourl == 'tps://s3-ap-southeast-1.amazonaws.com/urnotice/company/small/LogoUploadEmpty.png')
                            $scope.searchResult[i].logourl = "http://placehold.it/50x50";

                        $scope.searchResult[i].linkurl = "/#companydetails/" + $scope.searchResult[i].companyname.replace(/ /g, "_").replace(/\//g, "_OR_") + "/" + $scope.searchResult[i].guid;
                        $scope.pagination.show = true;
                    });

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

        $scope.selectCompany = function (selected) {
            console.log(selected);            
            location.href = "/#companydetails/" + selected.originalObject.companyname.replace(/ /g, "_").replace(/\//g, "_OR_") + "/" + selected.originalObject.guid;
        };

        $scope.searchCompany = function() {
            location.href = "/#search/?q=" + $("#companyName_value").val() + "&page=1&perpage=10";
        };

        $scope.setPage = function (pageNo) {
            $scope.currentPage = pageNo;

        };

        $scope.myFunct = function (keyEvent) {
            if (keyEvent.which === 13) {
                location.href = "/#search/?q=" + $("#companyName_value").val() + "&page=1&perpage=10";
            }
        }

        $scope.SearchPageSelectPaginationId = function () {
            console.log("/#search/?q=" + $scope.queryParam.q + "&page=" + $scope.queryParam.currentPage + "&perpage=10&totalMatch=" + $scope.queryParam.totalMatch + "");
            //getCompanySearchDetail(q, $scope.currentPage, perpage);
            //getCompanySearchDetail();
            console.log(userSession.selectedPagePagination);
            if (userSession.selectedPagePagination != '...')
                location.href = "/#search/?q=" + $scope.queryParam.q + "&page=" + $scope.queryParam.currentPage + "&perpage=10&totalMatch=" + $scope.queryParam.totalMatch + "";
        };
        
    });

    jQuery(".block-text .success-inner-content").each(function () {
        if (jQuery(this).text().length > 100) {
            var str = jQuery(this).text().substr(0, 98);
            var wordIndex = str.lastIndexOf(" ");

            jQuery(this).text(str.substr(0, wordIndex) + '..');
        }
    });
});



			

