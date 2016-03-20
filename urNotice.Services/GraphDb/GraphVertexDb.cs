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
    public class GraphVertexDb : IGraphVertexDb
    {
        public Dictionary<string, string> AddVertex(string email, string graphName, Dictionary<string, string> properties)
        {
            string url = TitanGraphConfig.Server;
            var response = CreateVertex(graphName, properties, url);            
            var orbitPageVertexDetail = new OrbitPageVertexDetail
            {
                url = url,
                vertexId = response[TitanGraphConstants.Id],
                graphName = graphName,
                properties = properties
            };

            IDynamoDb dynamoDbModel = new DynamoDb();
            dynamoDbModel.UpsertOrbitPageVertexDetail(orbitPageVertexDetail, email);
            
            return response;
        }
        private Dictionary<String, String> CreateVertex(string graphName, Dictionary<string, string> properties, string url)
        {
            var uri = new StringBuilder("/graphs/" + graphName + "/vertices?");
            
            foreach (KeyValuePair<string, string> property in properties)
            {
                uri.Append(property.Key + "=" + HttpUtility.UrlEncode(property.Value) + "&");
            }

            var client = new RestClient(url + uri.ToString());
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
            response[TitanGraphConstants.RexsterUri] = uri.ToString();
            return response;
        }
        public string GetVertexDetail(string gremlinQuery, string vertexId, string graphName, Dictionary<string, string> properties)
        {
            var uri = new StringBuilder("/graphs/" + graphName + "/vertices/" + vertexId);
            string url = TitanGraphConfig.Server;
            if (gremlinQuery != null)
            {
                uri.Append("/tp/gremlin?");
                uri.Append("script=" + gremlinQuery);
            }
            else if (properties.Count > 0)
            {
                uri.Append("?");
                foreach (KeyValuePair<string, string> property in properties)
                {
                    uri.Append(property.Key + "=" + property.Value + "&");
                }
            }

            var client = new RestClient(url + uri.ToString());
            var request = new RestRequest();

            request.Method = Method.GET;
            request.AddHeader("Accept", "application/json");
            request.Parameters.Clear();
            request.AddParameter("application/json", "", ParameterType.RequestBody);

            var res = client.Execute(request);
            var content = res.Content; // raw content as string 
            return content;
        }
    }
}
