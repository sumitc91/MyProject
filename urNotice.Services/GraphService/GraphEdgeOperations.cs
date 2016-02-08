using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using urNotice.Common.Infrastructure.Common.Constants;
using urNotice.Common.Infrastructure.Common.Enum;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Common.Infrastructure.Session;

namespace urNotice.Services.GraphService
{
    public class GraphEdgeOperations
    {
        public Dictionary<String, String> AddEdge(urNoticeSession session, string url, string edgeId, string graphName, Dictionary<string, string> properties, string accessKey, string secretKey)
        {

            //var uri = new StringBuilder(url + "/graphs/" + graphName + "/edges/" + edgeId + "?");
            var uri = new StringBuilder(url + "/graphs/" + graphName + "/edges?");

            foreach (KeyValuePair<string, string> property in properties)
            {
                uri.Append(property.Key + "=" + property.Value + "&");
            }

            //; name=" + user.email + "&gender=" + user.gender;

            var client = new RestClient(uri.ToString());
            var request = new RestRequest();

            request.Method = Method.POST;
            request.AddHeader("Accept", "application/json");
            request.Parameters.Clear();
            request.AddParameter("application/json", "", ParameterType.RequestBody);

            var res = client.Execute(request);
            var content = res.Content; // raw content as string 

            dynamic jsonResponse = JsonConvert.DeserializeObject(content);
            var response = new Dictionary<String, String>();
            response["status"] = "200";
            response[TitanGraphConstants.Id] = jsonResponse.results._id;


            // add edge to dynamodb.
            var edgeDetail = new OrbitPageEdgeDetail
            {
                url = url,
                edgeId = response[TitanGraphConstants.Id],
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

            return response;
        }
    }
}
