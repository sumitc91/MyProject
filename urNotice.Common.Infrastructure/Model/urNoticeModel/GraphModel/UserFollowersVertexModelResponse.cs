using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.GraphModel
{
    public class UserFollowersVertexModelResponse
    {
        public Boolean success { get; set; }
        public List<UserVertexModel> results { get; set; }
    }
}
