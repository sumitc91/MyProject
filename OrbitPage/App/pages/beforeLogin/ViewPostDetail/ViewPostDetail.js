'use strict';
define([appLocation.preLogin], function (app) {

    app.controller('beforeLoginViewPostDetail', function ($scope, $http, $upload, $timeout, $routeParams, $rootScope, CookieUtil) {
        $('title').html("edit page"); //TODO: change the title so cann't be tracked in log

        _.defer(function () { $scope.$apply(); });
        $scope.postVertexId = $routeParams.vertexId;

        $scope.CurrentUserDetails = {};
        $scope.UserPostList = [];
        
        getPostByVertexId();

        

        $scope.commentOnUserPost = function (postIndex) {
            console.log($scope.UserPostList[postIndex].postInfo._id, $scope.UserPostList[postIndex].postInfo.postUserComment);
            createNewMessageOnUserPost(postIndex);
            //createNewMessageOnUserPost($scope.UserPostList[postIndex].postInfo._id, $scope.UserPostList[postIndex].postInfo.postUserComment);
        };

        $scope.NewPostImageUrl = {
            //link_s:"https://s3-ap-southeast-1.amazonaws.com/urnotice/OrbitPage/User/Sumit/WallPost/9ac2bfce-a1eb-4a51-9f18-ad5591a72cc0.png"
        };
        
        function createNewMessageOnUserPost(postIndex) {

            //var userPostData = {
            //    Message: $scope.UserPostMessage,
            //    Image: $scope.NewPostImageUrl.link_m,                
            //    vertexId: $scope.visitedUserVertexId
            //};

            var url = ServerContextPath.userServer + '/User/UserCommentOnPost?message=' + $scope.UserPostList[postIndex].postInfo.postUserComment + '&image=' + '' + '&vertexId=' + $scope.UserPostList[postIndex].postInfo._id + '&wallVertexId=' + $scope.UserPostList[postIndex].postedToUser[0]._id + '&postPostedByVertexId=' + $scope.UserPostList[postIndex].userInfo[0]._id;
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
                getPostByVertexId();
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

        
        function getPostByVertexId() {
            var url = ServerContextPath.userServer + '/User/GetPostByVertexId?vertexId=' + $scope.postVertexId;
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
                    $scope.UserPostList = data.results;
                    console.log($scope.UserPostList);
                });

            });
        };

    });

      
});

