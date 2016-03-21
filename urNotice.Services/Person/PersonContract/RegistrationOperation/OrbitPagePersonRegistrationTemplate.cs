using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using urNotice.Common.Infrastructure.Common.Constants;
using urNotice.Common.Infrastructure.Encryption;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper;
using urNotice.Common.Infrastructure.Model.urNoticeModel.Solr;
using urNotice.Services.GraphDb;
using urNotice.Services.GraphDb.GraphDbContract;
using urNotice.Services.NoSqlDb.DynamoDb;
using urNotice.Services.Solr.SolrUser;

namespace urNotice.Services.Person.PersonContract.RegistrationOperation
{
    public abstract class OrbitPagePersonRegistrationTemplate : IOrbitPageRegistration
    {
        protected ISolrUser _solrUserModel;
        protected IGraphDbContract _graphDbContractModel;
        protected IDynamoDb _dynamoDbModel;
        protected bool IsValidationEmailRequired;
        public OrbitPagePersonRegistrationTemplate(ISolrUser solrUserModel, IDynamoDb dynamoDbModel, IGraphDbContract graphDbContractModel)
        {
            this._solrUserModel = solrUserModel;            
            this._dynamoDbModel = dynamoDbModel;
            this._graphDbContractModel = graphDbContractModel;
            IsValidationEmailRequired = false;
        }

        public ResponseModel<string> RegisterUser(RegisterationRequest req, HttpRequestBase request)
        {

            ResponseModel<string> validateInputResponse = ValidateInputResponse(req);
            if (validateInputResponse.AbortProcess) return validateInputResponse;


            ResponseModel<string> checkForUniqueUserName = CheckForUniqueUserName(req);
            if (checkForUniqueUserName.AbortProcess) return checkForUniqueUserName;

            var user = GenerateOrbitPageUserObject(req);
            if (user == null) return OrbitPageResponseModel.SetNotFound("user is null after passing through GenerateOrbitPageUserObject(req)."); 
            
            CheckAndSetReferralBonus(req);

            if (IsValidationEmailRequired)
            {
                SendAccountVerificationEmail(user, request);
            }

            return SaveUserToDb(user);
            
        }

        public void SetIsValidationEmailRequired(bool res)
        {
            this.IsValidationEmailRequired = res;
        }

        protected abstract ResponseModel<String> ValidateInputResponse(RegisterationRequest req);
        protected abstract ResponseModel<String> CheckForUniqueUserName(RegisterationRequest req);
        protected abstract bool CheckAndSetReferralBonus(RegisterationRequest req);
        protected abstract OrbitPageUser GenerateOrbitPageUserObject(RegisterationRequest req);
        protected abstract ResponseModel<String> SaveUserToDb(OrbitPageUser user);
        protected abstract ResponseModel<String> SendAccountVerificationEmail(OrbitPageUser user, HttpRequestBase request);

    }
}
