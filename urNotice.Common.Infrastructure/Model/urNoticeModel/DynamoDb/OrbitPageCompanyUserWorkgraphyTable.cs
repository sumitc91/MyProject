using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using urNotice.Common.Infrastructure.Model.urNoticeAnalyticsContext;
using urNotice.Common.Infrastructure.Model.urNoticeAuthContext;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb
{
    [DynamoDBTable("OrbitPageCompanyUserWorkgraphyTable")]
    public class OrbitPageCompanyUserWorkgraphyTable
    {
        [DynamoDBHashKey]    //Partition key
        public string DataType { get; set; }

        [DynamoDBRangeKey]  //Sort key
        public string ObjectId { get; set; }

        [DynamoDBProperty]
        public string CompareId { get; set; }

        [DynamoDBProperty]
        public string FacebookId { get; set; }

        [DynamoDBProperty]
        public string FacebookAuthToken { get; set; }

        [DynamoDBProperty]
        public DateTime? GoogleApiCheckLastSyncedDateTime { get; set; }

        [DynamoDBProperty]
        public OrbitPageUser OrbitPageUser { get; set; }

        [DynamoDBProperty]
        public OrbitPageCompany OrbitPageCompany { get; set; }

        [DynamoDBProperty]
        public OrbitPageDesignation OrbitPageDesignation { get; set; }

        [DynamoDBProperty]
        public OrbitPageVertexDetail OrbitPageVertexDetail { get; set; }

        [DynamoDBProperty]
        public OrbitPageEdgeDetail OrbitPageEdgeDetail { get; set; }

        [DynamoDBProperty]
        public List<VirtualFriendList> OrbitPageGoogleApiContact { get; set; }

        [DynamoDBProperty]
        public Boolean IsSolrUpdated { get; set; }

        [DynamoDBProperty]
        public DateTime CreatedDate { get; set; }

    }
}
