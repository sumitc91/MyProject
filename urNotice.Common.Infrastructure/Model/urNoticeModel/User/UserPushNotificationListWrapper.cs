using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.User
{
    public class UserPushNotificationListWrapper
    {
        public List<UserNotificationGraphModel> UserNotificationGraphModelList { get; set; }
        public HashSet<string> SignalRNotificationIds { get; set; }
    }
}
