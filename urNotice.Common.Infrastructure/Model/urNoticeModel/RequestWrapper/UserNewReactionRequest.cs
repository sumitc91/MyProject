using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper
{
    public class UserNewReactionRequest
    {
        public String Reaction { get; set; }        
        public String VertexId { get; set; }
        public String WallVertexId { get; set; }
        public String PostPostedByVertexId { get; set; }
    }
}
