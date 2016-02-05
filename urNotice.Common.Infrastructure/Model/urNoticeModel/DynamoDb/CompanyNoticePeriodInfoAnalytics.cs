using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb
{
    [DynamoDBTable("CompanyNoticePeriodInfoAnalytics")]
    public class CompanyNoticePeriodInfoAnalytics
    {
        [DynamoDBHashKey]  //Sort key
        public string CompanyId { get; set; }

        [DynamoDBRangeKey]
        public int LookingForChange { get; set; }

        [DynamoDBProperty]
        public int PostedAsAnonymous { get; set; }

        [DynamoDBProperty]
        public double? AvgMaxNoticePeriod { get; set; }

        [DynamoDBProperty]
        public double? AvgMinNoticePeriod { get; set; }

        [DynamoDBProperty]
        public double? AvgNoticePeriodServed { get; set; }

        [DynamoDBProperty]
        public double? AvgManagerBehaviourChanged { get; set; }

        [DynamoDBProperty]
        public double AvgHikeOffered { get; set; }

        [DynamoDBProperty]
        public double AvgHikePercentOffered { get; set; }
    }
}
