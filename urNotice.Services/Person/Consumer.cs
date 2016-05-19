using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using Newtonsoft.Json;
using SolrNet;
using urNotice.Common.Infrastructure.Common.Config;
using urNotice.Common.Infrastructure.Common.Constants;
using urNotice.Common.Infrastructure.Common.Enum;
using urNotice.Common.Infrastructure.commonMethods;
using urNotice.Common.Infrastructure.Encryption;
using urNotice.Common.Infrastructure.Model.Person;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Common.Infrastructure.Model.urNoticeModel.GraphModel;
using urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper;
using urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper.EditProfile;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper;
using urNotice.Common.Infrastructure.Model.urNoticeModel.Solr;
using urNotice.Common.Infrastructure.Model.urNoticeModel.User;
using urNotice.Common.Infrastructure.Session;
using urNotice.Common.Infrastructure.signalRPushNotifications;
using urNotice.Services.Email.EmailTemplate;
using urNotice.Services.GraphDb;
using urNotice.Services.GraphDb.GraphDbContract;
using urNotice.Services.Management.AccountManagement;
using urNotice.Services.Management.CompanyManagement;
using urNotice.Services.Management.NotificationManagement;
using urNotice.Services.Management.PostManagement;
using urNotice.Services.NoSqlDb.DynamoDb;
using urNotice.Services.Person.PersonContract.LoginOperation;
using urNotice.Services.Person.PersonContract.RegistrationOperation;
using urNotice.Services.Solr.SolrCompany;
using urNotice.Services.Solr.SolrUser;

namespace urNotice.Services.Person
{
    public class Consumer : IPerson
    {
        //private delegate void ContactUsEmailSendDelegate(String emails, ContactUsRequest req);

        //Account Management.
        public ResponseModel<LoginResponse> RegisterMe(RegisterationRequest req, HttpRequestBase request)
        {
            IAccountManagement accountManagementModel = new AccountManagementV1();
            return accountManagementModel.RegisterMe(req,request);
        }
        public ResponseModel<LoginResponse> SocialRegisterMe(RegisterationRequest req, HttpRequestBase request)
        {
            IAccountManagement accountManagementModel = new AccountManagementV1();
            return accountManagementModel.SocialRegisterMe(req,request);
        }
        public ResponseModel<LoginResponse> Login(string userName, string password, bool isSocialLogin)
        {
            IAccountManagement accountManagementModel = new AccountManagementV1();
            return accountManagementModel.Login(userName, password, isSocialLogin);            
        }
        public ResponseModel<OrbitPageUser> GetFullUserDetail(string userEmail)
        {
            throw new NotImplementedException();
        }
        public ResponseModel<ClientDetailsModel> GetPersonDetails(string userEmail)
        {
            IAccountManagement accountManagementModel = new AccountManagementV1();
            return accountManagementModel.GetPersonDetails(userEmail);
        }
        public ResponseModel<EditPersonModel> EditPersonDetails(urNoticeSession session, EditPersonModel editPersonModel)
        {
            IAccountManagement accountManagementModel = new AccountManagementV1();
            return accountManagementModel.EditPersonDetails(session,editPersonModel);
        }

        // Account Management - For Gremling Query
        public string GetUserNotification(urNoticeSession session, string from, string to)
        {
            IAccountManagement accountManagementModel = new AccountManagementV1();
            return accountManagementModel.GetUserNotification(session, from, to);
        }

        public string GetAllFollowers(string vertexId)
        {
            IAccountManagement accountManagementModel = new AccountManagementV1();
            return accountManagementModel.GetAllFollowers(vertexId);
        }

        public string GetUserFriendRequestNotification(urNoticeSession session, string from, string to)
        {
            IAccountManagement accountManagementModel = new AccountManagementV1();
            return accountManagementModel.GetUserFriendRequestNotification(session, from, to);
        }

        public string GetUserPost(string userVertexId, string @from, string to, string userEmail)
        {
            IAccountManagement accountManagementModel = new AccountManagementV1();
            return accountManagementModel.GetUserPost(userVertexId, from, to, userEmail);
        }
        public string GetUserOrbitFeedPost(string userVertexId, string @from, string to, string userEmail)
        {
            IAccountManagement accountManagementModel = new AccountManagementV1();
            return accountManagementModel.GetUserOrbitFeedPost(userVertexId, from, to, userEmail);
        }
        public string GetUserPostMessages(string userVertexId, string @from, string to, string userEmail)
        {
            IAccountManagement accountManagementModel = new AccountManagementV1();
            return accountManagementModel.GetUserPostMessages(userVertexId, from, to, userEmail);
        }
        public string GetUserPostLikes(string userVertexId, string @from, string to)
        {
            IAccountManagement accountManagementModel = new AccountManagementV1();
            return accountManagementModel.GetUserPostLikes(userVertexId, from, to);
        }
        public string GetPostByVertexId(string vertexId, string userEmail)
        {
            IAccountManagement accountManagementModel = new AccountManagementV1();
            return accountManagementModel.GetPostByVertexId(vertexId, userEmail);
        }

        public string GetUserNetworkDetail(urNoticeSession session,string vertexId, string @from, string to)
        {
            IAccountManagement accountManagementModel = new AccountManagementV1();
            return accountManagementModel.GetUserNetworkDetail(session, vertexId, from, to);
        }

        public long GetUserUnreadNotificationCount(urNoticeSession session)
        {
            IAccountManagement accountManagementModel = new AccountManagementV1();
            return accountManagementModel.GetUserUnreadNotificationCount(session);
        }
        public long GetUserUnreadFriendRequestNotificationCount(urNoticeSession session)
        {
            IAccountManagement accountManagementModel = new AccountManagementV1();
            return accountManagementModel.GetUserUnreadFriendRequestNotificationCount(session);
        }
        public ResponseModel<IDictionary<string, string>> UserConnectionRequest(urNoticeSession session, UserConnectionRequestModel userConnectionRequestModel, out HashSet<string> sendNotificationHashSetResponse)
        {
            IAccountManagement accountManagementModel = new AccountManagementV1();
            return accountManagementModel.UserConnectionRequest(session, userConnectionRequestModel, out sendNotificationHashSetResponse);
        }

        //Post Management.
        public ResponseModel<string> EditMessageDetails(urNoticeSession session, EditMessageRequest messageReq)
        {
            IPostManagement postManagementModel = new PostManagement();
            return postManagementModel.EditMessageDetails(session,messageReq);
        }
        public ResponseModel<UserPostVertexModel> CreateNewUserPost(urNoticeSession session, string message, string image, string userWallVertexId, out HashSet<string> sendNotificationResponse)
        {
            IPostManagement postManagementModel = new PostManagement();
            return postManagementModel.CreateNewUserPost(session,message,image,userWallVertexId,out sendNotificationResponse);  
        }


        public HashSet<string> SendNotificationToUser(urNoticeSession session, string userWallVertexId, string postVertexId, string commentVertexId, string postPostedByVertexId, string notificationType)
        {
            INotificationManagement notificationManagement = new NotificationManagement();
            return notificationManagement.SendNotificationToUser(session, userWallVertexId, postVertexId, commentVertexId, postPostedByVertexId, notificationType);
        }
        public ResponseModel<UserPostCommentModel> CreateNewCommentOnUserPost(urNoticeSession session, string message, string image, string postVertexId, string userWallVertexId, string postPostedByVertexId, out HashSet<string> sendNotificationResponse)
        {
            IPostManagement postManagementModel = new PostManagement();
            return postManagementModel.CreateNewCommentOnUserPost(session,message,image,postVertexId,userWallVertexId,postPostedByVertexId,out sendNotificationResponse);
        }
        public ResponseModel<String> DeleteCommentOnPost(urNoticeSession session, string vertexId)
        {
            IPostManagement postManagementModel = new PostManagement();
            return postManagementModel.DeleteCommentOnPost(session,vertexId);
        }
        public ResponseModel<UserVertexModel> CreateNewReactionOnUserPost(urNoticeSession session, UserNewReactionRequest userNewReactionRequest, out HashSet<string> sendNotificationResponse)
        {
            IPostManagement postManagementModel = new PostManagement();
            return postManagementModel.CreateNewReactionOnUserPost(session,userNewReactionRequest,out sendNotificationResponse);
        }
        public ResponseModel<String> RemoveReactionOnUserPost(urNoticeSession session, string vertexId)
        {
            IPostManagement postManagementModel = new PostManagement();
            return postManagementModel.RemoveReactionOnUserPost(session,vertexId);
        }
        

        public ResponseModel<string> ValidateAccountService(ValidateAccountRequest req)
        {
            IAccountManagement accountManagementModel = new AccountManagementV1();
            return accountManagementModel.ValidateAccountService(req);
        }
        public ResponseModel<string> ResendValidationCodeService(ValidateAccountRequest req, HttpRequestBase request)
        {
            IAccountManagement accountManagementModel = new AccountManagementV1();
            return accountManagementModel.ResendValidationCodeService(req,request);
        }
        public ResponseModel<string> ForgetPasswordService(string id, HttpRequestBase request)
        {
            IAccountManagement accountManagementModel = new AccountManagementV1();
            return accountManagementModel.ForgetPasswordService(id,request);
        }
        public ResponseModel<string> ResetPasswordService(ResetPasswordRequest req)
        {
            IAccountManagement accountManagementModel = new AccountManagementV1();
            return accountManagementModel.ResetPasswordService(req);
        }
        public ResponseModel<string> ContactUsService(ContactUsRequest req)
        {
            IAccountManagement accountManagementModel = new AccountManagementV1();
            return accountManagementModel.ContactUsService(req);
        }
        public ResponseModel<string> SeenNotification(string userName)
        {
            IAccountManagement accountManagementModel = new AccountManagementV1();
            return accountManagementModel.SeenNotification(userName);
        }


        //Company Management
        public SolrQueryResults<UnCompanySolr> CompanyDetailsById(string userVertexId, string cid)
        {
            ICompanyManagement companyManagementModel = new CompanyManagement();
            return companyManagementModel.CompanyDetailsById(userVertexId, cid);
        }
        public Dictionary<string, string> CreateNewCompanyDesignationEdge(urNoticeSession session, string designation, string salary,
            string jobFromYear, string jobToYear, string companyVertexId)
        {
            throw new NotImplementedException();
        }
        public bool CreateNewDesignation(string designationName, string createdBy)
        {
            throw new NotImplementedException();
        }
        public bool CreateNewCompanyDesignationSalary(string companyName, string designationName, string salary, string createdBy)
        {
            throw new NotImplementedException();
        }
        public bool CreateNewCompanyDesignationNoticePeriod(string companyName, string designationName, string noticePeriodRange,
            string createdBy)
        {
            throw new NotImplementedException();
        }
        public bool CreateNewCompany(OrbitPageCompany company, string createdBy)
        {
            throw new NotImplementedException();
        }

        

    }
}
