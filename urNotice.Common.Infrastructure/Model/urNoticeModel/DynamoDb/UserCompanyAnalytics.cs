using System;
using Amazon.DynamoDBv2.DataModel;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb
{
    [DynamoDBTable("UserCompanyAnalytics")]
    public class UserCompanyAnalytics
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

        [DynamoDBProperty]
        public Boolean IsFollowing { get; set; }

    }
}
