using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using urNotice.Common.Infrastructure.Common.Config;
using urNotice.Common.Infrastructure.Common.Enum;
using urNotice.Common.Infrastructure.commonMethods;
using urNotice.Common.Infrastructure.Model.urNoticeAnalyticsContext;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;

namespace urNotice.Services.NoSqlDb.DynamoDb
{
    public class DynamoDb:IDynamoDb
    {
        

        public OrbitPageCompanyUserWorkgraphyTable CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(OrbitPageCompanyUserWorkgraphyTable orbitPageCompanyUserWorkgraphyTable)
        {
            var context = GetDynamoDbContext();
            context.Save(orbitPageCompanyUserWorkgraphyTable);
            return orbitPageCompanyUserWorkgraphyTable;
        }

        public OrbitPageCompanyUserWorkgraphyTable UpsertOrbitPageGoogleApiContacts(List<VirtualFriendList> orbitPageGoogleApiContact, String userName,String startIndex)
        {
            return CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(
                        DynamoDbHashKeyDataType.GmailFriends.ToString(),
                        userName + "_" + startIndex,
                        userName,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        orbitPageGoogleApiContact,
                        null,
                        null,
                        null,
                        false,
                        null,
                        null,
                        null,
                        null
                        );
        }

        public OrbitPageCompanyUserWorkgraphyTable GetOrbitPageCompanyUserWorkgraphyTable(string dataType, string objectId,string compareId)
        {
            var context = GetDynamoDbContext();
            var res = context.Load<OrbitPageCompanyUserWorkgraphyTable>(dataType, objectId);
            return res;

            //IEnumerable<OrbitPageCompanyUserWorkgraphyTable> res = context.Scan<OrbitPageCompanyUserWorkgraphyTable>(
            //      new ScanCondition("DataType", ScanOperator.Equal, dataType),
            //      new ScanCondition("ObjectId", ScanOperator.Equal, objectId)
            //      );

            //return res.FirstOrDefault();
            
        }

        public OrbitPageCompanyUserWorkgraphyTable GetOrbitPageCompanyUserWorkgraphyTableUsingFacebookId(string dataType, string facebookId)
        {
            var context = GetDynamoDbContext();
            
            IEnumerable<OrbitPageCompanyUserWorkgraphyTable> res = context.Scan<OrbitPageCompanyUserWorkgraphyTable>(
                  new ScanCondition("FacebookId", ScanOperator.Equal, facebookId),
                  new ScanCondition("DataType", ScanOperator.Equal, dataType)
                  );

            return res.FirstOrDefault();

        }

        public OrbitPageCompanyUserWorkgraphyTable GetOrbitPageCompanyUserWorkgraphyTableUsingInOutVertex(string inV, string outV,string label)
        {
            var context = GetDynamoDbContext();

            IEnumerable<OrbitPageCompanyUserWorkgraphyTable> res = context.Scan<OrbitPageCompanyUserWorkgraphyTable>(
                new ScanCondition("DataType", ScanOperator.Equal, DynamoDbHashKeyDataType.EdgeDetail.ToString()),
                new ScanCondition("Label", ScanOperator.Equal, label),
                new ScanCondition("InV", ScanOperator.Equal, inV),
                new ScanCondition("OutV", ScanOperator.Equal, outV)
                  );
            return res.FirstOrDefault();
        }

        public IEnumerable<OrbitPageCompanyUserWorkgraphyTable> GetOrbitPageCompanyUserWorkgraphyTableUsingOutEdges(string outV)
        {
            var context = GetDynamoDbContext();

            IEnumerable<OrbitPageCompanyUserWorkgraphyTable> res = context.Scan<OrbitPageCompanyUserWorkgraphyTable>(
                new ScanCondition("DataType", ScanOperator.Equal, DynamoDbHashKeyDataType.EdgeDetail.ToString()),                               
                new ScanCondition("OutV", ScanOperator.Equal, outV)
                  );
            return res;
        }

        public IEnumerable<OrbitPageCompanyUserWorkgraphyTable> GetOrbitPageCompanyUserWorkgraphyTableUsingInEdges(string inV)
        {
            var context = GetDynamoDbContext();

            IEnumerable<OrbitPageCompanyUserWorkgraphyTable> res = context.Scan<OrbitPageCompanyUserWorkgraphyTable>(
                new ScanCondition("DataType", ScanOperator.Equal, DynamoDbHashKeyDataType.EdgeDetail.ToString()),                
                new ScanCondition("InV", ScanOperator.Equal, inV)
                  );
            return res;
        }

        public long? GetOrbitPageCompanyUserWorkgraphyTableLastSeenNotifiationTimeStamp(string userName,string notificationType)
        {
            var context = GetDynamoDbContext();
            string dataType = string.Empty;
            
            if (notificationType == NotificationEnum.Notifications.ToString())
            {
                dataType = DynamoDbHashKeyDataType.LastSeenNotification.ToString();
            }

            if (notificationType == NotificationEnum.FriendRequests.ToString())
            {
                dataType = DynamoDbHashKeyDataType.LastSeenFriendRequestNotification.ToString();
            }

            if (notificationType == NotificationEnum.Messages.ToString())
            {
                dataType = DynamoDbHashKeyDataType.LastSeenMessageNotification.ToString();
            }

            IEnumerable<OrbitPageCompanyUserWorkgraphyTable> res = context.Scan<OrbitPageCompanyUserWorkgraphyTable>(
                new ScanCondition("DataType", ScanOperator.Equal, dataType),
                new ScanCondition("ObjectId", ScanOperator.Equal, userName)                
                  );

            if (res != null)
            {
                var result = res.FirstOrDefault();
                if (result != null)
                {                    
                     return result.LastNotificationSeenTimeStamp;                    
                }
            }

            return null;
        }

        public bool DeleteOrbitPageCompanyUserWorkgraphyTable(OrbitPageCompanyUserWorkgraphyTable res)
        {
            var context = GetDynamoDbContext();
            context.Delete(res);
            return true;

        }

        public bool DeleteOrbitPageCompanyUserWorkgraphyTable(IEnumerable<OrbitPageCompanyUserWorkgraphyTable> resList)
        {
            //TODO:delete multiple items from dynamodb.
            //var context = GetDynamoDbContext();
            //context.Delete(resList);
            
            return true;
        }

        public OrbitPageCompanyUserWorkgraphyTable UpsertOrbitPageUpdateLastNotificationSeenTimeStamp(String userName,String dataType,long timeStamp)
        {
            return CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(
                    dataType,
                    userName,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    false,
                    timeStamp,
                    null,
                    null,
                    null
                    );
        }

        public OrbitPageCompanyUserWorkgraphyTable UpsertOrbitPageVertexDetail(OrbitPageVertexDetail orbitPageVertexDetail, String userName, HashSet<string> canEdit, HashSet<string> canDelete, HashSet<string> sendNotificationToUsers)
        {
            
            return CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(
                    DynamoDbHashKeyDataType.VertexDetail.ToString(),
                    orbitPageVertexDetail.vertexId,
                    userName,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    orbitPageVertexDetail,
                    null,
                    null,
                    false,
                    null,
                    canEdit,
                    canDelete,
                    sendNotificationToUsers
                    );
        }

        public OrbitPageCompanyUserWorkgraphyTable UpsertOrbitPageEdgeDetail(OrbitPageEdgeDetail orbitPageEdgeDetail, String userName,String inV,String outV)
        {
            var canEdit = new HashSet<string> { };
            var canDelete = new HashSet<string> { };

            return CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(
                    DynamoDbHashKeyDataType.EdgeDetail.ToString(),
                    orbitPageEdgeDetail.edgeId,
                    userName,
                    null,
                    null,
                    inV,
                    outV,
                    orbitPageEdgeDetail.properties[EdgePropertyEnum._label.ToString()],
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    orbitPageEdgeDetail,
                    null,
                    false,
                    null,
                    canEdit,
                    canDelete,
                    null
                    );
        }

        public OrbitPageCompanyUserWorkgraphyTable UpsertOrbitPageDesignation(String designationName, string designationVertexId)
        {
            return CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(
                            DynamoDbHashKeyDataType.Designation.ToString(),
                            designationName,
                            designationVertexId,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            false,
                            null,
                            null,
                            null,
                            null
                            );
        }

        public OrbitPageCompanyUserWorkgraphyTable UpsertOrbitPageCompany(OrbitPageCompany company, string companyVertexId)
        {            
            return CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(
                            DynamoDbHashKeyDataType.Company.ToString(),
                            company.CompanyName,
                            companyVertexId,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            company,
                            null,
                            null,
                            null,
                            null,
                            null,
                            false,
                            null,
                            null,
                            null,
                            null
                            );
        }

        public OrbitPageCompanyUserWorkgraphyTable UpsertOrbitPageUser(OrbitPageUser orbitPageUser, string accessToken)
        {
            return CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(
                            DynamoDbHashKeyDataType.OrbitPageUser.ToString(),
                            orbitPageUser.email,
                            orbitPageUser.username,
                            orbitPageUser.facebookId,
                            accessToken,
                            null,
                            null,
                            null,
                            orbitPageUser,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            false,
                            null,
                            null,
                            null,
                            null
                            );
        }

        public OrbitPageCompanyUserWorkgraphyTable UpsertOrbitPageWorkgraphy(OrbitPageWorkgraphy orbitPageWorkgraphy)
        {
            var sendNotificationToUsers = new HashSet<string> {orbitPageWorkgraphy.createdByVertexId};

            return CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(
                            DynamoDbHashKeyDataType.OrbitPageWorkgraphy.ToString(),
                            orbitPageWorkgraphy.workgraphyVertexId,
                            orbitPageWorkgraphy.createdByEmail,
                            null,
                            null,
                            null,
                            null,  
                            null,
                            null,
                            orbitPageWorkgraphy,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            false,
                            null,
                            null,
                            null,
                            sendNotificationToUsers
                            );
        }

        private OrbitPageCompanyUserWorkgraphyTable CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(
            string dataType,
            string objectId,
            string compareId,
            string facebookId,
            string facebookAuthKey,
            string inV,
            string outV,
            string label,
            OrbitPageUser orbitPageUser,
            OrbitPageWorkgraphy orbitPageWorkgraphy,
            OrbitPageCompany orbitPageCompany,
            OrbitPageDesignation orbitPageDesignation,
            List<VirtualFriendList> orbitPageGoogleApiContact,
            OrbitPageVertexDetail orbitPageVertexDetail,
            OrbitPageEdgeDetail orbitPageEdgeDetail,
            OrbitPageUserNotification orbitPageUserNotification,
            Boolean isSolrUpdated,
            long? lastNotificationSeenTimeStamp,
            HashSet<String> canEdit,
            HashSet<String> canDelete,
            HashSet<String> sendNotificationToUserList
            )
        {
            var context = GetDynamoDbContext();

            var orbitPageCompanyUserWorkgraphyTable = new OrbitPageCompanyUserWorkgraphyTable();
            orbitPageCompanyUserWorkgraphyTable.DataType = dataType;
            orbitPageCompanyUserWorkgraphyTable.ObjectId = objectId;
            orbitPageCompanyUserWorkgraphyTable.CompareId = compareId;
            orbitPageCompanyUserWorkgraphyTable.FacebookId = facebookId;
            orbitPageCompanyUserWorkgraphyTable.InV = inV;
            orbitPageCompanyUserWorkgraphyTable.OutV = outV;
            orbitPageCompanyUserWorkgraphyTable.Label = label;
            orbitPageCompanyUserWorkgraphyTable.OrbitPageUser = orbitPageUser;
            orbitPageCompanyUserWorkgraphyTable.OrbitPageWorkgraphy = orbitPageWorkgraphy;
            orbitPageCompanyUserWorkgraphyTable.OrbitPageCompany = orbitPageCompany;
            orbitPageCompanyUserWorkgraphyTable.OrbitPageDesignation = orbitPageDesignation;
            orbitPageCompanyUserWorkgraphyTable.IsSolrUpdated = isSolrUpdated;
            orbitPageCompanyUserWorkgraphyTable.OrbitPageGoogleApiContact = orbitPageGoogleApiContact;
            orbitPageCompanyUserWorkgraphyTable.FacebookAuthToken = facebookAuthKey;
            orbitPageCompanyUserWorkgraphyTable.OrbitPageVertexDetail = orbitPageVertexDetail;
            orbitPageCompanyUserWorkgraphyTable.OrbitPageEdgeDetail = orbitPageEdgeDetail;
            orbitPageCompanyUserWorkgraphyTable.CreatedDate = DateTimeUtil.GetUtcTime();
            orbitPageCompanyUserWorkgraphyTable.OrbitPageUserNotification = orbitPageUserNotification;
            orbitPageCompanyUserWorkgraphyTable.LastNotificationSeenTimeStamp = lastNotificationSeenTimeStamp;
            orbitPageCompanyUserWorkgraphyTable.CanEdit = canEdit;
            orbitPageCompanyUserWorkgraphyTable.CanDelete = canDelete;
            orbitPageCompanyUserWorkgraphyTable.SendNotificationToUserList = sendNotificationToUserList;

            context.Save(orbitPageCompanyUserWorkgraphyTable);
            return orbitPageCompanyUserWorkgraphyTable;
        }

        private static DynamoDBContext GetDynamoDbContext()
        {
            var client = new AmazonDynamoDBClient(AwsConfig._awsAccessKey,AwsConfig._awsSecretKey, RegionEndpoint.USWest2);
            var context = new DynamoDBContext(client);
            return context;
        }
    }
}
