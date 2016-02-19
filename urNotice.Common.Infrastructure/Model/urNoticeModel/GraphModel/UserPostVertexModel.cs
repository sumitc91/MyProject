using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.GraphModel
{
    public class UserPostVertexModel
    {

        public WallPostVertexModel postInfo { get; set; }
        public List<UserVertexModel> postedToUser { get; set; }
        public List<UserPostCommentModel> commentsInfo { get; set; }
        public List<UserVertexModel> userInfo { get; set; }
    }
}
