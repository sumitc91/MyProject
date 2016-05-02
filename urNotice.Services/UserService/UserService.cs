﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
using urNotice.Services.ErrorLogger;
using urNotice.Services.GraphDb;
using urNotice.Services.NoSqlDb.DynamoDb;
using urNotice.Services.Person;

namespace urNotice.Services.UserService
{
    public class UserService
    {
        private static readonly ILogger Logger = new Logger(Convert.ToString(MethodBase.GetCurrentMethod().DeclaringType));        
        
        public string GetUserPost(string userVertexId, string @from, string to, string userEmail)
        {
            string url = TitanGraphConfig.Server;
            string graphName = TitanGraphConfig.Graph;
            int messageStartIndex = 0;
            int messageEndIndex = 4;

            if (userEmail == null) userEmail = string.Empty;

            string gremlinQuery = "g.v(" + userVertexId + ").in('WallPost').sort{ a, b -> b.PostedTime <=> a.PostedTime }._()[" + from + ".." + to + "].transform{ [postInfo : it,likeInfo:it.in('Like')[0..1],likeInfoCount:it.in('Like').count(),isLiked:it.in('Like').has('Username','" + userEmail + "'), commentsInfo: it.in('Comment').sort{ a, b -> b.PostedTime <=> a.PostedTime }._()[" + messageStartIndex + ".." + messageEndIndex + "].transform{[commentInfo:it, commentedBy: it.in('Created'),likeCount:it.in('Like').count(),isLiked:it.in('Like').has('Username','" + userEmail + "')]},userInfo:it.in('Created')] }";            
            
            IGraphVertexDb graphVertexDb = new GraphVertexDb();
            string response = graphVertexDb.GetVertexDetail(gremlinQuery,userVertexId,TitanGraphConfig.Graph,null);

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
            sendNotificationResponse = consumerModel.SendNotificationToUser(session, userWallVertexId, postVertexId,addVertexResponse[TitanGraphConstants.Id], postPostedByVertexId, EdgeLabelEnum.CommentedOnPostNotification.ToString());

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
            sendNotificationResponse = consumerModel.SendNotificationToUser(session, userWallVertexId, vertexId,userNewReactionRequest.VertexId, postPostedByVertexId, EdgeLabelEnum.UserReaction.ToString());

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

        public string GetUserNotification(urNoticeSession session, string from, string to, string accessKey, string secretKey)
        {
            
            string gremlinQuery = "g.v(" + session.UserVertexId + ").outE('Notification').order{it.b.PostedDate <=> it.a.PostedDate}[" + from + ".." + to + "].transform{ [notificationInfo:it,postInfo:it.inV,notificationByUser:g.v(it.NotificationInitiatedByVertexId)]}";

            IGraphVertexDb graphVertexDb = new GraphVertexDb();
            string response = graphVertexDb.GetVertexDetail(gremlinQuery, session.UserVertexId, TitanGraphConfig.Graph, null);//new GraphVertexOperations().GetVertexDetail(url, gremlinQuery, userVertexId, graphName, null);
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
        public string GetPostByVertexId(string vertexId, string userEmail)
        {
            int messageStartIndex = 0;
            int messageEndIndex = 4;
            string gremlinQuery = "g.v(" + vertexId + ").transform{ [postInfo : it,postedToUser:it.out('WallPost'),likeInfo:it.in('Like')[0..1],likeInfoCount:it.in('Like').count(),isLiked:it.in('Like').has('Username','" + userEmail + "'), commentsInfo: it.in('Comment').sort{ a, b -> b.PostedTime <=> a.PostedTime }._()[" + messageStartIndex + ".." + messageEndIndex + "].transform{[commentInfo:it, commentedBy: it.in('Created'),likeCount:it.in('Like').count(),isLiked:it.in('Like').has('Username','" + userEmail + "')]},userInfo:it.in('Created')] }";
            //string gremlinQuery = "g.v(" + userVertexId + ").in('_label','WallPost').sort{it.PostedTime}.reverse()._().as('postInfo')[" + from + ".." + to + "].in('_label','Created').as('userInfo').select{it}{it}";

            IGraphVertexDb graphVertexDb = new GraphVertexDb();
            string response = graphVertexDb.GetVertexDetail(gremlinQuery,vertexId,TitanGraphConfig.Graph,null);//new GraphVertexOperations().GetVertexDetail(url, gremlinQuery, vertexId, graphName, null);

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
    }
}
