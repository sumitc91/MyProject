using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using urNotice.Common.Infrastructure.Common.Constants;
using urNotice.Common.Infrastructure.Common.Enum;
using urNotice.Common.Infrastructure.Encryption;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper;
using urNotice.Common.Infrastructure.Session;
using urNotice.Services.NoSqlDb.DynamoDb;
using urNotice.Services.Solr.SolrUser;

namespace urNotice.Services.Person.PersonContract.LoginOperation
{
    public class OrbitPageLogin : IOrbitPageLogin
    {
        public ResponseModel<LoginResponse> Login(string userName, string password, string returnUrl, string keepMeSignedIn, bool isSocialLogin)
        {
            var response =new ResponseModel<LoginResponse>();
            response.Payload = new LoginResponse();
            ISolrUser solrUserModel = new SolrUser();
            var userSolrDetail = solrUserModel.GetPersonData(userName, null, null, null, false);

            IDynamoDb dynamoDbModel = new DynamoDb();
            var userInfo = dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTable(
                DynamoDbHashKeyDataType.OrbitPageUser.ToString(),
                userSolrDetail.Email,
                null
                );

            if (userInfo.OrbitPageUser != null && (userInfo.OrbitPageUser.password == password))
            {
                if (userInfo.OrbitPageUser.isActive == CommonConstants.TRUE || isSocialLogin)
                {
                    response.Payload = CreateLoginResponseModel(userInfo.OrbitPageUser);
                    response.Status = 200;
                    try
                    {
                        userInfo.OrbitPageUser.keepMeSignedIn = keepMeSignedIn.Equals("true",
                            StringComparison.OrdinalIgnoreCase)
                            ? "true"
                            : "false";

                        dynamoDbModel.CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(userInfo);

                    }
                    catch (Exception ex)
                    {
                        //Todo:need to log exception.
                        
                        response.Status = 500;
                        response.Payload.Code = "500";
                    }
                }
                else
                {
                    response.Status = 403;
                    response.Payload.Code = "403";                    
                }                    
            }
            else
            {
                response.Status = 401;
                response.Payload.Code = "401";                
            }

            return response;
        }

        public LoginResponse CreateLoginResponseModel(OrbitPageUser userInfo)
        {
            var userData = new LoginResponse();

            var data = new Dictionary<string, string>();
            data["Username"] = userInfo.email;
            data["Password"] = userInfo.password;
            data["userGuid"] = userInfo.guid;

            var encryptedData = EncryptionClass.encryptUserDetails(data);
            userData.UTMZK = encryptedData["UTMZK"];
            userData.UTMZV = encryptedData["UTMZV"];
            userData.FirstName = userInfo.firstName;
            userData.LastName = userInfo.lastName;
            userData.Username = userInfo.username;
            userData.imageUrl = userInfo.imageUrl;
            userData.VertexId = userInfo.vertexId;

            var session = new urNoticeSession(userInfo.email, userInfo.vertexId);
            TokenManager.CreateSession(session);

            userData.UTMZT = session.SessionId;
            userData.TimeStamp = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            userData.Code = "200";

            return userData;
        }
    }
}
