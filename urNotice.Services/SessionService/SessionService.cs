using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using urNotice.Common.Infrastructure.Common.Enum;
using urNotice.Common.Infrastructure.Encryption;
using urNotice.Common.Infrastructure.Session;

namespace urNotice.Services.SessionService
{
    public class SessionService
    {
        public  urNoticeSession CheckAndValidateSession(HeaderManager headers,string authkey,string accessKey, string secretKey)
        {
            urNoticeSession session = new TokenManager().getSessionInfo(headers.AuthToken, headers);
            if (session != null)
            {
                return session;
            }
            else
            {
                if (headers == null)
                    return null;
                if (headers.AuthToken == null)
                    return null;


                string username = EncryptionClass.GetDecryptionValue(headers.AuthKey, authkey);

                var userInfo = new DynamoDbService.DynamoDbService().GetOrbitPageCompanyUserWorkgraphyTable(
                    DynamoDbHashKeyDataType.OrbitPageUser.ToString(),
                    username,
                    null,
                    accessKey,
                    secretKey
                    );


                if (userInfo != null)
                {
                    var data = new Dictionary<string, string>();
                    data["Password"] = headers.AuthValue;
                    data["userGuid"] = userInfo.OrbitPageUser.guid;

                    try
                    {
                        var decryptedData = EncryptionClass.decryptUserDetails(data);

                        if (userInfo.OrbitPageUser.keepMeSignedIn == "true" && userInfo.OrbitPageUser.password == decryptedData["UTMZV"])
                        {
                            var NewSession = new urNoticeSession(username, headers.AuthToken,userInfo.OrbitPageUser.vertexId);
                            TokenManager.CreateSession(NewSession);
                            return new TokenManager().getSessionInfo(headers.AuthToken, headers);
                        }
                        else
                        {
                            return null;
                        }

                    }
                    catch (Exception)
                    {

                        return null;
                    }


                }
                else
                {
                    return null;
                }
            }
        }
    }
}
