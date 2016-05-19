using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using urNotice.Common.Infrastructure.Common.Enum;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Common.Infrastructure.Model.urNoticeModel.Solr;

namespace urNotice.Common.Infrastructure.commonMethods
{
    public class OrbitPageUtil
    {
        public static string ConvertVirtualFriendListToUnVirtualFriendSolrPhoneWithoutFormatting(string id2)
        {
            return id2.Replace("tel:", "").Replace("-", "");            
        }

        public static string GetNotificationHashKeyUserEmail(string sendToEmail)
        {
            return DynamoDbHashKeyDataType.Notification + "_" +sendToEmail;
        }

        public static string GetCurrentTimeStampForGraphDb()
        {
            return "(long," + Convert.ToString(DateTimeUtil.GetUtcTime().Ticks) + ")";
        }

        public static string GetCurrentTimeStampForGraphDbFromGremlinServer()
        {
            return Convert.ToString(DateTimeUtil.GetUtcTime().Ticks);
        }

        public static string GenerateUniqueKeyForEdgeQuery(string inV, string label, string outV)
        {
            return inV + "-" + label + "-" + outV;
        }

        public static string GetDisplayName(OrbitPageUser userInfo)
        {
            return userInfo.firstName + " " + userInfo.lastName;
        }

        public static string GetDisplayName(UnUserSolr solrUserEmail)
        {
            return solrUserEmail.Firstname + " " + solrUserEmail.Lastname;
        }
    }
}
