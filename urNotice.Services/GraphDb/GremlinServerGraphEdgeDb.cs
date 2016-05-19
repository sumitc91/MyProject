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
using urNotice.Common.Infrastructure.commonMethods;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Services.NoSqlDb.DynamoDb;

namespace urNotice.Services.GraphDb
{
    public class GremlinServerGraphEdgeDb : IGraphEdgeDb
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
            dynamoDbModel.UpsertOrbitPageEdgeDetail(edgeDetail, userName, properties[EdgePropertyEnum._inV.ToString()], properties[EdgePropertyEnum._outV.ToString()]);

            //Adding edgeDetail for faster query.
            //dynamoDbModel.UpsertOrbitPageEdgeForQueryDetail(edgeDetail, userName, properties[EdgePropertyEnum._inV.ToString()], properties[EdgePropertyEnum._outV.ToString()]);

            return response;
        }

        public Dictionary<string, string> DeleteEdge(string inV, string outV, string label)
        {
            string url = TitanGraphConfig.Server;

            IDynamoDb dynamoDbModel = new DynamoDb();
            string uniqueKey = OrbitPageUtil.GenerateUniqueKeyForEdgeQuery(inV, label, outV);
            var edgeInfo = dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTable(
                       DynamoDbHashKeyDataType.EdgeDetail.ToString(),
                       uniqueKey,
                       null);

            if (edgeInfo == null)
                return null;

            var response = DeleteEdgeNative(TitanGraphConfig.Graph, edgeInfo.CompareId, url);
            dynamoDbModel.DeleteOrbitPageCompanyUserWorkgraphyTable(edgeInfo);

            //Deleting Edge detail creating for only query purpose.
            //string uniqueKey = OrbitPageUtil.GenerateUniqueKeyForEdgeQuery(inV, label, outV);
            //edgeInfo = dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTable(
            //            DynamoDbHashKeyDataType.EdgeDetail.ToString(),
            //            uniqueKey,
            //            null);

            //if(edgeInfo!=null)
            //    dynamoDbModel.DeleteOrbitPageCompanyUserWorkgraphyTable(edgeInfo);

            return response;
        }

        public Dictionary<string, string> AddEdgeAsync(string userName, string graphName, Dictionary<string, string> properties)
        {
            var addEdgeAsyncDelegate = new GremlinServerGraphEdgeDb.AddEdgeAsyncDelegate(AddEdge);
            addEdgeAsyncDelegate.BeginInvoke(userName, graphName, properties, null, null);
            return null;
        }

        private Dictionary<String, String> CreateEdge(string graphName, Dictionary<string, string> properties, string url)
        {
            var uri = new StringBuilder(url + "/?gremlin=");

            //http://localhost:8182/?gremlin=g.V(8320).next().addEdge("Using",g.V(12416).next(),"Desc","Item used by Person","time",12345)

            string graphProperties = string.Empty;

            //_outV must be the first parameter
            graphProperties += "'" + properties[EdgePropertyEnum._label.ToString()] + "', g.V(" + properties[EdgePropertyEnum._inV.ToString()] + ").next() ,";
            foreach (KeyValuePair<string, string> property in properties)
            {
                if (property.Key == EdgePropertyEnum._inV.ToString() || property.Key == EdgePropertyEnum._outV.ToString() || property.Key == EdgePropertyEnum._label.ToString())
                {
                    //do nothing.. May be in future we will write logic here.                    
                }
                else
                {
                    if (property.Key == EdgePropertyEnum.PostedDateLong.ToString() || property.Key == EdgePropertyEnum.SalaryAmount.ToString())
                        graphProperties += "'" + property.Key + "', " + property.Value + " ,";
                    else
                        graphProperties += "'" + property.Key + "', '" + property.Value + "' ,";
                }
            }

            if (!string.IsNullOrEmpty(graphProperties))
            {
                graphProperties = graphProperties.Substring(0, graphProperties.Length - 2);
            }

            uri.Append("g.V(" + properties[EdgePropertyEnum._outV.ToString()] + ").next().addEdge(" + graphProperties + ");");
            var client = new RestClient(uri.ToString());
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
            response["CreateEdgeStatus"] = "200";
            response[TitanGraphConstants.Id] = jsonResponse.result.data[0].id;
            response[TitanGraphConstants.RexsterUri] = url;
            return response;
        }

        private Dictionary<String, String> DeleteEdgeNative(string graphName, string edgeId, string url)
        {
            var uri = new StringBuilder(url + "/?gremlin=");
            //var uri = new StringBuilder(url + "/graphs/" + graphName + "/edges/" + edgeId);

            //http://localhost:8182/?gremlin=g.E('odxqo-6f4-2hat-9kw').drop()

            uri.Append("g.E('" + edgeId + "').drop();");

            var client = new RestClient(uri.ToString());
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
            response["DeleteEdgeStatus"] = "200";
            //response[TitanGraphConstants.Id] = jsonResponse.result.data[0].id;
            //response[TitanGraphConstants.RexsterUri] = url;
            return response;
        }
    }
}
