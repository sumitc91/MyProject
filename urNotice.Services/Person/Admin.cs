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
            var response = new ResponseModel<OrbitPageUser>();
            IDynamoDb dynamoDbModel = new DynamoDb();
            var userInfo = dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTable(
                DynamoDbHashKeyDataType.OrbitPageUser.ToString(),
                userEmail,
                null
                );

            response.Status = 200;
            response.Message = "success";
            response.Payload = userInfo.OrbitPageUser;
            return response;
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

        public ResponseModel<string> SeenNotification(string userName)
        {
            throw new NotImplementedException();
        }

        public HashSet<string> SendNotificationToUser(urNoticeSession session, string userWallVertexId, string postVertexId,
            string postPostedByVertexId, string notificationType)
        {
            throw new NotImplementedException();
        }

        public SolrQueryResults<UnCompanySolr> CompanyDetailsById(string userVertexId, string cid)
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

        //TODO: not used method.
        public Dictionary<string, string> CreateNewCompanyDesignationEdge(urNoticeSession session, string designation,
            string salary, string jobFromYear, string jobToYear, string companyVertexId)
        {
            var properties = new Dictionary<string, string>();

            properties[VertexPropertyEnum.Type.ToString()] = VertexLabelEnum.Salary.ToString();
            properties[VertexPropertyEnum.DesignationName.ToString()] = designation;
            properties[VertexPropertyEnum.Salary.ToString()] = salary;
            properties[VertexPropertyEnum.PostedTime.ToString()] = DateTimeUtil.GetUtcTimeString();
            properties[VertexPropertyEnum.JobFromYear.ToString()] = jobFromYear;
            properties[VertexPropertyEnum.JobToYear.ToString()] = jobToYear;

            var hashSet = new HashSet<String>() { session.UserVertexId };
            //var canEdit = new HashSet<String>() { session.UserVertexId};
            //var canDelete = new HashSet<String>() { session.UserVertexId };
            //var sendNotificationToUsers = new HashSet<String>() { session.UserVertexId };

            IGraphVertexDb graphVertexDb = new GraphVertexDb();
            IDictionary<string, string> addVertexResponse = graphVertexDb.AddVertex(session.UserName, TitanGraphConfig.Graph, properties, hashSet, hashSet, hashSet);

            properties = new Dictionary<string, string>();
            properties[EdgePropertyEnum._outV.ToString()] = session.UserVertexId;
            properties[EdgePropertyEnum._inV.ToString()] = addVertexResponse[TitanGraphConstants.Id];
            properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.Salary.ToString();

            IGraphEdgeDb graphEdgeDbModel = new GraphEdgeDb();
            IDictionary<string, string> addCreatedByEdgeResponse = graphEdgeDbModel.AddEdge(session.UserName, TitanGraphConfig.Graph, properties);
            
            properties = new Dictionary<string, string>();

            properties[EdgePropertyEnum._outV.ToString()] = addVertexResponse[TitanGraphConstants.Id];
            properties[EdgePropertyEnum._inV.ToString()] = companyVertexId;
            properties[EdgePropertyEnum.PostedBy.ToString()] = session.UserVertexId;

            properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.WallPost.ToString();
            properties[EdgePropertyEnum.PostedDate.ToString()] = DateTimeUtil.GetUtcTimeString();
            properties[EdgePropertyEnum.EdgeMessage.ToString()] = "";

            IDictionary<string, string> addEdgeResponse = graphEdgeDbModel.AddEdge(session.UserName, TitanGraphConfig.Graph, properties);
            return null;
        }

        public bool CreateNewDesignation(string designationName, string createdBy)
        {
            IGraphDbContract graphDbContract = new GraphDbContract();
            var response = graphDbContract.InsertNewDesignationInGraphDb(createdBy, designationName);

            IDynamoDb dynamoDbModel = new DynamoDb();
            dynamoDbModel.UpsertOrbitPageDesignation(designationName, response[TitanGraphConstants.Id]);

            ISolrDesignation solrDesignationModel = new SolrDesignation();
            solrDesignationModel.AddDesignation(response[TitanGraphConstants.Id], designationName, false);

            return true;
        }

        public bool CreateNewCompanyDesignationSalary(string companyName, string designationName, string salary,
            string createdBy)
        {
            //IGraphDbContract graphDbContract = new GraphDbContract();
            //var response = graphDbContract.InsertNewDesignationInGraphDb(createdBy, designationName);

            //IDynamoDb dynamoDbModel = new DynamoDb();
            //dynamoDbModel.UpsertOrbitPageDesignation(designationName, response[TitanGraphConstants.Id]);

            ISolrDesignation solrDesignationModel = new SolrDesignation();
            ISolrCompany solrCompanyModel = new SolrCompany();
            var designationDetails = solrDesignationModel.GetAbsoluteDesignationDetail(designationName);
            if (designationDetails != null && designationDetails.Count > 0)
            {
                var companyDetails = solrCompanyModel.GetAbsoluteCompanyDetailsAutocomplete(companyName);
                if (companyDetails != null && companyDetails.Count > 0)
                {
                    Dictionary<string, string> properties = new Dictionary<string, string>();
                    properties = new Dictionary<string, string>();
                    properties[EdgePropertyEnum._outV.ToString()] = companyDetails[0].guid;
                    properties[EdgePropertyEnum._inV.ToString()] = designationDetails[0].vertexId;
                    properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.Salary.ToString();
                    properties[EdgePropertyEnum.SalaryAmount.ToString()] = "(i," + Convert.ToString(Convert.ToDouble(salary) * 100000) + ")";
                    properties[EdgePropertyEnum.PostedDate.ToString()] = DateTimeUtil.GetUtcTimeString();

                    IGraphEdgeDb graphEdgeDbModel = new GraphEdgeDb();
                    IDictionary<string, string> addCreatedByEdgeResponse = graphEdgeDbModel.AddEdge(createdBy, TitanGraphConfig.Graph, properties);//new GraphEdgeOperations().AddEdge(session, TitanGraphConfig.Server, edgeId, TitanGraphConfig.Graph, properties, accessKey, secretKey);

                }
            }
            //solrDesignationModel.AddDesignation(response[TitanGraphConstants.Id], designationName, false);

            return true;
        }

        public bool CreateNewCompanyDesignationNoticePeriod(string companyName, string designationName, string noticePeriodRange,
            string createdBy)
        {
            //IGraphDbContract graphDbContract = new GraphDbContract();
            //var response = graphDbContract.InsertNewDesignationInGraphDb(createdBy, designationName);

            //IDynamoDb dynamoDbModel = new DynamoDb();
            //dynamoDbModel.UpsertOrbitPageDesignation(designationName, response[TitanGraphConstants.Id]);

            ISolrDesignation solrDesignationModel = new SolrDesignation();
            ISolrCompany solrCompanyModel = new SolrCompany();
            var designationDetails = solrDesignationModel.GetAbsoluteDesignationDetail(designationName);
            if (designationDetails != null && designationDetails.Count > 0)
            {
                var companyDetails = solrCompanyModel.GetAbsoluteCompanyDetailsAutocomplete(companyName);
                if (companyDetails != null && companyDetails.Count > 0)
                {
                    Dictionary<string, string> properties = new Dictionary<string, string>();
                    properties = new Dictionary<string, string>();
                    properties[EdgePropertyEnum._outV.ToString()] = companyDetails[0].guid;
                    properties[EdgePropertyEnum._inV.ToString()] = designationDetails[0].vertexId;
                    properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.NoticePeriodRange.ToString();
                    properties[EdgePropertyEnum.RangeValue.ToString()] = noticePeriodRange;
                    properties[EdgePropertyEnum.PostedDate.ToString()] = DateTimeUtil.GetUtcTimeString();

                    IGraphEdgeDb graphEdgeDbModel = new GraphEdgeDb();
                    IDictionary<string, string> addCreatedByEdgeResponse = graphEdgeDbModel.AddEdge(createdBy, TitanGraphConfig.Graph, properties);//new GraphEdgeOperations().AddEdge(session, TitanGraphConfig.Server, edgeId, TitanGraphConfig.Graph, properties, accessKey, secretKey);

                }
            }
            //solrDesignationModel.AddDesignation(response[TitanGraphConstants.Id], designationName, false);

            return true;
        }

        public bool CreateNewCompany(OrbitPageCompany company, string createdBy)
        {
            IGraphDbContract graphDbContract = new GraphDbContract();
            var response = graphDbContract.InsertNewCompanyInGraphDb(createdBy, company.CompanyName);

            IDynamoDb dynamoDbModel = new DynamoDb();
            dynamoDbModel.UpsertOrbitPageCompany(company, response[TitanGraphConstants.Id]);

            company.vertexId = response[TitanGraphConstants.Id];
            ISolrCompany solrCompanyModel = new SolrCompany();
            solrCompanyModel.InsertNewCompany(company, false);
            return true;
        }
    }
}
