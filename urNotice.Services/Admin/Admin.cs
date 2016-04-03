using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using urNotice.Common.Infrastructure.Common.Enum;
using urNotice.Common.Infrastructure.Model.Person;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper;
using urNotice.Services.NoSqlDb.DynamoDb;

namespace urNotice.Services.Admin
{
    public class Admin : IPerson
    {
        public ResponseModel<LoginResponse> RegisterMe(RegisterationRequest req, HttpRequestBase request)
        {
            throw new NotImplementedException();
        }

        public ResponseModel<LoginResponse> SocialRegisterMe(RegisterationRequest req, HttpRequestBase request)
        {
            throw new NotImplementedException();
        }

        public ResponseModel<LoginResponse> Login(string userName, string password, bool decryptPassword)
        {
            throw new NotImplementedException();
        }

        public ResponseModel<OrbitPageUser> GetFullUserDetail(string userEmail)
        {
            var response = new ResponseModel<OrbitPageUser>();
            IDynamoDb dynamoDbModel = new DynamoDb();
            var userInfo = dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTable(
                DynamoDbHashKeyDataType.OrbitPageUser.ToString(),
                userEmail,
                null
                );

            response.Status = 200;
            response.Message = "success";
            response.Payload = userInfo.OrbitPageUser;
            return response;
        }
    }
}
