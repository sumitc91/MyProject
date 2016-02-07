using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using urNotice.Common.Infrastructure.Common.Config;
using urNotice.Common.Infrastructure.Common.Constants;
using urNotice.Common.Infrastructure.commonMethods;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Services.GraphService;

namespace urNotice.Services.TitanService
{
    public class TitanService
    {
        public Dictionary<String, String> InsertNewUserToTitan(OrbitPageUser user, bool toBeOptimized)
        {

            string url = TitanGraphConfig.Server;
            string graphName = TitanGraphConfig.Graph;

            string vertexId = user.email;

            var properties = new Dictionary<string, string>();
            properties["FirstName"] = user.firstName;
            properties["LastName"] = user.lastName;
            properties["Username"] = user.email;
            properties["Gender"] = user.gender;
            properties["CreatedTime"] = DateTimeUtil.GetUtcTime().ToString("s");
            properties["ImageUrl"] = user.imageUrl;
            properties["CoverImageUrl"] = user.userCoverPic;

            Dictionary<string, string> addVertexResponse = new GraphVertexOperations().AddVertex(url, vertexId, graphName, properties);

            return addVertexResponse;
        }
    }
}
