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

        public IDictionary<string, string> CreateNewUserPost(urNoticeSession session, string message,string image,string vertexId, string accessKey, string secretKey)
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
            IDictionary<string, string> addVertexResponse = new GraphVertexOperations().AddVertex(session.UserName, TitanGraphConfig.Server, vertexId, TitanGraphConfig.Graph, properties, accessKey, secretKey);


            string edgeId = session.UserName + "_" + DateTime.Now.Ticks;
            properties = new Dictionary<string, string>();
            properties[EdgePropertyEnum._outV.ToString()] = session.UserVertexId;
            properties[EdgePropertyEnum._inV.ToString()] = addVertexResponse[TitanGraphConstants.Id];
            properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.Created.ToString();
            IDictionary<string, string> addCreatedByEdgeResponse = new GraphEdgeOperations().AddEdge(session, TitanGraphConfig.Server, edgeId, TitanGraphConfig.Graph, properties, accessKey, secretKey);
            
            //_outV=<id>&_label=friend&_inV=2&<key>=<key'>
            edgeId = session.UserName + "_" + DateTime.Now.Ticks;
            properties = new Dictionary<string, string>();

            if (vertexId != null && vertexId != session.UserVertexId)
            {
                properties[EdgePropertyEnum._outV.ToString()] = addVertexResponse[TitanGraphConstants.Id];
                properties[EdgePropertyEnum._inV.ToString()] = vertexId;
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

            var orbitPageUserNotification = new OrbitPageUserNotification
            {
                SentByUser = session.UserName,
                SentByUserVertexId = session.UserVertexId,
                SentToUser = null,
                SentToUserVertexId = vertexId,
                CreatedTime = DateTimeUtil.GetUtcTimeString(),
                Title = string.Format(UserNotificationText.PostOnUserWall,session.UserName),
                Body = null,
                PrimaryImage = UserNotificationText.PrimaryImage,
                Type = OrbitPageEnum.PostNotification.ToString(),
                VertexId = addVertexResponse[TitanGraphConstants.Id]
            };
            SendNotificationToUser(orbitPageUserNotification,accessKey,secretKey);

            response["status"] = "200";
            return response;
        }

        private IDictionary<string, string> SendNotificationToUser(OrbitPageUserNotification orbitPageUserNotification, string accessKey, string secretKey)
        {            
            new DynamoDbService.DynamoDbService().CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(
                    OrbitPageUtil.GetNotificationHashKeyUserEmail(orbitPageUserNotification.SentToUserVertexId),
                    DateTimeUtil.GetUtcTimeString(),
                    orbitPageUserNotification.SentByUser,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    orbitPageUserNotification,
                    false,
                    accessKey,
                    secretKey
                    );

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
            string gremlinQuery = "g.v(" + userVertexId + ").in('WallPost').sort{ a, b -> b.PostedTime <=> a.PostedTime }._()[" + from + ".." + to + "].transform{ [postInfo : it, commentsInfo: it.in('Comment').sort{ a, b -> b.PostedTime <=> a.PostedTime }._()[0..3].transform{[commentInfo:it, commentedBy: it.in('Created')]},userInfo:it.in('Created')] }";            
            //string gremlinQuery = "g.v(" + userVertexId + ").in('_label','WallPost').sort{it.PostedTime}.reverse()._().as('postInfo')[" + from + ".." + to + "].in('_label','Created').as('userInfo').select{it}{it}";
            string response = new GraphVertexOperations().GetVertexDetail(url, gremlinQuery, userVertexId, graphName, null);

            return response;
        }

        public IDictionary<string, string> CreateNewCommentOnUserPost(urNoticeSession session, string message, string image, string vertexId,string wallVertexId,string postPostedByVertexId, string accessKey, string secretKey)
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
            IDictionary<string, string> addVertexResponse = new GraphVertexOperations().AddVertex(session.UserName, TitanGraphConfig.Server, vertexId, TitanGraphConfig.Graph, properties, accessKey, secretKey);


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
            properties[EdgePropertyEnum._inV.ToString()] = vertexId;
            properties[EdgePropertyEnum.PostedBy.ToString()] = session.UserVertexId;

            properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.Comment.ToString();
            properties[EdgePropertyEnum.PostedDate.ToString()] = DateTimeUtil.GetUtcTimeString();
            properties[EdgePropertyEnum.EdgeMessage.ToString()] = "";

            IDictionary<string, string> addEdgeResponse = new GraphEdgeOperations().AddEdge(session, TitanGraphConfig.Server, edgeId, TitanGraphConfig.Graph, properties, accessKey, secretKey);

            var orbitPageUserWallCommentNotification = new OrbitPageUserNotification
            {
                SentByUser = session.UserName,
                SentByUserVertexId = session.UserVertexId,
                SentToUser = null,
                SentToUserVertexId = wallVertexId,
                CreatedTime = DateTimeUtil.GetUtcTimeString(),
                Title = string.Format(UserNotificationText.CommentedOnPostOnUserWall, session.UserName),
                Body = null,
                PrimaryImage = UserNotificationText.PrimaryImage,
                Type = OrbitPageEnum.PostNotification.ToString(),
                VertexId = addVertexResponse[TitanGraphConstants.Id]
            };

            SendNotificationToUser(orbitPageUserWallCommentNotification, accessKey, secretKey);

            if (wallVertexId != postPostedByVertexId)
            {
                var orbitPageUserWallCommentOnOtherNotification = new OrbitPageUserNotification
                {
                    SentByUser = session.UserName,
                    SentByUserVertexId = session.UserVertexId,
                    SentToUser = null,
                    SentToUserVertexId = postPostedByVertexId,
                    CreatedTime = DateTimeUtil.GetUtcTimeString(),
                    Title = string.Format(UserNotificationText.CommentedOnPostYouPostedOnUserWall, session.UserName, UserNotificationText.Friend),
                    Body = null,
                    PrimaryImage = UserNotificationText.PrimaryImage,
                    Type = OrbitPageEnum.PostNotification.ToString(),
                    VertexId = addVertexResponse[TitanGraphConstants.Id]
                };

                SendNotificationToUser(orbitPageUserWallCommentOnOtherNotification, accessKey, secretKey);
            }

            response["status"] = "200";
            return response;
        }
    }
}
