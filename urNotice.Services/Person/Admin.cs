using System;
using System.Collections.Generic;
using System.Web;
using SolrNet;
using urNotice.Common.Infrastructure.Common.Config;
using urNotice.Common.Infrastructure.Common.Constants;
using urNotice.Common.Infrastructure.Common.Enum;
using urNotice.Common.Infrastructure.commonMethods;
using urNotice.Common.Infrastructure.Model.Person;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Common.Infrastructure.Model.urNoticeModel.GraphModel;
using urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper;
using urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper.EditProfile;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper;
using urNotice.Common.Infrastructure.Model.urNoticeModel.Solr;
using urNotice.Common.Infrastructure.Session;
using urNotice.Services.GraphDb;
using urNotice.Services.GraphDb.GraphDbContract;
using urNotice.Services.Management.AccountManagement;
using urNotice.Services.Management.CompanyManagement;
using urNotice.Services.NoSqlDb.DynamoDb;
using urNotice.Services.Solr.SolrCompany;
using urNotice.Services.Solr.SolrDesignation;

namespace urNotice.Services.Person
{
    public class Admin : IPerson
    {
        public ResponseModel<LoginResponse> RegisterMe(RegisterationRequest req, HttpRequestBase request)
        {
            throw new NotImplementedException();
        }

        public ResponseModel<LoginResponse> SocialRegisterMe(RegisterationRequest req, HttpRequestBase request)
        {
            throw new NotImplementedException();
        }

        public ResponseModel<LoginResponse> Login(string userName, string password, bool decryptPassword)
        {
            throw new NotImplementedException();
        }

        public ResponseModel<OrbitPageUser> GetFullUserDetail(string userEmail)
        {
            IAccountManagement accountManagementModel = new AccountManagement();
            return accountManagementModel.GetFullUserDetail(userEmail);
        }

        public ResponseModel<ClientDetailsModel> GetPersonDetails(string username)
        {
            throw new NotImplementedException();
        }

        public ResponseModel<EditPersonModel> EditPersonDetails(urNoticeSession session, EditPersonModel editPersonModel)
        {
            throw new NotImplementedException();
        }

        public ResponseModel<string> EditMessageDetails(urNoticeSession session, EditMessageRequest messageReq)
        {
            throw new NotImplementedException();
        }

        public ResponseModel<UserPostVertexModel> CreateNewUserPost(urNoticeSession session, string message, string image, string userWallVertexId,
            out HashSet<string> sendNotificationResponse)
        {
            throw new NotImplementedException();
        }

        public ResponseModel<UserPostCommentModel> CreateNewCommentOnUserPost(urNoticeSession session, string message, string image, string postVertexId,
            string userWallVertexId, string postPostedByVertexId, out HashSet<string> sendNotificationResponse)
        {
            throw new NotImplementedException();
        }

        public ResponseModel<string> DeleteCommentOnPost(urNoticeSession session, string vertexId)
        {
            throw new NotImplementedException();
        }

        public ResponseModel<UserVertexModel> CreateNewReactionOnUserPost(urNoticeSession session, UserNewReactionRequest userNewReactionRequest,
            out HashSet<string> sendNotificationResponse)
        {
            throw new NotImplementedException();
        }

        public ResponseModel<string> RemoveReactionOnUserPost(urNoticeSession session, string vertexId)
        {
            throw new NotImplementedException();
        }

        public ResponseModel<string> SeenNotification(string userName)
        {
            throw new NotImplementedException();
        }

        public HashSet<string> SendNotificationToUser(urNoticeSession session, string userWallVertexId, string postVertexId,string commentVertexId,
            string postPostedByVertexId, string notificationType)
        {
            throw new NotImplementedException();
        }

        public SolrQueryResults<UnCompanySolr> CompanyDetailsById(string userVertexId, string cid)
        {
            throw new NotImplementedException();
        }

        public string GetUserNotification(urNoticeSession session, string @from, string to)
        {
            throw new NotImplementedException();
        }

        public string GetUserPost(string userVertexId, string @from, string to, string userEmail)
        {
            throw new NotImplementedException();
        }

        public string GetUserPostMessages(string userVertexId, string @from, string to, string userEmail)
        {
            throw new NotImplementedException();
        }

        public string GetUserPostLikes(string userVertexId, string @from, string to)
        {
            throw new NotImplementedException();
        }

        public string GetPostByVertexId(string vertexId, string userEmail)
        {
            throw new NotImplementedException();
        }

        public long GetUserUnreadNotificationCount(urNoticeSession session)
        {
            throw new NotImplementedException();
        }

        public ResponseModel<string> ValidateAccountService(ValidateAccountRequest req)
        {
            throw new NotImplementedException();
        }

        public ResponseModel<string> ResendValidationCodeService(ValidateAccountRequest req, HttpRequestBase request)
        {
            throw new NotImplementedException();
        }

        public ResponseModel<string> ForgetPasswordService(string id, HttpRequestBase request)
        {
            throw new NotImplementedException();
        }

        public ResponseModel<string> ResetPasswordService(ResetPasswordRequest req)
        {
            throw new NotImplementedException();
        }

        public ResponseModel<string> ContactUsService(ContactUsRequest req)
        {
            throw new NotImplementedException();
        }


        //Company Management.
        //TODO: not used method.
        public Dictionary<string, string> CreateNewCompanyDesignationEdge(urNoticeSession session, string designation,
            string salary, string jobFromYear, string jobToYear, string companyVertexId)
        {
            ICompanyManagement companyManagementModel = new CompanyManagement();
            return companyManagementModel.CreateNewCompanyDesignationEdge(session,designation,salary,jobFromYear,jobToYear,companyVertexId);
        }

        public bool CreateNewDesignation(string designationName, string createdBy)
        {
            ICompanyManagement companyManagementModel = new CompanyManagement();
            return companyManagementModel.CreateNewDesignation(designationName,createdBy);
        }
        public bool CreateNewCompanyDesignationSalary(string companyName, string designationName, string salary,
            string createdBy)
        {
            ICompanyManagement companyManagementModel = new CompanyManagement();
            return companyManagementModel.CreateNewCompanyDesignationSalary(companyName,designationName,salary,createdBy);
        }
        public bool CreateNewCompanyDesignationNoticePeriod(string companyName, string designationName, string noticePeriodRange,
            string createdBy)
        {
            ICompanyManagement companyManagementModel = new CompanyManagement();
            return companyManagementModel.CreateNewCompanyDesignationNoticePeriod(companyName,designationName,noticePeriodRange,createdBy);
        }
        public bool CreateNewCompany(OrbitPageCompany company, string createdBy)
        {
            ICompanyManagement companyManagementModel = new CompanyManagement();
            return companyManagementModel.CreateNewCompany(company,createdBy);
        }
    }
}
