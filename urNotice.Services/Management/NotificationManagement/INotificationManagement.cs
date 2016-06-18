using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using urNotice.Common.Infrastructure.Model.urNoticeModel.User;
using urNotice.Common.Infrastructure.Session;

namespace urNotice.Services.Management.NotificationManagement
{
    public interface INotificationManagement
    {
        HashSet<string> SendNotificationToUser(urNoticeSession session, string userWallVertexId, string postVertexId, string commentVertexId, string postPostedByVertexId, string notificationType, List<TaggedVertexIdModel> taggedVertexId);
    }
}
