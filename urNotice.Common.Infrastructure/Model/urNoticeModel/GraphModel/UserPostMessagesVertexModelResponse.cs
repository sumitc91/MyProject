using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.GraphModel
{
    public class UserPostMessagesVertexModelResponse
    {
        public Boolean success { get; set; }
        public List<UserPostCommentModel> results { get; set; }
    }
}
