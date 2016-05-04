using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.GraphModel
{
    public class UserFriendRequestNotificationVertexModel
    {
        public List<UserVertexModel> requestedBy { get; set; }
        public EdgeModel requestInfo { get; set; }

    }
}
