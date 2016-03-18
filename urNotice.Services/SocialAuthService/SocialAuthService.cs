using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Facebook;
using urNotice.Common.Infrastructure.Common.Constants;
using urNotice.Common.Infrastructure.Common.Enum;
using urNotice.Common.Infrastructure.Common.Logger;
using urNotice.Common.Infrastructure.commonMethods;
using urNotice.Common.Infrastructure.Encryption;
using urNotice.Common.Infrastructure.Model.Solr.SolrUser;
using urNotice.Common.Infrastructure.Model.urNoticeAuthContext;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper;
using urNotice.Common.Infrastructure.Model.urNoticeModel.Solr;
using urNotice.Common.Infrastructure.Session;
using urNotice.Common.Infrastructure.signalRPushNotifications;

namespace urNotice.Services.SocialAuthService
{
    public class SocialAuthService
    {
        private static readonly ILogger Logger = new Logger(Convert.ToString(MethodBase.GetCurrentMethod().DeclaringType));
        private DbContextException _dbContextException = new DbContextException();
        //private readonly urnoticeAuthEntities _db = new urnoticeAuthEntities();
        //private oAuthLinkedIn _oauth = new oAuthLinkedIn();

        public ResponseModel<LoginResponse> CheckAndSaveFacebookUserInfoIntoDatabase(string fid, string refKey, string access_token, bool isMobileApiCall,string accessKey,string secretKey)
        {
            var response = new ResponseModel<LoginResponse>();
            response.Payload = new LoginResponse();
            //var ifFacebookUserAlreadyRegistered = _db.FacebookAuths.SingleOrDefault(x => x.facebookId == fid);
            OrbitPageCompanyUserWorkgraphyTable userInfo = new DynamoDbService.DynamoDbService().GetOrbitPageCompanyUserWorkgraphyTableUsingFacebookId(
                        DynamoDbHashKeyDataType.OrbitPageUser.ToString(),
                        fid,
                        accessKey,
                        secretKey
                        );
            

            //var ifFacebookUserAlreadyRegistered = _db.FacebookAuths.SingleOrDefault(x => x.facebookId == fid);
            if (userInfo.OrbitPageUser != null)
            {
                ISolrUser solrUserModel = new SolrUser();
                var userSolr = solrUserModel.GetPersonData(userInfo.ObjectId, null, null, null,false);//new SolrService.SolrService().GetSolrUserData(userInfo.ObjectId, null, null, null);
                
                if (userSolr != null)
                {
                   
                    {
                        var data = new Dictionary<string, string>();
                        data["Username"] = userInfo.OrbitPageUser.email;
                        data["Password"] = userInfo.OrbitPageUser.password;
                        data["userGuid"] = userInfo.OrbitPageUser.guid;

                        var encryptedData = EncryptionClass.encryptUserDetails(data);

                        response.Payload = new LoginResponse();
                        response.Payload.UTMZK = encryptedData["UTMZK"];
                        response.Payload.UTMZV = encryptedData["UTMZV"];
                        response.Payload.TimeStamp = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                        response.Payload.FirstName = userInfo.OrbitPageUser.firstName;
                        response.Payload.imageUrl = userInfo.OrbitPageUser.imageUrl;
                        response.Payload.Code = "210";
                        
                        response.Status = 210;
                        response.Message = "user Login via facebook";
                        try
                        {
                            userInfo.OrbitPageUser.keepMeSignedIn = "true";
                                //keepMeSignedIn.Equals("true", StringComparison.OrdinalIgnoreCase) ? "true" : "false";
                            userInfo.OrbitPageUser.locked = CommonConstants.FALSE;

                            new DynamoDbService.DynamoDbService().CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(userInfo, accessKey, secretKey);

                            var session = new urNoticeSession(userInfo.ObjectId, userSolr.VertexId);
                            TokenManager.CreateSession(session);
                            response.Payload.UTMZT = session.SessionId;
                            return response;

                        }
                        catch (DbEntityValidationException e)
                        {
                            DbContextException.LogDbContextException(e);
                            response.Payload.Code = "500";

                            return response;
                        }
                    }
                    
                    response.Payload.Code = "403";
                }
                else
                {
                    response.Payload.Code = "403";
                }
            }
            else
            {
                //save user details in database ..

                var fb = new FacebookClient(userInfo.FacebookAuthToken);
                /*dynamic result = fb.Get("fql",
                            new { q = "SELECT uid, first_name, last_name, sex, pic_big_with_logo, username FROM user WHERE uid=me()" });*/

                dynamic result = fb.Get("me?fields=id,first_name,last_name,gender,picture{url},email");
                var email = result.email ?? result.id + "@facebook.com";

                ISolrUser solrUserModel = new SolrUser();
                UnUserSolr userSolr = solrUserModel.GetPersonData(email, null, null, fid,false);//new SolrService.SolrService().GetSolrUserData(email, null, null,fid);

                if (userSolr != null)
                {
                    //already registered.
                    //var user = _db.Users.SingleOrDefault(x => x.email == userSolr.Email);
                    if (userInfo.OrbitPageUser != null)
                    {
                        var data = new Dictionary<string, string>();
                        data["Username"] = userInfo.OrbitPageUser.email;
                        data["Password"] = userInfo.OrbitPageUser.password;
                        data["userGuid"] = userInfo.OrbitPageUser.guid;

                        var encryptedData = EncryptionClass.encryptUserDetails(data);

                        response.Payload = new LoginResponse();
                        response.Payload.UTMZK = encryptedData["UTMZK"];
                        response.Payload.UTMZV = encryptedData["UTMZV"];
                        response.Payload.TimeStamp = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                        response.Payload.FirstName = userInfo.OrbitPageUser.firstName;
                        response.Payload.imageUrl = userInfo.OrbitPageUser.imageUrl;
                        response.Payload.Code = "210";

                        response.Status = 210;
                        response.Message = "user Login via facebook";
                        try
                        {
                            userInfo.OrbitPageUser.keepMeSignedIn = "true";
                            userInfo.OrbitPageUser.fid = userInfo.FacebookId;
                            //keepMeSignedIn.Equals("true", StringComparison.OrdinalIgnoreCase) ? "true" : "false";
                            userInfo.OrbitPageUser.locked = CommonConstants.FALSE;

                            new DynamoDbService.DynamoDbService().CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(userInfo, accessKey, secretKey);

                            var session = new urNoticeSession(userInfo.ObjectId, userSolr.VertexId);
                            TokenManager.CreateSession(session);
                            response.Payload.UTMZT = session.SessionId;
                            return response;

                        }
                        catch (DbEntityValidationException e)
                        {
                            DbContextException.LogDbContextException(e);
                            response.Payload.Code = "500";

                            return response;
                        }
                    }
                    else
                        response.Payload.Code = "403";
                }
                else
                {
                    var guid = Guid.NewGuid().ToString();
                    var user = new OrbitPageUser
                    {
                        username = email,
                        fid = userInfo.FacebookId,
                        email = email,
                        password = EncryptionClass.Md5Hash(Guid.NewGuid().ToString()),
                        source = "facebook",
                        isActive = "true",
                        guid = guid,
                        fixedGuid = guid,
                        firstName = result.first_name,
                        lastName = result.last_name,
                        gender = result.gender,
                        imageUrl = result.picture.data.url,
                        priviledgeLevel = (short)PriviledgeLevel.None,
                        keepMeSignedIn = CommonConstants.TRUE
                    };

                    var userVertexIdInfo = new TitanService.TitanService().InsertNewUserToTitan(user, false,accessKey,secretKey);
                    user.vertexId = userVertexIdInfo[TitanGraphConstants.Id];

                    if (!CommonConstants.NA.Equals(refKey))
                    {
                        //new ReferralService().payReferralBonusAsync(refKey, user.Username, Constants.status_true);
                    }

                    try
                    {
                        new DynamoDbService.DynamoDbService().CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(
                            DynamoDbHashKeyDataType.OrbitPageUser.ToString(),
                            user.email,
                            user.username,
                            null,
                            null,
                            null,
                            null,
                            user,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            false,
                            accessKey,
                            secretKey
                            );

                        //new SolrService.SolrService().InsertNewUserToSolr(user, false);                        
                        solrUserModel.InsertNewUser(user, false);
                        
                        //new TitanService.TitanService().InsertNewUserToTitan(user, false);

                        SignalRController.BroadCastNewUserRegistration();


                        
                        userInfo.ObjectId = user.username;
                        new DynamoDbService.DynamoDbService().CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(userInfo, accessKey, secretKey);
                        //new SolrService.SolrService().InsertNewUserToSolr(user, false);

                        var data = new Dictionary<string, string>();
                        data["Username"] = user.username;
                        data["Password"] = user.password;
                        data["userGuid"] = user.guid;

                        var encryptedData = EncryptionClass.encryptUserDetails(data);

                        response.Payload = new LoginResponse();
                        response.Payload.UTMZK = encryptedData["UTMZK"];
                        response.Payload.UTMZV = encryptedData["UTMZV"];
                        response.Payload.TimeStamp = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                        response.Payload.Code = "210";
                        response.Status = 210;
                        response.Payload.FirstName = user.firstName;
                        response.Payload.imageUrl = user.imageUrl;
                        response.Message = "user Login via facebook";
                        try
                        {
                            var session = new urNoticeSession(userInfo.ObjectId, user.vertexId);
                            TokenManager.CreateSession(session);
                            response.Payload.UTMZT = session.SessionId;
                        }
                        catch (DbEntityValidationException e)
                        {
                            DbContextException.LogDbContextException(e);
                            response.Status = 500;
                            response.Message = "Internal Server Error !!";
                        }


                        //new UserMessageService().SendUserNotificationForAccountVerificationSuccess(
                        //    user.username, CommonConstants.userType_user
                        //);


                    }
                    catch (DbEntityValidationException e)
                    {
                        DbContextException.LogDbContextException(e);
                        response.Status = 500;
                        response.Message = "Internal Server Error !!!";
                    }
                }
                

            }

            return response;
        }
    }
}
