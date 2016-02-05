using System;
using Amazon.DynamoDBv2.DataModel;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb
{
    [DynamoDBTable("UserAnalytics")]
    public class UserAnalytics
    {
        [DynamoDBHashKey]    //Partition key
        public string UserId { get; set; }

        [DynamoDBProperty]  //Sort key
        public DateTime LoginDateTime { get; set; }

        [DynamoDBProperty]
        public string SystemDetails { get; set; }

        [DynamoDBProperty]
        public int Count { get; set; }
    }
}
