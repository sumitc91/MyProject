using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using urNotice.Common.Infrastructure.Common.Config;
using urNotice.Common.Infrastructure.Common.Constants;
using urNotice.Common.Infrastructure.Common.Enum;
using urNotice.Common.Infrastructure.commonMethods;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Services.GraphService;

namespace urNotice.Services.TitanService
{
    public class TitanService
    {
        public Dictionary<String, String> InsertNewUserToTitan(OrbitPageUser user, bool toBeOptimized, string accessKey, string secretKey)
        {

            string url = TitanGraphConfig.Server;
           
            var properties = new Dictionary<string, string>();
            properties[VertexPropertyEnum.Type.ToString()] = VertexLabelEnum.User.ToString();
            properties[VertexPropertyEnum.FirstName.ToString()] = user.firstName;
            properties[VertexPropertyEnum.LastName.ToString()] = user.lastName;
            properties[VertexPropertyEnum.Username.ToString()] = user.email;
            properties[VertexPropertyEnum.Gender.ToString()] = user.gender;
            properties[VertexPropertyEnum.CreatedTime.ToString()] = DateTimeUtil.GetUtcTimeString();
            properties[VertexPropertyEnum.ImageUrl.ToString()] = user.imageUrl;
            properties[VertexPropertyEnum.CoverImageUrl.ToString()] = user.userCoverPic;

            Dictionary<string, string> addVertexResponse = new GraphVertexOperations().AddVertex(user.email, url, user.email, TitanGraphConfig.Graph, properties, accessKey, secretKey);

            return addVertexResponse;
        }

        public Dictionary<String, String> InsertNewDesignationToTitan(string adminEmail,string designationName, bool toBeOptimized, string accessKey, string secretKey)
        {

            string url = TitanGraphConfig.Server;

            var properties = new Dictionary<string, string>();
            properties[VertexPropertyEnum.Type.ToString()] = VertexLabelEnum.Designation.ToString();
            properties[VertexPropertyEnum.DesignationName.ToString()] = designationName;            
            properties[VertexPropertyEnum.CreatedTime.ToString()] = DateTimeUtil.GetUtcTimeString();

            Dictionary<string, string> addVertexResponse = new GraphVertexOperations().AddVertex(adminEmail, url, designationName, TitanGraphConfig.Graph, properties, accessKey, secretKey);

            return addVertexResponse;
        }

        public Dictionary<String, String> InsertNewCompanyToTitan(string adminEmail, string companyName, bool toBeOptimized, string accessKey, string secretKey)
        {

            string url = TitanGraphConfig.Server;

            var properties = new Dictionary<string, string>();
            properties[VertexPropertyEnum.Type.ToString()] = VertexLabelEnum.Company.ToString();
            properties[VertexPropertyEnum.CompanyName.ToString()] = companyName;
            properties[VertexPropertyEnum.CreatedTime.ToString()] = DateTimeUtil.GetUtcTimeString();

            Dictionary<string, string> addVertexResponse = new GraphVertexOperations().AddVertex(adminEmail, url, companyName, TitanGraphConfig.Graph, properties, accessKey, secretKey);

            return addVertexResponse;
        }
    }
}
