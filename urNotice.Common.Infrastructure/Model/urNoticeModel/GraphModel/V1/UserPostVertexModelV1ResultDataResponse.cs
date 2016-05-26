using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.GraphModel.V1
{
    public class UserPostVertexModelV1ResultDataResponse
    {
        public VertexModelV1 wallpostinfo { get; set; }
        public VertexModelV1 userInfo { get; set; }
        public VertexModelV1 postedOn { get; set; }

        public List<VertexModelV1> likeInfo { get; set; }
        public List<VertexModelV1> isLiked { get; set; }

        public long likeInfoCount { get; set; }
        public long commentsCount { get; set; }

        public List<UserPostCommentsInfoVertexModelV1ResultDataResponse> commentsInfo { get; set; }

    }
}
