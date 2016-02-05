using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb
{
    [DynamoDBTable("CompanyAnalytics")]
    public class CompanyAnalytics
    {
        [DynamoDBHashKey]  //Sort key
        public string CompanyId { get; set; }

        [DynamoDBProperty]
        public long PageVisit { get; set; }

        [DynamoDBProperty]
        public DateTime LastActivity { get; set; }
        
        [DynamoDBProperty]
        public float Rating { get; set; }

        [DynamoDBProperty]
        public long RatingCount { get; set; }

        [DynamoDBProperty]
        public DateTime LastRatingActivity { get; set; }
    }
}
