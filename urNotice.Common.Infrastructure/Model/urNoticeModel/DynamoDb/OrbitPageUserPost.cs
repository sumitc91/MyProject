using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb
{
    public class OrbitPageUserPost1
    {
        public string PostMessage { get; set; }
        public List<string> PostImageUrlList { get; set; }
        public DateTime PostedTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
        public string PostedByUser { get; set; }
        public string VertexId { get; set; }
    }
}
