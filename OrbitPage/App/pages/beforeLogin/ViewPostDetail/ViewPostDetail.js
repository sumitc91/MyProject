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
        $scope.visitedUserVertexId = 0;
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

        $scope.reactionOnUserPostComment = function (postIndex, commentIndex) {
            createNewReactionOnUserPostComment(postIndex, commentIndex);
        };

        $scope.removeReactionOnUserPost = function (postIndex) {
            removeReactionOnUserPost(postIndex);
            //createNewMessageOnUserPost($scope.UserPostList[postIndex].postInfo._id, $scope.UserPostList[postIndex].postInfo.postUserComment);
        };

        $scope.removeReactionOnUserPostComment = function (postIndex, commentIndex) {
            removeReactionOnUserPostComment(postIndex, commentIndex);
        };

        $scope.removeImageOnUserPost = function (postIndex) {
            $scope.UserPostList[postIndex].postInfo.OriginalPostImage = $scope.UserPostList[postIndex].postInfo.PostImage;
            $scope.UserPostList[postIndex].postInfo.PostImage = "";
            //console.log(postIndex);
            //console.log($scope.UserPostList[postIndex].postInfo.OriginalPostImage);
            if ($scope.$root.$$phase != '$apply' && $scope.$root.$$phase != '$digest') {
                $scope.$apply();
            }
        };

        $scope.removeImageOnUserPostComment = function (postIndex, commentIndex) {

            $scope.UserPostList[postIndex].commentsInfo[commentIndex].commentInfo.OriginalPostImage = $scope.UserPostList[postIndex].commentsInfo[commentIndex].commentInfo.PostImage;
            $scope.UserPostList[postIndex].commentsInfo[commentIndex].commentInfo.PostImage = "";
            //console.log(postIndex);
            //console.log($scope.UserPostList[postIndex].postInfo.OriginalPostImage);
            if ($scope.$root.$$phase != '$apply' && $scope.$root.$$phase != '$digest') {
                $scope.$apply();
            }
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
                    if (data.results != null && data.results.length > 0) {
                        data.results = reverseCommentsInfoList(data.results);
                        $scope.UserPostList[postIndex].commentsInfo = appendOldCommentsToCommentList($scope.UserPostList[postIndex].commentsInfo, data.results);
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
                $scope.UserPostList[postIndex].likeInfoHtml = appentToCommentLikeString($scope.UserPostList[postIndex].likeInfoHtml,$scope.UserPostList[postIndex].likeInfoCount);
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

        function createNewReactionOnUserPostComment(postIndex, commentIndex) {

            var userPostReactionData = {
                Reaction: UserReaction.Like,
                VertexId: $scope.UserPostList[postIndex].commentsInfo[commentIndex].commentInfo._id,
                WallVertexId: $scope.visitedUserVertexId,
                PostPostedByVertexId: $scope.UserPostList[postIndex].commentsInfo[commentIndex].commentedBy[0]._id
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
                $scope.UserPostList[postIndex].commentsInfo[commentIndex].alreadyLiked = true;
                $scope.UserPostList[postIndex].commentsInfo[commentIndex].likeCount = $scope.UserPostList[postIndex].commentsInfo[commentIndex].likeCount + 1;
                $http({
                    url: url,
                    method: "POST",
                    data: userPostReactionData,
                    headers: headers
                }).success(function (data, status, headers, config) {
                    //$scope.persons = data; // assign  $scope.persons here as promise is resolved here
                    //stopBlockUI();                    
                    //$scope.UserPostList[postIndex].likeInfoHtml = appentToCommentLikeString($scope.UserPostList[postIndex].likeInfoHtml);
                    //$timeout(function() {
                    //    $scope.NewPostImageUrl.link_s = "";
                    //});

                }).error(function (data, status, headers, config) {

                });
            } else {
                showToastMessage("Warning", "Please Login to Make your reaction on post.");
            }

        };

        function removeReactionOnUserPost(postIndex) {

            var userPostReactionData = {
                Reaction: UserReaction.Like,
                VertexId: $scope.UserPostList[postIndex].postInfo._id,
                WallVertexId: $scope.visitedUserVertexId,
                PostPostedByVertexId: $scope.UserPostList[postIndex].userInfo[0]._id
            };

            var url = ServerContextPath.empty + '/User/RemoveReactionOnPost?vertexId=' + $scope.UserPostList[postIndex].postInfo._id;
            var headers = {
                'Content-Type': 'application/json',
                'UTMZT': $.cookie('utmzt'),
                'UTMZK': $.cookie('utmzk'),
                'UTMZV': $.cookie('utmzv'),
            };

            if ($rootScope.isUserLoggedIn) {
                //startBlockUI('wait..', 3);
                $scope.UserPostList[postIndex].alreadyLiked = false;
                $scope.UserPostList[postIndex].likeInfoHtml = removeFromCommentLikeString($scope.UserPostList[postIndex].likeInfoHtml, $scope.UserPostList[postIndex].likeInfoCount);
                //$scope.UserPostList[postIndex].likeInfoCount = $scope.UserPostList[postIndex].likeInfoCount - 1;
                $http({
                    url: url,
                    method: "POST",
                    data: userPostReactionData,
                    headers: headers
                }).success(function (data, status, headers, config) {
                    //$scope.persons = data; // assign  $scope.persons here as promise is resolved here
                    //stopBlockUI();                    
                    //$scope.UserPostList[postIndex].likeInfoHtml = appentToCommentLikeString($scope.UserPostList[postIndex].likeInfoHtml);
                    //$timeout(function() {
                    //    $scope.NewPostImageUrl.link_s = "";
                    //});

                }).error(function (data, status, headers, config) {

                });
            } else {
                showToastMessage("Warning", "Please Login to Make your reaction on post.");
            }

        };

        function removeReactionOnUserPostComment(postIndex, commentIndex) {

            var userPostReactionData = {
                Reaction: UserReaction.Like,
                VertexId: $scope.UserPostList[postIndex].commentsInfo[commentIndex].commentInfo._id,
                WallVertexId: $scope.visitedUserVertexId,
                PostPostedByVertexId: $scope.UserPostList[postIndex].commentsInfo[commentIndex].commentedBy[0]._id
            };

            var url = ServerContextPath.empty + '/User/RemoveReactionOnPost?vertexId=' + $scope.UserPostList[postIndex].commentsInfo[commentIndex].commentInfo._id;
            var headers = {
                'Content-Type': 'application/json',
                'UTMZT': $.cookie('utmzt'),
                'UTMZK': $.cookie('utmzk'),
                'UTMZV': $.cookie('utmzv'),
            };

            if ($rootScope.isUserLoggedIn) {
                //startBlockUI('wait..', 3);
                $scope.UserPostList[postIndex].commentsInfo[commentIndex].alreadyLiked = false;
                $scope.UserPostList[postIndex].commentsInfo[commentIndex].likeCount = $scope.UserPostList[postIndex].commentsInfo[commentIndex].likeCount - 1;
                //$scope.UserPostList[postIndex].likeInfoCount = $scope.UserPostList[postIndex].likeInfoCount - 1;

                $http({
                    url: url,
                    method: "POST",
                    data: userPostReactionData,
                    headers: headers
                }).success(function (data, status, headers, config) {
                    //$scope.persons = data; // assign  $scope.persons here as promise is resolved here
                    //stopBlockUI();                    
                    //$scope.UserPostList[postIndex].likeInfoHtml = appentToCommentLikeString($scope.UserPostList[postIndex].likeInfoHtml);
                    //$timeout(function() {
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
                Image: $scope.UserPostList[postIndex].postInfo.newCommentImage,
                VertexId: $scope.UserPostList[postIndex].postInfo._id,
                WallVertexId: $scope.UserPostList[postIndex].postedToUser[0]._id,
                PostPostedByVertexId: $scope.UserPostList[postIndex].userInfo[0]._id
            };

            if (isNullOrEmpty($scope.UserPostList[postIndex].postInfo.postUserComment) && isNullOrEmpty($scope.UserPostList[postIndex].postInfo.newCommentImage)) {
                showToastMessage("Warning", "You cannot submit empty message.");
                return;
            }

            var newCommentPosted = {
                "commentInfo": {
                    "PostImage": $scope.UserPostList[postIndex].postInfo.newCommentImage,
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
                ],
                "isAuthenticToEdit": true
            };

            var url = ServerContextPath.empty + '/User/UserCommentOnPost';
            var headers = {
                'Content-Type': 'application/json',
                'UTMZT': $.cookie('utmzt'),
                'UTMZK': $.cookie('utmzk'),
                'UTMZV': $.cookie('utmzv'),
            };
            if ($rootScope.isUserLoggedIn) {

                $scope.UserPostList[postIndex].postInfo.postUserComment = "";
                $scope.UserPostList[postIndex].postInfo.newCommentImage = "";
                $scope.UserPostList[postIndex].commentsInfo.push(newCommentPosted);
                var commentAddedAtIndex = $scope.UserPostList[postIndex].commentsInfo.length - 1;

                $scope.UserPostList[postIndex].commentsInfo[commentAddedAtIndex].loadingIcon = true;
                $scope.UserPostList[postIndex].commentsInfo[0].disableInputBox = true;

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

                    $scope.UserPostList[postIndex].commentsInfo[commentAddedAtIndex].loadingIcon = false;
                    $scope.UserPostList[postIndex].commentsInfo[commentAddedAtIndex].commentInfo._id = data.Payload.commentInfo._id;
                    $scope.UserPostList[postIndex].commentsInfo[0].disableInputBox = false;
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

        $scope.onEditFileSelectLogoUrl = function ($files, postIndex) {

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
                        $scope.UserPostList[postIndex].postInfo.PostImage = data.data.link_s;
                    });

                });

            }

        };
        
        $scope.onCommentImageFileUpload = function ($files, postIndex) {

            console.log("postIndex : " + postIndex);
            console.log();
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
                        $scope.UserPostList[postIndex].postInfo.newCommentImage = data.data.link_s;
                    });

                });

            }

        };

        $scope.onEditFileSelectLogoUrlForComments = function ($files, postIndex, commentIndex) {

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
                        $scope.UserPostList[postIndex].commentsInfo[commentIndex].commentInfo.PostImage = data.data.link_s;
                    });

                });

            }

        };

        $scope.removeUploadedCommentImage = function (postIndex) {
            $scope.UserPostList[postIndex].postInfo.newCommentImage = "";
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
                    //$scope.UserPostList = data.results;
                    if ($scope.UserPostList != null && data.results.length>0) {
                        var absoluteIndex = 0; // only 1 post available.
                        $scope.visitedUserVertexId = data.results[absoluteIndex].postedToUser[0]._id;
                        if (data.results[absoluteIndex].commentsInfo != null && data.results[absoluteIndex].commentsInfo.length > 0) {
                            data.results[absoluteIndex].commentsInfo = reverseCommentsInfoList(data.results[absoluteIndex].commentsInfo);
                        }

                        $scope.UserPostList.push(data.results[absoluteIndex]);
                        
                        $scope.UserPostList[absoluteIndex].likeInfoHtml = parseCommentLikeString($scope.UserPostList[absoluteIndex].likeInfo, $scope.UserPostList[absoluteIndex].likeInfoCount);
                        $scope.UserPostList[absoluteIndex].messageFromIndex = 0;
                        $scope.UserPostList[absoluteIndex].messageToIndex = $scope.UserPostList[absoluteIndex].messageFromIndex + messagesPerCall - 1;
                        if ($scope.UserPostList[absoluteIndex].isLiked != null && $scope.UserPostList[absoluteIndex].isLiked.length > 0) {
                            $scope.UserPostList[absoluteIndex].alreadyLiked = true;
                        } else {
                            $scope.UserPostList[absoluteIndex].alreadyLiked = false;
                        }
                        
                        //console.log($scope.UserPostList);
                    } else {
                        showToastMessage("Warning", "Post Not found.");
                    }
                });

            });
        };

        function appendOldCommentsToCommentList(oldList, newList) {
            var newCommentList = [];
            for (var j = 0; j < newList.length; j++) {
                newCommentList.push(newList[j]);
            }

            for (var k = 0; k < oldList.length; k++) {
                newCommentList.push(oldList[k]);
            }
            return newCommentList;
        }

        function reverseCommentsInfoList(newList) {
            var reversedList = [];
            for (var i = newList.length - 1; i >= 0; i--) {
                newList[i].editableMode = false;

                if (newList[i].isLiked.length > 0) {
                    newList[i].alreadyLiked = true;
                }

                if (newList[i].commentInfo.PostedByUser == $rootScope.clientDetailResponse.Email) {
                    newList[i].isAuthenticToEdit = true;
                } else {
                    newList[i].isAuthenticToEdit = false;
                }

                if ($scope.visitedUserVertexId == $rootScope.clientDetailResponse.VertexId) {
                    newList[i].isLoggedInUserWall = true;
                } else {
                    newList[i].isLoggedInUserWall = false;
                }

                reversedList.push(newList[i]);
            }
            return reversedList;
        }

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
                if (likeInfoCount > 0)
                    str += " liked this";
                else
                    str += "be the first one to like this";
            }
            return str;
        };

        function appentToCommentLikeString(str, likeInfoCount) {
            if (str == null) str = "";
            if (likeInfoCount > 0) {
                //str = "<a href='#/userprofile/" + $rootScope.clientDetailResponse.VertexId + "'>" + $rootScope.clientDetailResponse.Firstname + " " + $rootScope.clientDetailResponse.Lastname + "</a>," + str;
                str = "" + $rootScope.clientDetailResponse.Firstname + " " + $rootScope.clientDetailResponse.Lastname + "," + str;
            } else {
                str = "" + $rootScope.clientDetailResponse.Firstname + " " + $rootScope.clientDetailResponse.Lastname + " liked this";
            }
            return str;
        };

        function removeFromCommentLikeString(str, likeInfoCount) {
            if (str == null) str = "";
            if (likeInfoCount <= 1) {
                //str = "<a href='#/userprofile/" + $rootScope.clientDetailResponse.VertexId + "'>" + $rootScope.clientDetailResponse.Firstname + " " + $rootScope.clientDetailResponse.Lastname + "</a>," + str;
                str = "be the first one to like this";
            } else {
                str = str.replace($rootScope.clientDetailResponse.Firstname + " " + $rootScope.clientDetailResponse.Lastname + ",", "").replace($rootScope.clientDetailResponse.Firstname + " " + $rootScope.clientDetailResponse.Lastname, "");
            }
            return str;
        };

        $scope.enableEditOnUserPost = function (postIndex) {
            $scope.UserPostList[postIndex].postInfo.editableMode = true;
            $scope.UserPostList[postIndex].postInfo.OriginalPostMessage = $scope.UserPostList[postIndex].postInfo.PostMessage;
            if ($scope.$root.$$phase != '$apply' && $scope.$root.$$phase != '$digest') {
                $scope.$apply();
            }
        };

        $scope.cancelEditOnUserPost = function (postIndex) {
            $scope.UserPostList[postIndex].postInfo.editableMode = false;
            $scope.UserPostList[postIndex].postInfo.PostMessage = $scope.UserPostList[postIndex].postInfo.OriginalPostMessage;
            if ($scope.$root.$$phase != '$apply' && $scope.$root.$$phase != '$digest') {
                $scope.$apply();
            }
        };

        $scope.submitEditOnUserPost = function (postIndex) {
            $scope.UserPostList[postIndex].postInfo.editableMode = false;
            if ($scope.$root.$$phase != '$apply' && $scope.$root.$$phase != '$digest') {
                $scope.$apply();
            }

            submitEditPost(postIndex);
        };

        function submitEditPost(postIndex) {

            if (isNullOrEmpty($scope.UserPostList[postIndex].postInfo.PostMessage)) {
                showToastMessage("Warning", "You cann't submit empty message.");
                return;
            }

            //if ($scope.UserPostList[postIndex].postInfo.PostMessage == $scope.UserPostList[postIndex].postInfo.OriginalPostMessage) {
            //    //showToastMessage("Warning", "You cann't submit empty message.");
            //    return;
            //}

            var editMessageRequest = {
                message: $scope.UserPostList[postIndex].postInfo.PostMessage,
                imageUrl: $scope.UserPostList[postIndex].postInfo.PostImage,
                messageVertex: $scope.UserPostList[postIndex].postInfo._id,
                userVertex: $rootScope.clientDetailResponse.VertexId,
                userEmail: $rootScope.clientDetailResponse.Email,
                wallVertex: $scope.visitedUserVertexId
            };

            var url = ServerContextPath.empty + '/User/EditMessageDetails';
            var headers = {
                'Content-Type': 'application/json',
                'UTMZT': CookieUtil.getUTMZT(),
                'UTMZK': CookieUtil.getUTMZK(),
                'UTMZV': CookieUtil.getUTMZV(),
                '_ga': $.cookie('_ga')
            };

            //startBlockUI('wait..', 3);
            $scope.UserPostList[postIndex].postInfo.loadingIcon = true;
            $http({
                url: url,
                method: "POST",
                data: editMessageRequest,
                headers: headers
            }).success(function (data, status, headers, config) {
                //$scope.persons = data; // assign  $scope.persons here as promise is resolved here
                $scope.UserPostList[postIndex].postInfo.loadingIcon = false;
                //stopBlockUI();
                showToastMessage("Success", "Successfully Edited");
            }).error(function (data, status, headers, config) {

            });


        };

        $scope.uploadImageOncomment = function (postIndex) {

            $scope.currentUploadingPostIndex = postIndex;

            //$scope.UserPostList[postIndex].commentsInfo[commentIndex].editableMode = true;
            //$scope.UserPostList[postIndex].commentsInfo[commentIndex].commentInfo.OriginalPostMessage = $scope.UserPostList[postIndex].commentsInfo[commentIndex].commentInfo.PostMessage;
            if ($scope.$root.$$phase != '$apply' && $scope.$root.$$phase != '$digest') {
                $scope.$apply();
            }
            document.getElementById('my_comment_file').click();
        };

        $scope.enableEditcommentOnUserPost = function (postIndex, commentIndex) {
            $scope.UserPostList[postIndex].commentsInfo[commentIndex].editableMode = true;
            $scope.UserPostList[postIndex].commentsInfo[commentIndex].commentInfo.OriginalPostMessage = $scope.UserPostList[postIndex].commentsInfo[commentIndex].commentInfo.PostMessage;
            if ($scope.$root.$$phase != '$apply' && $scope.$root.$$phase != '$digest') {
                $scope.$apply();
            }
        };

        $scope.cancelEditcommentOnUserPost = function (postIndex, commentIndex) {
            $scope.UserPostList[postIndex].commentsInfo[commentIndex].editableMode = false;
            $scope.UserPostList[postIndex].commentsInfo[commentIndex].commentInfo.PostMessage = $scope.UserPostList[postIndex].commentsInfo[commentIndex].commentInfo.OriginalPostMessage;
            if ($scope.$root.$$phase != '$apply' && $scope.$root.$$phase != '$digest') {
                $scope.$apply();
            }
        };

        $scope.submitEditcommentOnUserPost = function (postIndex, commentIndex) {
            $scope.UserPostList[postIndex].commentsInfo[commentIndex].editableMode = false;
            if ($scope.$root.$$phase != '$apply' && $scope.$root.$$phase != '$digest') {
                $scope.$apply();
            }

            submitEditMessage(postIndex, commentIndex);
        };

        function submitEditMessage(postIndex, commentIndex) {

            if (isNullOrEmpty($scope.UserPostList[postIndex].commentsInfo[commentIndex].commentInfo.PostMessage)) {
                showToastMessage("Warning", "You cann't submit empty message.");
                return;
            }

            //if ($scope.UserPostList[postIndex].commentsInfo[commentIndex].commentInfo.PostMessage == $scope.UserPostList[postIndex].commentsInfo[commentIndex].commentInfo.OriginalPostMessage) {
            //    //showToastMessage("Warning", "You cann't submit empty message.");
            //    return;
            //}

            var editMessageRequest = {
                message: $scope.UserPostList[postIndex].commentsInfo[commentIndex].commentInfo.PostMessage,
                imageUrl: $scope.UserPostList[postIndex].commentsInfo[commentIndex].commentInfo.PostImage,
                messageVertex: $scope.UserPostList[postIndex].commentsInfo[commentIndex].commentInfo._id,
                userVertex: $rootScope.clientDetailResponse.VertexId,
                userEmail: $rootScope.clientDetailResponse.Email,
                wallVertex: $scope.visitedUserVertexId
            };

            var url = ServerContextPath.empty + '/User/EditMessageDetails';
            var headers = {
                'Content-Type': 'application/json',
                'UTMZT': CookieUtil.getUTMZT(),
                'UTMZK': CookieUtil.getUTMZK(),
                'UTMZV': CookieUtil.getUTMZV(),
                '_ga': $.cookie('_ga')
            };

            //startBlockUI('wait..', 3);
            $scope.UserPostList[postIndex].commentsInfo[commentIndex].loadingIcon = true;
            $http({
                url: url,
                method: "POST",
                data: editMessageRequest,
                headers: headers
            }).success(function (data, status, headers, config) {
                //$scope.persons = data; // assign  $scope.persons here as promise is resolved here
                $scope.UserPostList[postIndex].commentsInfo[commentIndex].loadingIcon = false;
                //stopBlockUI();
                showToastMessage("Success", "Successfully Edited");
            }).error(function (data, status, headers, config) {

            });


        };
    });

      
});

