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
using urNotice.Common.Infrastructure.commonMethods.Emails;
using urNotice.Common.Infrastructure.Encryption;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper;
using urNotice.Services.GraphDb.GraphDbContract;
using urNotice.Services.NoSqlDb.DynamoDb;
using urNotice.Services.Solr.SolrUser;

namespace urNotice.Services.Person.PersonContract.RegistrationOperation
{
    public class OrbitPagePersonRegistration : OrbitPagePersonRegistrationTemplate
    {
        public OrbitPagePersonRegistration(ISolrUser solrUserModel, IDynamoDb dynamoDbModel, IGraphDbContract graphDbContractModel) : base(solrUserModel, dynamoDbModel, graphDbContractModel)
        {
        }

        protected override ResponseModel<string> ValidateInputResponse(RegisterationRequest req)
        {
            if (req == null) return OrbitPageResponseModel.SetNotFound("req object cann't be null.");
            if (req.EmailId == null) return OrbitPageResponseModel.SetNotFound("EmailId cann't be null");
            if (req.Username == null) return OrbitPageResponseModel.SetNotFound("Username cann't be null");

            return OrbitPageResponseModel.SetOk("valid input");
        }

        protected override ResponseModel<string> CheckForUniqueUserName(RegisterationRequest req)
        {
            var solrUserEmail = _solrUserModel.GetPersonData(req.EmailId, req.Username, null, null, false);
            if (solrUserEmail != null) return OrbitPageResponseModel.SetAlreadyTaken("Sorry " + req.Username + " username is already taken");
            return OrbitPageResponseModel.SetOk("Username is unique");
        }

        protected override OrbitPageUser GenerateOrbitPageUserObject(RegisterationRequest req)
        {
            req.EmailId = req.EmailId.ToLower();
            req.Username = req.Username.ToLower();
            req.FirstName = FirstCharToUpper(req.FirstName);
            req.LastName = FirstCharToUpper(req.LastName);

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
                userCoverPic = CommonConstants.CompanySquareLogoNotAvailableImage
            };

            return user;
        }

        protected override ResponseModel<string> SaveUserToDb(OrbitPageUser user)
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
                OrbitPageResponseModel.SetInternalServerError("GraphDb Data insertion Failed.");
            }
            else if (!dynamoDbDataInserted)
            {
                //dynamoDb insertion failed..
                OrbitPageResponseModel.SetInternalServerError("DynamoDb Data insertion Failed.");
            }
            else if (!solrDbDataInserted)
            {
                //solr db insertion failed..
                OrbitPageResponseModel.SetInternalServerError("SolrDb Data insertion Failed.");
            }

            return OrbitPageResponseModel.SetOk("Registered Successfully.");
        }

        protected override ResponseModel<string> SendAccountVerificationEmail(OrbitPageUser user, HttpRequestBase request)
        {
            var sendEmail = new SendEmail();
            if (request.Url != null)
            {
                sendEmail.SendEmailMessage(user.email,
                    SmptCreateAccountConstants.SenderName,
                    SmptCreateAccountConstants.EmailTitle,
                    CreateAccountEmailBodyContent(request.Url.Authority, user),
                    null,
                    null,
                    SmptCreateAccountConstants.SenderName,
                    SmtpConfig.SmtpEmailFromDoNotReply
                    );

            }
            return OrbitPageResponseModel.SetOk("email sent successfully.");
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
