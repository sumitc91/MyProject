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

        public OrbitPageCompanyUserWorkgraphyTable CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(
            string dataType, 
            string objectId, 
            string compareId, 
            string facebookId, 
            string facebookAuthKey,
            string inV,
            string outV,
            OrbitPageUser orbitPageUser, 
            OrbitPageCompany orbitPageCompany, 
            OrbitPageDesignation orbitPageDesignation, 
            List<VirtualFriendList> orbitPageGoogleApiContact,
            OrbitPageVertexDetail orbitPageVertexDetail,
            OrbitPageEdgeDetail orbitPageEdgeDetail,
            OrbitPageUserNotification orbitPageUserNotification,
            Boolean isSolrUpdated, 
            string accessKey, 
            string secretKey)
        {
            var context = GetDynamoDbContext(accessKey, secretKey);

            var orbitPageCompanyUserWorkgraphyTable = new OrbitPageCompanyUserWorkgraphyTable();
            orbitPageCompanyUserWorkgraphyTable.DataType = dataType;
            orbitPageCompanyUserWorkgraphyTable.ObjectId = objectId;
            orbitPageCompanyUserWorkgraphyTable.CompareId = compareId;
            orbitPageCompanyUserWorkgraphyTable.FacebookId = facebookId;
            orbitPageCompanyUserWorkgraphyTable.InV = inV;
            orbitPageCompanyUserWorkgraphyTable.OutV = outV;
            orbitPageCompanyUserWorkgraphyTable.OrbitPageUser = orbitPageUser;
            orbitPageCompanyUserWorkgraphyTable.OrbitPageCompany = orbitPageCompany;
            orbitPageCompanyUserWorkgraphyTable.OrbitPageGoogleApiContact = orbitPageGoogleApiContact;
            orbitPageCompanyUserWorkgraphyTable.FacebookAuthToken = facebookAuthKey;
            orbitPageCompanyUserWorkgraphyTable.OrbitPageVertexDetail = orbitPageVertexDetail;
            orbitPageCompanyUserWorkgraphyTable.OrbitPageEdgeDetail = orbitPageEdgeDetail;
            orbitPageCompanyUserWorkgraphyTable.CreatedDate = DateTimeUtil.GetUtcTime();
            orbitPageCompanyUserWorkgraphyTable.OrbitPageUserNotification = orbitPageUserNotification;
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

        public IEnumerable<OrbitPageCompanyUserWorkgraphyTable> GetOrbitPageCompanyUserWorkgraphyTableForNotification(string vertexId, string accessKey, string secretKey)
        {
            var context = GetDynamoDbContext(accessKey, secretKey);
            //var res = context.Load<OrbitPageCompanyUserWorkgraphyTable>(dataType, objectId);

            IEnumerable<OrbitPageCompanyUserWorkgraphyTable> res = context.Scan<OrbitPageCompanyUserWorkgraphyTable>(                  
                  new ScanCondition("DataType", ScanOperator.Equal, OrbitPageUtil.GetNotificationHashKeyUserEmail(vertexId))
                  );

            return res;

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


        private static DynamoDBContext GetDynamoDbContext(string accessKey, string secretKey)
        {
             var client = new AmazonDynamoDBClient(accessKey, secretKey, RegionEndpoint.USWest2);
             var context = new DynamoDBContext(client);
            return context;
        }
    }
}
