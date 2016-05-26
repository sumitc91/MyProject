using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.GraphModel.V1
{
    public class UserNotificationVertexModelV1ResultDataResponse
    {
        public VertexModelV1 notificationInfo { get; set; }
        public VertexModelV1 postInfo { get; set; }
        public VertexModelV1 notificationByUser { get; set; }
    }
}
