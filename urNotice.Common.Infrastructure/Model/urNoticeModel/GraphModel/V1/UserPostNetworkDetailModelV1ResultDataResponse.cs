using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.GraphModel.V1
{
    public class UserPostNetworkDetailModelV1ResultDataResponse
    {
        public List<VertexModelV1> associateRequestSent { get; set; }
        public List<VertexModelV1> associateRequestReceived { get; set; }
        public List<VertexModelV1> followRequestSent { get; set; }
        public List<VertexModelV1> isFriend { get; set; }
    }
}
