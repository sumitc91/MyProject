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
using urNotice.Common.Infrastructure.Model.urNoticeModel.GraphModel;
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

        public Dictionary<string, string> UpdateVertex(string vertexId,string email, string graphName, Dictionary<string, string> properties)
        {
            string url = TitanGraphConfig.Server;

            var response = UpdateGraphVertex(vertexId,graphName, properties, url);
            var orbitPageVertexDetail = new OrbitPageVertexDetail
            {
                url = url,
                vertexId = vertexId,
                graphName = graphName,
                properties = response
            };

            IDynamoDb dynamoDbModel = new DynamoDb();
            dynamoDbModel.UpsertOrbitPageVertexDetail(orbitPageVertexDetail, email);

            return response;
        }

        private Dictionary<String, String> UpdateGraphVertex(string vertexId, string graphName, Dictionary<string, string> properties, string url)
        {
            var uri = new StringBuilder("/graphs/" + graphName + "/vertices/"+vertexId+"?");

            var oldProperties = GetUserVertexDetails(vertexId);
            var newProperties = new Dictionary<String, String>();

            if (oldProperties == null)
            {
                return null;
            }

            foreach (KeyValuePair<string, string> property in oldProperties)
            {
                newProperties[property.Key] = property.Value;
            }
            foreach (KeyValuePair<string, string> property in oldProperties)
            {
                //merge updated values in dictionary..
                if (properties.ContainsKey(property.Key))
                {
                    newProperties[property.Key] = properties[property.Key];
                }                    
            }

            foreach (KeyValuePair<string, string> property in newProperties)
            {                
                uri.Append(property.Key + "=" + HttpUtility.UrlEncode(property.Value) + "&");
            }

            if (properties.ContainsKey(VertexPropertyEnum.CoverImageUrl.ToString()) && string.IsNullOrEmpty(properties[VertexPropertyEnum.CoverImageUrl.ToString()]))
                properties[VertexPropertyEnum.CoverImageUrl.ToString()] = CommonConstants.CompanySquareLogoNotAvailableImage;

            var client = new RestClient(url + uri.ToString());
            var request = new RestRequest();

            request.Method = Method.PUT;
            request.AddHeader("Accept", "application/json");
            request.Parameters.Clear();
            request.AddParameter("application/json", "", ParameterType.RequestBody);

            var res = client.Execute(request);
            var content = res.Content; // raw content as string 

            //TODO:To check if failed.
            //dynamic jsonResponse = JsonConvert.DeserializeObject(content);
            
            return newProperties;
        }

        private Dictionary<string, string> GetUserVertexDetails(string vertexId)
        {
            IDynamoDb dynamoDbModel = new DynamoDb();
            var orbitPageCompanyUserWorkgraphyTable = dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTable(DynamoDbHashKeyDataType.VertexDetail.ToString(),
                vertexId, null);

            if (orbitPageCompanyUserWorkgraphyTable != null && orbitPageCompanyUserWorkgraphyTable.OrbitPageVertexDetail != null)
                return orbitPageCompanyUserWorkgraphyTable.OrbitPageVertexDetail.properties;

            return null;
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
