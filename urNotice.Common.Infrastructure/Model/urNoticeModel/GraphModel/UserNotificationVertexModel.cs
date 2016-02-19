using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.GraphModel
{
    public class UserNotificationVertexModel
    {
        public UserNotificationEdgeInfo notificationInfo { get; set; }
        public List<WallPostVertexModel> postInfo { get; set; }
        public UserVertexModel notificationByUser { get; set; }
    }
}
