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
    public class GremlinServerGraphVertexDb : IGraphVertexDb
    {
        public Dictionary<string, string> AddVertex(string email, string graphName, Dictionary<string, string> properties, HashSet<string> canEdit, HashSet<string> canDelete,
            HashSet<string> sendNotificationToUsers)
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
            dynamoDbModel.UpsertOrbitPageVertexDetail(orbitPageVertexDetail, email, canEdit, canDelete, sendNotificationToUsers);

            return response;
        }

        public Dictionary<string, string> UpdateVertex(string vertexId, string email, string graphName, Dictionary<string, string> properties)
        {
            string url = TitanGraphConfig.Server;
            var orbitPageCompanyUserWorkgraphyTable = GetVertexDetailsFromDynamoDb(vertexId);

            var response = UpdateGraphVertex(vertexId, graphName, properties, url, orbitPageCompanyUserWorkgraphyTable);
            var orbitPageVertexDetail = new OrbitPageVertexDetail
            {
                url = url,
                vertexId = vertexId,
                graphName = graphName,
                properties = response
            };

            orbitPageCompanyUserWorkgraphyTable.OrbitPageVertexDetail = orbitPageVertexDetail;
            IDynamoDb dynamoDbModel = new DynamoDb();
            dynamoDbModel.CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(orbitPageCompanyUserWorkgraphyTable);

            return response;
        }

        private Dictionary<String, String> UpdateGraphVertex(string vertexId, string graphName, Dictionary<string, string> properties, string url, OrbitPageCompanyUserWorkgraphyTable orbitPageCompanyUserWorkgraphyTable)
        {
            var uri = new StringBuilder("/?gremlin=");

            var oldProperties = new Dictionary<string, string>();

            if (orbitPageCompanyUserWorkgraphyTable != null && orbitPageCompanyUserWorkgraphyTable.OrbitPageVertexDetail != null)
                oldProperties = orbitPageCompanyUserWorkgraphyTable.OrbitPageVertexDetail.properties;

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

            //new properties that is not available in old properties
            foreach (KeyValuePair<string, string> property in properties)
            {
                //merge updated values in dictionary..
                if (!newProperties.ContainsKey(property.Key))
                {
                    newProperties[property.Key] = properties[property.Key];
                }
            }

            //if user want to delete previous uploaded image.
            if (oldProperties.ContainsKey(VertexPropertyEnum.CoverImageUrl.ToString()) &&
                !properties.ContainsKey(VertexPropertyEnum.CoverImageUrl.ToString()))
            {
                newProperties.Remove(VertexPropertyEnum.CoverImageUrl.ToString());
            }

            if (newProperties.ContainsKey(VertexPropertyEnum.CoverImageUrl.ToString()) && string.IsNullOrEmpty(newProperties[VertexPropertyEnum.CoverImageUrl.ToString()]))
                newProperties[VertexPropertyEnum.CoverImageUrl.ToString()] = CommonConstants.CompanySquareLogoNotAvailableImage;

            string graphProperties = string.Empty;

            //g.V(4192).property('name','William2').property('age',30)
            foreach (KeyValuePair<string, string> property in properties)
            {                
                if (property.Key == VertexPropertyEnum.CreatedTimeLong.ToString() || property.Key == VertexPropertyEnum.PostedTimeLong.ToString())
                    graphProperties += ".property('" + property.Key + "'," + property.Value + ")";
                else
                    graphProperties += ".property('" + property.Key + "','" + property.Value + "')";                
            }

            url = url + uri.ToString() + "g.V(" + vertexId + ")" + graphProperties;
    
            var client = new RestClient(url);
            var request = new RestRequest();

            request.Method = Method.GET;
            request.AddHeader("Accept", "application/json");
            request.Parameters.Clear();
            request.AddParameter("application/json", "", ParameterType.RequestBody);

            var res = client.Execute(request);
            var content = res.Content; // raw content as string 

            //dynamic jsonResponse = JsonConvert.DeserializeObject(content);
            //var response = new Dictionary<String, String>();

            //TODO:To check if failed.
            //dynamic jsonResponse = JsonConvert.DeserializeObject(content);

            return newProperties;
        }

        private OrbitPageCompanyUserWorkgraphyTable GetVertexDetailsFromDynamoDb(string vertexId)
        {
            IDynamoDb dynamoDbModel = new DynamoDb();
            var orbitPageCompanyUserWorkgraphyTable = dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTable(DynamoDbHashKeyDataType.VertexDetail.ToString(),
                vertexId, null);

            //if (orbitPageCompanyUserWorkgraphyTable != null && orbitPageCompanyUserWorkgraphyTable.OrbitPageVertexDetail != null)
            //    return orbitPageCompanyUserWorkgraphyTable.OrbitPageVertexDetail.properties;

            return orbitPageCompanyUserWorkgraphyTable;
        }
        private Dictionary<String, String> CreateVertex(string graphName, Dictionary<string, string> properties, string url)
        {
            var uri = new StringBuilder("/?gremlin=");
            //graph.addVertex(label, "person", "name", "marko", "age", 29);

            string graphProperties = string.Empty;

            foreach (KeyValuePair<string, string> property in properties)
            {
                if (property.Key == VertexPropertyEnum.Type.ToString())
                {
                    graphProperties += "label, '" + property.Value + "' ,";
                }
                else
                {
                    if (property.Key == VertexPropertyEnum.CreatedTimeLong.ToString() ||
                        property.Key == VertexPropertyEnum.PostedTimeLong.ToString())
                        graphProperties += "'" + property.Key + "', " + property.Value + " ,";
                    else
                    {
                        graphProperties += "'" + property.Key + "', '" + property.Value + "' ,";
                    }
                }                
            }

            if (!string.IsNullOrEmpty(graphProperties))
            {
                graphProperties = graphProperties.Substring(0, graphProperties.Length - 2);
            }

            url = url + uri.ToString() + "graph.addVertex(" + graphProperties + ");";
            var client = new RestClient(url);
            var request = new RestRequest();

            request.Method = Method.GET;
            request.AddHeader("Accept", "application/json");
            request.Parameters.Clear();
            request.AddParameter("application/json", "", ParameterType.RequestBody);

            var res = client.Execute(request);
            var content = res.Content; // raw content as string 

            dynamic jsonResponse = JsonConvert.DeserializeObject(content);
            var response = new Dictionary<String, String>();
            response["status"] = "200";
            response[TitanGraphConstants.Id] = jsonResponse.result.data[0].id;
            response[TitanGraphConstants.RexsterUri] = url;
            return response;
        }

        public string ExecuteGremlinQuery(string gremlinQuery)
        {
            string url = TitanGraphConfig.Server;
            var uri = new StringBuilder("/?gremlin=");
            url = url + uri.ToString() + gremlinQuery;
            var client = new RestClient(url);
            var request = new RestRequest();

            request.Method = Method.GET;
            request.AddHeader("Accept", "application/json");
            request.Parameters.Clear();
            request.AddParameter("application/json", "", ParameterType.RequestBody);

            var res = client.Execute(request);
            var content = res.Content; // raw content as string 
            return content;
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

        public Dictionary<string, string> DeleteVertex(string vertexId, string userVertexId, string label)
        {
            string url = TitanGraphConfig.Server;

            IDynamoDb dynamoDbModel = new DynamoDb();
            var vertexInfo = dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTable(DynamoDbHashKeyDataType.VertexDetail.ToString(), vertexId, null);
            if (vertexInfo == null)
                return null;

            //Get all inEdges to delete
            var allInEdgesInfo = dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTableUsingInEdges(vertexId);

            //Get all outEdges to delete
            var allOutEdgesInfo = dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTableUsingOutEdges(vertexId);

            var response = DeleteVertexNative(TitanGraphConfig.Graph, vertexId, url);
            dynamoDbModel.DeleteOrbitPageCompanyUserWorkgraphyTable(vertexInfo);
            dynamoDbModel.DeleteOrbitPageCompanyUserWorkgraphyTable(allInEdgesInfo);
            dynamoDbModel.DeleteOrbitPageCompanyUserWorkgraphyTable(allOutEdgesInfo);

            return response;
        }

        private Dictionary<String, String> DeleteVertexNative(string graphName, string vertexId, string url)
        {
            var uri = new StringBuilder(url + "/graphs/" + graphName + "/vertices/" + vertexId);

            //graphs/<graph>/edges/3

            var client = new RestClient(uri.ToString());
            var request = new RestRequest();

            request.Method = Method.DELETE;
            request.AddHeader("Accept", "application/json");
            request.Parameters.Clear();
            request.AddParameter("application/json", "", ParameterType.RequestBody);

            var res = client.Execute(request);
            var content = res.Content; // raw content as string 

            //dynamic jsonResponse = JsonConvert.DeserializeObject(content);
            var response = new Dictionary<String, String>();
            response["status"] = "200";
            return response;
        }
    }
}
