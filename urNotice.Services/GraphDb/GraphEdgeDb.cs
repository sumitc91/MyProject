using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using RestSharp;
using urNotice.Common.Infrastructure.Common.Config;
using urNotice.Common.Infrastructure.Common.Constants;
using urNotice.Common.Infrastructure.Common.Enum;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Services.NoSqlDb.DynamoDb;

namespace urNotice.Services.GraphDb
{
    public class GraphEdgeDb : IGraphEdgeDb
    {
        public Dictionary<string, string> AddEdge(string userName, string graphName, Dictionary<string, string> properties)
        {
            string url = TitanGraphConfig.Server;
            var response = CreateEdge(graphName, properties, url);            

            // add edge to dynamodb.
            var edgeDetail = new OrbitPageEdgeDetail
            {
                url = url,
                edgeId = response[TitanGraphConstants.Id],
                graphName = graphName,
                properties = properties
            };

            IDynamoDb dynamoDbModel = new DynamoDb();
            dynamoDbModel.UpsertOrbitPageEdgeDetail(edgeDetail, userName, properties[EdgePropertyEnum._inV.ToString()],properties[EdgePropertyEnum._outV.ToString()]);
            
            return response;
        }

        private Dictionary<String, String> CreateEdge(string graphName, Dictionary<string, string> properties, string url)
        {
            var uri = new StringBuilder(url + "/graphs/" + graphName + "/edges?");

            foreach (KeyValuePair<string, string> property in properties)
            {
                uri.Append(property.Key + "=" + HttpUtility.UrlEncode(property.Value) + "&");
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

            return response;
        }
    }
}
