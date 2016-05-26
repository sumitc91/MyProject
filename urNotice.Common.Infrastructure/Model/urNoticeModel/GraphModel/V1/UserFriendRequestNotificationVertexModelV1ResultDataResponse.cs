using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.GraphModel.V1
{
    public class UserFriendRequestNotificationVertexModelV1ResultDataResponse
    {
        public VertexModelV1 requestedBy { get; set; }
        public EdgeModelV1 requestInfo { get; set; }
    }
}
