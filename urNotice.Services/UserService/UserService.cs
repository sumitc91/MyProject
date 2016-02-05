using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using urNotice.Common.Infrastructure.Common.Constants;
using urNotice.Common.Infrastructure.Common.Enum;
using urNotice.Common.Infrastructure.Common.Logger;
using urNotice.Common.Infrastructure.commonMethods;
using urNotice.Common.Infrastructure.Model.urNoticeAuthContext;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper;

namespace urNotice.Services.UserService
{
    public class UserService
    {
        private static readonly ILogger Logger = new Logger(Convert.ToString(MethodBase.GetCurrentMethod().DeclaringType));
        private DbContextException _dbContextException = new DbContextException();
        //private readonly urnoticeAuthEntities _db = new urnoticeAuthEntities();

        public ResponseModel<ClientDetailsModel> GetClientDetails(string username,string accessKey,string secretKey)
        {
            var response = new ResponseModel<ClientDetailsModel>();

            try
            {
                //var clientDetailDbResult = _db.Users.SingleOrDefault(x => x.username == username);
                var userInfo = new DynamoDbService.DynamoDbService().GetOrbitPageCompanyUserWorkgraphyTable(
                DynamoDbHashKeyDataType.OrbitPageUser.ToString(),
                username,
                null,
                accessKey,
                secretKey
                );

                if (userInfo != null)
                {
                    var createClientDetailResponse = new ClientDetailsModel
                    {
                        FirstName = userInfo.OrbitPageUser.firstName,
                        LastName = userInfo.OrbitPageUser.lastName,
                        Username = userInfo.OrbitPageUser.email,
                        imageUrl = userInfo.OrbitPageUser.imageUrl == CommonConstants.NA ? CommonConstants.clientImageUrl : userInfo.OrbitPageUser.imageUrl,
                        gender = userInfo.OrbitPageUser.gender,
                        isLocked = userInfo.OrbitPageUser.locked
                    };

                    response.Status = 200;
                    response.Message = "success";
                    response.Payload = createClientDetailResponse;

                }
                else
                {
                    response.Status = 404;
                    response.Message = "username not found";
                }
            }
            catch (Exception)
            {
                response.Status = 500;
                response.Message = "exception occured !!!";
            }
            return response;
        }
    }
}
