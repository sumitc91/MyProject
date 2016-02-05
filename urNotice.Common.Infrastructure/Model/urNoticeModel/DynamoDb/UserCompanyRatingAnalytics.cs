using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb
{
    [DynamoDBTable("UserCompanyRatingAnalytics")]
    public class UserCompanyRatingAnalytics
    {
        [DynamoDBHashKey]    //Partition key
        public string UserId { get; set; }

        [DynamoDBRangeKey]  //Sort key
        public string CompanyId { get; set; }

        [DynamoDBProperty]
        public DateTime LastActivity { get; set; }

        [DynamoDBProperty]
        public int Count { get; set; }

        [DynamoDBProperty]
        public float Rating { get; set; }

        [DynamoDBProperty]
        public DateTime LastRatingActivity { get; set; }
    }
}
