'use strict';
define([appLocation.preLogin], function (app) {

    app.controller('beforeLoginUserProfile', function ($scope, $http, $upload, $timeout,$location, $routeParams, $rootScope, CookieUtil) {
        $('title').html("edit page"); //TODO: change the title so cann't be tracked in log

        $rootScope.userOrbitFeedList.show = true;
        _.defer(function () { $scope.$apply(); });
        $scope.visitedUserVertexId = $routeParams.vertexId;
       
        var messagesPerCall = 5;
        $scope.UserPostLikesPerCall = 5;

        $scope.CurrentUserDetails = {};
        $scope.UserPostList = [];
        
        

        $scope.UserPostListLastPageReached = false;
        $scope.UserPostListInfoAngular = {
            busy: false,
            after: 0,
            itemPerPage: 3
        };

        $scope.UserNetworkDetailHelper = {
            isFriendRequestSent: false,
            isFriendRequestReceived: false,
            isFollowing: false,
            isFriend:false,
            UserNetworkDetailHelperDataLoaded: false
        };

        getUserInformation();
        //getUserPost($scope.UserPostListInfoAngular.after, $scope.UserPostListInfoAngular.after + $scope.UserPostListInfoAngular.itemPerPage);

        $scope.createNewUserPost = function () {
            createNewUserPost();
            $scope.UserPostListInfoAngular.after = 0;
            //$scope.UserPostList = [];
        };

        $scope.commentOnUserPost = function (postIndex) {
            //console.log($scope.UserPostList[postIndex].postInfo._id, $scope.UserPostList[postIndex].postInfo.postUserComment);
            //console.log("postIndex : " + postIndex);
            createNewMessageOnUserPost(postIndex);

            //$scope.UserPostList[postIndex].messageFromIndex = $scope.UserPostList[postIndex].messageToIndex - 1;
            //console.log("$scope.UserPostList[postIndex].messageFromIndex : " + $scope.UserPostList[postIndex].messageFromIndex);
            $scope.UserPostListInfoAngular.after = 0;
            //$scope.UserPostList = [];
            //createNewMessageOnUserPost($scope.UserPostList[postIndex].postInfo._id, $scope.UserPostList[postIndex].postInfo.postUserComment);
        };

        $scope.deleteOnUserPostComment = function (postIndex, commentIndex) {
            deleteCommentOnUserPost(postIndex, commentIndex);
        };

        $scope.deleteUserPost = function (postIndex) {
            //console.log("deleteUserPost : "+postIndex);
            deleteOnUserPost(postIndex);
        };

        $scope.reactionOnUserPost = function (postIndex) {            
            createNewReactionOnUserPost(postIndex);
            //createNewMessageOnUserPost($scope.UserPostList[postIndex].postInfo._id, $scope.UserPostList[postIndex].postInfo.postUserComment);
        };

        $scope.reactionOnUserPostComment = function (postIndex,commentIndex) {
            createNewReactionOnUserPostComment(postIndex,commentIndex);            
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

        $scope.showLikedByUsers = function (postVertexId) {
            $scope.UserPostLikes = [];
            $scope.UserPostLikesFrom = 0;
            
            $scope.UserPostLikesTo = $scope.UserPostLikesFrom + $scope.UserPostLikesPerCall - 1;

            $scope.UserPostLikesCurrentPostVertexId = postVertexId;
            showLikedByUsersOnUserPost(postVertexId, $scope.UserPostLikesFrom, $scope.UserPostLikesTo);
            
        };

        $scope.showMoreLikedByUsers = function () {

            $scope.UserPostLikesFrom = $scope.UserPostLikesTo;
            $scope.UserPostLikesTo = $scope.UserPostLikesFrom + $scope.UserPostLikesPerCall - 1;
            showLikedByUsersOnUserPost($scope.UserPostLikesCurrentPostVertexId, $scope.UserPostLikesFrom, $scope.UserPostLikesTo);

        };

        $scope.loadMoreMessage = function (postVerexId, postIndex) {            
            $scope.UserPostList[postIndex].messageFromIndex = $scope.UserPostList[postIndex].messageToIndex;
            $scope.UserPostList[postIndex].messageToIndex = $scope.UserPostList[postIndex].messageFromIndex + messagesPerCall - 1;

            console.log("$scope.UserPostList[postIndex].messageFromIndex : " + $scope.UserPostList[postIndex].messageFromIndex);
            console.log("$scope.UserPostList[postIndex].messageToIndex : " + $scope.UserPostList[postIndex].messageToIndex);
            loadMoreMessage(postVerexId, postIndex, $scope.UserPostList[postIndex].messageFromIndex, $scope.UserPostList[postIndex].messageToIndex);            
        };

        $scope.makeConnectionRequest = function (userVertexId, connectingBody, connectionType) {
            makeConnectionRequest(userVertexId,connectingBody, connectionType);
        };

        $scope.removeUploadedPostImage = function() {
            $timeout(function () {
                $scope.NewPostImageUrl = {};
            });
        };

        $scope.closeModelAndNavigateTo = function (vid) {
            //console.log("inside closeModelAndNavigateTo");  
            $(".modal-backdrop.in").hide();
            $('#closeModalId').click();
            $location.url('/userprofile/'+vid);
            //window.location.href = "/#/userprofile/" + vid;
            
            //alert("Navigation not implemented yet.");
            //console.log("#/userprofile/" + vid);
        };
        
        $scope.UserPostListInfo = {};

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

        function loadUserNetworkDetail(vertexId, from, to) {
            var url = ServerContextPath.userServer + '/User/GetUserNetworkDetail?from=' + from + '&to=' + to + '&vertexId=' + vertexId;
            var headers = {
                'Content-Type': 'application/json',
                'UTMZT': $.cookie('utmzt'),
                'UTMZK': $.cookie('utmzk'),
                'UTMZV': $.cookie('utmzv'),
            };

            if (!$rootScope.isUserLoggedIn || $scope.visitedUserVertexId == $rootScope.clientDetailResponse.VertexId) {
                //only fetch this info if user is logged in.
                return;
            }

            //startBlockUI('wait..', 3);
            //$scope.UserNetworkDetailHelper.UserNetworkDetailHelperDataLoading = true;
            $.ajax({
                url: url,
                method: "GET",
                headers: headers
            }).done(function (data, status) {
                //stopBlockUI();
                //console.log(data.results);
                $scope.UserNetworkDetailHelper.UserNetworkDetailHelperDataLoaded = true;
                //$scope.UserNetworkDetailHelper.UserNetworkDetailHelperDataLoading = false;

                $scope.$apply(function () {
                    if (data.results != null && data.results.length > 0) {

                        if (data.results[0].associateRequestSent != null && data.results[0].associateRequestSent.length > 0) {
                            $scope.UserNetworkDetailHelper.isFriendRequestSent = true;
                        }
                        if (data.results[0].associateRequestReceived != null && data.results[0].associateRequestReceived.length > 0) {
                            $scope.UserNetworkDetailHelper.isFriendRequestReceived = true;
                        }
                        if (data.results[0].followRequestSent != null && data.results[0].followRequestSent.length > 0) {
                            $scope.UserNetworkDetailHelper.isFollowing = true;
                        }
                        if (data.results[0].isFriend != null && data.results[0].isFriend.length > 0) {
                            $scope.UserNetworkDetailHelper.isFriend = true;
                        }
                    }                    
                });

                

            });
        };

        function loadMoreMessage(vertexId, postIndex,from, to) {
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
                    //for (var i = 0; i < data.results.length; i++) {                        
                        //$scope.UserPostList[postIndex].commentsInfo.push(data.results[i]);
                    //}
                    
                });

            });
        };

        $scope.NewPostImageUrl = {
            //link_s:"https://s3-ap-southeast-1.amazonaws.com/urnotice/OrbitPage/User/Sumit/WallPost/9ac2bfce-a1eb-4a51-9f18-ad5591a72cc0.png"
        };
        function createNewUserPost() {

            var userPostData = {
                Message: $scope.UserPostMessage,
                Image: $scope.NewPostImageUrl.link_m,                
                VertexId: $scope.visitedUserVertexId
            };

            var url = ServerContextPath.empty + '/User/UserPost';
            var headers = {
                'Content-Type': 'application/json',
                'UTMZT': $.cookie('utmzt'),
                'UTMZK': $.cookie('utmzk'),
                'UTMZV': $.cookie('utmzv'),
            };

            console.log(userPostData);
            
            if (isNullOrEmpty(userPostData.Message) && isNullOrEmpty(userPostData.Image)) {
                showToastMessage("Warning", "You cannot submit Empty Post.");
                return;
            }
            
            if ($rootScope.isUserLoggedIn) {
                startBlockUI('wait..', 3);

                $http({
                    url: url,
                    method: "POST",
                    data: userPostData,
                    headers: headers
                }).success(function (data, status, headers, config) {
                    //$scope.persons = data; // assign  $scope.persons here as promise is resolved here
                    stopBlockUI();
                    $scope.UserPostList = [];
                    $timeout(function () {
                        $scope.NewPostImageUrl = {};
                    });

                    getUserPost(0, $scope.UserPostListInfoAngular.after + $scope.UserPostListInfoAngular.itemPerPage);
                    $scope.UserPostListInfoAngular.after = $scope.UserPostListInfoAngular.after + $scope.UserPostListInfoAngular.itemPerPage;
                    $scope.UserPostListLastPageReached = false;
                    //$scope.UserPostList.push(data.Payload);
                    $scope.UserPostMessage = "";

                    

                }).error(function (data, status, headers, config) {

                });
            } else {
                showToastMessage("Warning", "Please Login to create a post.");
            }
            
        };

        function makeConnectionRequest(userVertexId,connectingBody, connectionType) {

            var userNewConnectionData = {
                UserVertexId: userVertexId,
                ConnectionType: connectionType,
                ConnectingBody:connectingBody
            };

           
            var url = ServerContextPath.empty + '/User/UserConnectionRequest';
            var headers = {
                'Content-Type': 'application/json',
                'UTMZT': $.cookie('utmzt'),
                'UTMZK': $.cookie('utmzk'),
                'UTMZV': $.cookie('utmzv'),
            };

            if ($rootScope.isUserLoggedIn) {
                //startBlockUI('wait..', 3);
                //$scope.makeConnectionRequestLoading = true;
                showHideConnectionRequestLoadingOnButton(connectionType, true);
                $http({
                    url: url,
                    method: "POST",
                    data: userNewConnectionData,
                    headers: headers
                }).success(function (data, status, headers, config) {
                    //$scope.persons = data; // assign  $scope.persons here as promise is resolved here
                    //$scope.makeConnectionRequestLoading = false;
                    showHideConnectionRequestLoadingOnButton(connectionType, false);
                    //stopBlockUI();                    
                    if (connectingBody == UserConnectionRequestModel.AssociateUsers) {
                        if (connectionType == UserConnectionRequestModel.AssociateRequest) {
                            //friend request sent
                            $scope.UserNetworkDetailHelper.isFriendRequestSent = true;
                            $scope.UserNetworkDetailHelper.isFollowing = true;
                        }
                        else if (connectionType == UserConnectionRequestModel.AssociateFollow) {
                            //follow
                            $scope.UserNetworkDetailHelper.isFollowing = true;
                        }
                        else if (connectionType == UserConnectionRequestModel.AssociateAccept) {
                            //friend req accept
                            $scope.UserNetworkDetailHelper.isFriend = true;
                            $scope.UserNetworkDetailHelper.isFollowing = true;
                        }
                        else if (connectionType == UserConnectionRequestModel.AssociateReject) {
                            //reject
                            $scope.UserNetworkDetailHelper.isFriendRequestReceived = false;
                        }
                        else if (connectionType == UserConnectionRequestModel.RemoveFollow) {
                            //unfollow
                            $scope.UserNetworkDetailHelper.isFollowing = false;
                        }
                        else if (connectionType == UserConnectionRequestModel.Deassociate) {
                            //Deassociate
                            $scope.UserNetworkDetailHelper.isFriend = false;
                            $scope.UserNetworkDetailHelper.isFollowing = false;
                        }
                        else if (connectionType == UserConnectionRequestModel.AssociateRequestCancel) {
                            //Deassociate
                            $scope.UserNetworkDetailHelper.isFriendRequestSent = false;
                        }
                    }

                }).error(function (data, status, headers, config) {

                });
            } else {
                showToastMessage("Warning", "Please Login to Make a connection.");
            }

        };

        function showHideConnectionRequestLoadingOnButton(connectionType,show) {
            if (connectionType == UserConnectionRequestModel.AssociateRequest) {
                //friend request sent
                $scope.UserConnectionRequestModel.AssociateRequestLoading = show;
            }
            else if (connectionType == UserConnectionRequestModel.AssociateFollow) {
                //follow
                $scope.UserConnectionRequestModel.AssociateFollowLoading = show;
            }
            else if (connectionType == UserConnectionRequestModel.AssociateAccept) {
                //friend req accept
                $scope.UserConnectionRequestModel.AssociateAcceptLoading = show;
            }
            else if (connectionType == UserConnectionRequestModel.AssociateReject) {
                //reject
                $scope.UserConnectionRequestModel.AssociateRejectLoading = show;
            }
            else if (connectionType == UserConnectionRequestModel.RemoveFollow) {
                //unfollow
                $scope.UserConnectionRequestModel.RemoveFollowLoading = show;
            }
            else if (connectionType == UserConnectionRequestModel.Deassociate) {
                //Deassociate
                $scope.UserConnectionRequestModel.DeassociateLoading = show;
            }
            else if (connectionType == UserConnectionRequestModel.AssociateRequestCancel) {
                //Deassociate
                $scope.UserConnectionRequestModel.AssociateRequestCancelLoading = show;
            }
        }

        function createNewReactionOnUserPost(postIndex) {

            var userPostReactionData = {
                Reaction: UserReaction.Like,
                VertexId: $scope.UserPostList[postIndex].postInfo._id,
                WallVertexId: $scope.visitedUserVertexId,
                PostPostedByVertexId: $scope.UserPostList[postIndex].userInfo[0]._id,
                IsParentPost: true,
                ParentVertexId: $scope.UserPostList[postIndex].postInfo._id
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
                $scope.UserPostList[postIndex].likeInfoHtml = appentToCommentLikeString($scope.UserPostList[postIndex].likeInfoHtml, $scope.UserPostList[postIndex].likeInfoCount);
                $http({
                    url: url,
                    method: "POST",
                    data: userPostReactionData,
                    headers: headers
                }).success(function(data, status, headers, config) {
                    //$scope.persons = data; // assign  $scope.persons here as promise is resolved here
                    //stopBlockUI();                    
                    //$scope.UserPostList[postIndex].likeInfoHtml = appentToCommentLikeString($scope.UserPostList[postIndex].likeInfoHtml);
                    //$timeout(function() {
                    //    $scope.NewPostImageUrl.link_s = "";
                    //});

                }).error(function(data, status, headers, config) {

                });
            } else {
                showToastMessage("Warning", "Please Login to Make your reaction on post.");
            }

        };

        function createNewReactionOnUserPostComment(postIndex,commentIndex) {

            var userPostReactionData = {
                Reaction: UserReaction.Like,
                VertexId: $scope.UserPostList[postIndex].commentsInfo[commentIndex].commentInfo._id,
                WallVertexId: $scope.visitedUserVertexId,
                PostPostedByVertexId: $scope.UserPostList[postIndex].commentsInfo[commentIndex].commentedBy[0]._id,
                IsParentPost: false,
                ParentVertexId: $scope.UserPostList[postIndex].postInfo._id
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
                $scope.UserPostList[postIndex].commentsInfo[commentIndex].likeCount = $scope.UserPostList[postIndex].commentsInfo[commentIndex].likeCount+1;
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

        function deleteOnUserPost(postIndex) {

            var userPostReactionData = {
                Reaction: UserReaction.Like,
                VertexId: $scope.UserPostList[postIndex].postInfo._id,
                WallVertexId: $scope.visitedUserVertexId,
                PostPostedByVertexId: $scope.UserPostList[postIndex].userInfo[0]._id
            };

            var url = ServerContextPath.empty + '/User/DeleteCommentOnPost?vertexId=' + $scope.UserPostList[postIndex].postInfo._id;
            var headers = {
                'Content-Type': 'application/json',
                'UTMZT': $.cookie('utmzt'),
                'UTMZK': $.cookie('utmzk'),
                'UTMZV': $.cookie('utmzv'),
            };


            if ($rootScope.isUserLoggedIn) {
                spliceOnUserPost(postIndex);
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

        function spliceOnUserPost(postIndex) {            
            $scope.UserPostList.splice(postIndex, 1);
            if ($scope.$root.$$phase != '$apply' && $scope.$root.$$phase != '$digest') {
                $scope.$apply();
            }            
        };

        function deleteCommentOnUserPost(postIndex, commentIndex) {

            var userPostReactionData = {
                Reaction: UserReaction.Like,
                VertexId: $scope.UserPostList[postIndex].commentsInfo[commentIndex].commentInfo._id,
                WallVertexId: $scope.visitedUserVertexId,
                PostPostedByVertexId: $scope.UserPostList[postIndex].commentsInfo[commentIndex].commentedBy[0]._id
            };

            var url = ServerContextPath.empty + '/User/DeleteCommentOnPost?vertexId=' + $scope.UserPostList[postIndex].commentsInfo[commentIndex].commentInfo._id;
            var headers = {
                'Content-Type': 'application/json',
                'UTMZT': $.cookie('utmzt'),
                'UTMZK': $.cookie('utmzk'),
                'UTMZV': $.cookie('utmzv'),
            };

            
            if ($rootScope.isUserLoggedIn) {
                spliceCommentOnUserPost(postIndex, commentIndex);
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

        function spliceCommentOnUserPost(postIndex, commentIndex) {
            
            $scope.UserPostList[postIndex].commentsInfo.splice(commentIndex, 1);
            if ($scope.$root.$$phase != '$apply' && $scope.$root.$$phase != '$digest') {
                $scope.$apply();
            }            
        };

        function createNewMessageOnUserPost(postIndex) {

            
            var userPostCommentData = {
                Message: $scope.UserPostList[postIndex].postInfo.postUserComment,
                Image: $scope.UserPostList[postIndex].postInfo.newCommentImage,
                VertexId: $scope.UserPostList[postIndex].postInfo._id,
                WallVertexId:$scope.visitedUserVertexId,
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
                "isAuthenticToEdit": true,
                "likeCount": 0,
                "isLiked":[]
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
                //startBlockUI('wait..', 3);
                $http({
                    url: url,
                    method: "POST",
                    data: userPostCommentData,
                    headers: headers
                }).success(function (data, status, headers, config) {
                    //$scope.persons = data; // assign  $scope.persons here as promise is resolved here
                    //stopBlockUI();
                    //$scope.UserPostList = [];
                    //getUserPost(0, $scope.UserPostListInfoAngular.after + $scope.UserPostListInfoAngular.itemPerPage);
                    //$scope.UserPostList[postIndex].commentsInfo.push(newCommentPosted);
                    $scope.UserPostList[postIndex].commentsInfo[commentAddedAtIndex].loadingIcon = false;
                    $scope.UserPostList[postIndex].commentsInfo[commentAddedAtIndex].commentInfo._id = data.Payload.commentInfo._id;
                    $scope.UserPostList[postIndex].commentsInfo[0].disableInputBox = false;
                    $scope.userPostCommentData = "";

                    $timeout(function () {
                        $scope.NewPostImageUrl.link_s = "";
                    });

                }).error(function (data, status, headers, config) {

                });
            }
            else {
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

        $scope.onEditFileSelectLogoUrl = function ($files,postIndex) {

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

        $scope.onEditFileSelectLogoUrlForComments = function ($files, postIndex,commentIndex) {

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

        $scope.removeUploadedCommentImage = function(postIndex) {
            $scope.UserPostList[postIndex].postInfo.newCommentImage = "";
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
                    loadUserNetworkDetail($scope.visitedUserVertexId, 0, 1);
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
                    var absoluteIndex = 0;
                    if ($scope.UserPostList != null && data.results.length>0) {
                        for (var i = 0; i < data.results.length; i++) {

                            data.results[i].postInfo.editableMode = false;
                            if (data.results[i].commentsInfo != null && data.results[i].commentsInfo.length > 0) {
                                data.results[i].commentsInfo = reverseCommentsInfoList(data.results[i].commentsInfo);                                
                            }

                            $scope.UserPostList.push(data.results[i]);
                            //console.log("commentinfo2 new : " + $scope.UserPostList[0].commentsInfo[0].editableMode);
                            absoluteIndex = from + i;
                            //console.log("$scope.UserPostList : " + $scope.UserPostList);
                            //console.log("Ln 962: absoluteIndex : "+absoluteIndex);
                            $scope.UserPostList[absoluteIndex].likeInfoHtml = parseCommentLikeString($scope.UserPostList[absoluteIndex].likeInfo,$scope.UserPostList[absoluteIndex].likeInfoCount);
                            $scope.UserPostList[absoluteIndex].messageFromIndex = 0;
                            $scope.UserPostList[absoluteIndex].messageToIndex = $scope.UserPostList[absoluteIndex].messageFromIndex + messagesPerCall - 1;
                            if ($scope.UserPostList[absoluteIndex].isLiked != null && $scope.UserPostList[absoluteIndex].isLiked.length > 0) {
                                $scope.UserPostList[absoluteIndex].alreadyLiked = true;
                            } else {
                                $scope.UserPostList[absoluteIndex].alreadyLiked = false;
                            }

                        }
                    } else {                        
                        $scope.UserPostListLastPageReached = true;
                    }

                    //console.log($scope.UserPostList);
                });

                initializeHoverCard();
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

        function parseCommentLikeString(likeInfo,likeInfoCount) {
            var str = "";
            //console.log("likeInfo " + likeInfo);
            for (var i = 0; i < likeInfo.length; i++) {
                str += " " + likeInfo[i].FirstName + " " + likeInfo[i].LastName;
                //str += ' <span user_name="'+likeInfo[i].Username+'" class="show_hovercard" name="'+likeInfo[i].FirstName +' ' + likeInfo[i].LastName+'" profile_pic="'+likeInfo[i].ImageUrl+'" cover_pic="'+likeInfo[i].CoverImageUrl+'">'+likeInfo[i].FirstName+' '+likeInfo[i].LastName+'</span>';
                //str += '<a cover_pic="" profile_pic="https://lh3.googleusercontent.com/-XdUIqdMkCWA/AAAAAAAAAAI/AAAAAAAAAAA/4252rscbv5M/photo.jpg" name="Goop Chup" class="show_hovercard ng-binding" user_name="786goopchup@gmail.com" href="#/userprofile/17928960">Goop Chup</a>';
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

        function appentToCommentLikeString(str,likeInfoCount) {
            if (str == null) str = "";
            if (likeInfoCount > 0) {
                str = "" + $rootScope.clientDetailResponse.Firstname + " " + $rootScope.clientDetailResponse.Lastname + "," + str;
                //str = "" + $rootScope.clientDetailResponse.Firstname + " " + $rootScope.clientDetailResponse.Lastname + "," + str;
                //str += ' <span user_name="' + $rootScope.clientDetailResponse.Username + '" class="show_hovercard" name="' + $rootScope.clientDetailResponse.FirstName + ' ' + $rootScope.clientDetailResponse.LastName + '" profile_pic="' + $rootScope.clientDetailResponse.ImageUrl + '" cover_pic="' + $rootScope.clientDetailResponse.CoverImageUrl + '">' + $rootScope.clientDetailResponse.FirstName + ' ' + $rootScope.clientDetailResponse.LastName + '</span>,'+str;
            } else {
                str = "" + $rootScope.clientDetailResponse.Firstname + " " + $rootScope.clientDetailResponse.Lastname + " liked this";
                //str += ' <span user_name="' + $rootScope.clientDetailResponse.Username + '" class="show_hovercard" name="' + $rootScope.clientDetailResponse.FirstName + ' ' + $rootScope.clientDetailResponse.LastName + '" profile_pic="' + $rootScope.clientDetailResponse.ImageUrl + '" cover_pic="' + $rootScope.clientDetailResponse.CoverImageUrl + '">' + $rootScope.clientDetailResponse.FirstName + ' ' + $rootScope.clientDetailResponse.LastName + '</span>' + " liked this";
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
            $scope.UserPostList[postIndex].postInfo.PostImage = $scope.UserPostList[postIndex].postInfo.OriginalPostImage;
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

        $scope.enableEditcommentOnUserPost = function(postIndex, commentIndex) {           
            $scope.UserPostList[postIndex].commentsInfo[commentIndex].editableMode = true;
            $scope.UserPostList[postIndex].commentsInfo[commentIndex].commentInfo.OriginalPostMessage = $scope.UserPostList[postIndex].commentsInfo[commentIndex].commentInfo.PostMessage;
            if ($scope.$root.$$phase != '$apply' && $scope.$root.$$phase != '$digest') {
                $scope.$apply();
            }           
        };

        $scope.cancelEditcommentOnUserPost = function (postIndex, commentIndex) {            
            $scope.UserPostList[postIndex].commentsInfo[commentIndex].editableMode = false;
            $scope.UserPostList[postIndex].commentsInfo[commentIndex].commentInfo.PostMessage = $scope.UserPostList[postIndex].commentsInfo[commentIndex].commentInfo.OriginalPostMessage;
            $scope.UserPostList[postIndex].commentsInfo[commentIndex].commentInfo.PostImage = $scope.UserPostList[postIndex].commentsInfo[commentIndex].commentInfo.OriginalPostImage;
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

            //if ($scope.UserPostList[postIndex].commentsInfo[commentIndex].commentInfo.PostMessage == $scope.UserPostList[postIndex].commentsInfo[commentIndex].commentInfo.OriginalPostMessage
            //    && $scope.UserPostList[postIndex].commentsInfo[commentIndex].commentInfo.PostImage == $scope.UserPostList[postIndex].commentsInfo[commentIndex].commentInfo.OriginalPostImage) {
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

        $scope.UserPostListInfo.nextPage = function () {
            if ($scope.UserPostListInfoAngular.busy || $scope.UserPostListLastPageReached) return;
            $scope.UserPostListInfoAngular.busy = true;
            getUserPost($scope.UserPostListInfoAngular.after, $scope.UserPostListInfoAngular.after + $scope.UserPostListInfoAngular.itemPerPage);
            $scope.UserPostListInfoAngular.after = $scope.UserPostListInfoAngular.after + $scope.UserPostListInfoAngular.itemPerPage;
            //console.log("UserPostListInfo.nextPage called.");

        };

    });

    
   
});

