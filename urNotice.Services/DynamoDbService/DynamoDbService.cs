using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using urNotice.Common.Infrastructure.Common.Enum;
using urNotice.Common.Infrastructure.commonMethods;
using urNotice.Common.Infrastructure.Model.urNoticeAnalyticsContext;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper;

namespace urNotice.Services.DynamoDbService
{
    public class DynamoDbService
    {

        public UserCompanyAnalytics CreateUserCompanyAnalyticsItem(string accessKey, string secretKey, string userId, string companyId)
        {
            var context = GetDynamoDbContext(accessKey, secretKey);
            var userAnalytics = new UserCompanyAnalytics
            {
                UserId = userId,
                CompanyId = companyId,
                LastActivity = DateTimeUtil.GetUtcTime(),
                Count = 0
            };

            context.Save(userAnalytics);
            return userAnalytics;
        }

        public OrbitPageCompanyUserWorkgraphyTable CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(string dataType, string objectId, string compareId, string facebookId, string facebookAuthKey, OrbitPageUser orbitPageUser, OrbitPageCompany orbitPageCompany, OrbitPageDesignation orbitPageDesignation, List<VirtualFriendList> orbitPageGoogleApiContact, Boolean isSolrUpdated, string accessKey, string secretKey)
        {
            var context = GetDynamoDbContext(accessKey, secretKey);

            var orbitPageCompanyUserWorkgraphyTable = new OrbitPageCompanyUserWorkgraphyTable();
            orbitPageCompanyUserWorkgraphyTable.DataType = dataType;
            orbitPageCompanyUserWorkgraphyTable.ObjectId = objectId;
            orbitPageCompanyUserWorkgraphyTable.CompareId = compareId;
            orbitPageCompanyUserWorkgraphyTable.FacebookId = facebookId;
            orbitPageCompanyUserWorkgraphyTable.OrbitPageUser = orbitPageUser;
            orbitPageCompanyUserWorkgraphyTable.OrbitPageGoogleApiContact = orbitPageGoogleApiContact;
            orbitPageCompanyUserWorkgraphyTable.FacebookAuthToken = facebookAuthKey;
            orbitPageCompanyUserWorkgraphyTable.CreatedDate = DateTimeUtil.GetUtcTime();

            context.Save(orbitPageCompanyUserWorkgraphyTable);
            return orbitPageCompanyUserWorkgraphyTable;
        }

        public OrbitPageCompanyUserWorkgraphyTable CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(OrbitPageCompanyUserWorkgraphyTable orbitPageCompanyUserWorkgraphyTable, string accessKey, string secretKey)
        {
            var context = GetDynamoDbContext(accessKey, secretKey);
            context.Save(orbitPageCompanyUserWorkgraphyTable);
            return orbitPageCompanyUserWorkgraphyTable;
        }
        public OrbitPageCompanyUserWorkgraphyTable GetOrbitPageCompanyUserWorkgraphyTable(string dataType, string objectId, string compareId,string accessKey, string secretKey)
        {
            var context = GetDynamoDbContext(accessKey, secretKey);
            var res = context.Load<OrbitPageCompanyUserWorkgraphyTable>(dataType, objectId);

            //IEnumerable<UserCompanyAnalytics> res = context.Scan<UserCompanyAnalytics>(
            //      new ScanCondition("Count", ScanOperator.GreaterThan, 5),
            //      new ScanCondition("UserId", ScanOperator.Equal, "sumitchourasia91@gmail.com")
            //      );

            return res;

        }

        public OrbitPageCompanyUserWorkgraphyTable GetOrbitPageCompanyUserWorkgraphyTableUsingFacebookId(string dataType, string facebookId, string accessKey, string secretKey)
        {
            var context = GetDynamoDbContext(accessKey, secretKey);
            //var res = context.Load<OrbitPageCompanyUserWorkgraphyTable>(dataType, objectId);

            IEnumerable<OrbitPageCompanyUserWorkgraphyTable> res = context.Scan<OrbitPageCompanyUserWorkgraphyTable>(
                  new ScanCondition("FacebookId", ScanOperator.Equal, facebookId),
                  new ScanCondition("DataType", ScanOperator.Equal, dataType)
                  );

            return res.FirstOrDefault();

        }

        public static CompanyAnalytics CreateCompanyAnalyticsItem(string accessKey, string secretKey, string companyId)
        {
            var context = GetDynamoDbContext(accessKey, secretKey);
            var companyAnalytics = new CompanyAnalytics
            {
                CompanyId = companyId,
                PageVisit = 0,
                LastActivity = DateTimeUtil.GetUtcTime(),
                Rating = 0,
                RatingCount = 0
            };

            context.Save(companyAnalytics);
            return companyAnalytics;
        }

        public UserCompanyAnalyticsResponse IncrementUserCompanyAnalyticsCounter(string accessKey, string secretKey, string userId, string companyId)
        {
            var context = GetDynamoDbContext(accessKey, secretKey);
            var userCompanyAnalytics = GetUserCompanyAnalyticsDetail(accessKey, secretKey, userId, companyId) ?? CreateUserCompanyAnalyticsItem(accessKey, secretKey, userId, companyId);
            var userCompanyAnalyticsToBeReturned = new UserCompanyAnalyticsResponse
            {
                UserId = userCompanyAnalytics.UserId,
                CompanyId = userCompanyAnalytics.CompanyId,
                Count = userCompanyAnalytics.Count + 1,
                LastActivityString = userCompanyAnalytics.LastActivity.ToString(),
                LastActivity = userCompanyAnalytics.LastActivity
            };

            userCompanyAnalytics.Count = userCompanyAnalytics.Count + 1;
            userCompanyAnalytics.LastActivity = DateTimeUtil.GetUtcTime();


            var companyAnalytics = GetCompanyAnalyticsDetail(accessKey, secretKey, companyId) ?? CreateCompanyAnalyticsItem(accessKey, secretKey, companyId);
            var companyAnalyticsToBeReturned = new CompanyAnalytics
            {                
                CompanyId = companyAnalytics.CompanyId,
                PageVisit = companyAnalytics.PageVisit + 1,               
                LastActivity = companyAnalytics.LastActivity
            };

            companyAnalytics.PageVisit = companyAnalytics.PageVisit + 1;
            companyAnalytics.LastActivity = DateTimeUtil.GetUtcTime();

            context.Save(userCompanyAnalytics);
            context.Save(companyAnalytics);

            userCompanyAnalyticsToBeReturned.CompanyAnalytics = companyAnalyticsToBeReturned;

            return userCompanyAnalyticsToBeReturned;
        }

        public static UserCompanyAnalytics GetUserCompanyAnalyticsDetail(string accessKey, string secretKey,string userId, string companyId)
        {
            var context = GetDynamoDbContext(accessKey, secretKey);
            //var res = context.Load<UserCompanyAnalytics>(userId, companyId);

            IEnumerable<UserCompanyAnalytics> res = context.Scan<UserCompanyAnalytics>(
                  new ScanCondition("Count", ScanOperator.GreaterThan, 5),
                  new ScanCondition("UserId", ScanOperator.Equal, "sumitchourasia91@gmail.com")
                  );

            return res.FirstOrDefault();

        }

        public static CompanyAnalytics GetCompanyAnalyticsDetail(string accessKey, string secretKey,string companyId)
        {
            var context = GetDynamoDbContext(accessKey, secretKey);
            return context.Load<CompanyAnalytics>(companyId);

        }

        private static DynamoDBContext GetDynamoDbContext(string accessKey, string secretKey)
        {
             var client = new AmazonDynamoDBClient(accessKey, secretKey, RegionEndpoint.USWest2);
             var context = new DynamoDBContext(client);
            return context;
        }
    }
}
