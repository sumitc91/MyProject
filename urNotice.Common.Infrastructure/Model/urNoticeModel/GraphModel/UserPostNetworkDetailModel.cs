using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.GraphModel
{
    public class UserPostNetworkDetailModel
    {
        public List<UserVertexModel> associateRequestSent { get; set; }
        public List<UserVertexModel> associateRequestReceived { get; set; }
        public List<UserVertexModel> followRequestSent { get; set; }
        public List<UserVertexModel> isFriend { get; set; }
    }
}
