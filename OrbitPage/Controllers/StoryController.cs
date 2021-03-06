﻿using System.Web.Mvc;
using urNotice.Common.Infrastructure.Common.Config;
using urNotice.Common.Infrastructure.Common.Constants;
using urNotice.Common.Infrastructure.Common.Enum;
using urNotice.Common.Infrastructure.commonMethods;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.Workgraphy.Model;
using urNotice.Common.Infrastructure.Session;
using urNotice.Services.GraphDb.GraphDbContract;
using urNotice.Services.NoSqlDb.DynamoDb;
using urNotice.Services.SessionService;
using urNotice.Services.Solr.SolrUser;
using urNotice.Services.Solr.SolrWorkgraphy;
using urNotice.Services.Workgraphy;

namespace OrbitPage.Controllers
{
    public class StoryController : Controller
    {
        //
        // GET: /Story/

        private static string accessKey = AwsConfig._awsAccessKey;
        private static string secretKey = AwsConfig._awsSecretKey;
        private static string authKey = OrbitPageConfig.AuthKey;

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
            //IGraphDbContract graphDbContractModel = new GraphDbContract();
            IGraphDbContract graphDbContractModel = new GremlinServerGraphDbContract();
            var response = new ResponseModel<StoryPostResponse>();

            var headers = new HeaderManager(Request);
            urNoticeSession session = new SessionService().CheckAndValidateSession(headers, authKey, accessKey, secretKey);

            if (session == null)
            {
                string displayName = req.Data.name;
                session = new urNoticeSession(req.Data.email, displayName, CommonConstants.NA, CommonConstants.NA);
            }
            var isValidToken = TokenManager.IsValidSession(headers.AuthToken);

            IWorkgraphyService workgraphyService = new WorkgraphyService(solrUserModel,solrWorkgraphyModel, dynamoDbModel, graphDbContractModel);
            response = workgraphyService.PublishNewWorkgraphy(session, req, OrbitPageEnum.Workgraphy.ToString());
            
            return Json(response);
        }

        [System.Web.Mvc.HttpPost]
        public JsonResult CreateBlog(StoryPostRequest req)
        {
            ISolrUser solrUserModel = new SolrUser();
            ISolrWorkgraphy solrWorkgraphyModel = new SolrWorkgraphy();
            IDynamoDb dynamoDbModel = new DynamoDb();
            //IGraphDbContract graphDbContractModel = new GraphDbContract();
            IGraphDbContract graphDbContractModel = new GremlinServerGraphDbContract();
            var response = new ResponseModel<StoryPostResponse>();

            var headers = new HeaderManager(Request);
            urNoticeSession session = new SessionService().CheckAndValidateSession(headers, authKey, accessKey, secretKey);

            if (session == null)
            {
                string displayName = req.Data.name;
                session = new urNoticeSession(req.Data.email, displayName, CommonConstants.NA, CommonConstants.NA);
            }
            var isValidToken = TokenManager.IsValidSession(headers.AuthToken);

            IWorkgraphyService workgraphyService = new WorkgraphyService(solrUserModel, solrWorkgraphyModel, dynamoDbModel, graphDbContractModel);
            response = workgraphyService.PublishNewWorkgraphy(session, req, OrbitPageEnum.Blog.ToString());

            return Json(response);
        }
    }
}
