using System;
using Amazon.DynamoDBv2.DataModel;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb
{
    [DynamoDBTable("UserCompanyNoticePeriodInfoAnalytics")]
    public class UserCompanyNoticePeriodInfoAnalytics
    {
        [DynamoDBHashKey]    //Partition key
        public string UserId { get; set; }

        [DynamoDBRangeKey]  //Sort key
        public string CompanyId { get; set; }

        [DynamoDBProperty]
        public DateTime LastActivity { get; set; }

        [DynamoDBProperty]
        public bool IsAnonymous { get; set; }

        [DynamoDBProperty]
        public bool ShowAnonymous { get; set; }


        [DynamoDBProperty]
        public int? MaxNoticePeriod { get; set; }

        [DynamoDBProperty]
        public int? MinNoticePeriod { get; set; }

        [DynamoDBProperty]
        public int? NoticePeriodServed { get; set; }

        [DynamoDBProperty]
        public bool? BuyOutOption { get; set; }

        [DynamoDBProperty]
        public bool? LookingForChange { get; set; }

        [DynamoDBProperty]
        public string Suggestion { get; set; }

        [DynamoDBProperty]
        public string CreatedDate { get; set; }

        [DynamoDBProperty]
        public DateTime? JoiningDate { get; set; }

        [DynamoDBProperty]
        public DateTime? ResigningDate { get; set; }

        [DynamoDBProperty]
        public bool? ManagerBehaviourChanged { get; set; }

        [DynamoDBProperty]
        public bool IsVerified { get; set; }

        [DynamoDBProperty]
        public string VerifiedBy { get; set; }

        [DynamoDBProperty]
        public bool IsHikeOffered { get; set; }

        [DynamoDBProperty]
        public double HikePercentOffered { get; set; }

        [DynamoDBProperty]
        public bool? IsDbSynced { get; set; }
    }
}
