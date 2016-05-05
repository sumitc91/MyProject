using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using SolrNet;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Common.Infrastructure.Model.urNoticeModel.GraphModel;
using urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper;
using urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper.EditProfile;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper;
using urNotice.Common.Infrastructure.Model.urNoticeModel.Solr;
using urNotice.Common.Infrastructure.Session;

namespace urNotice.Common.Infrastructure.Model.Person
{
    public interface IPerson
    {
        ResponseModel<LoginResponse> RegisterMe(RegisterationRequest req, HttpRequestBase request);
        ResponseModel<LoginResponse> SocialRegisterMe(RegisterationRequest req, HttpRequestBase request);
        ResponseModel<LoginResponse> Login(string userName, string password,bool decryptPassword);
        ResponseModel<OrbitPageUser> GetFullUserDetail(string userEmail);
        ResponseModel<ClientDetailsModel> GetPersonDetails(string username);
        ResponseModel<EditPersonModel> EditPersonDetails(urNoticeSession session, EditPersonModel editPersonModel);
        ResponseModel<IDictionary<string, string>> UserConnectionRequest(urNoticeSession session, UserConnectionRequestModel userConnectionRequestModel);


        ResponseModel<string> EditMessageDetails(urNoticeSession session, EditMessageRequest messageReq);
        ResponseModel<UserPostVertexModel> CreateNewUserPost(urNoticeSession session, string message, string image, string userWallVertexId, out HashSet<string> sendNotificationResponse);
        ResponseModel<UserPostCommentModel> CreateNewCommentOnUserPost(urNoticeSession session, string message, string image, string postVertexId, string userWallVertexId, string postPostedByVertexId, out HashSet<string> sendNotificationResponse);
        ResponseModel<String> DeleteCommentOnPost(urNoticeSession session, string vertexId);
        ResponseModel<UserVertexModel> CreateNewReactionOnUserPost(urNoticeSession session,UserNewReactionRequest userNewReactionRequest, out HashSet<string> sendNotificationResponse);
        ResponseModel<String> RemoveReactionOnUserPost(urNoticeSession session, string vertexId);
        ResponseModel<string> SeenNotification(string userName);

        HashSet<string> SendNotificationToUser(urNoticeSession session, string userWallVertexId, string postVertexId,string commentVertexId, string postPostedByVertexId, string notificationType);

        SolrQueryResults<UnCompanySolr> CompanyDetailsById(string userVertexId, string cid);

        string GetUserNotification(urNoticeSession session, string from, string to);
        string GetUserFriendRequestNotification(urNoticeSession session, string from, string to);
        string GetUserPost(string userVertexId, string @from, string to, string userEmail);
        string GetUserPostMessages(string userVertexId, string @from, string to, string userEmail);
        string GetUserPostLikes(string userVertexId, string @from, string to);
        string GetPostByVertexId(string vertexId, string userEmail);
        string GetUserNetworkDetail(urNoticeSession session, string vertexId, string @from, string to);
        long GetUserUnreadNotificationCount(urNoticeSession session);
        long GetUserUnreadFriendRequestNotificationCount(urNoticeSession session);

        //anonymous services
        ResponseModel<String> ValidateAccountService(ValidateAccountRequest req);
        ResponseModel<String> ResendValidationCodeService(ValidateAccountRequest req, HttpRequestBase request);
        ResponseModel<String> ForgetPasswordService(string id, HttpRequestBase request);
        ResponseModel<String> ResetPasswordService(ResetPasswordRequest req);
        ResponseModel<String> ContactUsService(ContactUsRequest req);
        //admin services
        Dictionary<string, string> CreateNewCompanyDesignationEdge(urNoticeSession session, string designation,
            string salary, string jobFromYear, string jobToYear, string companyVertexId);
        bool CreateNewDesignation(string designationName, string createdBy);
        bool CreateNewCompanyDesignationSalary(string companyName, string designationName, string salary,
            string createdBy);
        bool CreateNewCompanyDesignationNoticePeriod(string companyName, string designationName, string noticePeriodRange,
            string createdBy);
        bool CreateNewCompany(OrbitPageCompany company, string createdBy);
    }
}
