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
            //console.log($scope.UserPostList[postIndex].postInfo._id, $scope.UserPostList[postIndex].postInfo.postUserComment);
            createNewMessageOnUserPost(postIndex);
            //createNewMessageOnUserPost($scope.UserPostList[postIndex].postInfo._id, $scope.UserPostList[postIndex].postInfo.postUserComment);
        };

        $scope.reactionOnUserPost = function (postIndex) {
            //console.log(postIndex);
            createNewReactionOnUserPost(postIndex);
            //createNewMessageOnUserPost($scope.UserPostList[postIndex].postInfo._id, $scope.UserPostList[postIndex].postInfo.postUserComment);
        };

        $scope.NewPostImageUrl = {
            //link_s:"https://s3-ap-southeast-1.amazonaws.com/urnotice/OrbitPage/User/Sumit/WallPost/9ac2bfce-a1eb-4a51-9f18-ad5591a72cc0.png"
        };
        
        function createNewReactionOnUserPost(postIndex) {

            var userPostCommentData = {
                Reaction: UserReaction.Like,                
                VertexId: $scope.UserPostList[postIndex].postInfo._id,
                WallVertexId: $scope.UserPostList[postIndex].postedToUser[0]._id,
                PostPostedByVertexId: $scope.UserPostList[postIndex].userInfo[0]._id
            };

            var url = ServerContextPath.empty + '/User/UserCommentOnPost';
            var headers = {
                'Content-Type': 'application/json',
                'UTMZT': $.cookie('utmzt'),
                'UTMZK': $.cookie('utmzk'),
                'UTMZV': $.cookie('utmzv'),
            };
            if ($rootScope.isUserLoggedIn) {
                startBlockUI('wait..', 3);
                $http({
                    url: url,
                    method: "POST",
                    data: userPostCommentData,
                    headers: headers
                }).success(function (data, status, headers, config) {
                    //$scope.persons = data; // assign  $scope.persons here as promise is resolved here
                    stopBlockUI();
                    //getPostByVertexId();
                    $scope.UserPostMessage = "";
                    $scope.UserPostList[postIndex].alreadyLiked = true;
                    $scope.UserPostList[postIndex].likeInfoHtml = appentToCommentLikeString($scope.UserPostList[postIndex].likeInfoHtml);
                    $timeout(function () {
                        $scope.NewPostImageUrl.link_s = "";
                    });

                }).error(function (data, status, headers, config) {

                });
            } else {
                showToastMessage("Warning", "Please Login to Make your reaction on post.");
            }
            

        };

        function createNewMessageOnUserPost(postIndex) {

            var userPostCommentData = {
                Message: $scope.UserPostList[postIndex].postInfo.postUserComment,
                Image: $scope.NewPostImageUrl.link_m,
                VertexId: $scope.UserPostList[postIndex].postInfo._id,
                WallVertexId: $scope.UserPostList[postIndex].postedToUser[0]._id,
                PostPostedByVertexId: $scope.UserPostList[postIndex].userInfo[0]._id
            };

            var url = ServerContextPath.empty + '/User/UserCommentOnPost';
            var headers = {
                'Content-Type': 'application/json',
                'UTMZT': $.cookie('utmzt'),
                'UTMZK': $.cookie('utmzk'),
                'UTMZV': $.cookie('utmzv'),
            };
            if ($rootScope.isUserLoggedIn) {
                startBlockUI('wait..', 3);
                $http({
                    url: url,
                    method: "POST",
                    data: userPostCommentData,
                    headers: headers
                }).success(function(data, status, headers, config) {
                    //$scope.persons = data; // assign  $scope.persons here as promise is resolved here
                    stopBlockUI();
                    getPostByVertexId();
                    $scope.UserPostMessage = "";

                    $timeout(function() {
                        $scope.NewPostImageUrl.link_s = "";
                    });

                }).error(function(data, status, headers, config) {

                });
            } else {
                showToastMessage("Warning", "Please Login to reply on post.");
            }
            

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
                    if ($scope.UserPostList != null && $scope.UserPostList.length > 0) {
                        var i = 0; // only 1 post available.
                        $scope.UserPostList[i].likeInfoHtml = parseCommentLikeString($scope.UserPostList[i].likeInfo);
                        if ($scope.UserPostList[i].isLiked != null && $scope.UserPostList[i].isLiked.length > 0) {
                            $scope.UserPostList[i].alreadyLiked = true;
                        } else {
                            $scope.UserPostList[i].alreadyLiked = false;
                        }
                        //console.log($scope.UserPostList);
                    } else {
                        showToastMessage("Warning", "Post Not found.");
                    }
                });

            });
        };

        function parseCommentLikeString(likeInfo) {
            var str = "";
            //console.log("str -1 " + str);
            for (var i = 0; i < likeInfo.length; i++) {
                if (i <= 5)
                    str += "<a href='#/userprofile/" + likeInfo[i]._id + "'>" + likeInfo[i].FirstName + "</a>,";

            }
            str += "...";
            return str;
        };

        function appentToCommentLikeString(str) {
            if (str == null) str = "";
            str = "<a href='#/userprofile/" + $rootScope.clientDetailResponse.VertexId + "'>" + $rootScope.clientDetailResponse.Firstname + "</a>," + str;
            return str;
        };

    });

      
});

