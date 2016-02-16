using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb
{
    public class OrbitPageUserNotification
    {
        public string SentByUser { get; set; }
        public string SentByUserVertexId { get; set; }
        public string SentToUser { get;set; }
        public string SentToUserVertexId { get; set; }
        public string PostVertexId { get; set; }
        public string CreatedTime { get; set; }
        public string Body { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string VertexId { get; set; }
        public string PrimaryImage { get; set; }
        public string SecondaryImage { get; set; }
    }
}
