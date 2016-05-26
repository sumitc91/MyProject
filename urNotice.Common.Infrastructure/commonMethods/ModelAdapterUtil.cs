using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using urNotice.Common.Infrastructure.Common.Enum;
using urNotice.Common.Infrastructure.Model.urNoticeModel.GraphModel;
using urNotice.Common.Infrastructure.Model.urNoticeModel.GraphModel.V1;

namespace urNotice.Common.Infrastructure.commonMethods
{
    public class ModelAdapterUtil
    {
        public static CompanyNoticePeriodVertexModelResponse GetCompanyNoticePeriodInfoResponse(CompanyNoticePeriodVertexModelV1Response companyNoticePeriodVertexModelV1Response)
        {
            var companyNoticePeriodVertexModelResponse = new CompanyNoticePeriodVertexModelResponse();
            companyNoticePeriodVertexModelResponse.success = false;
            companyNoticePeriodVertexModelResponse.results = new List<CompanyNoticePeriodVertexModel>();

            if (companyNoticePeriodVertexModelV1Response == null || 
                companyNoticePeriodVertexModelV1Response.result == null || 
                companyNoticePeriodVertexModelV1Response.result.data == null) 
                    return companyNoticePeriodVertexModelResponse;
            
            companyNoticePeriodVertexModelResponse.success = true;
            foreach (var companyNoticePeriodInfo in companyNoticePeriodVertexModelV1Response.result.data)
            {
                var companyNoticePeriodVertexModel = new CompanyNoticePeriodVertexModel();
                companyNoticePeriodVertexModel.designationInfo = ParseDesignationInfo(companyNoticePeriodInfo.designationInfo);
                companyNoticePeriodVertexModel.range = ParseNoticePeriodRangeInfo(companyNoticePeriodInfo.range);

                companyNoticePeriodVertexModelResponse.results.Add(companyNoticePeriodVertexModel);
            }
            return companyNoticePeriodVertexModelResponse;
        }

        public static CompanySalaryVertexModelResponse GetCompanySalaryInfoResponse(CompanySalaryVertexModelV1Response companySalaryVertexModelV1Response)
        {
            var companySalaryVertexModelResponse = new CompanySalaryVertexModelResponse();
            companySalaryVertexModelResponse.success = false;
            companySalaryVertexModelResponse.results = new List<CompanySalaryVertexModel>();

            if (companySalaryVertexModelV1Response == null ||
                companySalaryVertexModelV1Response.result == null ||
                companySalaryVertexModelV1Response.result.data == null)
                return companySalaryVertexModelResponse;

            companySalaryVertexModelResponse.success = true;
            foreach (var companySalaryInfo in companySalaryVertexModelV1Response.result.data)
            {
                var companySalaryVertexModel = new CompanySalaryVertexModel();
                companySalaryVertexModel.designationInfo = ParseDesignationInfo(companySalaryInfo.designationInfo);
                companySalaryVertexModel.salaryInfo = ParseSalaryInfo(companySalaryInfo.salaryInfo);

                companySalaryVertexModelResponse.results.Add(companySalaryVertexModel);
            }

            return companySalaryVertexModelResponse;
        }

        public static UserPostVertexModelResponse GetUserPostInfoResponse(UserPostVertexModelV1Response userPostVertexModelV1Response)
        {
            var response = new UserPostVertexModelResponse();
            response.success = false;
            response.results = new List<UserPostVertexModel>();

            if (userPostVertexModelV1Response == null ||
                userPostVertexModelV1Response.result == null ||
                userPostVertexModelV1Response.result.data == null)
                return response;

            response.success = true;
            foreach (var userPost in userPostVertexModelV1Response.result.data)
            {
                var userPostVertexModel = new UserPostVertexModel();
                userPostVertexModel.postInfo = ParsePostInfo(userPost.wallpostinfo);
                userPostVertexModel.postedOn = ParseUserListVertex(userPost.postedOn);
                userPostVertexModel.userInfo = ParseUserListVertex(userPost.userInfo);                
                userPostVertexModel.isLiked = ParseUserListVertex(userPost.isLiked);
                userPostVertexModel.likeInfo = ParseUserListVertex(userPost.likeInfo);
                userPostVertexModel.likeInfoCount = userPost.likeInfoCount.ToString(CultureInfo.InvariantCulture);

                userPostVertexModel.postedToUser = ParseUserListVertex(userPost.postedOn);
                userPostVertexModel.commentsCount = userPost.commentsCount.ToString(CultureInfo.InvariantCulture);
                userPostVertexModel.commentsInfo = ParsePostListInfo(userPost.commentsInfo);
                response.results.Add(userPostVertexModel);
            }

            return response;
        }

        public static UserPostLikesVertexModelResponse GetUserPostLikesVertexModelResponse(UserPostLikesVertexModelV1Response getUserPostMessagesResponseDeserialized)
        {
            var response = new UserPostLikesVertexModelResponse();
            response.success = false;
            response.results = new List<UserPostLikesModel>();

            if (getUserPostMessagesResponseDeserialized == null ||
                getUserPostMessagesResponseDeserialized.result == null ||
                getUserPostMessagesResponseDeserialized.result.data == null)
                return response;

            response.success = true;
            foreach (var getUserPostMessage in getUserPostMessagesResponseDeserialized.result.data)
            {
                var userPostLikesModel = new UserPostLikesModel();
                userPostLikesModel.likeInfo = ParseUserListVertexToSingleObject(getUserPostMessage);
                response.results.Add(userPostLikesModel);
            }

            return response;
        }

        public static UserNotificationVertexModelResponse GetUserNotificationVertexModelResponse(UserNotificationVertexV1ModelResponse clientNotificationDetailList)
        {
            var response = new UserNotificationVertexModelResponse();
            response.results = new List<UserNotificationVertexModel>();
            response.success = false;

            if (clientNotificationDetailList == null ||
                clientNotificationDetailList.result == null ||
                clientNotificationDetailList.result.data == null)
                return response;

            response.success = true;
            foreach (var clientNotificationDetail in clientNotificationDetailList.result.data)
            {
                var userNotificationVertexModel = new UserNotificationVertexModel();
                userNotificationVertexModel.notificationByUser = ParseUserListVertexToSingleObject(clientNotificationDetail.notificationByUser);
                userNotificationVertexModel.notificationInfo = ParseNotificationInfoVertex(clientNotificationDetail.notificationInfo);
                userNotificationVertexModel.postInfo = ParsePostInfoToList(clientNotificationDetail.postInfo);
                response.results.Add(userNotificationVertexModel);
            }

            return response;
        }

        public static UserFriendRequestNotificationVertexModelResponse GetUserFriendRequestNotificationVertexModelResponse(UserFriendRequestNotificationVertexV1ModelResponse clientFriendRequestNotificationDetailResponseDeserialized)
        {
            var response = new UserFriendRequestNotificationVertexModelResponse();
            response.results = new List<UserFriendRequestNotificationVertexModel>();
            response.success = false;

            if (clientFriendRequestNotificationDetailResponseDeserialized == null ||
                clientFriendRequestNotificationDetailResponseDeserialized.result == null ||
                clientFriendRequestNotificationDetailResponseDeserialized.result.data == null)
                return response;
            response.success = true;

            foreach (var clientFriendRequestNotificationDetail in clientFriendRequestNotificationDetailResponseDeserialized.result.data)
            {
                var userFriendRequestNotificationVertexModel = new UserFriendRequestNotificationVertexModel();
                userFriendRequestNotificationVertexModel.requestedBy = ParseUserListVertex(clientFriendRequestNotificationDetail.requestedBy);
                userFriendRequestNotificationVertexModel.requestInfo = ParseFriendRequestVertex(clientFriendRequestNotificationDetail.requestInfo);
                response.results.Add(userFriendRequestNotificationVertexModel);
            }
            return response;
        }

        public static UserPostNetworkDetailModelResponse UserPostNetworkDetailModelResponse(UserPostNetworkDetailModelV1Response getUserNetworkDetailResponseDeserialized)
        {
            var response = new UserPostNetworkDetailModelResponse();
            response.results = new List<UserPostNetworkDetailModel>();
            response.success = false;
            if (getUserNetworkDetailResponseDeserialized == null ||
                getUserNetworkDetailResponseDeserialized.result == null ||
                getUserNetworkDetailResponseDeserialized.result.data == null)
                return response;
            response.success = true;

            foreach (var getUserNetworkDetail in getUserNetworkDetailResponseDeserialized.result.data)
            {
                var userPostNetworkDetailModel = new UserPostNetworkDetailModel();
                userPostNetworkDetailModel.associateRequestReceived = ParseUserListVertex(getUserNetworkDetail.associateRequestReceived);
                userPostNetworkDetailModel.associateRequestSent = ParseUserListVertex(getUserNetworkDetail.associateRequestSent);
                userPostNetworkDetailModel.followRequestSent = ParseUserListVertex(getUserNetworkDetail.followRequestSent);
                userPostNetworkDetailModel.isFriend = ParseUserListVertex(getUserNetworkDetail.isFriend);
                response.results.Add(userPostNetworkDetailModel);
            }
            return response;

        }

        public static UserPostMessagesVertexModelResponse GetUserPostMessagesVertexModelResponse(UserPostMessagesVertexModelV1Response getUserPostMessagesResponseDeserialized)
        {
            var response = new UserPostMessagesVertexModelResponse();
            response.results = new List<UserPostCommentModel>();
            response.success = false;
            if (getUserPostMessagesResponseDeserialized == null ||
                getUserPostMessagesResponseDeserialized.result == null ||
                getUserPostMessagesResponseDeserialized.result.data == null)
                return response;
            response.success = true;

            foreach (var getUserNetworkDetail in getUserPostMessagesResponseDeserialized.result.data)
            {
                var userPostCommentModel = new UserPostCommentModel();
                userPostCommentModel.commentedBy = ParseUserListVertex(getUserNetworkDetail.commentedBy);
                userPostCommentModel.commentInfo = ParsePostInfo(getUserNetworkDetail.commentInfo);
                userPostCommentModel.isLiked = ParseUserListVertex(getUserNetworkDetail.isLiked);
                userPostCommentModel.likeCount = getUserNetworkDetail.likeCount;
                response.results.Add(userPostCommentModel);
            }
            return response;
        }

        public static UserFollowersVertexModelResponse GetUserFollowersVertexModelResponse(UserPostLikesVertexModelV1Response userFollowersDeserialized)
        {
            var response = new UserFollowersVertexModelResponse();
            response.success = false;
            response.results = new List<UserVertexModel>();

            if (userFollowersDeserialized == null ||
                userFollowersDeserialized.result == null ||
                userFollowersDeserialized.result.data == null)
                return response;

            response.success = true;
            foreach (var userFollower in userFollowersDeserialized.result.data)
            {
                var userVertexModel = new UserVertexModel();
                userVertexModel = ParseUserListVertexToSingleObject(userFollower);
                response.results.Add(userVertexModel);
            }

            return response;
        }

        private static EdgeModel ParseFriendRequestVertex(EdgeModelV1 requestInfo)
        {
            var edgeModel = new EdgeModel();
            edgeModel._id = requestInfo.id;
            edgeModel._inV = requestInfo.inV.ToString();
            edgeModel._label = requestInfo.label;
            edgeModel._outV = requestInfo.outV.ToString();
            edgeModel._type = requestInfo.type;
            if (requestInfo.properties.ContainsKey(EdgePropertyEnum.PostedDate.ToString()))
            {
                edgeModel.PostedDate = requestInfo.properties[EdgePropertyEnum.PostedDate.ToString()];
            }
            if (requestInfo.properties.ContainsKey(EdgePropertyEnum.PostedDateLong.ToString()))
            {
                edgeModel.PostedDateLong = Convert.ToInt64(requestInfo.properties[EdgePropertyEnum.PostedDateLong.ToString()]);
            }
            return edgeModel;
        }

        private static UserNotificationEdgeInfo ParseNotificationInfoVertex(VertexModelV1 notificationInfo)
        {
            var userNotificationEdgeInfo = new UserNotificationEdgeInfo();
            userNotificationEdgeInfo._id = notificationInfo.id.ToString(CultureInfo.InvariantCulture);
            userNotificationEdgeInfo._type = notificationInfo.type;
            userNotificationEdgeInfo.Type = notificationInfo.label;
            if (notificationInfo.properties.ContainsKey(VertexPropertyEnum.ParentPostId.ToString()))
            {
                userNotificationEdgeInfo.ParentPostId = notificationInfo.properties[VertexPropertyEnum.ParentPostId.ToString()][0].value;
            }
            if (notificationInfo.properties.ContainsKey(VertexPropertyEnum.PostedTime.ToString()))
            {
                userNotificationEdgeInfo.PostedDate = notificationInfo.properties[VertexPropertyEnum.PostedTime.ToString()][0].value;
            }
            return userNotificationEdgeInfo;
        }

        private static List<UserVertexModel> ParseUserListVertex(List<VertexModelV1> likeInfoList)
        {
            var userVertexModelList = new List<UserVertexModel>();
            if (likeInfoList != null)
            {
                foreach (var likeInfo in likeInfoList)
                {
                    userVertexModelList.Add(ParsUserVertex(likeInfo));
                }
            }
            return userVertexModelList;
        }

        private static List<UserVertexModel> ParseUserListVertex(VertexModelV1 postedOn)
        {
            var userVertexModelList = new List<UserVertexModel>();
            var userVertexModel = new UserVertexModel();
            userVertexModel = ParsUserVertex(postedOn);
            userVertexModelList.Add(userVertexModel);
            return userVertexModelList;
        }

        private static UserVertexModel ParseUserListVertexToSingleObject(VertexModelV1 postedOn)
        {                        
            return ParsUserVertex(postedOn);            
        }

        private static UserVertexModel ParsUserVertex(VertexModelV1 userVertex)
        {
            var userVertexModel = new UserVertexModel();
            userVertexModel._id = userVertex.id.ToString(CultureInfo.InvariantCulture);
            userVertexModel._type = userVertex.type;
            if (userVertex.properties.ContainsKey(VertexPropertyEnum.CoverImageUrl.ToString()))
            {
                userVertexModel.CoverImageUrl = userVertex.properties[VertexPropertyEnum.CoverImageUrl.ToString()][0].value;
            }
            if (userVertex.properties.ContainsKey(VertexPropertyEnum.CreatedTime.ToString()))
            {
                userVertexModel.CreatedTime = userVertex.properties[VertexPropertyEnum.CreatedTime.ToString()][0].value;
            }
            if (userVertex.properties.ContainsKey(VertexPropertyEnum.FirstName.ToString()))
            {
                userVertexModel.FirstName = userVertex.properties[VertexPropertyEnum.FirstName.ToString()][0].value;
            }
            if (userVertex.properties.ContainsKey(VertexPropertyEnum.Gender.ToString()))
            {
                userVertexModel.Gender = userVertex.properties[VertexPropertyEnum.Gender.ToString()][0].value;
            }
            if (userVertex.properties.ContainsKey(VertexPropertyEnum.ImageUrl.ToString()))
            {
                userVertexModel.ImageUrl = userVertex.properties[VertexPropertyEnum.ImageUrl.ToString()][0].value;
            }
            if (userVertex.properties.ContainsKey(VertexPropertyEnum.LastName.ToString()))
            {
                userVertexModel.LastName = userVertex.properties[VertexPropertyEnum.LastName.ToString()][0].value;
            }
            if (userVertex.properties.ContainsKey(VertexPropertyEnum.Username.ToString()))
            {
                userVertexModel.Username = userVertex.properties[VertexPropertyEnum.Username.ToString()][0].value;
            }
            return userVertexModel;
        }

        private static List<UserPostCommentModel> ParsePostListInfo(List<UserPostCommentsInfoVertexModelV1ResultDataResponse> commentsInfoList)
        {
            var response = new List<UserPostCommentModel>();
            if (commentsInfoList != null)
            {
                foreach (var commentsInfo in commentsInfoList)
                {
                    response.Add(ParseUserCommentPostInfo(commentsInfo));
                }
            }
            return response;
        }

        private static UserPostCommentModel ParseUserCommentPostInfo(UserPostCommentsInfoVertexModelV1ResultDataResponse wallpostinfo)
        {
            var userPostCommentModel = new UserPostCommentModel();
            userPostCommentModel.commentInfo = ParsePostInfo(wallpostinfo.commentData);
            userPostCommentModel.commentedBy = ParseUserListVertex(wallpostinfo.commentedBy);
            userPostCommentModel.isLiked = ParseUserListVertex(wallpostinfo.isCommentLiked);
            userPostCommentModel.likeCount = wallpostinfo.likeCount;
            return userPostCommentModel;
        }

        private static WallPostVertexModel ParsePostInfo(VertexModelV1 wallpostinfo)
        {
            var wallPostVertexModel = new WallPostVertexModel();
            wallPostVertexModel._id = wallpostinfo.id.ToString(CultureInfo.InvariantCulture);
            wallPostVertexModel._type = wallpostinfo.type;

            if (wallpostinfo.properties.ContainsKey(VertexPropertyEnum.PostedByUser.ToString()))
            {
                wallPostVertexModel.PostedByUser = wallpostinfo.properties[VertexPropertyEnum.PostedByUser.ToString()][0].value;
            }
            if (wallpostinfo.properties.ContainsKey(VertexPropertyEnum.PostedTime.ToString()))
            {
                wallPostVertexModel.PostedTime = wallpostinfo.properties[VertexPropertyEnum.PostedTime.ToString()][0].value;
            }
            if (wallpostinfo.properties.ContainsKey(VertexPropertyEnum.PostImage.ToString()))
            {
                wallPostVertexModel.PostImage = wallpostinfo.properties[VertexPropertyEnum.PostImage.ToString()][0].value;
            }
            if (wallpostinfo.properties.ContainsKey(VertexPropertyEnum.PostMessage.ToString()))
            {
                wallPostVertexModel.PostMessage = wallpostinfo.properties[VertexPropertyEnum.PostMessage.ToString()][0].value;
            }
            return wallPostVertexModel;
        }

        private static List<WallPostVertexModel> ParsePostInfoToList(VertexModelV1 wallpostinfo)
        {
            var wallPostVertexModelList = new List<WallPostVertexModel>();
            wallPostVertexModelList.Add(ParsePostInfo(wallpostinfo));
            return wallPostVertexModelList;
        }

        private static List<CompanySalaryInfoVertexModel> ParseSalaryInfo(EdgeModelV1 salaryInfo)
        {
            var companySalaryInfoVertexModelList = new List<CompanySalaryInfoVertexModel>();
            var companySalaryInfoVertexModel = new CompanySalaryInfoVertexModel();
            companySalaryInfoVertexModel._id = salaryInfo.id;
            companySalaryInfoVertexModel._inV = salaryInfo.inV.ToString(CultureInfo.InvariantCulture);
            companySalaryInfoVertexModel._outV = salaryInfo.outV.ToString(CultureInfo.InvariantCulture);

            if (salaryInfo.properties.ContainsKey(EdgePropertyEnum.PostedDate.ToString()))
            {
                companySalaryInfoVertexModel.PostedDate = salaryInfo.properties[EdgePropertyEnum.PostedDate.ToString()];
            }

            if (salaryInfo.properties.ContainsKey(EdgePropertyEnum.SalaryAmount.ToString()))
            {
                companySalaryInfoVertexModel.SalaryAmount = Convert.ToInt32(salaryInfo.properties[EdgePropertyEnum.SalaryAmount.ToString()]);
            }

            companySalaryInfoVertexModelList.Add(companySalaryInfoVertexModel);
            return companySalaryInfoVertexModelList;
        }

        private static List<CompanyNoticePeriodInfoVertexModel> ParseNoticePeriodRangeInfo(EdgeModelV1 range)
        {
            var companyNoticePeriodInfoVertexModelList = new List<CompanyNoticePeriodInfoVertexModel>();
            var companyNoticePeriodInfoVertexModel = new CompanyNoticePeriodInfoVertexModel();
            companyNoticePeriodInfoVertexModel._id = range.id;
            companyNoticePeriodInfoVertexModel._inV = range.inV.ToString(CultureInfo.InvariantCulture);
            companyNoticePeriodInfoVertexModel._outV = range.outV.ToString(CultureInfo.InvariantCulture);

            if (range.properties.ContainsKey(EdgePropertyEnum.PostedDate.ToString()))
            {                
                companyNoticePeriodInfoVertexModel.PostedDate = range.properties[EdgePropertyEnum.PostedDate.ToString()];
            }

            if (range.properties.ContainsKey(EdgePropertyEnum.RangeValue.ToString()))
            {
                companyNoticePeriodInfoVertexModel.RangeValue = range.properties[EdgePropertyEnum.RangeValue.ToString()];
            }

            companyNoticePeriodInfoVertexModelList.Add(companyNoticePeriodInfoVertexModel);
            return companyNoticePeriodInfoVertexModelList;
        }

        private static List<CompanyDesignationInfoVertexModel> ParseDesignationInfo(VertexModelV1 designationInfo)
        {
            var companyDesignationInfoVertexModelList = new List<CompanyDesignationInfoVertexModel>();
            var companyDesignationInfoVertexModel = new CompanyDesignationInfoVertexModel();
            companyDesignationInfoVertexModel._id = designationInfo.id.ToString(CultureInfo.InvariantCulture);
            if (designationInfo.properties.ContainsKey(VertexPropertyEnum.CreatedTime.ToString()))
            {
                companyDesignationInfoVertexModel.CreatedTime = designationInfo.properties[VertexPropertyEnum.CreatedTime.ToString()][0].value;
            }
            if (designationInfo.properties.ContainsKey(VertexPropertyEnum.DesignationName.ToString()))
            {
                companyDesignationInfoVertexModel.DesignationName = designationInfo.properties[VertexPropertyEnum.DesignationName.ToString()][0].value;
            }
            companyDesignationInfoVertexModelList.Add(companyDesignationInfoVertexModel);
            return companyDesignationInfoVertexModelList;
        }

    }
}
