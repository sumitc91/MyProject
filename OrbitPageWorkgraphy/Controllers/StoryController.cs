using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using urNotice.Common.Infrastructure.Common.Config;
using urNotice.Common.Infrastructure.Common.Constants;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.Workgraphy.Model;
using urNotice.Common.Infrastructure.Session;
using urNotice.Services.GraphDb.GraphDbContract;
using urNotice.Services.NoSqlDb.DynamoDb;
using urNotice.Services.SessionService;
using urNotice.Services.Solr.SolrUser;
using urNotice.Services.Solr.SolrWorkgraphy;
using urNotice.Services.Workgraphy;
namespace OrbitPageWorkgraphy.Controllers
{
    public class StoryController : Controller
    {
        //
        // GET: /Story/

        private static string accessKey = AwsConfig._awsAccessKey;// ConfigurationManager.AppSettings["AWSAccessKey"];
        private static string secretKey = AwsConfig._awsSecretKey;//ConfigurationManager.AppSettings["AWSSecretKey"];
        private static string authKey = OrbitPageConfig.AuthKey;//ConfigurationManager.AppSettings["AuthKey"];

        public ActionResult Index()
        {
            return View();
        }

        [System.Web.Mvc.HttpPost]
        public JsonResult CreateUrJobGraphy(StoryPostRequest req)
        {            
            ISolrUser solrUserModel = new SolrUser();
            ISolrWorkgraphy solrWorkgraphyModel = new SolrWorkgraphy();
            IDynamoDb dynamoDbModel = new DynamoDb();
            IGraphDbContract graphDbContractModel = new GraphDbContract();

            var response = new ResponseModel<StoryPostResponse>();

            var headers = new HeaderManager(Request);
            urNoticeSession session = new SessionService().CheckAndValidateSession(headers, authKey, accessKey, secretKey);

            if (session == null)
            {
                session = new urNoticeSession(req.Data.email, CommonConstants.NA);
            }
            //var isValidToken = TokenManager.IsValidSession(headers.AuthToken);

            //IWorkgraphyService workgraphyService = new WorkgraphyService(solrUserModel,solrWorkgraphyModel, dynamoDbModel, graphDbContractModel);
            //response = workgraphyService.PublishNewWorkgraphy(session,req);
            
            return Json(response);
        }
    }
}
