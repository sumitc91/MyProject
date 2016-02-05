using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using urNotice.Common.Infrastructure.Common.Constants;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;

namespace urNotice.Services.TitanService
{
    public class TitanService
    {
        public Dictionary<String, String> InsertNewUserToTitan(OrbitPageUser user, bool toBeOptimized)
        {
            String uri = "http://54.148.127.109:8182/graphs/graph/vertices/" + user.email + "?name=" + user.email + "&gender=" + user.gender;

            var client = new RestClient(uri);
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
