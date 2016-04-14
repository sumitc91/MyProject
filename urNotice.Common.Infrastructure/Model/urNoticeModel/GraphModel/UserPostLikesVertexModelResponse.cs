using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.GraphModel
{
    public class UserPostLikesVertexModelResponse
    {
        public Boolean success { get; set; }
        public List<UserPostLikesModel> results { get; set; }
    }
}
