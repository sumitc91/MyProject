using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using urNotice.Common.Infrastructure.Common.Constants;
using urNotice.Common.Infrastructure.Common.Enum;
using urNotice.Common.Infrastructure.Common.Logger;
using urNotice.Common.Infrastructure.commonMethods;
using urNotice.Common.Infrastructure.Model.urNoticeAuthContext;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper;
using urNotice.Common.Infrastructure.Session;
using urNotice.Services.GraphService;

namespace urNotice.Services.UserService
{
    public class UserService
    {
        private static readonly ILogger Logger = new Logger(Convert.ToString(MethodBase.GetCurrentMethod().DeclaringType));
        private DbContextException _dbContextException = new DbContextException();
        //private readonly urnoticeAuthEntities _db = new urnoticeAuthEntities();

        public ResponseModel<ClientDetailsModel> GetClientDetails(string username,string accessKey,string secretKey)
        {
            var response = new ResponseModel<ClientDetailsModel>();

            try
            {
                //var clientDetailDbResult = _db.Users.SingleOrDefault(x => x.username == username);
                var userInfo = new DynamoDbService.DynamoDbService().GetOrbitPageCompanyUserWorkgraphyTable(
                DynamoDbHashKeyDataType.OrbitPageUser.ToString(),
                username,
                null,
                accessKey,
                secretKey
                );

                if (userInfo != null)
                {
                    var createClientDetailResponse = new ClientDetailsModel
                    {
                        FirstName = userInfo.OrbitPageUser.firstName,
                        LastName = userInfo.OrbitPageUser.lastName,
                        Username = userInfo.OrbitPageUser.email,
                        imageUrl = userInfo.OrbitPageUser.imageUrl == CommonConstants.NA ? CommonConstants.clientImageUrl : userInfo.OrbitPageUser.imageUrl,
                        gender = userInfo.OrbitPageUser.gender,
                        isLocked = userInfo.OrbitPageUser.locked
                    };

                    response.Status = 200;
                    response.Message = "success";
                    response.Payload = createClientDetailResponse;

                }
                else
                {
                    response.Status = 404;
                    response.Message = "username not found";
                }
            }
            catch (Exception)
            {
                response.Status = 500;
                response.Message = "exception occured !!!";
            }
            return response;
        }

        public IDictionary<string, string> CreateNewUserPost(urNoticeSession session, string message, string accessKey, string secretKey)
        {
            var response = new Dictionary<string, string>();
            string url = "http://54.148.127.109:8182";            
            string graphName = "graph";

            string vertexId = session.UserName + "_" + DateTime.Now.Ticks;

            var properties = new Dictionary<string, string>();
            properties["PostMessage"] = message;
            properties["PostedByUser"] = session.UserName;
            properties["PostedTime"] = DateTimeUtil.GetUtcTime().ToString("s");
            properties["PostImage"] = "https://s3-ap-southeast-1.amazonaws.com/urnotice/company/medium/be159063-77ca-4729-a63b-8928380922e0.png";
            

            IDictionary<string,string> addVertexResponse = new GraphVertexOperations().AddVertex(url, vertexId, graphName, properties);

            var orbitPageVertexDetail = new OrbitPageVertexDetail
            {
                url = url,
                vertexId = addVertexResponse[TitanGraphConstants.Id],
                graphName = graphName,
                properties = properties
            };

            new DynamoDbService.DynamoDbService().CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(
                    DynamoDbHashKeyDataType.VertexDetail.ToString(),
                    orbitPageVertexDetail.vertexId,
                    session.UserName,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    orbitPageVertexDetail,
                    null,
                    false,
                    accessKey,
                    secretKey
                    );
            
            //_outV=<id>&_label=friend&_inV=2&<key>=<key'>
            string edgeId = session.UserName + "_" + DateTime.Now.Ticks;

            properties = new Dictionary<string, string>();
            properties[EdgePropertyEnum._outV.ToString()] = session.UserVertexId;
            properties[EdgePropertyEnum._inV.ToString()] = orbitPageVertexDetail.vertexId;
            properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.WallPost.ToString();
            properties[EdgePropertyEnum.PostedDate.ToString()] = DateTimeUtil.GetUtcTime().ToString("s");
            properties[EdgePropertyEnum.EdgeMessage.ToString()] = "";


            IDictionary<string, string> addEdgeResponse = new GraphEdgeOperations().AddEdge(url, edgeId, graphName, properties);

            var edgeDetail = new OrbitPageEdgeDetail
            {
                url = url,
                edgeId = addEdgeResponse[TitanGraphConstants.Id],
                graphName = graphName,
                properties = properties
            };
            new DynamoDbService.DynamoDbService().CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(
                    DynamoDbHashKeyDataType.EdgeDetail.ToString(),
                    edgeDetail.edgeId,
                    session.UserName,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    edgeDetail,
                    false,
                    accessKey,
                    secretKey
                    );

            response["status"] = "200";
            return response;
        }

        public string GetUserPost(urNoticeSession session, string @from, string to, string accessKey, string secretKey)
        {
            string url = "http://54.148.127.109:8182";            
            string graphName = "graph";            
            string outLabel = "WallPost";
            string gremlinQuery ="g.v(" + session.UserVertexId + ").out('_label','" + outLabel + "')[" + from + ".." + to + "]";
            string response = new GraphVertexOperations().GetVertexDetail(url, gremlinQuery, session.UserVertexId, graphName, null);

            return response;
        }
    }
}
