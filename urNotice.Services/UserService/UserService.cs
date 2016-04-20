using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using urNotice.Common.Infrastructure.Common.Config;
using urNotice.Common.Infrastructure.Common.Constants;
using urNotice.Common.Infrastructure.Common.Enum;
using urNotice.Common.Infrastructure.commonMethods;
using urNotice.Common.Infrastructure.Model.Person;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Common.Infrastructure.Model.urNoticeModel.GraphModel;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper;
using urNotice.Common.Infrastructure.Session;
using urNotice.Services.ErrorLogger;
using urNotice.Services.GraphDb;
using urNotice.Services.NoSqlDb.DynamoDb;
using urNotice.Services.Person;

namespace urNotice.Services.UserService
{
    public class UserService
    {
        private static readonly ILogger Logger = new Logger(Convert.ToString(MethodBase.GetCurrentMethod().DeclaringType));        
        //private readonly urnoticeAuthEntities _db = new urnoticeAuthEntities();

        //public ResponseModel<ClientDetailsModel> GetClientDetails(string username,string accessKey,string secretKey)
        //{
        //    var response = new ResponseModel<ClientDetailsModel>();

        //    try
        //    {
        //        //var clientDetailDbResult = _db.Users.SingleOrDefault(x => x.username == username);
        //        IDynamoDb dynamoDbModel = new DynamoDb();
        //        var userInfo = dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTable(
        //            DynamoDbHashKeyDataType.OrbitPageUser.ToString(),
        //            username,
        //            null
        //            );

        //        if (userInfo != null)
        //        {
        //            var createClientDetailResponse = new ClientDetailsModel
        //            {
        //                FirstName = userInfo.OrbitPageUser.firstName,
        //                LastName = userInfo.OrbitPageUser.lastName,
        //                Username = userInfo.OrbitPageUser.email,
        //                imageUrl = userInfo.OrbitPageUser.imageUrl == CommonConstants.NA ? CommonConstants.clientImageUrl : userInfo.OrbitPageUser.imageUrl,
        //                gender = userInfo.OrbitPageUser.gender,
        //                isLocked = userInfo.OrbitPageUser.locked
        //            };

        //            response.Status = 200;
        //            response.Message = "success";
        //            response.Payload = createClientDetailResponse;

        //        }
        //        else
        //        {
        //            response.Status = 404;
        //            response.Message = "username not found";
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        response.Status = 500;
        //        response.Message = "exception occured !!!";
        //    }
        //    return response;
        //}

        //public ResponseModel<UserPostVertexModel> CreateNewUserPost(urNoticeSession session, string message, string image, string userWallVertexId, out Dictionary<string, string> sendNotificationResponse)
        //{
        //    var response = new ResponseModel<UserPostVertexModel>();
            
        //    var properties = new Dictionary<string, string>();

        //    properties[VertexPropertyEnum.Type.ToString()] = VertexLabelEnum.Post.ToString();
        //    properties[VertexPropertyEnum.PostMessage.ToString()] = message;
        //    properties[VertexPropertyEnum.PostedByUser.ToString()] = session.UserName;
        //    properties[VertexPropertyEnum.PostedTime.ToString()] = DateTimeUtil.GetUtcTimeString();
        //    properties[VertexPropertyEnum.PostImage.ToString()] = image;

        //    IGraphVertexDb graphVertexDb = new GraphVertexDb();
        //    IDictionary<string, string> addVertexResponse = graphVertexDb.AddVertex(session.UserName,TitanGraphConfig.Graph,properties);//new GraphVertexOperations().AddVertex(session.UserName, TitanGraphConfig.Server, userWallVertexId, TitanGraphConfig.Graph, properties, accessKey, secretKey);

        //    string edgeId = session.UserName + "_" + DateTime.Now.Ticks;
        //    properties = new Dictionary<string, string>();
        //    properties[EdgePropertyEnum._outV.ToString()] = session.UserVertexId;
        //    properties[EdgePropertyEnum._inV.ToString()] = addVertexResponse[TitanGraphConstants.Id];
        //    properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.Created.ToString();

        //    IGraphEdgeDb graphEdgeDbModel = new GraphEdgeDb();
        //    IDictionary<string, string> addCreatedByEdgeResponse = graphEdgeDbModel.AddEdge(session.UserName,TitanGraphConfig.Graph,properties);//new GraphEdgeOperations().AddEdge(session, TitanGraphConfig.Server, edgeId, TitanGraphConfig.Graph, properties, accessKey, secretKey);
            
        //    //_outV=<id>&_label=friend&_inV=2&<key>=<key'>
        //    edgeId = session.UserName + "_" + DateTime.Now.Ticks;
        //    properties = new Dictionary<string, string>();

        //    if (userWallVertexId != null && userWallVertexId != session.UserVertexId)
        //    {
        //        properties[EdgePropertyEnum._outV.ToString()] = addVertexResponse[TitanGraphConstants.Id];
        //        properties[EdgePropertyEnum._inV.ToString()] = userWallVertexId;
        //        properties[EdgePropertyEnum.PostedBy.ToString()] = session.UserVertexId;
        //        //if (response.ContainsKey(CommonConstants.PushNotificationArray))
        //        //    response[CommonConstants.PushNotificationArray] = response[CommonConstants.PushNotificationArray] + CommonConstants.CommaDelimeter + userWallVertexId;
        //        //else
        //        //    response[CommonConstants.PushNotificationArray] = userWallVertexId;
        //    }
        //    else
        //    {
        //        properties[EdgePropertyEnum._outV.ToString()] = addVertexResponse[TitanGraphConstants.Id];
        //        properties[EdgePropertyEnum._inV.ToString()] = session.UserVertexId;
        //        properties[EdgePropertyEnum.PostedBy.ToString()] = session.UserVertexId;
        //    }
            
            
        //    properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.WallPost.ToString();
        //    properties[EdgePropertyEnum.PostedDate.ToString()] = DateTimeUtil.GetUtcTimeString();
        //    properties[EdgePropertyEnum.EdgeMessage.ToString()] = "";

        //    IDictionary<string, string> addEdgeResponse = graphEdgeDbModel.AddEdge(session.UserName,TitanGraphConfig.Graph,properties);//new GraphEdgeOperations().AddEdge(session, TitanGraphConfig.Server, edgeId, TitanGraphConfig.Graph, properties, accessKey, secretKey);

        //    sendNotificationResponse = SendNotificationToUser(session, userWallVertexId, addVertexResponse[TitanGraphConstants.Id],null, EdgeLabelEnum.WallPostNotification.ToString());
        //    var userPostVertexModel = new UserPostVertexModel();
        //    userPostVertexModel.postInfo = new WallPostVertexModel()
        //    {
        //        _id = addVertexResponse[TitanGraphConstants.Id],
        //        PostedByUser = session.UserName,
        //        PostedTime = DateTimeUtil.GetUtcTimeString(),
        //        PostImage = image,
        //        PostMessage = message
        //    };

        //    userPostVertexModel.userInfo = new List<UserVertexModel>();
        //    var userVertexModel = new UserVertexModel()
        //    {
        //        FirstName = session.UserName,//TODO: to fetch first name of user
        //        LastName = "",
        //        Username = session.UserName,
        //        CreatedTime = DateTimeUtil.GetUtcTimeString(),
        //        _id = userWallVertexId
        //    };

        //    userPostVertexModel.userInfo.Add(userVertexModel);

        //    return response;
        //}

        public Dictionary<string, string> CreateNewCompanyDesignationEdge(urNoticeSession session, string designation, string salary,string jobFromYear,string jobToYear,string companyVertexId)
        {
            
            var properties = new Dictionary<string, string>();

            properties[VertexPropertyEnum.Type.ToString()] = VertexLabelEnum.Salary.ToString();
            properties[VertexPropertyEnum.DesignationName.ToString()] = designation;
            properties[VertexPropertyEnum.Salary.ToString()] = salary;
            properties[VertexPropertyEnum.PostedTime.ToString()] = DateTimeUtil.GetUtcTimeString();
            properties[VertexPropertyEnum.JobFromYear.ToString()] = jobFromYear;
            properties[VertexPropertyEnum.JobToYear.ToString()] = jobToYear;

            IGraphVertexDb graphVertexDb = new GraphVertexDb();
            IDictionary<string, string> addVertexResponse = graphVertexDb.AddVertex(session.UserName, TitanGraphConfig.Graph, properties);//new GraphVertexOperations().AddVertex(session.UserName, TitanGraphConfig.Server, userWallVertexId, TitanGraphConfig.Graph, properties, accessKey, secretKey);

            properties = new Dictionary<string, string>();
            properties[EdgePropertyEnum._outV.ToString()] = session.UserVertexId;
            properties[EdgePropertyEnum._inV.ToString()] = addVertexResponse[TitanGraphConstants.Id];
            properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.Salary.ToString();

            IGraphEdgeDb graphEdgeDbModel = new GraphEdgeDb();
            IDictionary<string, string> addCreatedByEdgeResponse = graphEdgeDbModel.AddEdge(session.UserName, TitanGraphConfig.Graph, properties);//new GraphEdgeOperations().AddEdge(session, TitanGraphConfig.Server, edgeId, TitanGraphConfig.Graph, properties, accessKey, secretKey);

            //_outV=<id>&_label=friend&_inV=2&<key>=<key'>
            properties = new Dictionary<string, string>();

            
            properties[EdgePropertyEnum._outV.ToString()] = addVertexResponse[TitanGraphConstants.Id];
            properties[EdgePropertyEnum._inV.ToString()] = companyVertexId;
            properties[EdgePropertyEnum.PostedBy.ToString()] = session.UserVertexId;
            


            properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.WallPost.ToString();
            properties[EdgePropertyEnum.PostedDate.ToString()] = DateTimeUtil.GetUtcTimeString();
            properties[EdgePropertyEnum.EdgeMessage.ToString()] = "";

            IDictionary<string, string> addEdgeResponse = graphEdgeDbModel.AddEdge(session.UserName, TitanGraphConfig.Graph, properties);//new GraphEdgeOperations().AddEdge(session, TitanGraphConfig.Server, edgeId, TitanGraphConfig.Graph, properties, accessKey, secretKey);

            //var sendNotificationResponse = SendNotificationToUser(session, userWallVertexId, addVertexResponse[TitanGraphConstants.Id], null, EdgeLabelEnum.WallPostNotification.ToString(), accessKey, secretKey);

            //foreach (var kvp in sendNotificationResponse)
            //{
            //    if (response.ContainsKey(CommonConstants.PushNotificationArray))
            //        response[CommonConstants.PushNotificationArray] = response[CommonConstants.PushNotificationArray] +
            //                                                          CommonConstants.CommaDelimeter + kvp.Value;
            //}


            //sendNotificationResponse["status"] = "200";
            //return sendNotificationResponse;
            return null;
        }

        //private Dictionary<string, string> SendNotificationToUser(urNoticeSession session, string userWallVertexId, string postVertexId, string postPostedByVertexId, string notificationType)
        //{
        //    var properties = new Dictionary<string, string>();
        //    var response = new Dictionary<string, string>();
            
        //    var edgeId = session.UserName + "_" + DateTime.Now.Ticks;

        //    if (notificationType.Equals(EdgeLabelEnum.WallPostNotification.ToString()))
        //    {
        //        if (userWallVertexId != session.UserVertexId)
        //        {
        //            properties[EdgePropertyEnum._outV.ToString()] = userWallVertexId;
        //            properties[EdgePropertyEnum._inV.ToString()] = postVertexId;

        //            properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.Notification.ToString();
        //            properties[EdgePropertyEnum.NotificationInitiatedByVertexId.ToString()] = session.UserVertexId;
        //            properties[EdgePropertyEnum.Type.ToString()] = EdgeLabelEnum.WallPostNotification.ToString();
        //            properties[EdgePropertyEnum.PostedDate.ToString()] = DateTimeUtil.GetUtcTimeString();

        //            IGraphEdgeDb graphEdgeDbModel = new GraphEdgeDb();
        //            IDictionary<string, string> addEdgeResponse = graphEdgeDbModel.AddEdge(session.UserName,TitanGraphConfig.Graph,properties);//new GraphEdgeOperations().AddEdge(session, TitanGraphConfig.Server, edgeId, TitanGraphConfig.Graph, properties, accessKey, secretKey);

        //            if (response.ContainsKey(CommonConstants.PushNotificationArray))
        //                response[CommonConstants.PushNotificationArray] = response[CommonConstants.PushNotificationArray] + CommonConstants.CommaDelimeter + userWallVertexId;
        //            else
        //                response[CommonConstants.PushNotificationArray] = userWallVertexId;
        //        }
                
        //    }
        //    else if (notificationType.Equals(EdgeLabelEnum.CommentedOnPostNotification.ToString()))
        //    {
        //        if (userWallVertexId != session.UserVertexId)
        //        {
        //            properties[EdgePropertyEnum._outV.ToString()] = userWallVertexId;
        //            properties[EdgePropertyEnum._inV.ToString()] =postVertexId;

        //            properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.Notification.ToString();
        //            properties[EdgePropertyEnum.NotificationInitiatedByVertexId.ToString()] = session.UserVertexId;
        //            properties[EdgePropertyEnum.Type.ToString()] = EdgeLabelEnum.CommentedOnPostNotification.ToString();
        //            properties[EdgePropertyEnum.PostedDate.ToString()] = DateTimeUtil.GetUtcTimeString();

        //            IGraphEdgeDb graphEdgeDbModel = new GraphEdgeDb();
        //            IDictionary<string, string> addEdgeResponse = graphEdgeDbModel.AddEdge(session.UserName,TitanGraphConfig.Graph,properties);//new GraphEdgeOperations().AddEdge(session, TitanGraphConfig.Server, edgeId, TitanGraphConfig.Graph, properties, accessKey, secretKey);

        //            if (response.ContainsKey(CommonConstants.PushNotificationArray))
        //                response[CommonConstants.PushNotificationArray] = response[CommonConstants.PushNotificationArray] + CommonConstants.CommaDelimeter + userWallVertexId;
        //            else
        //                response[CommonConstants.PushNotificationArray] = userWallVertexId;
        //        }


        //        if (userWallVertexId != postPostedByVertexId && session.UserVertexId != postPostedByVertexId)
        //        {
        //            properties = new Dictionary<string, string>();

        //            properties[EdgePropertyEnum._outV.ToString()] = postPostedByVertexId;
        //            properties[EdgePropertyEnum._inV.ToString()] = postVertexId;

        //            properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.Notification.ToString();
        //            properties[EdgePropertyEnum.NotificationInitiatedByVertexId.ToString()] = session.UserVertexId;
        //            properties[EdgePropertyEnum.Type.ToString()] = EdgeLabelEnum.CommentedOnPostNotification.ToString();
        //            properties[EdgePropertyEnum.PostedDate.ToString()] = DateTimeUtil.GetUtcTimeString();

        //            IGraphEdgeDb graphEdgeDbModel = new GraphEdgeDb();
        //            IDictionary<string, string> addEdgeResponse = graphEdgeDbModel.AddEdge(session.UserName,TitanGraphConfig.Graph,properties);//new GraphEdgeOperations().AddEdge(session, TitanGraphConfig.Server, edgeId, TitanGraphConfig.Graph, properties, accessKey, secretKey);

        //            if (response.ContainsKey(CommonConstants.PushNotificationArray))
        //                response[CommonConstants.PushNotificationArray] = response[CommonConstants.PushNotificationArray] + CommonConstants.CommaDelimeter + postPostedByVertexId;
        //            else
        //                response[CommonConstants.PushNotificationArray] = postPostedByVertexId;
        //        }
                
        //    }
        //    else if (notificationType.Equals(EdgeLabelEnum.UserReaction.ToString()))
        //    {
        //        if (userWallVertexId != session.UserVertexId)
        //        {
        //            properties[EdgePropertyEnum._outV.ToString()] = userWallVertexId;
        //            properties[EdgePropertyEnum._inV.ToString()] = postVertexId;

        //            properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.Notification.ToString();
        //            properties[EdgePropertyEnum.NotificationInitiatedByVertexId.ToString()] = session.UserVertexId;
        //            properties[EdgePropertyEnum.Type.ToString()] = EdgeLabelEnum.UserReaction.ToString();
        //            properties[EdgePropertyEnum.PostedDate.ToString()] = DateTimeUtil.GetUtcTimeString();

        //            IGraphEdgeDb graphEdgeDbModel = new GraphEdgeDb();
        //            IDictionary<string, string> addEdgeResponse = graphEdgeDbModel.AddEdge(session.UserName, TitanGraphConfig.Graph, properties);//new GraphEdgeOperations().AddEdge(session, TitanGraphConfig.Server, edgeId, TitanGraphConfig.Graph, properties, accessKey, secretKey);

        //            if (response.ContainsKey(CommonConstants.PushNotificationArray))
        //                response[CommonConstants.PushNotificationArray] = response[CommonConstants.PushNotificationArray] + CommonConstants.CommaDelimeter + userWallVertexId;
        //            else
        //                response[CommonConstants.PushNotificationArray] = userWallVertexId;
        //        }


        //        if (userWallVertexId != postPostedByVertexId && session.UserVertexId != postPostedByVertexId)
        //        {
        //            properties = new Dictionary<string, string>();

        //            properties[EdgePropertyEnum._outV.ToString()] = postPostedByVertexId;
        //            properties[EdgePropertyEnum._inV.ToString()] = postVertexId;

        //            properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.Notification.ToString();
        //            properties[EdgePropertyEnum.NotificationInitiatedByVertexId.ToString()] = session.UserVertexId;
        //            properties[EdgePropertyEnum.Type.ToString()] = EdgeLabelEnum.UserReaction.ToString();
        //            properties[EdgePropertyEnum.PostedDate.ToString()] = DateTimeUtil.GetUtcTimeString();

        //            IGraphEdgeDb graphEdgeDbModel = new GraphEdgeDb();
        //            IDictionary<string, string> addEdgeResponse = graphEdgeDbModel.AddEdge(session.UserName, TitanGraphConfig.Graph, properties);//new GraphEdgeOperations().AddEdge(session, TitanGraphConfig.Server, edgeId, TitanGraphConfig.Graph, properties, accessKey, secretKey);

        //            if (response.ContainsKey(CommonConstants.PushNotificationArray))
        //                response[CommonConstants.PushNotificationArray] = response[CommonConstants.PushNotificationArray] + CommonConstants.CommaDelimeter + postPostedByVertexId;
        //            else
        //                response[CommonConstants.PushNotificationArray] = postPostedByVertexId;
        //        }

        //    }
        //    return response;
        //}

        public string GetUserPost(string userVertexId, string @from, string to, string userEmail)
        {
            string url = TitanGraphConfig.Server;
            string graphName = TitanGraphConfig.Graph;
            int messageStartIndex = 0;
            int messageEndIndex = 4;

            if (userEmail == null) userEmail = string.Empty;
            
            string gremlinQuery = "g.v(" + userVertexId + ").in('WallPost').sort{ a, b -> b.PostedTime <=> a.PostedTime }._()[" + from + ".." + to + "].transform{ [postInfo : it,likeInfo:it.in('Like')[0..1],likeInfoCount:it.in('Like').count(),isLiked:it.in('Like').has('Username','" + userEmail + "'), commentsInfo: it.in('Comment').sort{ a, b -> b.PostedTime <=> a.PostedTime }._()[" + messageStartIndex + ".." + messageEndIndex + "].transform{[commentInfo:it, commentedBy: it.in('Created')]},userInfo:it.in('Created')] }";            
            
            IGraphVertexDb graphVertexDb = new GraphVertexDb();
            string response = graphVertexDb.GetVertexDetail(gremlinQuery,userVertexId,TitanGraphConfig.Graph,null);

            return response;
        }

        public string GetUserPostMessages(string userVertexId, string @from, string to)
        {
            string url = TitanGraphConfig.Server;
            string graphName = TitanGraphConfig.Graph;

            
            string gremlinQuery = "g.v(" + userVertexId + ").in('Comment').sort{ a, b -> b.PostedTime <=> a.PostedTime }._()["+from+".."+to+"].transform{[commentInfo:it, commentedBy: it.in('Created')]}";
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

        public ResponseModel<UserPostCommentModel> CreateNewCommentOnUserPost(urNoticeSession session, string message, string image, string postVertexId, string userWallVertexId, string postPostedByVertexId, out Dictionary<string, string> sendNotificationResponse)
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

            IGraphVertexDb graphVertexDb = new GraphVertexDb();
            IDictionary<string, string> addVertexResponse = graphVertexDb.AddVertex(session.UserName,TitanGraphConfig.Graph,properties);//new GraphVertexOperations().AddVertex(session.UserName, TitanGraphConfig.Server, postVertexId, TitanGraphConfig.Graph, properties, accessKey, secretKey);

            string edgeId = session.UserName + "_" + DateTime.Now.Ticks;
            properties = new Dictionary<string, string>();
            properties[EdgePropertyEnum._outV.ToString()] = session.UserVertexId;
            properties[EdgePropertyEnum._inV.ToString()] = addVertexResponse[TitanGraphConstants.Id];
            properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.Created.ToString();

            IGraphEdgeDb graphEdgeDbModel = new GraphEdgeDb();
            IDictionary<string, string> addCreatedByEdgeResponse = graphEdgeDbModel.AddEdge(session.UserName,TitanGraphConfig.Graph,properties);//new GraphEdgeOperations().AddEdge(session, TitanGraphConfig.Server, edgeId, TitanGraphConfig.Graph, properties, accessKey, secretKey);

            //_outV=<id>&_label=friend&_inV=2&<key>=<key'>
            edgeId = session.UserName + "_" + DateTime.Now.Ticks;
            properties = new Dictionary<string, string>();
            
            properties[EdgePropertyEnum._outV.ToString()] = addVertexResponse[TitanGraphConstants.Id];
            properties[EdgePropertyEnum._inV.ToString()] = postVertexId;
            properties[EdgePropertyEnum.PostedBy.ToString()] = session.UserVertexId;

            properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.Comment.ToString();
            properties[EdgePropertyEnum.PostedDate.ToString()] = DateTimeUtil.GetUtcTimeString();
            properties[EdgePropertyEnum.EdgeMessage.ToString()] = "";

            IDictionary<string, string> addEdgeResponse = graphEdgeDbModel.AddEdge(session.UserName,TitanGraphConfig.Graph,properties);//new GraphEdgeOperations().AddEdge(session, TitanGraphConfig.Server, edgeId, TitanGraphConfig.Graph, properties, accessKey, secretKey);

            IPerson consumerModel = new Consumer();
            sendNotificationResponse = consumerModel.SendNotificationToUser(session, userWallVertexId, postVertexId, postPostedByVertexId, EdgeLabelEnum.CommentedOnPostNotification.ToString());

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

        public ResponseModel<UserVertexModel> CreateNewReactionOnUserPost(urNoticeSession session, string reaction, string vertexId, string userWallVertexId, string postPostedByVertexId, out Dictionary<string, string> sendNotificationResponse)
        {
            var response = new ResponseModel<UserVertexModel>();
            
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

            IPerson consumerModel = new Consumer();
            sendNotificationResponse = consumerModel.SendNotificationToUser(session, userWallVertexId, vertexId, postPostedByVertexId, EdgeLabelEnum.UserReaction.ToString());

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

            if (session == null || session.UserName ==null)
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
        public string GetUserNotification(string userVertexId,string from,string to, string accessKey, string secretKey)
        {
            
            string gremlinQuery = "g.v(" + userVertexId + ").outE('Notification').order{it.b.PostedDate <=> it.a.PostedDate}[" + from + ".." + to + "].transform{ [notificationInfo:it,postInfo:it.inV,notificationByUser:g.v(it.NotificationInitiatedByVertexId)]}";

            IGraphVertexDb graphVertexDb = new GraphVertexDb();
            string response = graphVertexDb.GetVertexDetail(gremlinQuery,userVertexId,TitanGraphConfig.Graph,null);//new GraphVertexOperations().GetVertexDetail(url, gremlinQuery, userVertexId, graphName, null);
            return response;
        }

        public string GetPostByVertexId(string vertexId, string userEmail)
        {
            int messageStartIndex = 0;
            int messageEndIndex = 4;
            string gremlinQuery = "g.v(" + vertexId + ").transform{ [postInfo : it,postedToUser:it.out('WallPost'),likeInfo:it.in('Like')[0..1],likeInfoCount:it.in('Like').count(),isLiked:it.in('Like').has('Username','" + userEmail + "'), commentsInfo: it.in('Comment').sort{ a, b -> b.PostedTime <=> a.PostedTime }._()[" + messageStartIndex + ".." + messageEndIndex + "].transform{[commentInfo:it, commentedBy: it.in('Created')]},userInfo:it.in('Created')] }";
            //string gremlinQuery = "g.v(" + userVertexId + ").in('_label','WallPost').sort{it.PostedTime}.reverse()._().as('postInfo')[" + from + ".." + to + "].in('_label','Created').as('userInfo').select{it}{it}";

            IGraphVertexDb graphVertexDb = new GraphVertexDb();
            string response = graphVertexDb.GetVertexDetail(gremlinQuery,vertexId,TitanGraphConfig.Graph,null);//new GraphVertexOperations().GetVertexDetail(url, gremlinQuery, vertexId, graphName, null);

            return response;
        }
    }
}
