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
        private delegate Dictionary<string, string> AddEdgeAsyncDelegate(string userName, string graphName, Dictionary<string, string> properties);

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

        public Dictionary<string, string> DeleteEdge(string inV, string outV,string label)
        {
            string url = TitanGraphConfig.Server;
            
            IDynamoDb dynamoDbModel = new DynamoDb();
            var edgeInfo = dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTableUsingInOutVertex(                        
                        inV,
                        outV,
                        label);
            if (edgeInfo == null)
                return null;

            var response = DeleteEdgeNative(TitanGraphConfig.Graph, edgeInfo.ObjectId, url);
            dynamoDbModel.DeleteOrbitPageCompanyUserWorkgraphyTable(edgeInfo);
            return response;
        }

        public Dictionary<string, string> AddEdgeAsync(string userName, string graphName, Dictionary<string, string> properties)
        {
            var addEdgeAsyncDelegate = new AddEdgeAsyncDelegate(AddEdge);
            addEdgeAsyncDelegate.BeginInvoke(userName, graphName, properties, null, null);
            return null;
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
            response["CreateEdgeStatus"] = "200";
            response[TitanGraphConstants.Id] = jsonResponse.results._id;

            return response;
        }

        private Dictionary<String, String> DeleteEdgeNative(string graphName, string edgeId, string url)
        {
            var uri = new StringBuilder(url + "/graphs/" + graphName + "/edges/"+edgeId);

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
