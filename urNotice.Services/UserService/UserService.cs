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
using urNotice.Common.Infrastructure.Common.Logger;
using urNotice.Common.Infrastructure.commonMethods;
using urNotice.Common.Infrastructure.Model.urNoticeAuthContext;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper;
using urNotice.Common.Infrastructure.Session;
using urNotice.Services.GraphService;

namespace urNotice.Services.UserService
{
    public class UserService
    {
        private static readonly ILogger Logger = new Logger(Convert.ToString(MethodBase.GetCurrentMethod().DeclaringType));
        private DbContextException _dbContextException = new DbContextException();
        //private readonly urnoticeAuthEntities _db = new urnoticeAuthEntities();

        public ResponseModel<ClientDetailsModel> GetClientDetails(string username,string accessKey,string secretKey)
        {
            var response = new ResponseModel<ClientDetailsModel>();

            try
            {
                //var clientDetailDbResult = _db.Users.SingleOrDefault(x => x.username == username);
                var userInfo = new DynamoDbService.DynamoDbService().GetOrbitPageCompanyUserWorkgraphyTable(
                DynamoDbHashKeyDataType.OrbitPageUser.ToString(),
                username,
                null,
                accessKey,
                secretKey
                );

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
            }
            catch (Exception)
            {
                response.Status = 500;
                response.Message = "exception occured !!!";
            }
            return response;
        }

        public IDictionary<string, string> CreateNewUserPost(urNoticeSession session, string message,string image,string userWallVertexId, string accessKey, string secretKey)
        {
            var response = new Dictionary<string, string>();
            //string url = TitanGraphConfig.Server;
            //string graphName = TitanGraphConfig.Graph;

            var properties = new Dictionary<string, string>();

            properties[VertexPropertyEnum.Type.ToString()] = VertexLabelEnum.Post.ToString();
            properties[VertexPropertyEnum.PostMessage.ToString()] = message;
            properties[VertexPropertyEnum.PostedByUser.ToString()] = session.UserName;
            properties[VertexPropertyEnum.PostedTime.ToString()] = DateTimeUtil.GetUtcTimeString();
            properties[VertexPropertyEnum.PostImage.ToString()] = image;
            IDictionary<string, string> addVertexResponse = new GraphVertexOperations().AddVertex(session.UserName, TitanGraphConfig.Server, userWallVertexId, TitanGraphConfig.Graph, properties, accessKey, secretKey);


            string edgeId = session.UserName + "_" + DateTime.Now.Ticks;
            properties = new Dictionary<string, string>();
            properties[EdgePropertyEnum._outV.ToString()] = session.UserVertexId;
            properties[EdgePropertyEnum._inV.ToString()] = addVertexResponse[TitanGraphConstants.Id];
            properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.Created.ToString();
            IDictionary<string, string> addCreatedByEdgeResponse = new GraphEdgeOperations().AddEdge(session, TitanGraphConfig.Server, edgeId, TitanGraphConfig.Graph, properties, accessKey, secretKey);
            
            //_outV=<id>&_label=friend&_inV=2&<key>=<key'>
            edgeId = session.UserName + "_" + DateTime.Now.Ticks;
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

            IDictionary<string, string> addEdgeResponse = new GraphEdgeOperations().AddEdge(session, TitanGraphConfig.Server, edgeId, TitanGraphConfig.Graph, properties, accessKey, secretKey);

            SendNotificationToUser(session, userWallVertexId, addVertexResponse[TitanGraphConstants.Id],null, EdgeLabelEnum.WallPostNotification.ToString(), accessKey, secretKey);

            response["status"] = "200";
            return response;
        }

        private IDictionary<string, string> SendNotificationToUser(urNoticeSession session, string userWallVertexId, string postVertexId, string postPostedByVertexId, string notificationType, string accessKey, string secretKey)
        {
            var properties = new Dictionary<string, string>();

            
            var edgeId = session.UserName + "_" + DateTime.Now.Ticks;

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

                    IDictionary<string, string> addEdgeResponse = new GraphEdgeOperations().AddEdge(session, TitanGraphConfig.Server, edgeId, TitanGraphConfig.Graph, properties, accessKey, secretKey);

                }
                
            }
            else if (notificationType.Equals(EdgeLabelEnum.CommentedOnPostNotification.ToString()))
            {
                if (userWallVertexId != session.UserVertexId)
                {
                    properties[EdgePropertyEnum._outV.ToString()] = userWallVertexId;
                    properties[EdgePropertyEnum._inV.ToString()] =postVertexId;

                    properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.Notification.ToString();
                    properties[EdgePropertyEnum.NotificationInitiatedByVertexId.ToString()] = session.UserVertexId;
                    properties[EdgePropertyEnum.Type.ToString()] = EdgeLabelEnum.CommentedOnPostNotification.ToString();
                    properties[EdgePropertyEnum.PostedDate.ToString()] = DateTimeUtil.GetUtcTimeString();

                    IDictionary<string, string> addEdgeResponse = new GraphEdgeOperations().AddEdge(session, TitanGraphConfig.Server, edgeId, TitanGraphConfig.Graph, properties, accessKey, secretKey);
   
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

                    IDictionary<string, string> addEdgeResponse = new GraphEdgeOperations().AddEdge(session, TitanGraphConfig.Server, edgeId, TitanGraphConfig.Graph, properties, accessKey, secretKey);
                }
                
            }
            return null;
        }

        public string GetUserPost(string userVertexId, string @from, string to, string accessKey, string secretKey)
        {
            string url = TitanGraphConfig.Server;
            string graphName = TitanGraphConfig.Graph;            
            
            //g.v(2569472).as('userInfo').out('_label','WallPost').as('postInfo')[0..2].select{it}{it}
            //g.v(768).in('_label','WallPost').as('postInfo')[0..10].in('_label','Created').as('userInfo').select{it}{it}
            //g.v(768).as('userInfo').in('_label','WallPost').sort{it.PostedTime}.reverse()._().as('postInfo')[0..10].select{it}{it}
            //g.v((512)).in('WallPost').sort{ a, b -> b.PostedTime <=> a.PostedTime }._().transform{ [postInfo : it, comments: it.in('Comment').toList(),userInfo:it.in('Created')] }
            //g.v((512)).in('WallPost').sort{ a, b -> b.PostedTime <=> a.PostedTime }._()[0..1].transform{ [postInfo : it, commentsInfo: it.in('Comment').sort{ a, b -> b.PostedTime <=> a.PostedTime }._()[0..1].transform{[commentInfo:it, commentedBy: it.in('Created')]},userInfo:it.in('Created')] }
            string gremlinQuery = "g.v(" + userVertexId + ").in('WallPost').sort{ a, b -> b.PostedTime <=> a.PostedTime }._()[" + from + ".." + to + "].transform{ [postInfo : it, commentsInfo: it.in('Comment').sort{ a, b -> a.PostedTime <=> b.PostedTime }._()[0..3].transform{[commentInfo:it, commentedBy: it.in('Created')]},userInfo:it.in('Created')] }";            
            //string gremlinQuery = "g.v(" + userVertexId + ").in('_label','WallPost').sort{it.PostedTime}.reverse()._().as('postInfo')[" + from + ".." + to + "].in('_label','Created').as('userInfo').select{it}{it}";
            string response = new GraphVertexOperations().GetVertexDetail(url, gremlinQuery, userVertexId, graphName, null);

            return response;
        }

        public IDictionary<string, string> CreateNewCommentOnUserPost(urNoticeSession session, string message, string image, string postVertexId, string userWallVertexId, string postPostedByVertexId, string accessKey, string secretKey)
        {
            var response = new Dictionary<string, string>();
            //string url = TitanGraphConfig.Server;
            //string graphName = TitanGraphConfig.Graph;

            var properties = new Dictionary<string, string>();
            properties[VertexPropertyEnum.PostMessage.ToString()] = message;
            properties[VertexPropertyEnum.PostedByUser.ToString()] = session.UserName;
            properties[VertexPropertyEnum.PostedTime.ToString()] = DateTimeUtil.GetUtcTimeString();
            properties[VertexPropertyEnum.PostImage.ToString()] = image;
            properties[VertexPropertyEnum.Type.ToString()] = VertexLabelEnum.Comment.ToString();
            IDictionary<string, string> addVertexResponse = new GraphVertexOperations().AddVertex(session.UserName, TitanGraphConfig.Server, postVertexId, TitanGraphConfig.Graph, properties, accessKey, secretKey);


            string edgeId = session.UserName + "_" + DateTime.Now.Ticks;
            properties = new Dictionary<string, string>();
            properties[EdgePropertyEnum._outV.ToString()] = session.UserVertexId;
            properties[EdgePropertyEnum._inV.ToString()] = addVertexResponse[TitanGraphConstants.Id];
            properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.Created.ToString();
            IDictionary<string, string> addCreatedByEdgeResponse = new GraphEdgeOperations().AddEdge(session, TitanGraphConfig.Server, edgeId, TitanGraphConfig.Graph, properties, accessKey, secretKey);

            //_outV=<id>&_label=friend&_inV=2&<key>=<key'>
            edgeId = session.UserName + "_" + DateTime.Now.Ticks;
            properties = new Dictionary<string, string>();
            
            properties[EdgePropertyEnum._outV.ToString()] = addVertexResponse[TitanGraphConstants.Id];
            properties[EdgePropertyEnum._inV.ToString()] = postVertexId;
            properties[EdgePropertyEnum.PostedBy.ToString()] = session.UserVertexId;

            properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.Comment.ToString();
            properties[EdgePropertyEnum.PostedDate.ToString()] = DateTimeUtil.GetUtcTimeString();
            properties[EdgePropertyEnum.EdgeMessage.ToString()] = "";

            IDictionary<string, string> addEdgeResponse = new GraphEdgeOperations().AddEdge(session, TitanGraphConfig.Server, edgeId, TitanGraphConfig.Graph, properties, accessKey, secretKey);


            SendNotificationToUser(session, userWallVertexId, postVertexId,postPostedByVertexId, EdgeLabelEnum.CommentedOnPostNotification.ToString(), accessKey, secretKey);

            

            response["status"] = "200";
            return response;
        }

        public string GetUserNotification(string userVertexId,string from,string to, string accessKey, string secretKey)
        {
            string url = TitanGraphConfig.Server;
            string graphName = TitanGraphConfig.Graph;

            string gremlinQuery = "g.v(" + userVertexId + ").outE('Notification').order{it.b.PostedDate <=> it.a.PostedDate}[" + from + ".." + to + "].transform{ [notificationInfo:it,postInfo:it.inV,notificationByUser:g.v(it.NotificationInitiatedByVertexId)]}";
            string response = new GraphVertexOperations().GetVertexDetail(url, gremlinQuery, userVertexId, graphName, null);
            return response;
        }

        public string GetPostByVertexId(string vertexId, string accessKey, string secretKey)
        {
            string url = TitanGraphConfig.Server;
            string graphName = TitanGraphConfig.Graph;

            string gremlinQuery = "g.v(" + vertexId + ").transform{ [postInfo : it,postedToUser:it.out('WallPost'), commentsInfo: it.in('Comment').sort{ a, b -> a.PostedTime <=> b.PostedTime }._()[0..3].transform{[commentInfo:it, commentedBy: it.in('Created')]},userInfo:it.in('Created')] }";
            //string gremlinQuery = "g.v(" + userVertexId + ").in('_label','WallPost').sort{it.PostedTime}.reverse()._().as('postInfo')[" + from + ".." + to + "].in('_label','Created').as('userInfo').select{it}{it}";
            string response = new GraphVertexOperations().GetVertexDetail(url, gremlinQuery, vertexId, graphName, null);

            return response;
        }
    }
}
