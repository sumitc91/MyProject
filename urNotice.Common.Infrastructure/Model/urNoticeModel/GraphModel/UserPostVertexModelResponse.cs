using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.GraphModel
{
    public class UserPostVertexModelResponse
    {
        public Boolean success { get; set; }
        public List<UserPostVertexModel> results { get; set; }
    }
}
