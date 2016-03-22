using System;
using System.Web;
using urNotice.Common.Infrastructure.Common.Constants;
using urNotice.Common.Infrastructure.Model.Person;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper;
using urNotice.Common.Infrastructure.signalRPushNotifications;
using urNotice.Services.GraphDb.GraphDbContract;
using urNotice.Services.NoSqlDb.DynamoDb;
using urNotice.Services.Person.PersonContract.LoginOperation;
using urNotice.Services.Person.PersonContract.RegistrationOperation;
using urNotice.Services.Solr.SolrUser;

namespace urNotice.Services.Person
{
    public class Consumer : IPerson
    {
        public ResponseModel<LoginResponse> RegisterMe(RegisterationRequest req, HttpRequestBase request)
        {
            ISolrUser solrUserModel = new SolrUser();
            IDynamoDb dynamoDbModel = new DynamoDb();
            IGraphDbContract graphDbContractModel = new GraphDbContract();

            IOrbitPageRegistration orbitPageRegistration = new OrbitPagePersonRegistration(solrUserModel, dynamoDbModel, graphDbContractModel);
            orbitPageRegistration.SetIsValidationEmailRequired(true); //email validation is required.
            var response = orbitPageRegistration.RegisterUser(req, request);
            //SendAccountCreationValidationEmail.SendAccountCreationValidationEmailMessage(req, guid, request);
            SignalRController.BroadCastNewUserRegistration();
            return response;
        }

        public ResponseModel<LoginResponse> SocialRegisterMe(RegisterationRequest req, HttpRequestBase request)
        {
            ISolrUser solrUserModel = new SolrUser();
            IDynamoDb dynamoDbModel = new DynamoDb();
            IGraphDbContract graphDbContractModel = new GraphDbContract();

            IOrbitPageRegistration orbitPageRegistration = new OrbitPagePersonRegistration(solrUserModel, dynamoDbModel, graphDbContractModel);
            orbitPageRegistration.SetIsValidationEmailRequired(false); //email validation not required for social.
            var response = orbitPageRegistration.RegisterUser(req, request);
            //SendAccountCreationValidationEmail.SendAccountCreationValidationEmailMessage(req, guid, request);
            SignalRController.BroadCastNewUserRegistration();
            return response;
        }

        public ResponseModel<LoginResponse> Login(string userName, string password, bool isSocialLogin)
        {
            IOrbitPageLogin loginModel = new OrbitPageLogin();

            return loginModel.Login(userName, password, null, CommonConstants.TRUE, isSocialLogin);
            
        }
    }
}
