using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.User
{
    public class UserNewPostRequest
    {
        public String Message { get; set; }
        public String MessageHtml { get; set; }
        public List<TaggedVertexIdModel> TaggedVertexId { get; set; } 
        public String Image { get; set; }
        public String VertexId { get; set; }
    }
}
