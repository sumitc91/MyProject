using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.GraphModel
{
    public class UserPostCommentModel
    {
        public WallPostVertexModel commentInfo { get; set; }
        public List<UserVertexModel> commentedBy { get; set; }

        public long likeCount { get; set; }
        public List<UserVertexModel> isLiked { get; set; } 
    }
}
