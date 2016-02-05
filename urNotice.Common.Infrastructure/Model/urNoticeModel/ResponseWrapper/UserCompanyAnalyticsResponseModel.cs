using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Common.Infrastructure.Model.urNoticeRatingContext;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper
{
    public class UserCompanyAnalyticsResponseModel
    {
        //public Rating Rating { get; set; }
        public UserCompanyAnalyticsResponse UserCompanyAnalytics { get; set; }
    }
}
