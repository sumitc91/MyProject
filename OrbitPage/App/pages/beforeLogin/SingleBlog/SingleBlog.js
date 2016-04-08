'use strict';
define([appLocation.preLogin], function (app) {
    app.controller('beforeLoginSingleBlog', function ($scope, $http,$routeParams, $rootScope, Restangular, CookieUtil) {
        $('title').html("index"); //TODO: change the title so cann't be tracked in log
        
        //detectIfUserLoggedIn();
        $scope.WorkgraphyVertexId = $routeParams.storyid;

        getParticularWorkgraphyWithVertexId();
        function getParticularWorkgraphyWithVertexId() {

            $scope.currentPage = 0;
            $scope.perpage = 10;
            $scope.totalMatch = 10;
            var url = ServerContextPath.solrServer + '/Search/GetParticularWorkgraphyWithVertexId?vertexId=' + $scope.WorkgraphyVertexId;
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
                //console.log(data);
                if (data.Status == "200") {
                    //showToastMessage("Success", data.Message);

                    $scope.totalMatch = data.Message;
                    
                    $scope.WorkGraphyDetail = data.Payload[0];
                    console.log($scope.WorkGraphyDetail);
                    if ($scope.$root.$$phase != '$apply' && $scope.$root.$$phase != '$digest') {
                        $scope.$apply();
                    }
                }
                else {
                    showToastMessage("Warning", data.Message);
                }
            });


        }
    });
});



			

