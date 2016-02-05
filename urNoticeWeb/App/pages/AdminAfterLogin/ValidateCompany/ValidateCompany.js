'use strict';
define([appLocation.adminPostLogin], function (app) {

    app.controller('AdminAfterLoginValidateCompany', function ($scope, $http, $route, $rootScope, $upload, $routeParams, CookieUtil) {
        $scope.logoImage = { url: logoImage };
        //showToastMessage("Error", "Title of the Template cann't be empty");
        $('title').html("index"); //TODO: change the title so cann't be tracked in log

        $scope.NewCompany = {
            isPrimaryCompany:true,
            parentCompanyName: "",
            parentCompanyId:"",
            companyName:"",
            website: "",
            headquarters: "",
            size: "",
            founded: "",
            type: "",
            industry: "",
            revenue: "",
            descriptions: "",
            mission: "",
            founder: "",            
            logoUrl: "",
            squareLogoUrl: "",
            specialities: "",
            telephone:""

        };
        $scope.companyName = "";

        $scope.addCompany = function (selected) {
            console.log(selected);
            $scope.NewCompany.parentCompanyId = selected.originalObject.companyid;
            $scope.selectedParentCompany = selected.originalObject.companyname;
            $scope.selectedParentLogo = selected.originalObject.squarelogourl;

            $scope.NewCompany.website = selected.originalObject.website;
            $scope.NewCompany.squareLogoUrl = selected.originalObject.squarelogourl;
            $scope.NewCompany.logoUrl = selected.originalObject.logourl;
            $scope.NewCompany.universalName = selected.originalObject.companyname;            

            //$scope.$apply();
            if ($scope.$root.$$phase != '$apply' && $scope.$root.$$phase != '$digest') {
                $scope.$apply();
            }

            console.log($scope.NewCompany.parentCompanyId);
        };

        $scope.FetchAndLoadCompanyDataToDB = function getBeforeLoginUserProjectDetails() {

            if ((!$scope.NewCompany.isPrimaryCompany) && ($scope.NewCompany.parentCompanyId == null || $scope.NewCompany.parentCompanyId == "")) {
                showToastMessage("Error", "Please select primary company before submitting !");
                return;
            }
            if ($scope.FetchAndLoadCompanyDataToDBModel == null) {
                showToastMessage("Error", "FetchAndLoadCompanyDataToDBModel cannot be empty !");
                return;
            }

            var url = ServerContextPath.empty + '/Admin/FetchAndLoadCompanyDataToDB?cid=' + $scope.FetchAndLoadCompanyDataToDBModel + '&isPrimaryCompany=' + $scope.NewCompany.isPrimaryCompany + '&parentCompanyId=' + $scope.NewCompany.parentCompanyId;
            var headers = {
                'Content-Type': 'application/json',
                'UTMZT': $.cookie('utmzt'),
                'UTMZK': $.cookie('utmzk'),
                'UTMZV': $.cookie('utmzv')
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
                    showToastMessage("Success", data.Message);
                }
                else{
                    showToastMessage("Warning", data.Message);
                }
            });
        };

        $scope.FetchAndLoadCompanyDataToDBFromTo = function getBeforeLoginUserProjectDetails() {

            if ((!$scope.NewCompany.isPrimaryCompany) && ($scope.NewCompany.parentCompanyId == null || $scope.NewCompany.parentCompanyId == "")) {
                showToastMessage("Error", "Please select primary company before submitting !");
                return;
            }
            if ($scope.FetchAndLoadCompanyDataToDBModelFrom == null && $scope.FetchAndLoadCompanyDataToDBModelTo == null) {
                showToastMessage("Error", "FetchAndLoadCompanyDataToDBModelFrom To cannot be empty !");
                return;
            }

            var url = ServerContextPath.empty + '/Admin/FetchAndLoadCompanyDataToDbFromTo?cid=' + $scope.FetchAndLoadCompanyDataToDBModel + '&isPrimaryCompany=' + $scope.NewCompany.isPrimaryCompany + '&parentCompanyId=' + $scope.NewCompany.parentCompanyId + '&fromCompanyId=' + $scope.FetchAndLoadCompanyDataToDBModelFrom + '&toCompanyId=' + $scope.FetchAndLoadCompanyDataToDBModelTo;
            var headers = {
                'Content-Type': 'application/json',
                'UTMZT': $.cookie('utmzt'),
                'UTMZK': $.cookie('utmzk'),
                'UTMZV': $.cookie('utmzv')
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
                    showToastMessage("Success", data.Message);
                }
                else {
                    showToastMessage("Warning", data.Message);
                }
            });
        };

        $scope.SubmitNewCompanyToServer = function () {

            if ($scope.details == null || $scope.details == undefined) {
                showToastMessage("Error", "Please Fill compulsory fields");
                return;
            }
            console.log("$scope.companyName : " + $scope.companyName);
            console.log($scope.details.geometry.location);
            
            $scope.NewCompany.companyName = $scope.NewCompany.parentCompanyName;
            var newDesignationData = {
                companyDetails: $scope.NewCompany,
                location: $scope.details.address_components,
                formatted_address: $scope.details.formatted_address,
                //geolocation: $scope.details.geometry.viewport,
                geolocation:$scope.details.geometry.location,
                ImgurList: userSession.imgurImageTemplateModeratingPhotos,
            };


            var url = ServerContextPath.empty + '/Admin/AddNewCompany';
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
                data: newDesignationData,
                headers: headers
            }).success(function(data, status, headers, config) {
                //$scope.persons = data; // assign  $scope.persons here as promise is resolved here
                stopBlockUI();
                if (data.Status == "200") {
                    showToastMessage("Success", data.Message);
                    userSession.listOfImgurImages = [];
                } else if (data.Status == "202") {
                    showToastMessage("Warning", data.Message);

                } else if (data.Status == "500") {
                    showToastMessage("Error", data.Message);

                }
            }).error(function(data, status, headers, config) {

            });


        };

        $scope.UpdateCompanySolr = function () {

            console.log("UpdateCompanySolr");
            var url = ServerContextPath.empty + '/Admin/LoadCompanyDataFromMySqlToSolr';
            var headers = {
                'Content-Type': 'application/json',
                'UTMZT': CookieUtil.getUTMZT(),
                'UTMZK': CookieUtil.getUTMZK(),
                'UTMZV': CookieUtil.getUTMZV()
            };
            startBlockUI('wait..', 3);
            $http({
                url: url,
                method: "GET",                
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

        $scope.onFileSelectLogoUrl = function ($files) {

            startBlockUI('wait..', 3);
            //$files: an array of files selected, each file has name, size, and type.
            for (var i = 0; i < $files.length; i++) {
                var file = $files[i];
                $scope.upload = $upload.upload({
                    url: '/Upload/UploadAngularFileOnImgUr', //UploadAngularFileOnImgUr                    
                    data: { myObj: $scope.myModelObj },
                    file: file, // or list of files ($files) for html5 only                    
                }).progress(function (evt) {
                    console.log('percent: ' + parseInt(100.0 * evt.loaded / evt.total));
                }).success(function (data, status, headers, config) {

                    stopBlockUI();

                    $scope.NewCompany.squareLogoUrl = data.data.link_s;

                });

            }

        };

        $scope.onFileSelectSquareLogoUrl = function ($files) {

            startBlockUI('wait..', 3);
            //$files: an array of files selected, each file has name, size, and type.
            for (var i = 0; i < $files.length; i++) {
                var file = $files[i];
                $scope.upload = $upload.upload({
                    url: '/Upload/UploadAngularFileOnImgUr', //UploadAngularFileOnImgUr                    
                    data: { myObj: $scope.myModelObj },
                    file: file, // or list of files ($files) for html5 only                    
                }).progress(function (evt) {
                    console.log('percent: ' + parseInt(100.0 * evt.loaded / evt.total));
                }).success(function (data, status, headers, config) {

                    stopBlockUI();

                    $scope.NewCompany.logoUrl = data.data.link_s;

                });

            }

        };

        $scope.result = '';
        //    $scope.details = ''
        $scope.options = {};

        $scope.form = {
            type: 'geocode',
            //bounds: { SWLat: 49, SWLng: -97, NELat: 50, NELng: -96 },
            //country: 'ca',
            typesEnabled: false,
            boundsEnabled: false,
            componentEnabled: false,
            watchEnter: true
        };

        //watch form for changes
        $scope.watchForm = function () {
            //showToastMessage("Success", "Pressed Enter");
            return $scope.form;
        };
        $scope.$watch($scope.watchForm, function () {
            $scope.checkForm();
        }, true);


        //set options from form selections
        $scope.checkForm = function () {

            $scope.options = {};

            $scope.options.watchEnter = $scope.form.watchEnter;

            //showToastMessage("Success", "Pressed Enter");
            /*if ($scope.form.typesEnabled) {
                $scope.options.types = $scope.form.type
            }
            if ($scope.form.boundsEnabled) {

                var SW = new google.maps.LatLng($scope.form.bounds.SWLat, $scope.form.bounds.SWLng)
                var NE = new google.maps.LatLng($scope.form.bounds.NELat, $scope.form.bounds.NELng)
                var bounds = new google.maps.LatLngBounds(SW, NE);
                $scope.options.bounds = bounds

            }
            if ($scope.form.componentEnabled) {
                $scope.options.country = $scope.form.country
            }*/
        };

    });

});

