using System;
using System.Collections.Generic;
using System.Linq;
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
using urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper;
using urNotice.Common.Infrastructure.Model.urNoticeModel.User;
using urNotice.Common.Infrastructure.Session;
using urNotice.Services.GraphDb;
using urNotice.Services.Management.NotificationManagement;
using urNotice.Services.NoSqlDb.DynamoDb;
using urNotice.Services.Person;

namespace urNotice.Services.Management.PostManagement
{
    public class PostManagement : IPostManagement
    {
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
        public ResponseModel<UserPostVertexModel> CreateNewUserPost(urNoticeSession session, string message, string image, string userWallVertexId,List<TaggedVertexIdModel> taggedVertexId, out HashSet<string> sendNotificationResponse)
        {
            var response = new ResponseModel<UserPostVertexModel>();
            var properties = new Dictionary<string, string>();

            properties[VertexPropertyEnum.Type.ToString()] = VertexLabelEnum.Post.ToString();
            properties[VertexPropertyEnum.PostMessage.ToString()] = message;
            properties[VertexPropertyEnum.PostedByUser.ToString()] = session.UserName;
            properties[VertexPropertyEnum.PostedTime.ToString()] = DateTimeUtil.GetUtcTimeString();
            properties[VertexPropertyEnum.PostedTimeLong.ToString()] = OrbitPageUtil.GetCurrentTimeStampForGraphDb();
            properties[VertexPropertyEnum.PostImage.ToString()] = image;


            var canEdit = new HashSet<String>() { session.UserVertexId };

            var canDelete = new HashSet<String>();
            canDelete.Add(session.UserVertexId);
            canDelete.Add(userWallVertexId);

            var sendNotificationToUsers = new HashSet<String>();
            sendNotificationToUsers.Add(session.UserVertexId);
            sendNotificationToUsers.Add(userWallVertexId);

            IGraphVertexDb graphVertexDb = new GraphVertexDb();
            IDictionary<string, string> addVertexResponse = graphVertexDb.AddVertex(session.UserName, TitanGraphConfig.Graph, properties, canEdit, canDelete, sendNotificationToUsers);

            response.Payload = new UserPostVertexModel();
            response.Payload.postInfo = new WallPostVertexModel()
            {
                _id = addVertexResponse[TitanGraphConstants.Id]
            };


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

            INotificationManagement notificationManagement = new NotificationManagement.NotificationManagement();
            sendNotificationResponse = notificationManagement.SendNotificationToUser(session, userWallVertexId, addVertexResponse[TitanGraphConstants.Id], addVertexResponse[TitanGraphConstants.Id], null, EdgeLabelEnum.WallPostNotification.ToString(), taggedVertexId);
            
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
        public ResponseModel<UserPostCommentModel> CreateNewCommentOnUserPost(urNoticeSession session, string message, string image, string postVertexId, string userWallVertexId, string postPostedByVertexId,List<TaggedVertexIdModel> taggedVertexId, out HashSet<string> sendNotificationResponse)
        {
            var response = new ResponseModel<UserPostCommentModel>();
            response.Payload = new UserPostCommentModel();
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
            
            response.Payload.commentInfo = new WallPostVertexModel()
            {
                _id = addVertexResponse[TitanGraphConstants.Id]
            };

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
            sendNotificationResponse = consumerModel.SendNotificationToUser(session, userWallVertexId, postVertexId, addVertexResponse[TitanGraphConstants.Id], postPostedByVertexId, EdgeLabelEnum.CommentedOnPostNotification.ToString(), taggedVertexId);

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
        public ResponseModel<UserVertexModel> CreateNewReactionOnUserPost(urNoticeSession session, UserNewReactionRequest userNewReactionRequest,List<TaggedVertexIdModel> taggedVertexId, out HashSet<string> sendNotificationResponse)
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
            sendNotificationResponse = consumerModel.SendNotificationToUser(session, userWallVertexId, vertexId, userNewReactionRequest.VertexId, postPostedByVertexId, EdgeLabelEnum.UserReaction.ToString(), taggedVertexId);

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

        
    }
}
