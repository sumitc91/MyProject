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
    public class GraphVertexOperations
    {
        public Dictionary<String, String> AddVertex(string email, string url, string vertexId, string graphName, Dictionary<string, string> properties, string accessKey, string secretKey)
        {
            
            //var uri = new StringBuilder("/graphs/" + graphName + "/vertices/" + vertexId + "?");
            var uri = new StringBuilder("/graphs/" + graphName + "/vertices?");

            foreach (KeyValuePair<string, string> property in properties)
            {
                uri.Append(property.Key + "=" + property.Value + "&");
            }

                //; name=" + user.email + "&gender=" + user.gender;

            var client = new RestClient(url+uri.ToString());
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

            var orbitPageVertexDetail = new OrbitPageVertexDetail
            {
                url = url,
                vertexId = response[TitanGraphConstants.Id],
                graphName = graphName,
                properties = properties
            };

            new DynamoDbService.DynamoDbService().CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(
                    DynamoDbHashKeyDataType.VertexDetail.ToString(),
                    orbitPageVertexDetail.vertexId,
                    email,
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
                    accessKey,
                    secretKey
                    );

            return response;
        }


        public string GetVertexDetail(string url,string gremlinQuery, string vertexId, string graphName, Dictionary<string, string> properties)
        {

        //http://localhost:8182/graphs/graph/vertices/2571776/tp/gremlin?script=g.v(2569472).out(“_label”,”WallPost”)[0..1]
            //
            var uri = new StringBuilder("/graphs/" + graphName + "/vertices/" + vertexId);

            if (gremlinQuery != null)
            {
                uri.Append("/tp/gremlin?");
                uri.Append("script="+gremlinQuery);                
            }
            else if (properties.Count > 0)
            {
                uri.Append("?");
                foreach (KeyValuePair<string, string> property in properties)
                {
                    uri.Append(property.Key + "=" + property.Value + "&");
                }
            }
            

            //; name=" + user.email + "&gender=" + user.gender;

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
