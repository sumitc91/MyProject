﻿using System;
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
using urNotice.Common.Infrastructure.commonMethods;
using urNotice.Common.Infrastructure.Encryption;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper;
using urNotice.Common.Infrastructure.Model.urNoticeModel.Solr;
using urNotice.Common.Infrastructure.Session;
using urNotice.Common.Infrastructure.signalRPushNotifications;
using urNotice.Services.ErrorLogger;
using urNotice.Services.GraphDb.GraphDbContract;
using urNotice.Services.NoSqlDb.DynamoDb;
using urNotice.Services.Solr.SolrUser;

namespace urNotice.Services.SocialAuthService
{
    public class SocialAuthService
    {
        private static readonly ILogger Logger = new Logger(Convert.ToString(MethodBase.GetCurrentMethod().DeclaringType));        
        //private readonly urnoticeAuthEntities _db = new urnoticeAuthEntities();
        //private oAuthLinkedIn _oauth = new oAuthLinkedIn();

        public ResponseModel<LoginResponse> CheckAndSaveFacebookUserInfoIntoDatabase(string fid, string refKey, string access_token, bool isMobileApiCall,string accessKey,string secretKey)
        {
            var response = new ResponseModel<LoginResponse>();
            response.Payload = new LoginResponse();
            //var ifFacebookUserAlreadyRegistered = _db.FacebookAuths.SingleOrDefault(x => x.facebookId == fid);
            IDynamoDb dynamoDbModel = new DynamoDb();
            var userInfo = dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTableUsingFacebookId(DynamoDbHashKeyDataType.OrbitPageUser.ToString(), fid);

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
                            
                            dynamoDbModel.CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(userInfo);

                            string displayName = OrbitPageUtil.GetDisplayName(userInfo.OrbitPageUser); 
                            var session = new urNoticeSession(userInfo.ObjectId,displayName, userSolr.VertexId);
                            TokenManager.CreateSession(session);
                            response.Payload.UTMZT = session.SessionId;
                            return response;

                        }
                        catch (Exception e)
                        {
                            //DbContextException.LogDbContextException(e);
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

                            dynamoDbModel.CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(userInfo);

                            string displayName = OrbitPageUtil.GetDisplayName(userInfo.OrbitPageUser); 
                            var session = new urNoticeSession(userInfo.ObjectId,displayName, userSolr.VertexId);
                            TokenManager.CreateSession(session);
                            response.Payload.UTMZT = session.SessionId;
                            return response;

                        }
                        catch (Exception e)
                        {
                            //DbContextException.LogDbContextException(e);
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

                    //IGraphDbContract graphDbContract = new GraphDbContract();
                    IGraphDbContract graphDbContract = new GremlinServerGraphDbContract();
                    var userVertexIdInfo = graphDbContract.InsertNewUserInGraphDb(user);
                    
                    user.vertexId = userVertexIdInfo[TitanGraphConstants.Id];

                    if (!CommonConstants.NA.Equals(refKey))
                    {
                        //new ReferralService().payReferralBonusAsync(refKey, user.Username, Constants.status_true);
                    }

                    try
                    {
                        
                        dynamoDbModel.UpsertOrbitPageUser(user,null);
                        
                        //new SolrService.SolrService().InsertNewUserToSolr(user, false);                        
                        solrUserModel.InsertNewUser(user, false);
                        
                        //new TitanService.TitanService().InsertNewUserToTitan(user, false);

                        SignalRController.BroadCastNewUserRegistration();


                        
                        userInfo.ObjectId = user.username;                        
                        dynamoDbModel.CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(userInfo);

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
                            string displayName = OrbitPageUtil.GetDisplayName(userInfo.OrbitPageUser); 
                            var session = new urNoticeSession(userInfo.ObjectId,displayName, user.vertexId);
                            TokenManager.CreateSession(session);
                            response.Payload.UTMZT = session.SessionId;
                        }
                        catch (Exception e)
                        {
                            //DbContextException.LogDbContextException(e);
                            response.Status = 500;
                            response.Message = "Internal Server Error !!";
                        }


                        //new UserMessageService().SendUserNotificationForAccountVerificationSuccess(
                        //    user.username, CommonConstants.userType_user
                        //);


                    }
                    catch (Exception e)
                    {
                        //DbContextException.LogDbContextException(e);
                        response.Status = 500;
                        response.Message = "Internal Server Error !!!";
                    }
                }
                

            }

            return response;
        }
    }
}
