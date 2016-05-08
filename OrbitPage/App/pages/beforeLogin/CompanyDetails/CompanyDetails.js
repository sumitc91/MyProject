'use strict';
define([appLocation.preLogin], function (app) {
    app.controller('ModalInstanceCtrl', function ($scope, $uibModalInstance, items) {

        $scope.items = items;
        $scope.userCompanyAnalyticsLoaded = false;
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

    google.charts.load('current', { packages: ['corechart', 'bar'] });

    app.run([
        '$route', '$rootScope', '$location', function($route, $rootScope, $location) {
            var original = $location.path;
            $location.path = function(path, reload) {
                if (reload === false) {
                    var lastRoute = $route.current;
                    var un = $rootScope.$on('$locationChangeSuccess', function() {
                        $route.current = lastRoute;
                        un();
                    });
                }
                return original.apply($location, [path]);
            };
        }
    ]);

    app.controller('beforeLoginCompanyDetails', function ($scope, $http, $route,$uibModal,$log, $rootScope, $routeParams, $location, $timeout, CookieUtil) {
        $('title').html("indexcd"); //TODO: change the title so cann't be tracked in log
        
        //$rootScope.sitehosturl = $location.host();//"localhost:40287";
        //if ($location.host() == "localhost") {
        //    $rootScope.sitehosturl = "localhost:40287";
        //}

        $rootScope.sitehosturl = "www.orbitpage.com/searchapi";
        $scope.ratingCount = 0;
        $scope.companyid = $routeParams.companyid;
        $scope.tabid = $routeParams.tabid;
        $scope.companyDetails = {
            
        };

        $scope.companyActiveTab = {
            Profile: true,
            Salary: false,
            NoticePeriod: false,
            Workgraphy: false
        };

        if (isNullOrEmpty($scope.tabid)) {
            $scope.companyActiveTab.Profile=true;
        } else {
            if ($scope.tabid == "Profile") {
                $scope.companyActiveTab.Profile=true;
            }
            else if ($scope.tabid == "Salary") {
                $scope.companyActiveTab.Salary = true;
            }
            else if ($scope.tabid == "NoticePeriod") {
                $scope.companyActiveTab.NoticePeriod = true;
            }
            else if ($scope.tabid == "Workgraphy") {
                $scope.companyActiveTab.Workgraphy = true;
            }
        }

        $scope.competitorDetails = [{}];
        CompanyDetailsById();

        function CompanyDetailsById() {
            var url = ServerContextPath.solrServer + '/Search/CompanyDetailsById?cid=' + $scope.companyid;
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
                //console.log(data);
                if (data.Status == "200") {
                    //showToastMessage("Success", data.Message);
                    $scope.companyDetails = data.Payload[0];
                    if ($scope.companyDetails.logourl == 'tps://s3-ap-southeast-1.amazonaws.com/urnotice/company/small/LogoUploadEmpty.png' || $scope.companyDetails.logourl == '')
                        $scope.companyDetails.logourl = "http://placehold.it/350x150";
                    $scope.averageRatingInDoubleFormat = $scope.companyDetails.averagerating;
                    $scope.companyDetails.averagerating = Math.round($scope.companyDetails.averagerating);
                    $scope.$apply();
                    showGoogleChart();
                    //console.log($scope.companyDetails);                    
                    getCompanyCompetitorsDetail($scope.companyDetails.size, $scope.companyDetails.rating, $scope.companyDetails.speciality);
                    companySalaryDetailsById();
                    companyNoticePeriodDetailsById();
                    companyWorkgraphyDetailsById();
                }
                else {
                    showToastMessage("Warning", data.Message);
                }
            });

        }
        
        function companySalaryDetailsById() {
            var url = ServerContextPath.userServer + '/User/GetCompanySalaryInfo?from=0&to=2&vertexId=' + $scope.companyid;
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
                //console.log(data);
                $scope.CompanySalaryDetails = data.results;
                
            });

        }

        function companyNoticePeriodDetailsById() {
            var url = ServerContextPath.userServer + '/User/GetCompanyNoticePeriodInfo?from=0&to=2&vertexId=' + $scope.companyid;
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
                //console.log(data);
                $scope.CompanyNoticePeriodDetails = data.results;

            });

        }

        function companyWorkgraphyDetailsById() {
            var url = ServerContextPath.userServer + '/User/GetCompanyWorkgraphyInfo?from=0&to=10&vertexId=' + $scope.companyid;
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
                //console.log(data);
                $scope.CompanyWorkgraphyDetails = data.results[0].workgraphyInfo;
                $scope.countVisit = data.results[0].count;
                $scope.userCountVisit = data.results[0].userCount;
                $scope.userCompanyAnalyticsLoaded = true;
                if ($scope.$root.$$phase != '$apply' && $scope.$root.$$phase != '$digest') {
                    $scope.$apply();
                }
                
            });

        }

        function showGoogleChart() {
            //google.charts.load('current', { packages: ['corechart', 'bar'] });
            //google.charts.load('41', { packages: ['corechart', 'bar'] });
            google.charts.setOnLoadCallback(drawBasic);
        }
        function drawBasic() {

            var data = google.visualization.arrayToDataTable([
              ['Fields', 'Rating', ],
              ['Rating', $scope.companyDetails.rating],
              ['Work Life Balance', $scope.companyDetails.workLifeBalanceRating],
              ['Salary', $scope.companyDetails.salaryRating],
              ['Company Culture', $scope.companyDetails.companyCultureRating],
              ['Career Growth', $scope.companyDetails.careerGrowthRating]
            ]);

            var options = {
                title: 'Company Rating Scale',
                chartArea: { width: '50%' },
                hAxis: {
                    title: 'Rating',
                    minValue: 0
                },
                vAxis: {
                    title: 'Fields'
                }
            };

            var chart = new google.visualization.BarChart(document.getElementById('chart_div'));

            chart.draw(data, options);
        }
        

        //getUserRatingStatus();

        function getCompanyCompetitorsDetail(size, rating, speciality) {
            var url = ServerContextPath.solrServer + '/Search/GetCompanyCompetitorsDetail?size=' + size + '&rating=' + rating + '&speciality=' + speciality;
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
                //console.log(data);
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

        $scope.changeBrowserUrl = function (tabType) {
            
            $location.path('companydetails/' + $routeParams.companyName + '/' + $routeParams.companyid + '/' + tabType, false);            
            //console.log(tabType);
        };

        $scope.myFunct = function(keyEvent) {
            if (keyEvent.which === 13) {
                location.href = "/#search/?q=" + $("#companyName_value").val() + "&page=1&perpage=10";
            }
        };

        $scope.searchCompany = function() {
            location.href = "/#search/?q=" + $("#companyName_value").val() + "&page=1&perpage=10";
        };

        $scope.selectCompany = function (selected) {
            //console.log(selected);
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



			

