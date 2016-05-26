using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.GraphModel.V1
{
    public class UserPostMessagesVertexModelV1ResultDataResponse
    {
        public VertexModelV1 commentInfo { get; set; }
        public VertexModelV1 commentedBy { get; set; }
        public long likeCount { get; set; }
        public List<VertexModelV1> isLiked { get; set; }
    }
}
