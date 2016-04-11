﻿using System;
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
                        orbitPageGoogleApiContact,
                        null,
                        null,
                        null,
                        false
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
        public OrbitPageCompanyUserWorkgraphyTable UpsertOrbitPageVertexDetail(OrbitPageVertexDetail orbitPageVertexDetail, String userName)
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
                    orbitPageVertexDetail,
                    null,
                    null,
                    false                    
                    );
        }

        public OrbitPageCompanyUserWorkgraphyTable UpsertOrbitPageEdgeDetail(OrbitPageEdgeDetail orbitPageEdgeDetail, String userName,String inV,String outV)
        {
            return CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(
                    DynamoDbHashKeyDataType.EdgeDetail.ToString(),
                    orbitPageEdgeDetail.edgeId,
                    userName,
                    null,
                    null,
                    inV,
                    outV,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    orbitPageEdgeDetail,
                    null,
                    false                    
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
                            false                           
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
                            company,
                            null,
                            null,
                            null,
                            null,
                            null,
                            false                            
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
                            orbitPageUser,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            false                            
                            );
        }

        public OrbitPageCompanyUserWorkgraphyTable UpsertOrbitPageWorkgraphy(OrbitPageWorkgraphy orbitPageWorkgraphy)
        {
            return CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(
                            DynamoDbHashKeyDataType.OrbitPageWorkgraphy.ToString(),
                            orbitPageWorkgraphy.workgraphyVertexId,
                            orbitPageWorkgraphy.createdByEmail,
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
                            false
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
            OrbitPageUser orbitPageUser,
            OrbitPageWorkgraphy orbitPageWorkgraphy,
            OrbitPageCompany orbitPageCompany,
            OrbitPageDesignation orbitPageDesignation,
            List<VirtualFriendList> orbitPageGoogleApiContact,
            OrbitPageVertexDetail orbitPageVertexDetail,
            OrbitPageEdgeDetail orbitPageEdgeDetail,
            OrbitPageUserNotification orbitPageUserNotification,
            Boolean isSolrUpdated
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
            orbitPageCompanyUserWorkgraphyTable.OrbitPageUser = orbitPageUser;
            orbitPageCompanyUserWorkgraphyTable.OrbitPageWorkgraphy = orbitPageWorkgraphy;
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

        private static DynamoDBContext GetDynamoDbContext()
        {
            var client = new AmazonDynamoDBClient(AwsConfig._awsAccessKey,AwsConfig._awsSecretKey, RegionEndpoint.USWest2);
            var context = new DynamoDBContext(client);
            return context;
        }
    }
}
