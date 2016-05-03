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

        public ResponseModel<EditPersonModel> EditPersonDetails(urNoticeSession session, EditPersonModel editPersonModel)
        {
            var response = new ResponseModel<EditPersonModel>();

            if (session.UserName != editPersonModel.Email)
            {
                response.Status = 401;
                response.Message = "Unauthenticated";
                return response;
            }

            IDynamoDb dynamoDbModel = new DynamoDb();
            var userInfo = dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTable(
                        DynamoDbHashKeyDataType.OrbitPageUser.ToString(),
                        editPersonModel.Email,
                        null);            
            if (userInfo != null)
            {
                //update dynamodb
                var orbitPageCompanyUserWorkgraphyTable = new OrbitPageCompanyUserWorkgraphyTable();
                orbitPageCompanyUserWorkgraphyTable = GenerateOrbitPageUserObject(userInfo, editPersonModel);                                
                dynamoDbModel.CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(orbitPageCompanyUserWorkgraphyTable);

                //update graphDb
                var properties = new Dictionary<string, string>();
                properties[VertexPropertyEnum.Type.ToString()] = VertexLabelEnum.User.ToString();

                if(!string.IsNullOrEmpty(editPersonModel.FirstName))
                    properties[VertexPropertyEnum.FirstName.ToString()] = editPersonModel.FirstName;

                if(!string.IsNullOrEmpty(editPersonModel.LastName))
                    properties[VertexPropertyEnum.LastName.ToString()] = editPersonModel.LastName;
                

                if(!string.IsNullOrEmpty(editPersonModel.ImageUrl))
                    properties[VertexPropertyEnum.ImageUrl.ToString()] = editPersonModel.ImageUrl;
                
                if(!string.IsNullOrEmpty(editPersonModel.CoverPic))
                    properties[VertexPropertyEnum.CoverImageUrl.ToString()] = editPersonModel.CoverPic;

                IGraphVertexDb graphVertexDbModel = new GraphVertexDb();
                graphVertexDbModel.UpdateVertex(userInfo.OrbitPageUser.vertexId, editPersonModel.Email,TitanGraphConfig.Graph,properties);

                //update solr
                ISolrUser solrUserModel = new SolrUser();
                solrUserModel.InsertNewUser(orbitPageCompanyUserWorkgraphyTable.OrbitPageUser,false);

                response.Status = 200;
                response.Message = "success";
                response.Payload = editPersonModel;
            }
            else
            {
                response.Status = 404;
                response.Message = "username not found";
            }

            return response;
        }

        public ResponseModel<string> EditMessageDetails(urNoticeSession session, EditMessageRequest messageReq)
        {
            var response = new ResponseModel<string>();

            if (session.UserVertexId != messageReq.userVertex)
            {
                response.Status = 401;
                response.Message = "Unauthenticated";
                return response;
            }

            
            //update graphDb
            var properties = new Dictionary<string, string>();
            
            if (!string.IsNullOrEmpty(messageReq.message))
                properties[VertexPropertyEnum.PostMessage.ToString()] = messageReq.message;
            
            if (!string.IsNullOrEmpty(messageReq.imageUrl))
                properties[VertexPropertyEnum.PostImage.ToString()] = messageReq.imageUrl;
            else
                properties[VertexPropertyEnum.PostImage.ToString()] = "";
            
            IGraphVertexDb graphVertexDbModel = new GraphVertexDb();
            graphVertexDbModel.UpdateVertex(messageReq.messageVertex, session.UserName, TitanGraphConfig.Graph, properties);            
            
            response.Status = 200;
            response.Message = "success";
            response.Payload = "successfully edited.";
            
            return response;
        }

        private OrbitPageCompanyUserWorkgraphyTable GenerateOrbitPageUserObject(
            OrbitPageCompanyUserWorkgraphyTable orbitPageCompanyUserWorkgraphyTable, EditPersonModel editPersonModel)
        {
            
            orbitPageCompanyUserWorkgraphyTable.OrbitPageUser.firstName = editPersonModel.FirstName;
            orbitPageCompanyUserWorkgraphyTable.OrbitPageUser.lastName = editPersonModel.LastName;
            orbitPageCompanyUserWorkgraphyTable.OrbitPageUser.imageUrl = editPersonModel.ImageUrl;
            orbitPageCompanyUserWorkgraphyTable.OrbitPageUser.userCoverPic = editPersonModel.CoverPic;            
            orbitPageCompanyUserWorkgraphyTable.OrbitPageUser.lastUpdatedDate = DateTimeUtil.GetUtcTime();

            return orbitPageCompanyUserWorkgraphyTable;
        }

        public ResponseModel<UserPostVertexModel> CreateNewUserPost(urNoticeSession session, string message, string image, string userWallVertexId, out HashSet<string> sendNotificationResponse)
        {
            var response = new ResponseModel<UserPostVertexModel>();
            var properties = new Dictionary<string, string>();

            properties[VertexPropertyEnum.Type.ToString()] = VertexLabelEnum.Post.ToString();
            properties[VertexPropertyEnum.PostMessage.ToString()] = message;
            properties[VertexPropertyEnum.PostedByUser.ToString()] = session.UserName;
            properties[VertexPropertyEnum.PostedTime.ToString()] = DateTimeUtil.GetUtcTimeString();
            properties[VertexPropertyEnum.PostedTimeLong.ToString()] = OrbitPageUtil.GetCurrentTimeStampForGraphDb();
            properties[VertexPropertyEnum.PostImage.ToString()] = image;

            
            var canEdit = new HashSet<String>() { session.UserVertexId};
            
            var canDelete = new HashSet<String>();
            canDelete.Add(session.UserVertexId);            
            canDelete.Add(userWallVertexId);

            var sendNotificationToUsers = new HashSet<String>();
            sendNotificationToUsers.Add(session.UserVertexId);
            sendNotificationToUsers.Add(userWallVertexId);

            IGraphVertexDb graphVertexDb = new GraphVertexDb();
            IDictionary<string, string> addVertexResponse = graphVertexDb.AddVertex(session.UserName, TitanGraphConfig.Graph, properties, canEdit, canDelete, sendNotificationToUsers);
            
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
            properties[EdgePropertyEnum.PostedDateLong.ToString()] = OrbitPageUtil.GetCurrentTimeStampForGraphDb();
            //properties[EdgePropertyEnum.EdgeMessage.ToString()] = "";

            IDictionary<string, string> addEdgeResponse = graphEdgeDbModel.AddEdge(session.UserName, TitanGraphConfig.Graph, properties);//new GraphEdgeOperations().AddEdge(session, TitanGraphConfig.Server, edgeId, TitanGraphConfig.Graph, properties, accessKey, secretKey);

            sendNotificationResponse = SendNotificationToUser(session, userWallVertexId, addVertexResponse[TitanGraphConstants.Id],addVertexResponse[TitanGraphConstants.Id], null, EdgeLabelEnum.WallPostNotification.ToString());
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

        public ResponseModel<UserPostCommentModel> CreateNewCommentOnUserPost(urNoticeSession session, string message, string image, string postVertexId, string userWallVertexId, string postPostedByVertexId, out HashSet<string> sendNotificationResponse)
        {
            var response = new ResponseModel<UserPostCommentModel>();
            //string url = TitanGraphConfig.Server;
            //string graphName = TitanGraphConfig.Graph;

            var properties = new Dictionary<string, string>();
            properties[VertexPropertyEnum.PostMessage.ToString()] = message;
            properties[VertexPropertyEnum.PostedByUser.ToString()] = session.UserName;
            properties[VertexPropertyEnum.PostedTime.ToString()] = DateTimeUtil.GetUtcTimeString();
            properties[VertexPropertyEnum.PostImage.ToString()] = image;
            properties[VertexPropertyEnum.Type.ToString()] = VertexLabelEnum.Comment.ToString();

            var canEdit = new HashSet<String>() { session.UserVertexId };

            var canDelete = new HashSet<String>();
            canDelete.Add(session.UserVertexId);
            canDelete.Add(userWallVertexId);

            var sendNotificationToUsers = new HashSet<String>();
            sendNotificationToUsers.Add(session.UserVertexId);
            sendNotificationToUsers.Add(userWallVertexId);

            IGraphVertexDb graphVertexDb = new GraphVertexDb();
            IDictionary<string, string> addVertexResponse = graphVertexDb.AddVertex(session.UserName, TitanGraphConfig.Graph, properties, canEdit, canDelete, sendNotificationToUsers);//new GraphVertexOperations().AddVertex(session.UserName, TitanGraphConfig.Server, postVertexId, TitanGraphConfig.Graph, properties, accessKey, secretKey);

            string edgeId = session.UserName + "_" + DateTime.Now.Ticks;
            properties = new Dictionary<string, string>();
            properties[EdgePropertyEnum._outV.ToString()] = session.UserVertexId;
            properties[EdgePropertyEnum._inV.ToString()] = addVertexResponse[TitanGraphConstants.Id];
            properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.Created.ToString();

            IGraphEdgeDb graphEdgeDbModel = new GraphEdgeDb();
            IDictionary<string, string> addCreatedByEdgeResponse = graphEdgeDbModel.AddEdge(session.UserName, TitanGraphConfig.Graph, properties);//new GraphEdgeOperations().AddEdge(session, TitanGraphConfig.Server, edgeId, TitanGraphConfig.Graph, properties, accessKey, secretKey);

            //_outV=<id>&_label=friend&_inV=2&<key>=<key'>
            edgeId = session.UserName + "_" + DateTime.Now.Ticks;
            properties = new Dictionary<string, string>();

            properties[EdgePropertyEnum._outV.ToString()] = addVertexResponse[TitanGraphConstants.Id];
            properties[EdgePropertyEnum._inV.ToString()] = postVertexId;
            properties[EdgePropertyEnum.PostedBy.ToString()] = session.UserVertexId;

            properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.Comment.ToString();
            properties[EdgePropertyEnum.PostedDate.ToString()] = DateTimeUtil.GetUtcTimeString();
            properties[EdgePropertyEnum.EdgeMessage.ToString()] = "";

            IDictionary<string, string> addEdgeResponse = graphEdgeDbModel.AddEdge(session.UserName, TitanGraphConfig.Graph, properties);//new GraphEdgeOperations().AddEdge(session, TitanGraphConfig.Server, edgeId, TitanGraphConfig.Graph, properties, accessKey, secretKey);

            IPerson consumerModel = new Consumer();
            sendNotificationResponse = consumerModel.SendNotificationToUser(session, userWallVertexId, postVertexId, addVertexResponse[TitanGraphConstants.Id], postPostedByVertexId, EdgeLabelEnum.CommentedOnPostNotification.ToString());

            var userPostCommentModel = new UserPostCommentModel();
            userPostCommentModel.commentedBy = new List<UserVertexModel>();

            var userVertexModel = new UserVertexModel()
            {
                FirstName = session.UserName,//TODO: to fetch first name of user
                LastName = "",
                Username = session.UserName,
                CreatedTime = DateTimeUtil.GetUtcTimeString(),
                _id = userWallVertexId
            };

            userPostCommentModel.commentedBy.Add(userVertexModel);

            userPostCommentModel.commentInfo = new WallPostVertexModel()
            {
                _id = addVertexResponse[TitanGraphConstants.Id],
                PostedByUser = session.UserName,
                PostedTime = DateTimeUtil.GetUtcTimeString(),
                PostImage = image,
                PostMessage = message
            };

            response.Payload = userPostCommentModel;
            response.Status = 200;

            return response;
        }

        public ResponseModel<String> DeleteCommentOnPost(urNoticeSession session, string vertexId)
        {
            var response = new ResponseModel<String>();

            if (session == null || session.UserName == null)
            {
                response.Status = 401;
                response.Message = "Unauthenticated";
                return response;
            }

            //IGraphEdgeDb graphEdgeDbModel = new GraphEdgeDb();
            //graphEdgeDbModel.DeleteEdge(vertexId, session.UserVertexId, UserReactionEnum.Like.ToString());

            IGraphVertexDb graphVertexDbModel = new GraphVertexDb();
            graphVertexDbModel.DeleteVertex(vertexId, session.UserVertexId, VertexLabelEnum.Comment.ToString());

            response.Payload = "success";
            response.Status = 200;

            return response;
        }

        public ResponseModel<UserVertexModel> CreateNewReactionOnUserPost(urNoticeSession session, UserNewReactionRequest userNewReactionRequest, out HashSet<string> sendNotificationResponse)
        {
            var response = new ResponseModel<UserVertexModel>();
            var reaction = userNewReactionRequest.Reaction;

            var vertexId = userNewReactionRequest.VertexId;

            var userWallVertexId = userNewReactionRequest.WallVertexId;
            var postPostedByVertexId = userNewReactionRequest.PostPostedByVertexId;

            var properties = new Dictionary<string, string>();
            if (reaction.Equals(UserReactionEnum.Like.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                //properties[EdgePropertyEnum.Reaction.ToString()] = UserReactionEnum.Reacted.ToString();
                properties[EdgePropertyEnum._label.ToString()] = UserReactionEnum.Like.ToString();
            }

            properties[EdgePropertyEnum._outV.ToString()] = session.UserVertexId;
            properties[EdgePropertyEnum._inV.ToString()] = vertexId;
            properties[EdgePropertyEnum.PostedDate.ToString()] = DateTimeUtil.GetUtcTimeString();


            IGraphEdgeDb graphEdgeDbModel = new GraphEdgeDb();
            IDictionary<string, string> addCreatedByEdgeResponse = graphEdgeDbModel.AddEdge(session.UserName, TitanGraphConfig.Graph, properties);//new GraphEdgeOperations().AddEdge(session, TitanGraphConfig.Server, edgeId, TitanGraphConfig.Graph, properties, accessKey, secretKey);

            //if comment is liked.
            if (!userNewReactionRequest.IsParentPost)
                vertexId = userNewReactionRequest.ParentVertexId;

            IPerson consumerModel = new Consumer();
            sendNotificationResponse = consumerModel.SendNotificationToUser(session, userWallVertexId, vertexId, userNewReactionRequest.VertexId, postPostedByVertexId, EdgeLabelEnum.UserReaction.ToString());

            var userLikeInfo = new UserVertexModel()
            {
                _id = session.UserVertexId,
            };

            response.Payload = userLikeInfo;
            response.Status = 200;

            return response;
        }

        public ResponseModel<String> RemoveReactionOnUserPost(urNoticeSession session, string vertexId)
        {
            var response = new ResponseModel<String>();

            if (session == null || session.UserName == null)
            {
                response.Status = 401;
                response.Message = "Unauthenticated";
                return response;
            }

            IGraphEdgeDb graphEdgeDbModel = new GraphEdgeDb();
            graphEdgeDbModel.DeleteEdge(vertexId, session.UserVertexId, UserReactionEnum.Like.ToString());

            response.Payload = "success";
            response.Status = 200;

            return response;
        }
        public SolrQueryResults<UnCompanySolr> CompanyDetailsById(string userVertexId,string cid)
        {
            ISolrCompany solrCompanyModel = new SolrCompany();
            var response = solrCompanyModel.CompanyDetailsById(cid);

            if (userVertexId != null)
            {
                IGraphDbContract graphDbContractModel = new GraphDbContract();
                //graphDbContractModel.
            }

            return response;
        }

        public string GetUserNotification(urNoticeSession session, string from, string to, string accessKey, string secretKey)
        {

            string gremlinQuery = "g.v(" + session.UserVertexId + ").outE('Notification').order{it.b.PostedDate <=> it.a.PostedDate}[" + from + ".." + to + "].transform{ [notificationInfo:it,postInfo:it.inV,notificationByUser:g.v(it.NotificationInitiatedByVertexId)]}";

            IGraphVertexDb graphVertexDb = new GraphVertexDb();
            string response = graphVertexDb.GetVertexDetail(gremlinQuery, session.UserVertexId, TitanGraphConfig.Graph, null);//new GraphVertexOperations().GetVertexDetail(url, gremlinQuery, userVertexId, graphName, null);
            return response;
        }

        public string GetUserPost(string userVertexId, string @from, string to, string userEmail)
        {
            string url = TitanGraphConfig.Server;
            string graphName = TitanGraphConfig.Graph;
            int messageStartIndex = 0;
            int messageEndIndex = 4;

            if (userEmail == null) userEmail = string.Empty;

            string gremlinQuery = "g.v(" + userVertexId + ").in('WallPost').sort{ a, b -> b.PostedTime <=> a.PostedTime }._()[" + from + ".." + to + "].transform{ [postInfo : it,likeInfo:it.in('Like')[0..1],likeInfoCount:it.in('Like').count(),isLiked:it.in('Like').has('Username','" + userEmail + "'), commentsInfo: it.in('Comment').sort{ a, b -> b.PostedTime <=> a.PostedTime }._()[" + messageStartIndex + ".." + messageEndIndex + "].transform{[commentInfo:it, commentedBy: it.in('Created'),likeCount:it.in('Like').count(),isLiked:it.in('Like').has('Username','" + userEmail + "')]},userInfo:it.in('Created')] }";

            IGraphVertexDb graphVertexDb = new GraphVertexDb();
            string response = graphVertexDb.GetVertexDetail(gremlinQuery, userVertexId, TitanGraphConfig.Graph, null);

            return response;
        }

        public string GetUserPostMessages(string userVertexId, string @from, string to, string userEmail)
        {
            string url = TitanGraphConfig.Server;
            string graphName = TitanGraphConfig.Graph;


            string gremlinQuery = "g.v(" + userVertexId + ").in('Comment').sort{ a, b -> b.PostedTime <=> a.PostedTime }._()[" + from + ".." + to + "].transform{[commentInfo:it, commentedBy: it.in('Created'),,likeCount:it.in('Like').count(),isLiked:it.in('Like').has('Username','" + userEmail + "')]}";
            //string gremlinQuery = "g.v(" + userVertexId + ").in('_label','WallPost').sort{it.PostedTime}.reverse()._().as('postInfo')[" + from + ".." + to + "].in('_label','Created').as('userInfo').select{it}{it}";

            IGraphVertexDb graphVertexDb = new GraphVertexDb();
            string response = graphVertexDb.GetVertexDetail(gremlinQuery, userVertexId, TitanGraphConfig.Graph, null);//new GraphVertexOperations().GetVertexDetail(url, gremlinQuery, userVertexId, graphName, null);

            return response;
        }

        public string GetUserPostLikes(string userVertexId, string @from, string to)
        {
            string url = TitanGraphConfig.Server;
            string graphName = TitanGraphConfig.Graph;


            string gremlinQuery = "g.v(" + userVertexId + ").in('Like').sort{ a, b -> b.PostedTime <=> a.PostedTime }._()[" + from + ".." + to + "].transform{[likeInfo:it]}";
            //string gremlinQuery = "g.v(" + userVertexId + ").in('_label','WallPost').sort{it.PostedTime}.reverse()._().as('postInfo')[" + from + ".." + to + "].in('_label','Created').as('userInfo').select{it}{it}";

            IGraphVertexDb graphVertexDb = new GraphVertexDb();
            string response = graphVertexDb.GetVertexDetail(gremlinQuery, userVertexId, TitanGraphConfig.Graph, null);//new GraphVertexOperations().GetVertexDetail(url, gremlinQuery, userVertexId, graphName, null);

            return response;
        }

        public string GetPostByVertexId(string vertexId, string userEmail)
        {
            int messageStartIndex = 0;
            int messageEndIndex = 4;
            string gremlinQuery = "g.v(" + vertexId + ").transform{ [postInfo : it,postedToUser:it.out('WallPost'),likeInfo:it.in('Like')[0..1],likeInfoCount:it.in('Like').count(),isLiked:it.in('Like').has('Username','" + userEmail + "'), commentsInfo: it.in('Comment').sort{ a, b -> b.PostedTime <=> a.PostedTime }._()[" + messageStartIndex + ".." + messageEndIndex + "].transform{[commentInfo:it, commentedBy: it.in('Created'),likeCount:it.in('Like').count(),isLiked:it.in('Like').has('Username','" + userEmail + "')]},userInfo:it.in('Created')] }";
            //string gremlinQuery = "g.v(" + userVertexId + ").in('_label','WallPost').sort{it.PostedTime}.reverse()._().as('postInfo')[" + from + ".." + to + "].in('_label','Created').as('userInfo').select{it}{it}";

            IGraphVertexDb graphVertexDb = new GraphVertexDb();
            string response = graphVertexDb.GetVertexDetail(gremlinQuery, vertexId, TitanGraphConfig.Graph, null);//new GraphVertexOperations().GetVertexDetail(url, gremlinQuery, vertexId, graphName, null);

            return response;
        }

        public long GetUserUnreadNotificationCount(urNoticeSession session)
        {
            IDynamoDb dynamoDbModel = new DynamoDb();
            long? lastNotificationSeenTimeStamp =
                dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTableLastSeenNotifiationTimeStamp(session.UserName);
            string gremlinQuery = "g.v(" + session.UserVertexId + ").outE('Notification').has('PostedDateLong',T.gte," + lastNotificationSeenTimeStamp + ").count()";

            IGraphVertexDb graphVertexDb = new GraphVertexDb();
            string response = graphVertexDb.GetVertexDetail(gremlinQuery, session.UserVertexId, TitanGraphConfig.Graph, null);//new GraphVertexOperations().GetVertexDetail(url, gremlinQuery, userVertexId, graphName, null);

            var getUserUnreadNotificationsDeserialized =
                        JsonConvert.DeserializeObject<UserPostUnreadNotificationsResponse>(response);

            return getUserUnreadNotificationsDeserialized.results[0];
        }

        public ResponseModel<string> ValidateAccountService(ValidateAccountRequest req)
        {
            var response = new ResponseModel<string>();

            IDynamoDb dynamoDbModel = new DynamoDb();
            var userInfo = dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTable(
                DynamoDbHashKeyDataType.OrbitPageUser.ToString(),
                req.userName,
                null
                );            

            if (userInfo != null)
            {

                if (userInfo.OrbitPageUser == null)
                {
                    response.Status = 500;
                    response.Message = "Internal Server Error";                    
                    return response;
                }
                if (userInfo.OrbitPageUser.isActive == "true")
                {
                    response.Status = 405;
                    response.Message = "already active user";
                    return response;
                }
                userInfo.OrbitPageUser.isActive = "true";

                try
                {
                    dynamoDbModel.CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(userInfo);

                }
                catch (Exception ex)
                {
                    //Todo:need to log exception.
                    response.Status = 500;
                    response.Message = "Failed";
                    return response;
                }
                response.Status = 200;
                response.Message = "validated";
                return response;
            }
            response.Status = 402;
            response.Message = "link expired";
            return response;
        }

        public ResponseModel<string> ResendValidationCodeService(ValidateAccountRequest req, HttpRequestBase request)
        {
            var response = new ResponseModel<string>();
            IDynamoDb dynamoDbModel = new DynamoDb();
            var userInfo = dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTable(
                DynamoDbHashKeyDataType.OrbitPageUser.ToString(),
                req.userName,
                null
                );

            if (userInfo != null)
            {
                if (userInfo.OrbitPageUser.isActive == CommonConstants.TRUE)
                {
                    // Account has been already validated.
                    response.Status = 402;
                    response.Message = "warning";
                    return response;
                }

                userInfo.OrbitPageUser.validateUserKeyGuid = Guid.NewGuid().ToString();
                try
                {
                    dynamoDbModel.CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(userInfo);
                    SendAccountCreationValidationEmail.SendAccountValidationEmailMessage(req.userName, userInfo.OrbitPageUser.validateUserKeyGuid, request);
                }
                catch (Exception e)
                {
                    //DbContextException.LogDbContextException(e);
                    response.Status = 500;
                    response.Message = "Internal Server Error !!!";
                    return response;
                }
                response.Status = 200;
                response.Message = "success";
                return response;
            }
            // User Doesn't Exist
            response.Status = 404;
            response.Message = "warning";
            return response;
        }

        public ResponseModel<string> ForgetPasswordService(string id, HttpRequestBase request)
        {
            var response = new ResponseModel<string>();
            id = id.ToLower();

            IDynamoDb dynamoDbModel = new DynamoDb();
            var userInfo = dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTable(
                DynamoDbHashKeyDataType.OrbitPageUser.ToString(),
                id,
                null
                );

            if (userInfo != null)
            {
                if ((userInfo.OrbitPageUser.isActive.Equals(CommonConstants.FALSE, StringComparison.InvariantCulture)))
                {
                    // User account has not validated yet
                    response.Status = 402;
                    response.Message = "warning";
                    return response;
                }

                userInfo.OrbitPageUser.forgetPasswordGuid = Guid.NewGuid().ToString();
                try
                {
                    dynamoDbModel.CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(userInfo);

                    var forgetPasswordValidationEmail = new ForgetPasswordValidationEmail();
                    forgetPasswordValidationEmail.SendForgetPasswordValidationEmailMessage(id, userInfo.OrbitPageUser.forgetPasswordGuid, request, DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture));
                }
                catch (Exception e)
                {
                    //DbContextException.LogDbContextException(e);
                    response.Status = 500;
                    response.Message = "Internal Server Error";
                    return response;
                }
            }
            else
            {
                // User doesn't exist
                response.Status = 404;
                response.Message = "warning";
                return response;
            }
            response.Status = 200;
            response.Message = "Success";
            return response;
        }

        public ResponseModel<string> ResetPasswordService(ResetPasswordRequest req)
        {
            var response = new ResponseModel<string>();
            
            IDynamoDb dynamoDbModel = new DynamoDb();
            var userInfo = dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTable(
                DynamoDbHashKeyDataType.OrbitPageUser.ToString(),
                req.Username,
                null
                );

            if (userInfo != null && userInfo.OrbitPageUser.forgetPasswordGuid == req.Guid && userInfo.OrbitPageUser.forgetPasswordGuid != CommonConstants.NA)
            {
                userInfo.OrbitPageUser.forgetPasswordGuid = CommonConstants.NA;

                var password = EncryptionClass.Md5Hash(req.Password);
                userInfo.OrbitPageUser.password = password;
                userInfo.OrbitPageUser.locked = CommonConstants.FALSE;

                try
                {
                    dynamoDbModel.CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(userInfo);
                }
                catch (Exception e)
                {
                    //DbContextException.LogDbContextException(e);
                    response.Status = 500;
                    response.Message = "Internal Server Error.";                    
                    return response;
                }
                response.Status = 200;
                response.Message = "Success";
                return response;
            }
            response.Status = 402;
            response.Message = "link expired";
            return response;
        }

        public ResponseModel<string> ContactUsService(ContactUsRequest req)
        {
            //TODO: save in some database..
            var response = new ResponseModel<string>();
            var contactUsData = new ContactUs
            {
                Name = req.Name,
                Phone = req.Phone,
                RepliedBy = CommonConstants.NA,
                RepliedDateTime = CommonConstants.NA,
                ReplyMessage = CommonConstants.NA,
                Status = CommonConstants.status_open,
                Type = req.Type,
                dateTime = DateTime.Now,
                emailId = req.Email,
                heading = CommonConstants.NA,
                message = req.Message,
                username = req.Email
            };

            //_db.contactUs.Add(contactUsData);

            try
            {
                //_db.SaveChanges();
                //var contactUsEmailDelegate = ContactUsEmailSendDelegate(SendContactUsEmail.SendContactUsEmailMessage);

                string emailIds = req.SendMeACopy.Equals(CommonConstants.status_true,
                    StringComparison.CurrentCultureIgnoreCase)
                    ? ServerConfig.ContactUsReceivingEmailIds + "," + req.Email
                    : ServerConfig.ContactUsReceivingEmailIds;

                //contactUsEmailDelegate.BeginInvoke(emailIds, req, null, null); //invoking the method

                //SendAccountCreationValidationEmail.SendContactUsEmailMessage(req.SendMeACopy.Equals(Constants.status_true,StringComparison.CurrentCultureIgnoreCase) ? ConfigurationManager.AppSettings["ContactUsReceivingEmailIds"].ToString(CultureInfo.InvariantCulture)+","+req.Email : ConfigurationManager.AppSettings["ContactUsReceivingEmailIds"].ToString(CultureInfo.InvariantCulture), req);
            }
            catch (Exception e)
            {
                //DbContextException.LogDbContextException(e);
                response.Status = 500;
                response.Message = "Internal Server Error.";                
                return response;
            }
            response.Status = 200;
            response.Message = "success";
            return response;
        }

        public ResponseModel<string> SeenNotification(string userName)
        {
            var response = new ResponseModel<string>();
            IDynamoDb dynamoDbModel = new DynamoDb();
            dynamoDbModel.UpsertOrbitPageUpdateLastNotificationSeenTimeStamp(userName, DateTimeUtil.GetUtcTime().Ticks);
            response.Status = 200;
            response.Message = "success";
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

        public bool CreateNewCompanyDesignationNoticePeriod(string companyName, string designationName, string noticePeriodRange,
            string createdBy)
        {
            throw new NotImplementedException();
        }

        public bool CreateNewCompany(OrbitPageCompany company, string createdBy)
        {
            throw new NotImplementedException();
        }

        public HashSet<string> SendNotificationToUser(urNoticeSession session, string userWallVertexId, string postVertexId,string commentVertexId, string postPostedByVertexId, string notificationType)
        {            
            var userPushNotificationListWrapper = new UserPushNotificationListWrapper();
            userPushNotificationListWrapper.UserNotificationGraphModelList = new List<UserNotificationGraphModel>();
            userPushNotificationListWrapper.SignalRNotificationIds = new HashSet<string>();
            var orbitPageCompanyUserWorkgraphyTable = new OrbitPageCompanyUserWorkgraphyTable();
            var usersToBeNotified = new HashSet<string>();
            var usersToBeIgnored = new HashSet<string>();

            var finalSendNotificationsToUser = new HashSet<string>();

            var notificationModel = new UserNotificationGraphModel();

            if (notificationType.Equals(EdgeLabelEnum.WallPostNotification.ToString()))
            {
                orbitPageCompanyUserWorkgraphyTable = GetUsersToBeNotifiedForVertex(postVertexId);
                if (orbitPageCompanyUserWorkgraphyTable != null &&
                    orbitPageCompanyUserWorkgraphyTable.SendNotificationToUserList != null)
                    usersToBeNotified = orbitPageCompanyUserWorkgraphyTable.SendNotificationToUserList;

                finalSendNotificationsToUser = usersToBeNotified;
                if (userWallVertexId == session.UserVertexId)
                {
                    usersToBeIgnored.Add(userWallVertexId);
                }

                if (usersToBeNotified.Contains(session.UserVertexId))
                {
                    usersToBeIgnored.Add(session.UserVertexId);
                }

                finalSendNotificationsToUser.ExceptWith(usersToBeIgnored);

                foreach (var userIds in finalSendNotificationsToUser)
                {
                    notificationModel = new UserNotificationGraphModel
                    {
                        _outV = userIds,
                        _inV = postVertexId,
                        _label = EdgeLabelEnum.Notification.ToString(),
                        NotificationInitiatedByVertexId = session.UserVertexId,
                        Type = EdgeLabelEnum.WallPostNotification.ToString(),
                        UserName = session.UserName,
                        parentPostId = postVertexId
                    };
                    userPushNotificationListWrapper.UserNotificationGraphModelList.Add(notificationModel);
                    userPushNotificationListWrapper.SignalRNotificationIds.Add(userIds);
                }

            }
            else if (notificationType.Equals(EdgeLabelEnum.CommentedOnPostNotification.ToString()))
            {
                orbitPageCompanyUserWorkgraphyTable = GetUsersToBeNotifiedForVertex(postVertexId);
                if (orbitPageCompanyUserWorkgraphyTable != null &&
                    orbitPageCompanyUserWorkgraphyTable.SendNotificationToUserList != null)
                    usersToBeNotified = orbitPageCompanyUserWorkgraphyTable.SendNotificationToUserList;

                finalSendNotificationsToUser = usersToBeNotified;
                if (userWallVertexId == session.UserVertexId)
                {
                    usersToBeIgnored.Add(userWallVertexId);
                }

                if (userWallVertexId == postPostedByVertexId || session.UserVertexId == postPostedByVertexId)
                {
                    usersToBeIgnored.Add(userWallVertexId);
                }

                if (usersToBeNotified.Contains(session.UserVertexId))
                {
                    usersToBeIgnored.Add(session.UserVertexId);
                }

                finalSendNotificationsToUser.ExceptWith(usersToBeIgnored);
                foreach (var userIds in finalSendNotificationsToUser)
                {
                    notificationModel = new UserNotificationGraphModel
                    {
                        _outV = userIds,
                        _inV = commentVertexId,
                        _label = EdgeLabelEnum.Notification.ToString(),
                        NotificationInitiatedByVertexId = session.UserVertexId,
                        Type = EdgeLabelEnum.CommentedOnPostNotification.ToString(),
                        UserName = session.UserName,
                        parentPostId = postVertexId
                    };
                    userPushNotificationListWrapper.UserNotificationGraphModelList.Add(notificationModel);
                    userPushNotificationListWrapper.SignalRNotificationIds.Add(userIds);
                }

            }
            else if (notificationType.Equals(EdgeLabelEnum.UserReaction.ToString()))
            {
                orbitPageCompanyUserWorkgraphyTable = GetUsersToBeNotifiedForVertex(commentVertexId);
                if (orbitPageCompanyUserWorkgraphyTable != null &&
                    orbitPageCompanyUserWorkgraphyTable.SendNotificationToUserList != null)
                    usersToBeNotified = orbitPageCompanyUserWorkgraphyTable.SendNotificationToUserList;

                finalSendNotificationsToUser = usersToBeNotified;
                if (userWallVertexId == session.UserVertexId)
                {
                    usersToBeIgnored.Add(userWallVertexId);
                }

                if (userWallVertexId == postPostedByVertexId || session.UserVertexId == postPostedByVertexId)
                {
                    usersToBeIgnored.Add(userWallVertexId);
                }

                if (usersToBeNotified.Contains(session.UserVertexId))
                {
                    usersToBeIgnored.Add(session.UserVertexId);
                }

                finalSendNotificationsToUser.ExceptWith(usersToBeIgnored);
                foreach (var userIds in finalSendNotificationsToUser)
                {
                    notificationModel = new UserNotificationGraphModel
                    {
                        _outV = userIds,
                        _inV = commentVertexId,
                        _label = EdgeLabelEnum.Notification.ToString(),
                        NotificationInitiatedByVertexId = session.UserVertexId,
                        Type = EdgeLabelEnum.UserReaction.ToString(),
                        UserName = session.UserName,
                        parentPostId = postVertexId
                    };
                    userPushNotificationListWrapper.UserNotificationGraphModelList.Add(notificationModel);
                    userPushNotificationListWrapper.SignalRNotificationIds.Add(userIds);
                }
            }

            SendNotificationListToUsers(userPushNotificationListWrapper);

            if (notificationType.Equals(EdgeLabelEnum.CommentedOnPostNotification.ToString()))
            {
                if (orbitPageCompanyUserWorkgraphyTable != null && orbitPageCompanyUserWorkgraphyTable.SendNotificationToUserList != null && orbitPageCompanyUserWorkgraphyTable.SendNotificationToUserList.Add(session.UserVertexId))
                {
                    IDynamoDb dynamoDbModel = new DynamoDb();
                    dynamoDbModel.CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(orbitPageCompanyUserWorkgraphyTable);
                }
            }

            return userPushNotificationListWrapper.SignalRNotificationIds;
        }

        private OrbitPageCompanyUserWorkgraphyTable GetUsersToBeNotifiedForVertex(string vertexId)
        {
            IDynamoDb dynamoDbModel =new  DynamoDb();
            var orbitPageCompanyUserWorkgraphyTable = dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTable(DynamoDbHashKeyDataType.VertexDetail.ToString(), vertexId, null);
            if (orbitPageCompanyUserWorkgraphyTable != null)
                return orbitPageCompanyUserWorkgraphyTable;

            return null;
        }

        private void SendNotificationListToUsers(UserPushNotificationListWrapper userPushNotificationListWrapper)
        {
            foreach (var userNotificationGraphModel in userPushNotificationListWrapper.UserNotificationGraphModelList)
            {
                var properties = new Dictionary<string, string>();
                properties[EdgePropertyEnum._outV.ToString()] = userNotificationGraphModel._outV;
                properties[EdgePropertyEnum._inV.ToString()] = userNotificationGraphModel._inV;

                properties[EdgePropertyEnum._label.ToString()] = userNotificationGraphModel._label;
                properties[EdgePropertyEnum.NotificationInitiatedByVertexId.ToString()] = userNotificationGraphModel.NotificationInitiatedByVertexId;
                properties[EdgePropertyEnum.ParentPostId.ToString()] = userNotificationGraphModel.parentPostId;
                properties[EdgePropertyEnum.Type.ToString()] = userNotificationGraphModel.Type;
                properties[EdgePropertyEnum.PostedDate.ToString()] = DateTimeUtil.GetUtcTimeString();
                properties[EdgePropertyEnum.PostedDateLong.ToString()] = OrbitPageUtil.GetCurrentTimeStampForGraphDb();

                IGraphEdgeDb graphEdgeDbModel = new GraphEdgeDb();
                IDictionary<string, string> addEdgeResponse = graphEdgeDbModel.AddEdge(userNotificationGraphModel.UserName, TitanGraphConfig.Graph, properties);
            }            
        }

    }
}
