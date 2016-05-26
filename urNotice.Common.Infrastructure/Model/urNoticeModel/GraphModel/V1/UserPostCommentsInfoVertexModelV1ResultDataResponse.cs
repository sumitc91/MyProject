using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.GraphModel.V1
{
    public class UserPostCommentsInfoVertexModelV1ResultDataResponse
    {
        public VertexModelV1 commentData { get; set; }
        public VertexModelV1 commentedBy { get; set; }
        public long likeCount { get; set; }
        public List<VertexModelV1> isCommentLiked { get; set; }
    }
}
