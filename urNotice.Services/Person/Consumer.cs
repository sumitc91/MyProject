using System;
using System.Collections.Generic;
using System.Web;
using urNotice.Common.Infrastructure.Common.Config;
using urNotice.Common.Infrastructure.Common.Constants;
using urNotice.Common.Infrastructure.Common.Enum;
using urNotice.Common.Infrastructure.commonMethods;
using urNotice.Common.Infrastructure.Model.Person;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Common.Infrastructure.Model.urNoticeModel.GraphModel;
using urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper;
using urNotice.Common.Infrastructure.Session;
using urNotice.Common.Infrastructure.signalRPushNotifications;
using urNotice.Services.GraphDb;
using urNotice.Services.GraphDb.GraphDbContract;
using urNotice.Services.NoSqlDb.DynamoDb;
using urNotice.Services.Person.PersonContract.LoginOperation;
using urNotice.Services.Person.PersonContract.RegistrationOperation;
using urNotice.Services.Solr.SolrUser;

namespace urNotice.Services.Person
{
    public class Consumer : IPerson
    {
        public ResponseModel<LoginResponse> RegisterMe(RegisterationRequest req, HttpRequestBase request)
        {
            ISolrUser solrUserModel = new SolrUser();
            IDynamoDb dynamoDbModel = new DynamoDb();
            IGraphDbContract graphDbContractModel = new GraphDbContract();

            IOrbitPageRegistration orbitPageRegistration = new OrbitPagePersonRegistration(solrUserModel, dynamoDbModel, graphDbContractModel);
            orbitPageRegistration.SetIsValidationEmailRequired(true); //email validation is required.
            var response = orbitPageRegistration.RegisterUser(req, request);
            //SendAccountCreationValidationEmail.SendAccountCreationValidationEmailMessage(req, guid, request);
            SignalRController.BroadCastNewUserRegistration();
            return response;
        }

        public ResponseModel<LoginResponse> SocialRegisterMe(RegisterationRequest req, HttpRequestBase request)
        {
            ISolrUser solrUserModel = new SolrUser();
            IDynamoDb dynamoDbModel = new DynamoDb();
            IGraphDbContract graphDbContractModel = new GraphDbContract();

            IOrbitPageRegistration orbitPageRegistration = new OrbitPagePersonRegistration(solrUserModel, dynamoDbModel, graphDbContractModel);
            orbitPageRegistration.SetIsValidationEmailRequired(false); //email validation not required for social.
            var response = orbitPageRegistration.RegisterUser(req, request);
            //SendAccountCreationValidationEmail.SendAccountCreationValidationEmailMessage(req, guid, request);
            SignalRController.BroadCastNewUserRegistration();
            return response;
        }

        public ResponseModel<LoginResponse> Login(string userName, string password, bool isSocialLogin)
        {
            IOrbitPageLogin loginModel = new OrbitPageLogin();

            return loginModel.Login(userName, password, null, CommonConstants.TRUE, isSocialLogin);
            
        }

        public ResponseModel<OrbitPageUser> GetFullUserDetail(string userEmail)
        {
            throw new NotImplementedException();
        }

        public ResponseModel<ClientDetailsModel> GetPersonDetails(string userEmail)
        {
            var response = new ResponseModel<ClientDetailsModel>();

            IDynamoDb dynamoDbModel = new DynamoDb();
            var userInfo = dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTable(
                        DynamoDbHashKeyDataType.OrbitPageUser.ToString(),
                        userEmail,
                        null);

            if (userInfo != null)
            {
                var createClientDetailResponse = new ClientDetailsModel
                {
                    FirstName = userInfo.OrbitPageUser.firstName,
                    LastName = userInfo.OrbitPageUser.lastName,
                    Username = userInfo.OrbitPageUser.email,
                    imageUrl = userInfo.OrbitPageUser.imageUrl == CommonConstants.NA ? CommonConstants.clientImageUrl : userInfo.OrbitPageUser.imageUrl,
                    gender = userInfo.OrbitPageUser.gender,
                    isLocked = userInfo.OrbitPageUser.locked
                };

                response.Status = 200;
                response.Message = "success";
                response.Payload = createClientDetailResponse;

            }
            else
            {
                response.Status = 404;
                response.Message = "username not found";
            }

            return response;
        }

        public ResponseModel<UserPostVertexModel> CreateNewUserPost(urNoticeSession session, string message, string image,string userWallVertexId, out Dictionary<string, string> sendNotificationResponse)
        {
            var response = new ResponseModel<UserPostVertexModel>();
            var properties = new Dictionary<string, string>();

            properties[VertexPropertyEnum.Type.ToString()] = VertexLabelEnum.Post.ToString();
            properties[VertexPropertyEnum.PostMessage.ToString()] = message;
            properties[VertexPropertyEnum.PostedByUser.ToString()] = session.UserName;
            properties[VertexPropertyEnum.PostedTime.ToString()] = DateTimeUtil.GetUtcTimeString();
            properties[VertexPropertyEnum.PostImage.ToString()] = image;

            IGraphVertexDb graphVertexDb = new GraphVertexDb();
            IDictionary<string, string> addVertexResponse = graphVertexDb.AddVertex(session.UserName, TitanGraphConfig.Graph, properties);
            
            properties = new Dictionary<string, string>();
            properties[EdgePropertyEnum._outV.ToString()] = session.UserVertexId;
            properties[EdgePropertyEnum._inV.ToString()] = addVertexResponse[TitanGraphConstants.Id];
            properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.Created.ToString();

            IGraphEdgeDb graphEdgeDbModel = new GraphEdgeDb();
            IDictionary<string, string> addCreatedByEdgeResponse = graphEdgeDbModel.AddEdge(session.UserName, TitanGraphConfig.Graph, properties);//new GraphEdgeOperations().AddEdge(session, TitanGraphConfig.Server, edgeId, TitanGraphConfig.Graph, properties, accessKey, secretKey);

            properties = new Dictionary<string, string>();

            if (userWallVertexId != null && userWallVertexId != session.UserVertexId)
            {
                properties[EdgePropertyEnum._outV.ToString()] = addVertexResponse[TitanGraphConstants.Id];
                properties[EdgePropertyEnum._inV.ToString()] = userWallVertexId;
                properties[EdgePropertyEnum.PostedBy.ToString()] = session.UserVertexId;                
            }
            else
            {
                properties[EdgePropertyEnum._outV.ToString()] = addVertexResponse[TitanGraphConstants.Id];
                properties[EdgePropertyEnum._inV.ToString()] = session.UserVertexId;
                properties[EdgePropertyEnum.PostedBy.ToString()] = session.UserVertexId;
            }


            properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.WallPost.ToString();
            properties[EdgePropertyEnum.PostedDate.ToString()] = DateTimeUtil.GetUtcTimeString();
            properties[EdgePropertyEnum.EdgeMessage.ToString()] = "";

            IDictionary<string, string> addEdgeResponse = graphEdgeDbModel.AddEdge(session.UserName, TitanGraphConfig.Graph, properties);//new GraphEdgeOperations().AddEdge(session, TitanGraphConfig.Server, edgeId, TitanGraphConfig.Graph, properties, accessKey, secretKey);

            sendNotificationResponse = SendNotificationToUser(session, userWallVertexId, addVertexResponse[TitanGraphConstants.Id], null, EdgeLabelEnum.WallPostNotification.ToString());
            var userPostVertexModel = new UserPostVertexModel();
            userPostVertexModel.postInfo = new WallPostVertexModel()
            {
                _id = addVertexResponse[TitanGraphConstants.Id],
                PostedByUser = session.UserName,
                PostedTime = DateTimeUtil.GetUtcTimeString(),
                PostImage = image,
                PostMessage = message
            };

            userPostVertexModel.userInfo = new List<UserVertexModel>();
            var userVertexModel = new UserVertexModel()
            {
                FirstName = session.UserName,//TODO: to fetch first name of user
                LastName = "",
                Username = session.UserName,
                CreatedTime = DateTimeUtil.GetUtcTimeString(),
                _id = userWallVertexId
            };

            userPostVertexModel.userInfo.Add(userVertexModel);

            return response;  
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

        public bool CreateNewCompany(OrbitPageCompany company, string createdBy)
        {
            throw new NotImplementedException();
        }

        private Dictionary<string, string> SendNotificationToUser(urNoticeSession session, string userWallVertexId, string postVertexId, string postPostedByVertexId, string notificationType)
        {
            var properties = new Dictionary<string, string>();
            var response = new Dictionary<string, string>();

            if (notificationType.Equals(EdgeLabelEnum.WallPostNotification.ToString()))
            {
                if (userWallVertexId != session.UserVertexId)
                {
                    properties[EdgePropertyEnum._outV.ToString()] = userWallVertexId;
                    properties[EdgePropertyEnum._inV.ToString()] = postVertexId;

                    properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.Notification.ToString();
                    properties[EdgePropertyEnum.NotificationInitiatedByVertexId.ToString()] = session.UserVertexId;
                    properties[EdgePropertyEnum.Type.ToString()] = EdgeLabelEnum.WallPostNotification.ToString();
                    properties[EdgePropertyEnum.PostedDate.ToString()] = DateTimeUtil.GetUtcTimeString();

                    IGraphEdgeDb graphEdgeDbModel = new GraphEdgeDb();
                    IDictionary<string, string> addEdgeResponse = graphEdgeDbModel.AddEdge(session.UserName, TitanGraphConfig.Graph, properties);//new GraphEdgeOperations().AddEdge(session, TitanGraphConfig.Server, edgeId, TitanGraphConfig.Graph, properties, accessKey, secretKey);

                    if (response.ContainsKey(CommonConstants.PushNotificationArray))
                        response[CommonConstants.PushNotificationArray] = response[CommonConstants.PushNotificationArray] + CommonConstants.CommaDelimeter + userWallVertexId;
                    else
                        response[CommonConstants.PushNotificationArray] = userWallVertexId;
                }

            }
            else if (notificationType.Equals(EdgeLabelEnum.CommentedOnPostNotification.ToString()))
            {
                if (userWallVertexId != session.UserVertexId)
                {
                    properties[EdgePropertyEnum._outV.ToString()] = userWallVertexId;
                    properties[EdgePropertyEnum._inV.ToString()] = postVertexId;

                    properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.Notification.ToString();
                    properties[EdgePropertyEnum.NotificationInitiatedByVertexId.ToString()] = session.UserVertexId;
                    properties[EdgePropertyEnum.Type.ToString()] = EdgeLabelEnum.CommentedOnPostNotification.ToString();
                    properties[EdgePropertyEnum.PostedDate.ToString()] = DateTimeUtil.GetUtcTimeString();

                    IGraphEdgeDb graphEdgeDbModel = new GraphEdgeDb();
                    IDictionary<string, string> addEdgeResponse = graphEdgeDbModel.AddEdge(session.UserName, TitanGraphConfig.Graph, properties);//new GraphEdgeOperations().AddEdge(session, TitanGraphConfig.Server, edgeId, TitanGraphConfig.Graph, properties, accessKey, secretKey);

                    if (response.ContainsKey(CommonConstants.PushNotificationArray))
                        response[CommonConstants.PushNotificationArray] = response[CommonConstants.PushNotificationArray] + CommonConstants.CommaDelimeter + userWallVertexId;
                    else
                        response[CommonConstants.PushNotificationArray] = userWallVertexId;
                }


                if (userWallVertexId != postPostedByVertexId && session.UserVertexId != postPostedByVertexId)
                {
                    properties = new Dictionary<string, string>();

                    properties[EdgePropertyEnum._outV.ToString()] = postPostedByVertexId;
                    properties[EdgePropertyEnum._inV.ToString()] = postVertexId;

                    properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.Notification.ToString();
                    properties[EdgePropertyEnum.NotificationInitiatedByVertexId.ToString()] = session.UserVertexId;
                    properties[EdgePropertyEnum.Type.ToString()] = EdgeLabelEnum.CommentedOnPostNotification.ToString();
                    properties[EdgePropertyEnum.PostedDate.ToString()] = DateTimeUtil.GetUtcTimeString();

                    IGraphEdgeDb graphEdgeDbModel = new GraphEdgeDb();
                    IDictionary<string, string> addEdgeResponse = graphEdgeDbModel.AddEdge(session.UserName, TitanGraphConfig.Graph, properties);//new GraphEdgeOperations().AddEdge(session, TitanGraphConfig.Server, edgeId, TitanGraphConfig.Graph, properties, accessKey, secretKey);

                    if (response.ContainsKey(CommonConstants.PushNotificationArray))
                        response[CommonConstants.PushNotificationArray] = response[CommonConstants.PushNotificationArray] + CommonConstants.CommaDelimeter + postPostedByVertexId;
                    else
                        response[CommonConstants.PushNotificationArray] = postPostedByVertexId;
                }

            }
            else if (notificationType.Equals(EdgeLabelEnum.UserReaction.ToString()))
            {
                if (userWallVertexId != session.UserVertexId)
                {
                    properties[EdgePropertyEnum._outV.ToString()] = userWallVertexId;
                    properties[EdgePropertyEnum._inV.ToString()] = postVertexId;

                    properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.Notification.ToString();
                    properties[EdgePropertyEnum.NotificationInitiatedByVertexId.ToString()] = session.UserVertexId;
                    properties[EdgePropertyEnum.Type.ToString()] = EdgeLabelEnum.UserReaction.ToString();
                    properties[EdgePropertyEnum.PostedDate.ToString()] = DateTimeUtil.GetUtcTimeString();

                    IGraphEdgeDb graphEdgeDbModel = new GraphEdgeDb();
                    IDictionary<string, string> addEdgeResponse = graphEdgeDbModel.AddEdge(session.UserName, TitanGraphConfig.Graph, properties);//new GraphEdgeOperations().AddEdge(session, TitanGraphConfig.Server, edgeId, TitanGraphConfig.Graph, properties, accessKey, secretKey);

                    if (response.ContainsKey(CommonConstants.PushNotificationArray))
                        response[CommonConstants.PushNotificationArray] = response[CommonConstants.PushNotificationArray] + CommonConstants.CommaDelimeter + userWallVertexId;
                    else
                        response[CommonConstants.PushNotificationArray] = userWallVertexId;
                }


                if (userWallVertexId != postPostedByVertexId && session.UserVertexId != postPostedByVertexId)
                {
                    properties = new Dictionary<string, string>();

                    properties[EdgePropertyEnum._outV.ToString()] = postPostedByVertexId;
                    properties[EdgePropertyEnum._inV.ToString()] = postVertexId;

                    properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.Notification.ToString();
                    properties[EdgePropertyEnum.NotificationInitiatedByVertexId.ToString()] = session.UserVertexId;
                    properties[EdgePropertyEnum.Type.ToString()] = EdgeLabelEnum.UserReaction.ToString();
                    properties[EdgePropertyEnum.PostedDate.ToString()] = DateTimeUtil.GetUtcTimeString();

                    IGraphEdgeDb graphEdgeDbModel = new GraphEdgeDb();
                    IDictionary<string, string> addEdgeResponse = graphEdgeDbModel.AddEdge(session.UserName, TitanGraphConfig.Graph, properties);//new GraphEdgeOperations().AddEdge(session, TitanGraphConfig.Server, edgeId, TitanGraphConfig.Graph, properties, accessKey, secretKey);

                    if (response.ContainsKey(CommonConstants.PushNotificationArray))
                        response[CommonConstants.PushNotificationArray] = response[CommonConstants.PushNotificationArray] + CommonConstants.CommaDelimeter + postPostedByVertexId;
                    else
                        response[CommonConstants.PushNotificationArray] = postPostedByVertexId;
                }

            }
            return response;
        }
    }
}
