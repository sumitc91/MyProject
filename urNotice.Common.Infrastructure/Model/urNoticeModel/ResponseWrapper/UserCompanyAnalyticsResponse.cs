using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper
{
    public class UserCompanyAnalyticsResponse
    {        
        public string UserId { get; set; }
   
        public string CompanyId { get; set; }

        public String LastActivityString { get; set; }

        public DateTime LastActivity { get; set; }
        public int Count { get; set; }

        public CompanyAnalytics CompanyAnalytics { get; set; }
    }
}
