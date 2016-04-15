'use strict';
define([appLocation.preLogin], function (app) {

    app.controller('beforeLoginViewPostDetail', function ($scope, $http, $upload, $timeout,$location, $routeParams, $rootScope, CookieUtil) {
        $('title').html("edit page"); //TODO: change the title so cann't be tracked in log

        _.defer(function () { $scope.$apply(); });
        $scope.postVertexId = $routeParams.vertexId;
        var messagesPerCall = 5;
        $scope.UserPostLikesPerCall = 5;

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
        
        $scope.closeModelAndNavigateTo = function (vid) {
            //console.log("inside closeModelAndNavigateTo");  
            $(".modal-backdrop.in").hide();
            $('#closeModalId').click();
            $location.url('/userprofile/' + vid);
            //window.location.href = "/#/userprofile/" + vid;

            //alert("Navigation not implemented yet.");
            //console.log("#/userprofile/" + vid);
        };

        $scope.showLikedByUsers = function (postVertexId) {
            $scope.UserPostLikes = [];
            $scope.UserPostLikesFrom = 0;

            $scope.UserPostLikesTo = $scope.UserPostLikesFrom + $scope.UserPostLikesPerCall - 1;

            $scope.UserPostLikesCurrentPostVertexId = postVertexId;
            showLikedByUsersOnUserPost(postVertexId, $scope.UserPostLikesFrom, $scope.UserPostLikesTo);

        };

        $scope.showMoreLikedByUsers = function () {

            $scope.UserPostLikesFrom = $scope.UserPostLikesTo + 1;
            $scope.UserPostLikesTo = $scope.UserPostLikesFrom + $scope.UserPostLikesPerCall - 1;
            showLikedByUsersOnUserPost($scope.UserPostLikesCurrentPostVertexId, $scope.UserPostLikesFrom, $scope.UserPostLikesTo);

        };

        $scope.loadMoreMessage = function (postVerexId, postIndex) {
            
            $scope.UserPostList[postIndex].messageFromIndex = $scope.UserPostList[postIndex].messageToIndex + 1;
            $scope.UserPostList[postIndex].messageToIndex = $scope.UserPostList[postIndex].messageFromIndex + messagesPerCall - 1;

            loadMoreMessage(postVerexId, postIndex, $scope.UserPostList[postIndex].messageFromIndex, $scope.UserPostList[postIndex].messageToIndex);            
        };

        function showLikedByUsersOnUserPost(vertexId, from, to) {
            var url = ServerContextPath.userServer + '/User/GetUserPostLikes?from=' + from + '&to=' + to + '&vertexId=' + vertexId;
            var headers = {
                'Content-Type': 'application/json',
                'UTMZT': $.cookie('utmzt'),
                'UTMZK': $.cookie('utmzk'),
                'UTMZV': $.cookie('utmzv'),
            };
            //startBlockUI('wait..', 3);  
            $scope.UserPostLikesLoading = true;
            $.ajax({
                url: url,
                method: "GET",
                headers: headers
            }).done(function (data, status) {
                //stopBlockUI();
                //console.log(data.results);
                $scope.UserPostLikesLoading = false;
                for (var i = 0; i < data.results.length; i++) {
                    $scope.UserPostLikes.push(data.results[i]);
                }
                //$scope.UserPostLikes = data.results;
                if ($scope.$root.$$phase != '$apply' && $scope.$root.$$phase != '$digest') {
                    $scope.$apply();
                }
            });
        };

        function loadMoreMessage(vertexId, postIndex, from, to) {
            var url = ServerContextPath.userServer + '/User/GetUserPostMessages?from=' + from + '&to=' + to + '&vertexId=' + vertexId;
            var headers = {
                'Content-Type': 'application/json',
                'UTMZT': $.cookie('utmzt'),
                'UTMZK': $.cookie('utmzk'),
                'UTMZV': $.cookie('utmzv'),
            };
            //startBlockUI('wait..', 3);
            $scope.UserPostList[postIndex].loadingIcon = true;
            $.ajax({
                url: url,
                method: "GET",
                headers: headers
            }).done(function (data, status) {
                //stopBlockUI();
                //console.log(data.results);
                $scope.UserPostList[postIndex].loadingIcon = false;
                $scope.$apply(function () {
                    for (var i = 0; i < data.results.length; i++) {
                        $scope.UserPostList[postIndex].commentsInfo.push(data.results[i]);
                    }

                });

            });
        };

        function createNewReactionOnUserPost(postIndex) {

            var userPostCommentData = {
                Reaction: UserReaction.Like,                
                VertexId: $scope.UserPostList[postIndex].postInfo._id,
                WallVertexId: $scope.UserPostList[postIndex].postedToUser[0]._id,
                PostPostedByVertexId: $scope.UserPostList[postIndex].userInfo[0]._id
            };

            var url = ServerContextPath.empty + '/User/UserReactionOnPost';
            var headers = {
                'Content-Type': 'application/json',
                'UTMZT': $.cookie('utmzt'),
                'UTMZK': $.cookie('utmzk'),
                'UTMZV': $.cookie('utmzv'),
            };
            if ($rootScope.isUserLoggedIn) {
                //startBlockUI('wait..', 3);
                $scope.UserPostList[postIndex].alreadyLiked = true;
                $scope.UserPostList[postIndex].likeInfoHtml = appentToCommentLikeString($scope.UserPostList[postIndex].likeInfoHtml);
                $http({
                    url: url,
                    method: "POST",
                    data: userPostCommentData,
                    headers: headers
                }).success(function (data, status, headers, config) {
                    //$scope.persons = data; // assign  $scope.persons here as promise is resolved here
                    //stopBlockUI();
                    //getPostByVertexId();
                    //$scope.UserPostMessage = "";
                    //$scope.UserPostList[postIndex].alreadyLiked = true;
                    //$scope.UserPostList[postIndex].likeInfoHtml = appentToCommentLikeString($scope.UserPostList[postIndex].likeInfoHtml);
                    //$timeout(function () {
                    //    $scope.NewPostImageUrl.link_s = "";
                    //});

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

            var newCommentPosted = {
                "commentInfo": {
                    "PostImage": $scope.NewPostImageUrl.link_m,
                    "PostedByUser": $rootScope.clientDetailResponse.Email,
                    "PostedTime": new Date($.now()),
                    "PostMessage": $scope.UserPostList[postIndex].postInfo.postUserComment,
                    "_id": "",
                    "_type": null
                },
                "commentedBy": [
                    {
                        "FirstName": $rootScope.clientDetailResponse.Firstname,
                        "LastName": $rootScope.clientDetailResponse.Lastname,
                        "Username": $rootScope.clientDetailResponse.Email,
                        "Gender": $rootScope.clientDetailResponse.Gender,
                        "CreatedTime": new Date($.now()),
                        "ImageUrl": $rootScope.clientDetailResponse.Profilepic,
                        "CoverImageUrl": $rootScope.clientDetailResponse.Coverpic,
                        "_id": $rootScope.clientDetailResponse.VertexId,
                        "_type": null
                    }
                ]
            };

            var url = ServerContextPath.empty + '/User/UserCommentOnPost';
            var headers = {
                'Content-Type': 'application/json',
                'UTMZT': $.cookie('utmzt'),
                'UTMZK': $.cookie('utmzk'),
                'UTMZV': $.cookie('utmzv'),
            };
            if ($rootScope.isUserLoggedIn) {
                //startBlockUI('wait..', 3);
                var commentsNewList = [];
                commentsNewList.push(newCommentPosted);
                $scope.UserPostList[postIndex].postInfo.postUserComment = "";

                for (var i = 0; i < $scope.UserPostList[postIndex].commentsInfo.length; i++) {
                    commentsNewList.push($scope.UserPostList[postIndex].commentsInfo[i]);
                }

                $scope.UserPostList[postIndex].commentsInfo = commentsNewList;
                $scope.UserPostList[postIndex].commentsInfo[0].loadingIcon = true;

                $http({
                    url: url,
                    method: "POST",
                    data: userPostCommentData,
                    headers: headers
                }).success(function(data, status, headers, config) {
                    //$scope.persons = data; // assign  $scope.persons here as promise is resolved here
                    //stopBlockUI();
                    //getPostByVertexId();
                    $scope.UserPostMessage = "";

                    $scope.UserPostList[postIndex].commentsInfo[0].loadingIcon = false;
                    $scope.userPostCommentData = "";

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
                        $scope.UserPostList[i].messageFromIndex = 0;
                        $scope.UserPostList[i].messageToIndex = $scope.UserPostList[i].messageFromIndex + messagesPerCall - 1;
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

        function parseCommentLikeString(likeInfo, likeInfoCount) {
            var str = "";
            //console.log("likeInfo " + likeInfo);
            for (var i = 0; i < likeInfo.length; i++) {
                str += " " + likeInfo[i].FirstName + " " + likeInfo[i].LastName;
                if (i != likeInfo.length - 1) {
                    str += ",";
                } else {
                    str += " ";
                }
            }
            if (likeInfoCount > 2) {
                str += "and " + (likeInfoCount - likeInfo.length) + " more liked this";
            } else {
                str += " liked this";
            }
            return str;
        };

        function appentToCommentLikeString(str) {
            if (str == null) str = "";
            str = "<a href='#/userprofile/" + $rootScope.clientDetailResponse.VertexId + "'>" + $rootScope.clientDetailResponse.Firstname + " " + $rootScope.clientDetailResponse.Lastname + "</a>," + str;
            return str;
        };

    });

      
});

