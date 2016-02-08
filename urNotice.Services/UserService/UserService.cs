using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using urNotice.Common.Infrastructure.Common.Config;
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

        public IDictionary<string, string> CreateNewUserPost(urNoticeSession session, string message,string image, string accessKey, string secretKey)
        {
            var response = new Dictionary<string, string>();
            string url = TitanGraphConfig.Server;
            string graphName = TitanGraphConfig.Graph;

            string vertexId = session.UserName + "_" + DateTime.Now.Ticks;

            var properties = new Dictionary<string, string>();
            properties["PostMessage"] = message;
            properties["PostedByUser"] = session.UserName;
            properties["PostedTime"] = DateTimeUtil.GetUtcTime().ToString("s");
            properties["PostImage"] = image;
            IDictionary<string, string> addVertexResponse = new GraphVertexOperations().AddVertex(session.UserName,url, vertexId, graphName, properties, accessKey, secretKey);


            string edgeId = session.UserName + "_" + DateTime.Now.Ticks;
            properties = new Dictionary<string, string>();
            properties[EdgePropertyEnum._outV.ToString()] = session.UserVertexId;
            properties[EdgePropertyEnum._inV.ToString()] = addVertexResponse[TitanGraphConstants.Id];
            properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.Created.ToString();                      
            IDictionary<string, string> addCreatedByEdgeResponse = new GraphEdgeOperations().AddEdge(session,url, edgeId, graphName, properties,accessKey,secretKey);
            
            //_outV=<id>&_label=friend&_inV=2&<key>=<key'>
            edgeId = session.UserName + "_" + DateTime.Now.Ticks;
            properties = new Dictionary<string, string>();
            properties[EdgePropertyEnum._outV.ToString()] = addVertexResponse[TitanGraphConstants.Id];
            properties[EdgePropertyEnum._inV.ToString()] = session.UserVertexId;
            properties[EdgePropertyEnum._label.ToString()] = EdgeLabelEnum.WallPost.ToString();
            properties[EdgePropertyEnum.PostedBy.ToString()] = session.UserVertexId;
            properties[EdgePropertyEnum.PostedDate.ToString()] = DateTimeUtil.GetUtcTime().ToString("s");
            properties[EdgePropertyEnum.EdgeMessage.ToString()] = "";

            IDictionary<string, string> addEdgeResponse = new GraphEdgeOperations().AddEdge(session,url, edgeId, graphName, properties, accessKey,secretKey);

            response["status"] = "200";
            return response;
        }

        public string GetUserPost(string userVertexId, string @from, string to, string accessKey, string secretKey)
        {
            string url = TitanGraphConfig.Server;
            string graphName = TitanGraphConfig.Graph;            
            string outLabel = "WallPost";
            //g.v(2569472).as('userInfo').out('_label','WallPost').as('postInfo')[0..2].select{it}{it}
            string gremlinQuery = "g.v(" + userVertexId + ").as('userInfo').in('_label','" + outLabel + "').as('postInfo')[" + from + ".." + to + "].select{it}{it}";
            string response = new GraphVertexOperations().GetVertexDetail(url, gremlinQuery, userVertexId, graphName, null);

            return response;
        }
    }
}
