using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Facebook;
using Newtonsoft.Json;
using SolrNet.Impl;
using urNotice.Common.Infrastructure.Common.Constants;
using urNotice.Common.Infrastructure.Common.Enum;
using urNotice.Common.Infrastructure.commonMethods;
using urNotice.Common.Infrastructure.Encryption;
using urNotice.Common.Infrastructure.Model.Person;
using urNotice.Common.Infrastructure.Model.urNoticeAnalyticsContext;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper;
using urNotice.Common.Infrastructure.Session;
using urNotice.Common.Infrastructure.signalRPushNotifications;
using urNotice.Services.ErrorLogger;
using urNotice.Services.GraphDb.GraphDbContract;
using urNotice.Services.NoSqlDb.DynamoDb;
using urNotice.Services.Person;
using urNotice.Services.Person.PersonContract.LoginOperation;
using urNotice.Services.Person.PersonContract.RegistrationOperation;
using urNotice.Services.SessionService;
using urNotice.Services.SocialAuthService;
using urNotice.Services.SocialAuthService.Facebook;
using urNotice.Services.SocialAuthService.google;
using urNotice.Services.SocialAuthService.linkedin;
using urNotice.Services.Solr.SolrUser;

namespace OrbitPage.Controllers
{
    public class SocialAuthController : Controller
    {
        //
        // GET: /SocialAuth/
        private static readonly ILogger Logger = new Logger(Convert.ToString(MethodBase.GetCurrentMethod().DeclaringType));        
        //private readonly urnoticeAuthEntities _db = new urnoticeAuthEntities();
        //private readonly urnoticeAnalyticsEntities _dbAnalytics = new urnoticeAnalyticsEntities();

        private static string accessKey = ConfigurationManager.AppSettings["AWSAccessKey"];
        private static string secretKey = ConfigurationManager.AppSettings["AWSSecretKey"];
        private static string authKey = ConfigurationManager.AppSettings["AuthKey"];

        private oAuthLinkedIn _oauth = new oAuthLinkedIn();
        string fb_scope = "email,user_friends";
        //string fb_scope = "email";
        string gmail_scope = "email profile https://www.google.com/m8/feeds/";
        //string gmail_scope = "email profile";

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Login(string type)
        {
            return Json("test", JsonRequestBehavior.AllowGet);
        }

        public JsonResult FacebookLoginMobileApi()
        {
            var response = new ResponseModel<LoginResponse>();
            String fid = Request.QueryString["fid"];
            String access_token = Request.QueryString["accessToken"];
            try
            {
                response = new SocialAuthService().CheckAndSaveFacebookUserInfoIntoDatabase(fid, CommonConstants.NA, access_token, true,accessKey,secretKey);
            }
            catch (Exception ex)
            {
                //Todo:need to log exception.
                response.Status = 500;
                response.Message = "Failed";
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult userMapping()
        {
            var response = new ResponseModel<LoginResponse>();

            String fid = Request.QueryString["fid"];
            String refKey = Request.QueryString["refKey"];
            var headers = new HeaderManager(Request);
            if (headers.AuthToken != null)
            {
                urNoticeSession session = new SessionService().CheckAndValidateSession(headers, authKey, accessKey, secretKey);
                var isValidToken = TokenManager.IsValidSession(headers.AuthToken);
                if (isValidToken)
                {
                    //var facebookUserMap = _db.FacebookAuths.SingleOrDefault(x => x.facebookId == fid);
                    IDynamoDb dynamoDbModel = new DynamoDb();
                    var userInfo = dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTableUsingFacebookId(DynamoDbHashKeyDataType.OrbitPageUser.ToString(),fid);
                    
                    //facebookUserMap.username = session.UserName;
                    try
                    {
                       //_db.SaveChanges();
                        response.Status = 209;
                        response.Message = "success-";
                    }
                    catch (Exception ex)
                    {
                        //Todo:need to log exception.
                        response.Status = 500;
                        response.Message = "Failed";
                    }
                }
            }
            else
            {
                //TODO:need to call method socialauthservice
                response = new SocialAuthService().CheckAndSaveFacebookUserInfoIntoDatabase(fid, refKey, CommonConstants.NA, false,accessKey,secretKey);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GoogleLoginGetRedirectUri(string type)
        {
            var response = new ResponseModel<LoginResponse>();
            String code = Request.QueryString["code"];
            String refKey = Request.QueryString["refKey"];
            string app_id = "";
            string app_secret = "";

            if (Request.Url.Authority.Contains("localhost"))
            {
                app_id = ConfigurationManager.AppSettings["googleAppID"].ToString();
                app_secret = ConfigurationManager.AppSettings["googleAppSecret"].ToString();
            }
            else
            {
                app_id = ConfigurationManager.AppSettings["googleAppIDCautom"].ToString();
                app_secret = ConfigurationManager.AppSettings["googleAppSecretCautom"].ToString();
            }

            string returnUrl = "http://" + Request.Url.Authority + "/SocialAuth/GoogleLogin";
            if (code == null)
            {
                response.Status = 199;
                /*response.Message = (string.Format(
                    "https://accounts.google.com/o/oauth2/auth?scope={0}&state=%2Fprofile&redirect_uri={1}&response_type=code&client_id={2}&approval_prompt=force",
                    gmail_scope, returnUrl, app_id));*/

                response.Message = (string.Format(
                    "https://accounts.google.com/o/oauth2/v2/auth?scope={0}&state=%2Fprofile&redirect_uri={1}&response_type=code&client_id={2}",
                    gmail_scope, returnUrl, app_id));

                //Response.Redirect(ReturnUrl);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateUserRefKey()
        {
            var response = new ResponseModel<string>();
            response.Status = 201;
            var headers = new HeaderManager(Request);
            if (headers.AuthToken != null)
            {
                urNoticeSession session = new SessionService().CheckAndValidateSession(headers, authKey, accessKey, secretKey);
                var isValidToken = TokenManager.IsValidSession(headers.AuthToken);
                if (isValidToken)
                {
                    String refKey = Request.QueryString["refKey"];

                    //if (!string.IsNullOrEmpty(refKey))
                    //{
                    //    new ReferralService().payReferralBonusAsync(refKey, session.UserName, CommonConstants.TRUE);
                    //}
                    try
                    {
                        response.Status = 200;
                        response.Message = "success-";
                    }
                    catch (Exception ex)
                    {
                        //Todo:need to log exception.
                        response.Status = 500;
                        response.Message = "Failed";
                    }
                }
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GoogleLogin(string type)
        {
            var response = new ResponseModel<LoginResponse>();
            String code = Request.QueryString["code"];
            String refKey = Request.QueryString["refKey"];
            string app_id = "";
            string app_secret = "";

            if (Request.Url.Authority.Contains("localhost"))
            {
                app_id = ConfigurationManager.AppSettings["googleAppID"].ToString();
                app_secret = ConfigurationManager.AppSettings["googleAppSecret"].ToString();
            }
            else
            {
                app_id = ConfigurationManager.AppSettings["googleAppIDCautom"].ToString();
                app_secret = ConfigurationManager.AppSettings["googleAppSecretCautom"].ToString();
            }


            string returnUrl = "http://" + Request.Url.Authority + "/SocialAuth/GoogleLogin";
            if (code == null)
            {
                var ReturnUrl = (string.Format(
                    "https://accounts.google.com/o/oauth2/auth?scope={0}&state=%2Fprofile&redirect_uri={1}&response_type=code&client_id={2}&approval_prompt=force",
                    gmail_scope, returnUrl, app_id));
                Response.Redirect(ReturnUrl);
            }
            else
            {
                string access_token = getGoogleAuthToken(returnUrl, gmail_scope, code, app_id, app_secret);

                String URI = "https://www.googleapis.com/oauth2/v1/userinfo?access_token=" + access_token;
                WebClient webClient = new WebClient();
                Stream stream = webClient.OpenRead(URI);
                string googleUserDetailString;

                /*I have not used any JSON parser because I do not want to use any extra dll/3rd party dll*/
                using (StreamReader br = new StreamReader(stream))
                {
                    googleUserDetailString = br.ReadToEnd();
                }
                var googleUserDetails = JsonConvert.DeserializeObject<googleUserDetails>(Convert.ToString(googleUserDetailString));

                //TODO:tobe deleted.
                //new GoogleService().GetUserFriendListAsync(access_token, googleUserDetails.email);

                ISolrUser solrUserModel = new SolrUser();                
                var solrUser = solrUserModel.GetPersonData(googleUserDetails.email, null, null, null, false); //new SolrService().GetSolrUserData(googleUserDetails.email, null, null, null);
                
                if (solrUser != null)
                {
                    IDynamoDb dynamoDbModel = new DynamoDb();
                    var userInfo = dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTable(
                        DynamoDbHashKeyDataType.OrbitPageUser.ToString(),
                        googleUserDetails.email,
                        null);
                    
                    if (userInfo == null)
                    {
                        response.Message = "User not found";
                        response.Status = 401;
                    }
                    else
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
                        response.Payload.VertexId = userInfo.OrbitPageUser.vertexId;
                        response.Payload.Code = "210";
                        response.Status = 210;
                        response.Message = "user Login via google";
                        try
                        {
                            userInfo.OrbitPageUser.keepMeSignedIn = "true";//keepMeSignedIn.Equals("true", StringComparison.OrdinalIgnoreCase) ? "true" : "false";
                            userInfo.OrbitPageUser.locked = CommonConstants.FALSE;
                            
                            if (userInfo.GoogleApiCheckLastSyncedDateTime == null)
                            {
                                new GoogleService().GetUserFriendListAsync(access_token, googleUserDetails.email,accessKey,secretKey);
                                userInfo.GoogleApiCheckLastSyncedDateTime = DateTimeUtil.GetUtcTime();

                            }

                            dynamoDbModel.CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(userInfo);
                            
                            var session = new urNoticeSession(userInfo.OrbitPageUser.email, userInfo.OrbitPageUser.vertexId);
                            TokenManager.CreateSession(session);
                            response.Payload.UTMZT = session.SessionId;
                            ViewBag.umtzt = response.Payload.UTMZT;
                            ViewBag.umtzk = response.Payload.UTMZK;
                            ViewBag.umtzv = response.Payload.UTMZV;
                            ViewBag.userName = response.Payload.FirstName;
                            ViewBag.userImageUrl = response.Payload.imageUrl;
                            ViewBag.vertexid = response.Payload.VertexId;
                            ViewBag.isNewUser = "false";
                            return View();

                        }
                        catch (Exception ex)
                        {
                            //Todo:need to log exception.
                            response.Status = 500;
                            response.Message = "Failed";
                        }
                    }

                    return Json(response, JsonRequestBehavior.AllowGet);
                    
                }
                else
                {
                    //add user to database.

                    var guid = Guid.NewGuid().ToString();

                    if (googleUserDetails.picture == null || googleUserDetails.picture == "") googleUserDetails.picture = CommonConstants.NA; // if picture is not available.
                    if (googleUserDetails.gender == null || googleUserDetails.gender == "") googleUserDetails.gender = CommonConstants.NA; // if picture is not available.

                    var user = new OrbitPageUser
                    {
                        username = googleUserDetails.email,
                        email = googleUserDetails.email,
                        password = EncryptionClass.Md5Hash(Guid.NewGuid().ToString()),
                        //fid = googleUserDetails.id,
                        source = "google",
                        isActive = "true",
                        guid = Guid.NewGuid().ToString(),
                        fixedGuid = Guid.NewGuid().ToString(),
                        firstName = googleUserDetails.given_name,
                        lastName = googleUserDetails.family_name,
                        gender = googleUserDetails.gender,
                        imageUrl = googleUserDetails.picture,
                        priviledgeLevel = (short)PriviledgeLevel.None,
                        keepMeSignedIn = CommonConstants.TRUE                        
                    };


                    IGraphDbContract graphDbContract = new GraphDbContract();
                    var userVertexIdInfo = graphDbContract.InsertNewUserInGraphDb(user);
                    user.vertexId = userVertexIdInfo[TitanGraphConstants.Id];
                    
                    try
                    {
                        IDynamoDb dynamoDbModel = new DynamoDb();
                        dynamoDbModel.UpsertOrbitPageUser(user,null);
                        //new DynamoDbService().CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(
                        //    DynamoDbHashKeyDataType.OrbitPageUser.ToString(),
                        //    user.email,
                        //    user.username,
                        //    null,
                        //    null,
                        //    null,
                        //    null,
                        //    user,
                        //    null,
                        //    null,
                        //    null,
                        //    null,
                        //    null,
                        //    null,
                        //    false,
                        //    accessKey,
                        //    secretKey
                        //    );

                        //new SolrService().InsertNewUserToSolr(user, false);
                        
                        solrUserModel.InsertNewUser(user, false);
                        
                        //new TitanService.TitanService().InsertNewUserToTitan(user, false);

                        SignalRController.BroadCastNewUserRegistration();

                        //new SolrService().InsertNewUserToSolr(user, false);
                        new GoogleService().GetUserFriendListAsync(access_token, googleUserDetails.email,accessKey,secretKey);

                        var data = new Dictionary<string, string>();
                        data["Username"] = user.email;
                        data["Password"] = user.password;
                        data["userGuid"] = user.guid;

                        var encryptedData = EncryptionClass.encryptUserDetails(data);

                        response.Payload = new LoginResponse();
                        response.Payload.UTMZK = encryptedData["UTMZK"];
                        response.Payload.UTMZV = encryptedData["UTMZV"];
                        response.Payload.TimeStamp = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                        response.Payload.Code = "210";
                        response.Payload.FirstName = user.firstName;
                        response.Payload.imageUrl = user.imageUrl;
                        response.Payload.VertexId = user.vertexId;
                        response.Status = 210;
                        response.Message = "user Login via google";
                        try
                        {
                            var session = new urNoticeSession(user.email,user.vertexId);
                            TokenManager.CreateSession(session);
                            response.Payload.UTMZT = session.SessionId;

                            ViewBag.umtzt = response.Payload.UTMZT;
                            ViewBag.umtzk = response.Payload.UTMZK;
                            ViewBag.umtzv = response.Payload.UTMZV;
                            ViewBag.userName = response.Payload.FirstName;
                            ViewBag.userImageUrl = response.Payload.imageUrl;
                            ViewBag.vertexid = response.Payload.VertexId;
                            ViewBag.isNewUser = "true";



                            //new UserMessageService().SendUserNotificationForAccountVerificationSuccess(
                            //    user.username, CommonConstants.userType_user
                            //);


                            return View();
                        }
                        catch (Exception ex)
                        {
                            //Todo:need to log exception.
                            response.Status = 500;
                            response.Message = "Failed";
                        }

                    }
                    catch (Exception ex)
                    {
                        //Todo:need to log exception.
                        response.Status = 500;
                        response.Message = "Failed";
                    }
                }
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult LinkedinLoginGetRedirectUri(string type)
        //{
        //    var response = new ResponseModel<LoginResponse>();

        //    String AbsoluteUri = Request.Url.AbsoluteUri;
        //    string oauth_token = Request.QueryString["oauth_token"];
        //    string oauth_verifier = Request.QueryString["oauth_verifier"];
        //    String refKey = Request.QueryString["refKey"];
        //    string authLink = string.Empty;
        //    if (oauth_token == null || oauth_verifier == null)
        //    {
        //        authLink = CreateAuthorization();
        //        var linkedInApiData = new linkedinAuth
        //        {
        //            oauth_Token = _oauth.Token,
        //            oauth_TokenSecret = _oauth.TokenSecret,
        //            oauth_verifier = ""
        //        };
        //        _db.linkedinAuths.Add(linkedInApiData);
        //        try
        //        {
        //            _db.SaveChanges();
        //            response.Status = 199;
        //            response.Message = authLink;
        //            //Response.Redirect(authLink);
        //        }
        //        catch (DbEntityValidationException e)
        //        {
        //            DbContextException.LogDbContextException(e);
        //            response.Status = 500;
        //            response.Message = "Internal Server Error !!!";
        //        }
        //    }
        //    return Json(response, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult LinkedinLogin(string type)
        //{
        //    var response = new ResponseModel<LoginResponse>();

        //    String AbsoluteUri = Request.Url.AbsoluteUri;
        //    string oauth_token = Request.QueryString["oauth_token"];
        //    string oauth_verifier = Request.QueryString["oauth_verifier"];
        //    String refKey = Request.QueryString["refKey"];
        //    string authLink = string.Empty;
        //    if (oauth_token != null && oauth_verifier != null)
        //    {
        //        var linkedinApiDataResponse = _db.linkedinAuths.SingleOrDefault(x => x.oauth_Token == oauth_token);
        //        if (linkedinApiDataResponse != null)
        //        {
        //            GetAccessToken(oauth_token, linkedinApiDataResponse.oauth_TokenSecret, oauth_verifier);
        //            String UserDetailString = RequestProfile(_oauth.Token, _oauth.TokenSecret, oauth_verifier);
        //            var linkedinUserDetails = JsonConvert.DeserializeObject<linkedinUserDataWrapper>(Convert.ToString(UserDetailString));
        //            _db.linkedinAuths.Attach(linkedinApiDataResponse);
        //            _db.linkedinAuths.Remove(linkedinApiDataResponse);
        //            var ifUserAlreadyRegistered = _db.Users.SingleOrDefault(x => x.username == linkedinUserDetails.emailAddress);
        //            if (ifUserAlreadyRegistered != null)
        //            {

        //                var data = new Dictionary<string, string>();
        //                data["Username"] = ifUserAlreadyRegistered.username;
        //                data["Password"] = ifUserAlreadyRegistered.password;
        //                data["userGuid"] = ifUserAlreadyRegistered.guid;

        //                var encryptedData = EncryptionClass.encryptUserDetails(data);

        //                response.Payload = new LoginResponse();
        //                response.Payload.UTMZK = encryptedData["UTMZK"];
        //                response.Payload.UTMZV = encryptedData["UTMZV"];
        //                response.Payload.TimeStamp = DateTime.Now.ToString(CultureInfo.InvariantCulture);
        //                response.Payload.Code = "210";
        //                response.Status = 210;
        //                response.Message = "user Login via facebook";
        //                try
        //                {
        //                    ifUserAlreadyRegistered.keepMeSignedIn = "true";//keepMeSignedIn.Equals("true", StringComparison.OrdinalIgnoreCase) ? "true" : "false";
        //                    ifUserAlreadyRegistered.locked = CommonConstants.FALSE;
        //                    _db.SaveChanges();

        //                    var session = new urNoticeSession(ifUserAlreadyRegistered.username);
        //                    TokenManager.CreateSession(session);
        //                    response.Payload.UTMZT = session.SessionId;
        //                    ViewBag.umtzt = response.Payload.UTMZT;
        //                    ViewBag.umtzk = response.Payload.UTMZK;
        //                    ViewBag.umtzv = response.Payload.UTMZV;
        //                    return View();

        //                }
        //                catch (DbEntityValidationException e)
        //                {
        //                    DbContextException.LogDbContextException(e);
        //                    response.Payload.Code = "500";

        //                    return Json(response, JsonRequestBehavior.AllowGet);
        //                }
        //            }
        //            else
        //            {
        //                //add user to database.

        //                var guid = Guid.NewGuid().ToString();

        //                if (linkedinUserDetails.pictureUrl == null || linkedinUserDetails.pictureUrl == "") linkedinUserDetails.pictureUrl = CommonConstants.NA; // if picture is not available.

        //                var user = new User
        //                {
        //                    username = linkedinUserDetails.emailAddress,
        //                    password = EncryptionClass.Md5Hash(Guid.NewGuid().ToString()),
        //                    source = "linkedin",
        //                    isActive = "true",
        //                    guid = Guid.NewGuid().ToString(),
        //                    fixedGuid = Guid.NewGuid().ToString(),
        //                    firstName = linkedinUserDetails.firstName,
        //                    lastName = linkedinUserDetails.lastName,
        //                    gender = CommonConstants.NA,
        //                    imageUrl = linkedinUserDetails.pictureUrl,
        //                    priviledgeLevel = (short)PriviledgeLevel.None
        //                };
        //                _db.Users.Add(user);

        //                try
        //                {
        //                    _db.SaveChanges();

        //                    var data = new Dictionary<string, string>();
        //                    data["Username"] = user.username;
        //                    data["Password"] = user.password;
        //                    data["userGuid"] = user.guid;

        //                    var encryptedData = EncryptionClass.encryptUserDetails(data);

        //                    response.Payload = new LoginResponse();
        //                    response.Payload.UTMZK = encryptedData["UTMZK"];
        //                    response.Payload.UTMZV = encryptedData["UTMZV"];
        //                    response.Payload.TimeStamp = DateTime.Now.ToString(CultureInfo.InvariantCulture);
        //                    response.Payload.Code = "210";
        //                    response.Status = 210;
        //                    response.Message = "user Login via linkedin";
        //                    try
        //                    {
        //                        var session = new urNoticeSession(user.username);
        //                        TokenManager.CreateSession(session);
        //                        response.Payload.UTMZT = session.SessionId;

        //                        ViewBag.umtzt = response.Payload.UTMZT;
        //                        ViewBag.umtzk = response.Payload.UTMZK;
        //                        ViewBag.umtzv = response.Payload.UTMZV;
        //                        ViewBag.isNewUser = "true";



        //                        //new UserMessageService().SendUserNotificationForAccountVerificationSuccess(
        //                        //    user.username, CommonConstants.userType_user
        //                        //);


        //                        return View();
        //                    }
        //                    catch (DbEntityValidationException e)
        //                    {
        //                        DbContextException.LogDbContextException(e);
        //                        response.Status = 500;
        //                        response.Message = "Internal Server Error !!";
        //                    }

        //                }
        //                catch (DbEntityValidationException e)
        //                {
        //                    DbContextException.LogDbContextException(e);
        //                    response.Status = 500;
        //                    response.Message = "Internal Server Error !!!";
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        authLink = CreateAuthorization();
        //        var linkedInApiData = new linkedinAuth
        //        {
        //            oauth_Token = _oauth.Token,
        //            oauth_TokenSecret = _oauth.TokenSecret,
        //            oauth_verifier = ""
        //        };
        //        _db.linkedinAuths.Add(linkedInApiData);
        //        try
        //        {
        //            _db.SaveChanges();
        //            Response.Redirect(authLink);
        //        }
        //        catch (DbEntityValidationException e)
        //        {
        //            DbContextException.LogDbContextException(e);
        //            response.Status = 500;
        //            response.Message = "Internal Server Error !!!";
        //        }

        //    }
        //    ViewBag.code = response.Status;
        //    return View();
        //}

        //public ActionResult LinkedinLoginCancelled(string type)
        //{
        //    var response = new ResponseModel<LoginResponse>();

        //    String AbsoluteUri = Request.Url.AbsoluteUri;
        //    string oauth_token = Request.QueryString["oauth_token"];
        //    string oauth_verifier = Request.QueryString["oauth_verifier"];
        //    string authLink = string.Empty;
        //    if (oauth_token != null && oauth_verifier != null)
        //    {

        //    }
        //    else
        //    {
        //        authLink = CreateAuthorization();
        //        Response.Redirect(authLink);
        //    }
        //    return Json(response, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult FBLoginGetRedirectUri(string type)
        {
            var response = new ResponseModel<string>();

            String code = Request.QueryString["code"];
            string app_id = string.Empty;
            string app_secret = string.Empty;
            string returnUrl = "http://" + Request.Url.Authority + "/SocialAuth/FBLogin/facebook/";
            if (Request.Url.Authority.Contains("localhost"))
            {
                app_id = ConfigurationManager.AppSettings["FacebookAppID"].ToString();
                app_secret = ConfigurationManager.AppSettings["FacebookAppSecret"].ToString();
            }
            else
            {
                app_id = ConfigurationManager.AppSettings["FacebookAppIDCautom"].ToString();
                app_secret = ConfigurationManager.AppSettings["FacebookAppSecretCautom"].ToString();
            }

            if (code == null)
            {
                response.Status = 199;
                response.Message = (string.Format(
                    "https://graph.facebook.com/oauth/authorize?client_id={0}&redirect_uri={1}&scope={2}",
                    app_id, returnUrl, fb_scope));

                //Response.Redirect(response.Payload);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FBLogin(string type)
        {
            var response = new ResponseModel<LoginResponse>();

            String code = Request.QueryString["code"];
            String refKey = Request.QueryString["refKey"];
            string app_id = string.Empty;
            string app_secret = string.Empty;
            string returnUrl = "http://" + Request.Url.Authority + "/SocialAuth/FBLogin/facebook/";
            if (Request.Url.Authority.Contains("localhost"))
            {
                app_id = ConfigurationManager.AppSettings["FacebookAppID"].ToString();
                app_secret = ConfigurationManager.AppSettings["FacebookAppSecret"].ToString();
            }
            else
            {
                app_id = ConfigurationManager.AppSettings["FacebookAppIDCautom"].ToString();
                app_secret = ConfigurationManager.AppSettings["FacebookAppSecretCautom"].ToString();
            }

            
            if (code == null)
            {
                
                var ReturnUrl = (string.Format(
                    "https://graph.facebook.com/oauth/authorize?client_id={0}&redirect_uri={1}&scope={2}",
                    app_id, returnUrl, fb_scope));

                //return Json(response,JsonRequestBehavior.AllowGet);
                Response.Redirect(ReturnUrl);
            }
            else
            {
               
                    string access_token = new FacebookService().getFacebookAuthToken(returnUrl, fb_scope, code, app_id, app_secret);
                    var fb = new FacebookClient(access_token);
                    //dynamic result = fb.Get("fql",
                    //            new { q = "SELECT uid, name, first_name, middle_name, last_name, sex, locale, pic_small_with_logo, pic_big_with_logo, pic_square_with_logo, pic_with_logo, username FROM user WHERE uid=me()" });

                    dynamic result = fb.Get("me?fields=id,first_name,last_name,gender,picture{url},email");
                    var email = result.email ?? result.id + "@facebook.com";

                    var FacebookAuthData = new FacebookAuth();
                    FacebookAuthData.username = CommonConstants.NA;
                    FacebookAuthData.AuthToken = access_token;
                    FacebookAuthData.datetime = DateTime.Now.ToString();
                    FacebookAuthData.facebookId = Convert.ToString(result.id);
                    FacebookAuthData.facebookUsername = result.id;

                    IDynamoDb dynamoDbModel = new DynamoDb();
                    OrbitPageCompanyUserWorkgraphyTable userInfo = dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTable(
                        DynamoDbHashKeyDataType.OrbitPageUser.ToString(),
                        email,
                        null
                        );
                    
                    if (userInfo != null)
                    {

                        IPerson loginModel = new Consumer();

                        response = loginModel.Login(userInfo.OrbitPageUser.email,userInfo.OrbitPageUser.password,true);                        
                        response.Payload.Code = "210";                        
                        response.Status = 210;
                        response.Message = "user Login via facebook";
                        
                        userInfo.OrbitPageUser.isActive = CommonConstants.TRUE;
                        userInfo.OrbitPageUser.keepMeSignedIn = CommonConstants.TRUE;
                        userInfo.OrbitPageUser.locked = CommonConstants.FALSE;
                        userInfo.FacebookAuthToken = access_token;
                        userInfo.FacebookId = FacebookAuthData.facebookId;

                        dynamoDbModel.CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(userInfo);
                        
                        
                        ViewBag.umtzt = response.Payload.UTMZT;
                        ViewBag.umtzk = response.Payload.UTMZK;
                        ViewBag.umtzv = response.Payload.UTMZV;
                        ViewBag.vertexid = response.Payload.VertexId;
                        ViewBag.userName = response.Payload.FirstName;
                        ViewBag.userImageUrl = response.Payload.imageUrl;
                        ViewBag.isNewUser = CommonConstants.FALSE;
                        return View();

                    }

                RegisterationRequest req = new RegisterationRequest()
                {
                    EmailId = email,
                    FirstName = result.first_name,
                    Gender = result.gender,
                    ImageUrl = result.picture.data.url,
                    LastName = result.last_name,
                    Password = EncryptionClass.Md5Hash(Guid.NewGuid().ToString()),
                    Referral = CommonConstants.NA,
                    Source = CommonConstants.Facebook,
                    Username = email,
                    fid = Convert.ToString(result.id)
                };

                IPerson registrationModel = new Consumer();
                response = registrationModel.SocialRegisterMe(req, Request);
                
                
                
                try
                {                    
                    
                    response.Payload.Code = "210";                    
                    response.Status = 210;
                    response.Message = "user Login via google";
                    try
                    {                        
                        ViewBag.umtzt = response.Payload.UTMZT;
                        ViewBag.umtzk = response.Payload.UTMZK;
                        ViewBag.umtzv = response.Payload.UTMZV;
                        ViewBag.vertexid = response.Payload.VertexId;
                        ViewBag.userName = response.Payload.FirstName;
                        ViewBag.userImageUrl = response.Payload.imageUrl;
                        ViewBag.isNewUser = "true";

                        return View();
                    }
                    catch (Exception ex)
                    {
                        //Todo:need to log exception.
                        response.Status = 500;
                        response.Message = "Failed";
                    }

                }
                catch (Exception ex)
                {
                    //Todo:need to log exception.
                    response.Status = 500;
                    response.Message = "Failed";
                }
            }
            return View();
        }

        protected string CreateAuthorization()
        {
            return _oauth.AuthorizationLinkGet();
        }

        protected void GetAccessToken(string Auth_token, string TokenSecret, string Auth_verifier)
        {
            _oauth.Token = Auth_token;
            _oauth.TokenSecret = TokenSecret;
            _oauth.Verifier = Auth_verifier;
            _oauth.AccessTokenGet(Auth_token);
        }

        protected void SendStatusUpdate(string AccessToken, string AccessTokenSecret, string Auth_verifier)
        {
            _oauth.Token = AccessToken;
            _oauth.TokenSecret = AccessTokenSecret;
            _oauth.Verifier = Auth_verifier;

            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
            xml += "<current-status>It's really working .</current-status>";

            string response = _oauth.APIWebRequest("PUT", "http://api.linkedin.com/v1/people/~/current-status", xml);
            //if (response == "")
            //    txtApiResponse.Text = "Your new status updated";

        }

        protected string RequestProfile(string AccessToken, string AccessTokenSecret, string Auth_verifier)
        {
            _oauth.Token = AccessToken;
            _oauth.TokenSecret = AccessTokenSecret;
            _oauth.Verifier = Auth_verifier;
            return _oauth.APIWebRequest("GET", "https://api.linkedin.com/v1/people/~:(id,first-name,last-name,industry,email-address,picture-url)?format=json", null);
        }

        protected string RequestProfileImage(string AccessToken, string AccessTokenSecret, string Auth_verifier)
        {
            _oauth.Token = AccessToken;
            _oauth.TokenSecret = AccessTokenSecret;
            _oauth.Verifier = Auth_verifier;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(_oauth.APIWebRequest("GET", "http://api.linkedin.com/v1/people/~/picture-urls::(original)", null));
            return JsonConvert.SerializeXmlNode(doc).Replace(@"@", @"").Remove(1, 44);
        }

        private string getGoogleAuthToken(string returnUrl, string scope, string code, string app_id, string app_secret)
        {
            byte[] buffer = Encoding.ASCII.GetBytes("code=" + code + "&client_id=" + app_id + "&client_secret=" + app_secret + "&redirect_uri=" + returnUrl + "&grant_type=authorization_code");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://accounts.google.com/o/oauth2/token");
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = buffer.Length;

            Stream strm = request.GetRequestStream();
            strm.Write(buffer, 0, buffer.Length);
            strm.Close();

            HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse();
            Stream responseStream = webResponse.GetResponseStream();
            StreamReader responseStreamReader = new StreamReader(responseStream);
            string result = responseStreamReader.ReadToEnd();//parse token from result
            var googleAccessTokenResponse = JsonConvert.DeserializeObject<googleAccessTokenWrapper>(Convert.ToString(result));
            return googleAccessTokenResponse.access_token;
        }

    }
}
