using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using urNotice.Common.Infrastructure.Common.Config;
using urNotice.Common.Infrastructure.Common.Constants;
using urNotice.Common.Infrastructure.Common.Constants.EmailConstants;
using urNotice.Common.Infrastructure.commonMethods;
using urNotice.Common.Infrastructure.Encryption;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper;
using urNotice.Services.Email;
using urNotice.Services.Email.EmailFromGmail;
using urNotice.Services.Email.EmailFromMandrill;
using urNotice.Services.GraphDb.GraphDbContract;
using urNotice.Services.NoSqlDb.DynamoDb;
using urNotice.Services.Person.PersonContract.LoginOperation;
using urNotice.Services.Solr.SolrUser;

namespace urNotice.Services.Person.PersonContract.RegistrationOperation
{
    public class OrbitPagePersonRegistration : OrbitPagePersonRegistrationTemplate
    {
        public OrbitPagePersonRegistration(ISolrUser solrUserModel, IDynamoDb dynamoDbModel, IGraphDbContract graphDbContractModel) : base(solrUserModel, dynamoDbModel, graphDbContractModel)
        {
        }

        protected override ResponseModel<LoginResponse> ValidateInputResponse(RegisterationRequest req)
        {
            if (req == null) return OrbitPageResponseModel.SetNotFound("req object cann't be null.",new LoginResponse());
            if (req.EmailId == null) return OrbitPageResponseModel.SetNotFound("EmailId cann't be null", new LoginResponse());
            if (req.Username == null) return OrbitPageResponseModel.SetNotFound("Username cann't be null", new LoginResponse());

            return OrbitPageResponseModel.SetOk("valid input", new LoginResponse());
        }

        protected override ResponseModel<LoginResponse> CheckForUniqueUserName(RegisterationRequest req)
        {
            var solrUserEmail = _solrUserModel.GetPersonData(req.EmailId, req.Username, null, null, false);
            if (solrUserEmail != null) return OrbitPageResponseModel.SetAlreadyTaken("Sorry " + req.Username + " username is already taken", new LoginResponse());
            return OrbitPageResponseModel.SetOk("Username is unique", new LoginResponse());
        }

        protected override OrbitPageUser GenerateOrbitPageUserObject(RegisterationRequest req)
        {
            req.EmailId = req.EmailId.ToLower();
            req.Username = req.Username.ToLower();
            req.FirstName = FirstCharToUpper(req.FirstName);
            req.LastName = FirstCharToUpper(req.LastName);

            if (IsValidationEmailRequired)            
            {
                if (req.Gender.Equals("m") || req.Gender.Equals("M") || req.Gender.ToLower().Equals("male"))
                {
                    req.ImageUrl = CommonConstants.MaleProfessionalAvatar;
                    req.Gender = CommonConstants.male;
                }
                else
                {
                    req.ImageUrl = CommonConstants.FemaleProfessionalAvatar;
                    req.Gender = CommonConstants.female;
                }
            }
            

            var guid = CreateNewGuid();
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
                imageUrl = req.ImageUrl,
                priviledgeLevel = (short)PriviledgeLevel.None,
                validateUserKeyGuid = guid,
                userCoverPic = CommonConstants.CompanySquareLogoNotAvailableImage,
                keepMeSignedIn = CommonConstants.TRUE
            };

            if(!string.IsNullOrWhiteSpace(req.fid))
            {
                user.fid = req.fid;
            }

            if (!IsValidationEmailRequired)
            {
                user.isActive = CommonConstants.TRUE;
            }

            return user;
        }

        protected override ResponseModel<LoginResponse> SaveUserToDb(OrbitPageUser user)
        {
            

            bool graphDbDataInserted = true;
            bool dynamoDbDataInserted = true;
            bool solrDbDataInserted = true;

            //TODO: need to implement try catch for roll back if any exception occurs
            try
            {
                var userVertexIdInfo = _graphDbContractModel.InsertNewUserInGraphDb(user);
                user.vertexId = userVertexIdInfo[TitanGraphConstants.Id];
                try
                {
                    _dynamoDbModel.UpsertOrbitPageUser(user, null);
                    try
                    {
                        _solrUserModel.InsertNewUser(user, false);
                    }
                    catch (Exception)
                    {
                        solrDbDataInserted = false;
                    }

                }
                catch (Exception)
                {
                    dynamoDbDataInserted = false;
                }
            }
            catch (Exception)
            {
                graphDbDataInserted = false;
            }

            if (!graphDbDataInserted)
            {
                //graph db insertion failed..
                OrbitPageResponseModel.SetInternalServerError("GraphDb Data insertion Failed.", new LoginResponse());
            }
            else if (!dynamoDbDataInserted)
            {
                //dynamoDb insertion failed..
                OrbitPageResponseModel.SetInternalServerError("DynamoDb Data insertion Failed.", new LoginResponse());
            }
            else if (!solrDbDataInserted)
            {
                //solr db insertion failed..
                OrbitPageResponseModel.SetInternalServerError("SolrDb Data insertion Failed.", new LoginResponse());
            }

            IOrbitPageLogin loginModel = new OrbitPageLogin();
            response.Payload = loginModel.CreateLoginResponseModel(user);
            response.Status = 200;
            response.Message = "Registered Successfully.";
            return response;
        }

        protected override ResponseModel<string> SendAccountVerificationEmail(OrbitPageUser user, HttpRequestBase request)
        {            
            if (request != null && request.Url != null)
            {
                //IEmail emailModel = new EmailFromGmail();
                IEmail emailModel = new EmailFromMandrill();
                emailModel.SendEmail(user.email,
                    SmptCreateAccountConstants.SenderName,
                    SmptCreateAccountConstants.EmailTitle,
                    CreateAccountEmailBodyContent(request.Url.Authority, user),
                    null,
                    null,
                    SmptCreateAccountConstants.SenderName,
                    SmtpConfig.SmtpEmailFromDoNotReply
                    );
            }
            return OrbitPageResponseModel.SetOk("email sent successfully.", String.Empty);
        }

        protected override bool CheckAndSetReferralBonus(RegisterationRequest req)
        {
            if (!CommonConstants.NA.Equals(req.Referral))
            {
                //new ReferralService().payReferralBonusAsync(req.Referral, req.Username, Constants.status_false);
            }
            return false;
        }

        private static String CreateNewGuid()
        {
            return Guid.NewGuid().ToString();
        }

        private static string FirstCharToUpper(string input)
        {
            if (String.IsNullOrEmpty(input))
                return CommonConstants.NA;
            return input.First().ToString(CultureInfo.InvariantCulture).ToUpper() + input.Substring(1);
        }

        private static string CreateAccountEmailBodyContent(String requestUrlAuthority, OrbitPageUser user)
        {
            var template = File.ReadAllText(HttpContext.Current.Server.MapPath("~/EmailTemplate/CreateAccountEmail.html"));

            var encryptedUserInfo = new Dictionary<String, String>();
            encryptedUserInfo["EMAIL"] = user.email;
            encryptedUserInfo["KEY"] = user.guid;

            string messageBody = template.Replace("{1}", "http://" + SmptCreateAccountConstants.AccountDomain + "/#/" + "validate/" + encryptedUserInfo["EMAIL"] + "/" + encryptedUserInfo["KEY"]);
            return messageBody;
        }
    }
}
