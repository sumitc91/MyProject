using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using urNotice.Common.Infrastructure.Common.Constants;
using urNotice.Common.Infrastructure.Common.Enum;
using urNotice.Common.Infrastructure.Common.Logger;
using urNotice.Common.Infrastructure.commonMethods;
using urNotice.Common.Infrastructure.commonMethods.Emails;
using urNotice.Common.Infrastructure.Encryption;
using urNotice.Common.Infrastructure.Model.urNoticeAuthContext;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper;
using urNotice.Common.Infrastructure.signalRPushNotifications;
using urNotice.Services.NoSqlDb.DynamoDb;
using urNotice.Services.Solr.SolrUser;

namespace urNotice.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private static readonly ILogger Logger = new Logger(Convert.ToString(MethodBase.GetCurrentMethod().DeclaringType));
        //private readonly urnoticeAuthEntities _db = new urnoticeAuthEntities();

        public delegate void ContactUsEmailSendDelegate(String emails, ContactUsRequest req);
        

        public ResponseModel<string> UserRegistration(RegisterationRequest req, HttpRequestBase request,string accessKey, string secretKey)
        {
            var response = new ResponseModel<String>();
            if (req.EmailId == null)
            {
                response.Status = 404;
                response.Message = "EmailId cann't be null";
                Logger.Info("EmailId cann't be null");
                return response;
            }

            if (req.Username == null)
            {
                response.Status = 404;
                response.Message = "Username cann't be null";
                Logger.Info("Username cann't be null");
                return response;
            }

            req.EmailId = req.EmailId.ToLower();
            req.Username = req.Username.ToLower();
            req.FirstName = FirstCharToUpper(req.FirstName);
            req.LastName = FirstCharToUpper(req.LastName);

            ISolrUser solrUserModel = new SolrUser();
            var solrUserEmail = solrUserModel.GetPersonData(req.EmailId, req.Username, null, null,false);//new SolrService.SolrService().GetSolrUserData(req.EmailId,req.Username,null,null);
            
            if (solrUserEmail != null)
            {
                response.Status = RegistrationConstants.USERNAME_ALREADY_TAKEN_CODE;
                response.Message = RegistrationConstants.USERNAME_ALREADY_TAKEN_MSG;
                Logger.Info("Username/Email Already Taken");
                return response;
            }

            var guid = Guid.NewGuid().ToString();
            string imgurl;
            if (req.Gender.Equals("m") || req.Gender.Equals("M") || req.Gender.ToLower().Equals("male"))
            {
                imgurl = CommonConstants.MaleProfessionalAvatar;
                req.Gender = CommonConstants.male;
            }
            else
            {
                imgurl = CommonConstants.FemaleProfessionalAvatar;
                req.Gender = CommonConstants.female;
            }
                

            var user = new OrbitPageUser
            {
                username = req.Username,
                email = req.EmailId,
                password = EncryptionClass.Md5Hash(req.Password),
                source = req.Source,
                isActive = CommonConstants.FALSE,
                guid = guid,
                fixedGuid = guid,
                firstName = req.FirstName,
                lastName = req.LastName,
                gender = req.Gender.ToLower(),
                imageUrl = imgurl,                
                priviledgeLevel = (short) PriviledgeLevel.None,
                validateUserKeyGuid = guid,
                userCoverPic = CommonConstants.CompanySquareLogoNotAvailableImage
            };
            //_db.Users.Add(user);

            if (!CommonConstants.NA.Equals(req.Referral))
            {
                //new ReferralService().payReferralBonusAsync(req.Referral, req.Username, Constants.status_false);
            }
            
            
            var userVertexIdInfo = new TitanService.TitanService().InsertNewUserToTitan(user, false,accessKey,secretKey);
            user.vertexId = userVertexIdInfo[TitanGraphConstants.Id];
            try
            {
               // _db.SaveChanges();
                IDynamoDb dynamoDbModel = new DynamoDb();
                dynamoDbModel.UpsertOrbitPageUser(user,null);                
                solrUserModel.InsertNewUser(user,false);

                //new TitanService.TitanService().InsertNewUserToTitan(user, false);

                SendAccountCreationValidationEmail.SendAccountCreationValidationEmailMessage(req, guid, request);
                SignalRController.BroadCastNewUserRegistration();
            }
            catch (DbEntityValidationException e)
            {
                DbContextException.LogDbContextException(e);
                response.Status = CommonConstants.SERVER_ERROR_CODE;
                response.Message = CommonConstants.SERVER_ERROR_MSG;
                return response;
            }

            response.Status = CommonConstants.SUCCESS_CODE;
            response.Message = CommonConstants.SUCCESS_MSG;
            response.Payload = RegistrationConstants.ACCOUNT_CREATED_MSG;

            return response;
        }

        public LoginResponse WebLogin(string userName, string password, string returnUrl, string keepMeSignedIn,string accessKey, string secretKey)
        {
            var userData = new LoginResponse();
            ISolrUser solrUserModel = new SolrUser();
            var userSolrDetail = solrUserModel.GetPersonData(userName, null, null, null,false);//new SolrService.SolrService().GetSolrUserData(userName,null,null,null); //userName can be email,username,phone.

            IDynamoDb dynamoDbModel = new DynamoDb();
            var userInfo = dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTable(
                DynamoDbHashKeyDataType.OrbitPageUser.ToString(),
                userSolrDetail.Email,
                null
                );

            //var user = _db.Users.SingleOrDefault(x => x.email == userSolrDetail.Email && x.password == passwrod);

            if (userInfo.OrbitPageUser != null && userInfo.OrbitPageUser.password == password)
            {
                //var user = _db.Users.SingleOrDefault(x => x.email == userSolrDetail.Email && x.isActive == "true");
                if (userInfo.OrbitPageUser.isActive == CommonConstants.TRUE)
                {
                    var data = new Dictionary<string, string>();
                    data["Username"] = userInfo.OrbitPageUser.email;
                    data["Password"] = userInfo.OrbitPageUser.password;
                    data["userGuid"] = userInfo.OrbitPageUser.guid;

                    var encryptedData = EncryptionClass.encryptUserDetails(data);
                    userData.UTMZK = encryptedData["UTMZK"];
                    userData.UTMZV = encryptedData["UTMZV"];
                    userData.FirstName = userInfo.OrbitPageUser.firstName;
                    userData.LastName = userInfo.OrbitPageUser.lastName;
                    userData.Username = userInfo.OrbitPageUser.username;
                    userData.imageUrl = userInfo.OrbitPageUser.imageUrl;
                    userData.VertexId = userInfo.OrbitPageUser.vertexId;

                    userData.TimeStamp = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                    userData.Code = "200";
                    try
                    {
                        userInfo.OrbitPageUser.keepMeSignedIn = keepMeSignedIn.Equals("true", StringComparison.OrdinalIgnoreCase) ? "true" : "false";
                        
                        dynamoDbModel.CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(userInfo);
                        
                    }
                    catch (DbEntityValidationException e)
                    {
                        DbContextException.LogDbContextException(e);
                        userData.Code = "500";
                        return userData;
                    }
                }
                else
                    userData.Code = "403";
            }
            else
                userData.Code = "401";
            return userData;
        }

        public ResponseModel<String> ValidateAccountService(ValidateAccountRequest req,string accessKey, string secretKey)
        {
            var response = new ResponseModel<string>();

            IDynamoDb dynamoDbModel = new DynamoDb();
            var userInfo = dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTable(
                DynamoDbHashKeyDataType.OrbitPageUser.ToString(),
                req.userName,
                null
                );

            //_db.ValidateUserKeys.SingleOrDefault(x => x.Username == req.userName && x.guid == req.guid);

            if (userInfo != null)
            {

                if (userInfo.OrbitPageUser == null)
                {
                    response.Status = 500;
                    response.Message = "Internal Server Error";
                    Logger.Info("Validate Account : " + req.userName);
                    return response;
                }
                if (userInfo.OrbitPageUser.isActive == "true")
                {
                    response.Status = 405;
                    response.Message = "already active user";
                    return response;
                }
                userInfo.OrbitPageUser.isActive = "true";
               
                try
                {                    
                    dynamoDbModel.CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(userInfo);

                }
                catch (DbEntityValidationException e)
                {
                    DbContextException.LogDbContextException(e);
                    response.Status = 500;
                    response.Message = "Internal Server Error";
                    return response;
                }
                response.Status = 200;
                response.Message = "validated";
                return response;
            }
            response.Status = 402;
            response.Message = "link expired";
            return response;
        }

        public ResponseModel<String> ResendValidationCodeService(ValidateAccountRequest req, HttpRequestBase request, string accessKey, string secretKey)
        {
            var response = new ResponseModel<string>();

            IDynamoDb dynamoDbModel = new DynamoDb();
            var userInfo = dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTable(
                DynamoDbHashKeyDataType.OrbitPageUser.ToString(),
                req.userName,
                null
                );

           
            if (userInfo != null)
            {

                if (userInfo.OrbitPageUser.isActive == "true")
                {
                    // Account has been already validated.
                    response.Status = 402;
                    response.Message = "warning";
                    return response;
                }
                
                userInfo.OrbitPageUser.validateUserKeyGuid = Guid.NewGuid().ToString();
               
                try
                {                    
                    dynamoDbModel.CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(userInfo);

                    SendAccountCreationValidationEmail.SendAccountValidationEmailMessage(req.userName, userInfo.OrbitPageUser.validateUserKeyGuid, request);
                }
                catch (DbEntityValidationException e)
                {
                    DbContextException.LogDbContextException(e);
                    response.Status = 500;
                    response.Message = "Internal Server Error !!!";
                    return response;
                }
                response.Status = 200;
                response.Message = "success";
                return response;
            }
            // User Doesn't Exist
            response.Status = 404;
            response.Message = "warning";
            return response;
        }

        public ResponseModel<String> ForgetPasswordService(string id, HttpRequestBase request, string accessKey, string secretKey)
        {
            var response = new ResponseModel<string>();
            id = id.ToLower();

            IDynamoDb dynamoDbModel = new DynamoDb();
            var userInfo = dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTable(
                DynamoDbHashKeyDataType.OrbitPageUser.ToString(),
                id,
                null
                );

            if (userInfo != null)
            {

                if ((userInfo.OrbitPageUser.isActive.Equals("false", StringComparison.InvariantCulture)))
                {
                    // User account has not validated yet
                    response.Status = 402;
                    response.Message = "warning";
                    return response;
                }

                userInfo.OrbitPageUser.forgetPasswordGuid = Guid.NewGuid().ToString();
                try
                {                    
                    dynamoDbModel.CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(userInfo);
                    
                    var forgetPasswordValidationEmail = new ForgetPasswordValidationEmail();
                    forgetPasswordValidationEmail.SendForgetPasswordValidationEmailMessage(id, userInfo.OrbitPageUser.forgetPasswordGuid, request, DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture));
                }
                catch (DbEntityValidationException e)
                {
                    DbContextException.LogDbContextException(e);
                    response.Status = 500;
                    response.Message = "Internal Server Error";
                    return response;
                }
            }
            else
            {
                // User doesn't exist
                response.Status = 404;
                response.Message = "warning";
                return response;
            }
            response.Status = 200;
            response.Message = "Success";
            return response;
        }

        public ResponseModel<String> ResetPasswordService(ResetPasswordRequest req, string accessKey, string secretKey)
        {
            var response = new ResponseModel<string>();
            //EncryptionClass.GetDecryptionValue(req.Username, ConfigurationManager.AppSettings["AuthKey"]);

            IDynamoDb dynamoDbModel = new DynamoDb();
            var userInfo = dynamoDbModel.GetOrbitPageCompanyUserWorkgraphyTable(
                DynamoDbHashKeyDataType.OrbitPageUser.ToString(),
                req.Username,
                null
                );


            if (userInfo != null && userInfo.OrbitPageUser.forgetPasswordGuid == req.Guid && userInfo.OrbitPageUser.forgetPasswordGuid != CommonConstants.NA)
            {
                userInfo.OrbitPageUser.forgetPasswordGuid = CommonConstants.NA;
                
                var password = EncryptionClass.Md5Hash(req.Password);
                userInfo.OrbitPageUser.password =password;
                userInfo.OrbitPageUser.locked =CommonConstants.FALSE;

                try
                {                    
                    dynamoDbModel.CreateOrUpdateOrbitPageCompanyUserWorkgraphyTable(userInfo);                   
                }
                catch (DbEntityValidationException e)
                {
                    DbContextException.LogDbContextException(e);
                    response.Status = 500;
                    response.Message = "Internal Server Error.";
                    Logger.Info("Save new Reseted Password : " + req.Username);
                    return response;
                }
                response.Status = 200;
                response.Message = "Success";
                return response;
            }
            response.Status = 402;
            response.Message = "link expired";
            return response;
        }

        public ResponseModel<String> ContactUsService(ContactUsRequest req)
        {
            //TODO: save in some database..
            var response = new ResponseModel<string>();
            var contactUsData = new contactU
            {
                Name = req.Name,
                Phone = req.Phone,
                RepliedBy = CommonConstants.NA,
                RepliedDateTime = CommonConstants.NA,
                ReplyMessage = CommonConstants.NA,
                Status = CommonConstants.status_open,
                Type = req.Type,
                dateTime = DateTime.Now,
                emailId = req.Email,
                heading = CommonConstants.NA,
                message = req.Message,
                username = req.Email
            };

            //_db.contactUs.Add(contactUsData);

            try
            {
               //_db.SaveChanges();
                var contactUsEmailDelegate = new ContactUsEmailSendDelegate(SendContactUsEmail.SendContactUsEmailMessage);

                string emailIds = req.SendMeACopy.Equals(CommonConstants.status_true,
                    StringComparison.CurrentCultureIgnoreCase)
                    ? ServerConfig.ContactUsReceivingEmailIds + "," + req.Email
                    : ServerConfig.ContactUsReceivingEmailIds;

                contactUsEmailDelegate.BeginInvoke(emailIds, req, null, null); //invoking the method

                //SendAccountCreationValidationEmail.SendContactUsEmailMessage(req.SendMeACopy.Equals(Constants.status_true,StringComparison.CurrentCultureIgnoreCase) ? ConfigurationManager.AppSettings["ContactUsReceivingEmailIds"].ToString(CultureInfo.InvariantCulture)+","+req.Email : ConfigurationManager.AppSettings["ContactUsReceivingEmailIds"].ToString(CultureInfo.InvariantCulture), req);
            }
            catch (DbEntityValidationException e)
            {
                DbContextException.LogDbContextException(e);
                response.Status = 500;
                response.Message = "Internal Server Error.";
                Logger.Info("Error occured in contact us");
                return response;
            }
            response.Status = 200;
            response.Message = "success";
            return response;
        }


        public static string FirstCharToUpper(string input)
        {
            if (String.IsNullOrEmpty(input))
                return CommonConstants.NA;
            return input.First().ToString(CultureInfo.InvariantCulture).ToUpper() + input.Substring(1);
        }
    }
}
