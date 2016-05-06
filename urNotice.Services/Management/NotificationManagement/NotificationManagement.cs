using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using urNotice.Common.Infrastructure.Common.Config;
using urNotice.Common.Infrastructure.Common.Enum;
using urNotice.Common.Infrastructure.commonMethods;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Common.Infrastructure.Model.urNoticeModel.User;
using urNotice.Common.Infrastructure.Session;
using urNotice.Services.GraphDb;
using urNotice.Services.NoSqlDb.DynamoDb;

namespace urNotice.Services.Management.NotificationManagement
{
    public class NotificationManagement : INotificationManagement
    {
        public HashSet<string> SendNotificationToUser(urNoticeSession session, string userWallVertexId, string postVertexId, string commentVertexId, string postPostedByVertexId, string notificationType)
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
                //if (userWallVertexId == session.UserVertexId)
                //{
                //    usersToBeIgnored.Add(userWallVertexId);
                //}

                //if (usersToBeNotified.Contains(session.UserVertexId))
                //{
                //    usersToBeIgnored.Add(session.UserVertexId);
                //}
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
            else if (notificationType.Equals(EdgeLabelEnum.CommentedOnPostNotification.ToString()))
            {
                orbitPageCompanyUserWorkgraphyTable = GetUsersToBeNotifiedForVertex(postVertexId);
                if (orbitPageCompanyUserWorkgraphyTable != null &&
                    orbitPageCompanyUserWorkgraphyTable.SendNotificationToUserList != null)
                    usersToBeNotified = orbitPageCompanyUserWorkgraphyTable.SendNotificationToUserList;

                finalSendNotificationsToUser = usersToBeNotified;
                //if (userWallVertexId == session.UserVertexId)
                //{
                //    usersToBeIgnored.Add(userWallVertexId);
                //}

                //if (userWallVertexId == postPostedByVertexId || session.UserVertexId == postPostedByVertexId)
                //{
                //    usersToBeIgnored.Add(userWallVertexId);
                //}

                //if (usersToBeNotified.Contains(session.UserVertexId))
                //{
                //    usersToBeIgnored.Add(session.UserVertexId);
                //}

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
                //if (userWallVertexId == session.UserVertexId)
                //{
                //    usersToBeIgnored.Add(userWallVertexId);
                //}

                //if (userWallVertexId == postPostedByVertexId || session.UserVertexId == postPostedByVertexId)
                //{
                //    usersToBeIgnored.Add(userWallVertexId);
                //}

                //if (usersToBeNotified.Contains(session.UserVertexId))
                //{
                //    usersToBeIgnored.Add(session.UserVertexId);
                //}

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
            IDynamoDb dynamoDbModel = new DynamoDb();
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
