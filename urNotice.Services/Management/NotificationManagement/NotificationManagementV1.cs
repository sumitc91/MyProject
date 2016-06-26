using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using urNotice.Common.Infrastructure.Common.Config;
using urNotice.Common.Infrastructure.Common.Constants;
using urNotice.Common.Infrastructure.Common.Enum;
using urNotice.Common.Infrastructure.commonMethods;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Common.Infrastructure.Model.urNoticeModel.User;
using urNotice.Common.Infrastructure.Session;
using urNotice.Services.GraphDb;
using urNotice.Services.NoSqlDb.DynamoDb;

namespace urNotice.Services.Management.NotificationManagement
{
    public class NotificationManagementV1:INotificationManagement
    {
        public HashSet<string> SendNotificationToUser(urNoticeSession session, string userWallVertexId, string postVertexId, string commentVertexId, string postPostedByVertexId, string notificationType, List<TaggedVertexIdModel> taggedVertexId)
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
                //orbitPageCompanyUserWorkgraphyTable = GetUsersToBeNotifiedForVertex(postVertexId);
                //if (orbitPageCompanyUserWorkgraphyTable != null &&
                //    orbitPageCompanyUserWorkgraphyTable.SendNotificationToUserList != null)
                //    usersToBeNotified = orbitPageCompanyUserWorkgraphyTable.SendNotificationToUserList;
                usersToBeNotified.Add(userWallVertexId);
                finalSendNotificationsToUser = usersToBeNotified;
                
                usersToBeIgnored.Add(session.UserVertexId);

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
            else if (notificationType.Equals(EdgeLabelEnum.PostTagNotification.ToString()))
            {
                
                usersToBeIgnored.Add(session.UserVertexId);

                if (taggedVertexId != null)
                {
                    foreach (var userIds in taggedVertexId)
                    {

                        if (usersToBeIgnored.Contains(userIds.VertexId))
                            continue;

                        notificationModel = new UserNotificationGraphModel
                        {
                            _outV = userIds.VertexId,
                            _inV = postVertexId,
                            _label = EdgeLabelEnum.Notification.ToString(),
                            NotificationInitiatedByVertexId = session.UserVertexId,
                            Type = EdgeLabelEnum.PostTagNotification.ToString(),
                            UserName = session.UserName,
                            parentPostId = postVertexId
                        };
                        userPushNotificationListWrapper.UserNotificationGraphModelList.Add(notificationModel);
                        userPushNotificationListWrapper.SignalRNotificationIds.Add(userIds.VertexId);
                    }
                }

            }
            else if (notificationType.Equals(EdgeLabelEnum.CommentedOnPostNotification.ToString()))
            {
                orbitPageCompanyUserWorkgraphyTable = GetUsersToBeNotifiedForVertex(postVertexId);
                if (orbitPageCompanyUserWorkgraphyTable != null &&
                    orbitPageCompanyUserWorkgraphyTable.SendNotificationToUserList != null)
                    usersToBeNotified = orbitPageCompanyUserWorkgraphyTable.SendNotificationToUserList;

                finalSendNotificationsToUser = usersToBeNotified;
                
                usersToBeIgnored.Add(session.UserVertexId);

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
                
                usersToBeIgnored.Add(session.UserVertexId);

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

            SendNotificationListToUsers(session,userPushNotificationListWrapper);

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
            IDynamoDb dynamoDbModel = new DynamoDb();
            var orbitPageCompanyUserWorkgraphyTable = dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTable(DynamoDbHashKeyDataType.VertexDetail.ToString(), vertexId, null);
            if (orbitPageCompanyUserWorkgraphyTable != null)
                return orbitPageCompanyUserWorkgraphyTable;

            return null;
        }

        private void SendNotificationListToUsers(urNoticeSession session,UserPushNotificationListWrapper userPushNotificationListWrapper)
        {
            foreach (var userNotificationGraphModel in userPushNotificationListWrapper.UserNotificationGraphModelList)
            {
                var properties = new Dictionary<string, string>();


                properties[VertexPropertyEnum.Type.ToString()] = VertexLabelEnum.Notification.ToString();
                properties[VertexPropertyEnum.ParentPostId.ToString()] = userNotificationGraphModel.parentPostId;
                properties[VertexPropertyEnum.Type.ToString()] = userNotificationGraphModel.Type;
                properties[VertexPropertyEnum.PostedTime.ToString()] = DateTimeUtil.GetUtcTimeString();
                properties[VertexPropertyEnum.PostedTimeLong.ToString()] = OrbitPageUtil.GetCurrentTimeStampForGraphDbFromGremlinServer();

                var canEdit = new HashSet<String>() { session.UserVertexId };

                var canDelete = new HashSet<String>();
                canDelete.Add(session.UserVertexId);
                canDelete.Add(userNotificationGraphModel._outV);

                var sendNotificationToUsers = new HashSet<String>();
                
                IGraphVertexDb graphVertexDb = new GremlinServerGraphVertexDb();
                IDictionary<string, string> addVertexResponse = graphVertexDb.AddVertex(session.UserName, TitanGraphConfig.Graph, properties, canEdit, canDelete, sendNotificationToUsers);

                if (addVertexResponse == null)
                    return;

                properties[EdgePropertyEnum._outV.ToString()] = userNotificationGraphModel.NotificationInitiatedByVertexId;// who created notification
                properties[EdgePropertyEnum._inV.ToString()] = addVertexResponse[TitanGraphConstants.Id]; // to created notification vertex
                properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.CreatedNotification.ToString();                                                
                
                IGraphEdgeDb graphEdgeDbModel = new GremlinServerGraphEdgeDb();
                IDictionary<string, string> addEdgeResponse = graphEdgeDbModel.AddEdge(userNotificationGraphModel.UserName, TitanGraphConfig.Graph, properties);

                properties[EdgePropertyEnum._outV.ToString()] = addVertexResponse[TitanGraphConstants.Id]; // created notification vertex
                properties[EdgePropertyEnum._inV.ToString()] = userNotificationGraphModel._outV; // to whom we need to notify.
                properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.NotificationSent.ToString();
                
                graphEdgeDbModel = new GremlinServerGraphEdgeDb();
                addEdgeResponse = graphEdgeDbModel.AddEdge(userNotificationGraphModel.UserName, TitanGraphConfig.Graph, properties);

                properties[EdgePropertyEnum._outV.ToString()] = userNotificationGraphModel._inV; // post vertex
                properties[EdgePropertyEnum._inV.ToString()] = addVertexResponse[TitanGraphConstants.Id]; // to created notification vertex.
                properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.RelatedPost.ToString();

                graphEdgeDbModel = new GremlinServerGraphEdgeDb();
                addEdgeResponse = graphEdgeDbModel.AddEdge(userNotificationGraphModel.UserName, TitanGraphConfig.Graph, properties);
            }
        }
    }
}
