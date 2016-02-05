using System.Collections.Generic;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper
{
    public class UserNotificationsResponse
    {
        public string UnreadNotifications { get; set; }
        public string CountLabelType { get; set; }
        public List<UserNotificationList> NotificationList { get; set; } 
    }
}