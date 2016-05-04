using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using urNotice.Common.Infrastructure.Model.NoSqlDb;
using urNotice.Common.Infrastructure.Model.urNoticeAnalyticsContext;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;

namespace urNotice.Services.NoSqlDb.DynamoDb
{
    public interface IDynamoDb:INoSqlDb
    {
        OrbitPageCompanyUserWorkgraphyTable CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(OrbitPageCompanyUserWorkgraphyTable orbitPageCompanyUserWorkgraphyTable);
        OrbitPageCompanyUserWorkgraphyTable UpsertOrbitPageUser(OrbitPageUser orbitPageUser,String accessToken);
        OrbitPageCompanyUserWorkgraphyTable UpsertOrbitPageUpdateLastNotificationSeenTimeStamp(String userName,String dataType,long timeStamp);
        OrbitPageCompanyUserWorkgraphyTable UpsertOrbitPageWorkgraphy(OrbitPageWorkgraphy orbitPageWorkgraphy);
        OrbitPageCompanyUserWorkgraphyTable UpsertOrbitPageDesignation(String designationName,String designationVertexId);
        OrbitPageCompanyUserWorkgraphyTable UpsertOrbitPageCompany(OrbitPageCompany company, string companyVertexId);
        OrbitPageCompanyUserWorkgraphyTable UpsertOrbitPageEdgeDetail(OrbitPageEdgeDetail orbitPageEdgeDetail, String userName,String inV,String outV);

        OrbitPageCompanyUserWorkgraphyTable UpsertOrbitPageEdgeForQueryDetail(OrbitPageEdgeDetail orbitPageEdgeDetail,String userName, String inV, String outV);
        OrbitPageCompanyUserWorkgraphyTable UpsertOrbitPageVertexDetail(OrbitPageVertexDetail orbitPageVertexDetail, String userName, HashSet<string> canEdit, HashSet<string> canDelete, HashSet<string> sendNotificationToUsers);
        OrbitPageCompanyUserWorkgraphyTable UpsertOrbitPageGoogleApiContacts(List<VirtualFriendList> orbitPageGoogleApiContact, String userName, String startIndex);
        OrbitPageCompanyUserWorkgraphyTable GetOrbitPageCompanyUserWorkgraphyTable(string dataType, string objectId,string compareId);
        OrbitPageCompanyUserWorkgraphyTable GetOrbitPageCompanyUserWorkgraphyTableUsingFacebookId(string dataType,string facebookId);
        long? GetOrbitPageCompanyUserWorkgraphyTableLastSeenNotifiationTimeStamp(string userName, string notificationType);
        OrbitPageCompanyUserWorkgraphyTable GetOrbitPageCompanyUserWorkgraphyTableUsingInOutVertex(string inV,string outV,string label);

        IEnumerable<OrbitPageCompanyUserWorkgraphyTable> GetOrbitPageCompanyUserWorkgraphyTableUsingOutEdges(string outV);

        IEnumerable<OrbitPageCompanyUserWorkgraphyTable> GetOrbitPageCompanyUserWorkgraphyTableUsingInEdges(string inV);
        bool DeleteOrbitPageCompanyUserWorkgraphyTable(OrbitPageCompanyUserWorkgraphyTable res);

        bool DeleteOrbitPageCompanyUserWorkgraphyTable(IEnumerable<OrbitPageCompanyUserWorkgraphyTable> resList);
    }
}
