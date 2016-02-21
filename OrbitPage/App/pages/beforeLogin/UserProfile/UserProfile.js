'use strict';
define([appLocation.preLogin], function (app) {

    app.controller('beforeLoginUserProfile', function ($scope, $http, $upload, $timeout, $routeParams, $rootScope, CookieUtil) {
        $('title').html("edit page"); //TODO: change the title so cann't be tracked in log

        _.defer(function () { $scope.$apply(); });
        $scope.visitedUserVertexId = $routeParams.vertexId;

        $scope.CurrentUserDetails = {};
        $scope.UserPostList = [];
        $scope.UserPostListInfoAngular = {
            busy: false,
            after: 0,
            itemPerPage: 2
        };

        getUserInformation();
        //getUserPost($scope.UserPostListInfoAngular.after, $scope.UserPostListInfoAngular.after + $scope.UserPostListInfoAngular.itemPerPage);

        $scope.createNewUserPost = function () {
            createNewUserPost();
            $scope.UserPostListInfoAngular.after = 0;
            $scope.UserPostList = [];
        };

        $scope.commentOnUserPost = function (postIndex) {
            console.log($scope.UserPostList[postIndex].postInfo._id, $scope.UserPostList[postIndex].postInfo.postUserComment);
            createNewMessageOnUserPost(postIndex);
            //createNewMessageOnUserPost($scope.UserPostList[postIndex].postInfo._id, $scope.UserPostList[postIndex].postInfo.postUserComment);
        };

        $scope.UserPostListInfo = {};


        $scope.NewPostImageUrl = {
            //link_s:"https://s3-ap-southeast-1.amazonaws.com/urnotice/OrbitPage/User/Sumit/WallPost/9ac2bfce-a1eb-4a51-9f18-ad5591a72cc0.png"
        };
        function createNewUserPost() {

            //var userPostData = {
            //    Message: $scope.UserPostMessage,
            //    Image: $scope.NewPostImageUrl.link_m,                
            //    vertexId: $scope.visitedUserVertexId
            //};

            var url = ServerContextPath.userServer + '/User/UserPost?message=' + $scope.UserPostMessage + '&image=' + $scope.NewPostImageUrl.link_m+'&vertexId=' + $scope.visitedUserVertexId;
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
                getUserPost(0, $scope.UserPostListInfoAngular.after + $scope.UserPostListInfoAngular.itemPerPage);
                $scope.UserPostMessage = "";

                $timeout(function () {
                    $scope.NewPostImageUrl.link_s = "";
                });
                
            });
        };

        function createNewMessageOnUserPost(postIndex) {

            //var userPostData = {
            //    Message: $scope.UserPostMessage,
            //    Image: $scope.NewPostImageUrl.link_m,                
            //    vertexId: $scope.visitedUserVertexId
            //};

            var url = ServerContextPath.userServer + '/User/UserCommentOnPost?message=' + $scope.UserPostList[postIndex].postInfo.postUserComment + '&image=' + '' + '&vertexId=' + $scope.UserPostList[postIndex].postInfo._id + '&wallVertexId=' + $scope.visitedUserVertexId + '&postPostedByVertexId=' + $scope.UserPostList[postIndex].userInfo[0]._id;
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
                getUserPost(0, $scope.UserPostListInfoAngular.after + $scope.UserPostListInfoAngular.itemPerPage);
                $scope.UserPostMessage = "";

                $timeout(function () {
                    $scope.NewPostImageUrl.link_s = "";
                });

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
                        
                    $timeout(function () {
                        $scope.NewPostImageUrl = data.data;
                    });
                    
                        
                    
                });

            }

        };

        function getUserInformation() {
            var url = ServerContextPath.solrServer + '/Search/UserDetailsById?uid=' + $scope.visitedUserVertexId;
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
                $scope.$apply(function () {
                    $scope.CurrentUserDetails = data.Payload[0];
                    //console.log($scope.CurrentUserDetails);
                });
                
            });
        };

        function getUserPost(from,to) {
            var url = ServerContextPath.userServer + '/User/GetUserPost?from='+from+'&to='+to+'&vertexId=' + $scope.visitedUserVertexId;
            var headers = {
                'Content-Type': 'application/json',
                'UTMZT': $.cookie('utmzt'),
                'UTMZK': $.cookie('utmzk'),
                'UTMZV': $.cookie('utmzv'),
            };
            //startBlockUI('wait..', 3);
            $scope.UserPostListInfoAngular.busy = true;
            $.ajax({
                url: url,
                method: "GET",
                headers: headers
            }).done(function (data, status) {
                //stopBlockUI();
                $scope.UserPostListInfoAngular.busy = false;
                $scope.$apply(function () {
                    //$scope.UserPostList = data.results;

                    if ($scope.UserPostList != null) {
                        for (var i = 0; i < data.results.length; i++) {
                            $scope.UserPostList.push(data.results[i]);
                            //console.log($rootScope.clientNotificationDetailResponse);
                        }
                    }
                    else
                        $scope.UserPostList = data.results;

                    //console.log($scope.UserPostList);
                });

            });
        };

        $scope.UserPostListInfo.nextPage = function () {
            if ($scope.UserPostListInfoAngular.busy) return;
            $scope.UserPostListInfoAngular.busy = true;
            getUserPost($scope.UserPostListInfoAngular.after, $scope.UserPostListInfoAngular.after + $scope.UserPostListInfoAngular.itemPerPage);
            $scope.UserPostListInfoAngular.after = $scope.UserPostListInfoAngular.after + $scope.UserPostListInfoAngular.itemPerPage+1;
            console.log("UserPostListInfo.nextPage called.");

        };
    });

    
   
});

