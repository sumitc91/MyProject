using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using Microsoft.Practices.ServiceLocation;
using SolrNet;
using System.Web.Mvc;
using SolrNet.Commands.Parameters;
using urNotice.Common.Infrastructure.Common.Constants;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper;
using urNotice.Common.Infrastructure.Model.urNoticeModel.Solr;
using urNotice.Common.Infrastructure.Session;
using urNotice.Services.ErrorLogger;
using urNotice.Services.GraphDb.GraphDbContract;
using urNotice.Services.NoSqlDb.DynamoDb;
using urNotice.Services.SessionService;
using urNotice.Services.Solr.SolrCompany;
using urNotice.Services.Solr.SolrDesignation;
using urNotice.Services.Solr.SolrUser;
using urNotice.Services.Solr.SolrWorkgraphy;
using urNotice.Services.Workgraphy;

namespace urNoticeSolr.Controllers
{
    public class SearchController : Controller
    {
        //
        // GET: /Search/

        private static readonly ILogger Logger =
            new Logger(Convert.ToString(MethodBase.GetCurrentMethod().DeclaringType));

        private static string accessKey = ConfigurationManager.AppSettings["AWSAccessKey"];
        private static string secretKey = ConfigurationManager.AppSettings["AWSSecretKey"];
        private static string authKey = ConfigurationManager.AppSettings["AuthKey"];

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult AddDesignation()
        {
            var response = new ResponseModel<String>();
            //TODO: to be implemented 
            response.Status = 200;
            response.Message = "Addes";
            return Json(response, JsonRequestBehavior.AllowGet);


        }

        public JsonResult GetDesignationDetails()
        {
            var response = new ResponseModel<SolrQueryResults<UnDesignationSolr>>();
            var queryText = Request.QueryString["q"].ToString(CultureInfo.InvariantCulture);
            try
            {
                ISolrDesignation solrDesignationModel = new SolrDesignation();
                response.Payload = solrDesignationModel.GetDesignationDetails(queryText);
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
            
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDetails()
        {
            var response = new ResponseModel<UnUserSolr>();

            var userType = Request.QueryString["userType"].ToString(CultureInfo.InvariantCulture);
            var headers = new HeaderManager(Request);
            urNoticeSession session = new SessionService().CheckAndValidateSession(headers, authKey, accessKey, secretKey);

            var isValidToken = TokenManager.IsValidSession(headers.AuthToken);
            if (isValidToken)
            {
                ISolrUser solrUserModel = new SolrUser();
                response.Payload = solrUserModel.GetPersonData(session.UserName, null, null, null, true);
                response.Status = 200;
                response.Message = "Success";
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            else
            {
                
                response.Status = 401;
                response.Message = "Unauthorized";
                return Json(response, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult IsUsernameExist()
        {
            var response = new ResponseModel<Boolean>();
            var queryText = Request.QueryString["q"].ToString(CultureInfo.InvariantCulture);            
            try
            {                                                
                ISolrUser solrUserModel = new SolrUser();
                var solrQuery = solrUserModel.GetPersonData(queryText,null,null,null,false);

                if (solrQuery == null)
                {
                    response.Status = 200;
                    response.Message = "user is unique.";
                    response.Payload = false;
                }
                else
                {
                    response.Status = 202;
                    response.Message = "user already exists.";
                    response.Payload = true;
                }
                    
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }            
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCompanyDetailsAutocomplete()
        {
            var response = new ResponseModel<SolrQueryResults<UnCompanySolr>>();
            var queryText = Request.QueryString["q"].ToString(CultureInfo.InvariantCulture);
            try
            {
                ISolrCompany solrCompanyModel = new SolrCompany();
                response.Payload = solrCompanyModel.GetCompanyDetailsAutocomplete(queryText);
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
            response.Status = 401;
            response.Message = "Unauthorized";
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserDetailsAutocomplete()
        {
            var response = new ResponseModel<SolrQueryResults<UnUserSolr>>();
            var queryText = Request.QueryString["q"].ToString(CultureInfo.InvariantCulture);
            try
            {
                ISolrUser solrUserModel = new SolrUser();
                response.Payload = solrUserModel.GetUserDetailsAutocomplete(queryText);
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
            response.Status = 401;
            response.Message = "Unauthorized";
            return Json(response, JsonRequestBehavior.AllowGet);
        }
      
        public JsonResult CompanyDetailsById()
        {
            var response = new ResponseModel<SolrQueryResults<UnCompanySolr>>();
            var cid = Request.QueryString["cid"].ToString(CultureInfo.InvariantCulture);
            try
            {
                ISolrCompany solrCompanyModel = new SolrCompany();
                response.Payload = solrCompanyModel.CompanyDetailsById(cid);
                response.Status = 200;
                response.Message = "Success";
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                response.Status = 500;
                response.Message = "Exception occured";
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UserDetailsById()
        {
            var response = new ResponseModel<SolrQueryResults<UnUserSolr>>();
            var uid = Request.QueryString["uid"].ToString(CultureInfo.InvariantCulture);
            try
            {
                ISolrUser solrUserModel = new SolrUser();
                response.Payload = solrUserModel.UserDetailsById(uid);
                response.Status = 200;
                response.Message = "Success";
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                response.Status = 500;
                response.Message = "Exception occured";
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserMutualFriendsDetail()
        {
            var response = new ResponseModel<IEnumerable<string>>();
            var username = Request.QueryString["username"].ToString(CultureInfo.InvariantCulture);

            var headers = new HeaderManager(Request);
            new TokenManager().getSessionInfo(headers.AuthToken, headers);
            var isValidToken = TokenManager.IsValidSession(headers.AuthToken);

            String user1 = "sumitchourasia91@gmail.com";
            String user2 = "abhinavsrivastava189@gmail.com";
            try
            {

                var solr = ServiceLocator.Current.GetInstance<ISolrReadOnlyOperations<UnVirtualFriendSolr>>();
                var solrQuery = new SolrQuery("user1:" + user1);
                var solrQueryExecute = solr.Query(solrQuery, new QueryOptions
                {
                    //Rows = 1000,
                    Start = 0,
                    Fields = new[] { "user2" }
                });



                var solrQuery2 = new SolrQuery("user1:" + user2);
                var solrQueryExecute2 = solr.Query(solrQuery2, new QueryOptions
                {
                    //Rows = 1000,
                    Start = 0,
                    Fields = new[] { "user2" }
                });

                var result = solrQueryExecute.Select(r => r.User2).Intersect(solrQueryExecute2.Select(r => r.User2));

                response.Payload = result;
                response.Status = 200;
                response.Message = "Success";
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                response.Status = 500;
                response.Message = "Exception occured";
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Search()
        {
            var response = new ResponseModel<Dictionary<String, Object>>();
            var queryResponse = new Dictionary<String, Object>();
            var q = Request.QueryString["q"].ToString(CultureInfo.InvariantCulture);
            var page = Request.QueryString["page"].ToString(CultureInfo.InvariantCulture);
            var perpage = Request.QueryString["perpage"].ToString(CultureInfo.InvariantCulture);

            var totalMatch = "";
            if (Request.QueryString["totalMatch"] != null && Request.QueryString["totalMatch"] != "null" && Request.QueryString["totalMatch"] != "undefined")
            {
                totalMatch = Request.QueryString["totalMatch"].ToString(CultureInfo.InvariantCulture);
            }
            
            try
            {
                ISolrCompany solrCompanyModel = new SolrCompany();
                queryResponse["result"] = solrCompanyModel.Search(q,page,perpage,totalMatch);
                queryResponse["count"] = totalMatch;
                response.Payload = queryResponse;
                response.Status = 200;
                response.Message = "Success";
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                response.Status = 500;
                response.Message = "Exception occured";
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLatestWorkgraphy()
        {
            var response = new ResponseModel<SolrQueryResults<UnWorkgraphySolr>>();

            ISolrUser solrUserModel = new SolrUser();
            ISolrWorkgraphy solrWorkgraphyModel = new SolrWorkgraphy();
            IDynamoDb dynamoDbModel = new DynamoDb();
            IGraphDbContract graphDbContractModel = new GraphDbContract();

            var queryResponse = new Dictionary<String, Object>();
            //var q = Request.QueryString["q"].ToString(CultureInfo.InvariantCulture);
            var page = Convert.ToInt32(Request.QueryString["page"].ToString(CultureInfo.InvariantCulture));
            var perpage = Convert.ToInt32(Request.QueryString["perpage"].ToString(CultureInfo.InvariantCulture));

            var totalMatch = "";
            if (Request.QueryString["totalMatch"] != null && Request.QueryString["totalMatch"] != "null" && Request.QueryString["totalMatch"] != "undefined")
            {
                totalMatch = Request.QueryString["totalMatch"].ToString(CultureInfo.InvariantCulture);
            }

            try
            {
                IWorkgraphyService workgraphyService = new WorkgraphyService(solrUserModel, solrWorkgraphyModel, dynamoDbModel, graphDbContractModel);
                response.Payload = workgraphyService.GetLatestWorkgraphy(page, perpage);
                queryResponse["count"] = totalMatch;
                response.Message = totalMatch;
                response.Status = 200;
                
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                response.Status = 500;
                response.Message = "Exception occured";
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetParticularWorkgraphyWithVertexId()
        {
            var response = new ResponseModel<SolrQueryResults<UnWorkgraphySolr>>();

            ISolrUser solrUserModel = new SolrUser();
            ISolrWorkgraphy solrWorkgraphyModel = new SolrWorkgraphy();
            IDynamoDb dynamoDbModel = new DynamoDb();
            IGraphDbContract graphDbContractModel = new GraphDbContract();

            var queryResponse = new Dictionary<String, Object>();
            var vertexId = Convert.ToInt32(Request.QueryString["vertexId"].ToString(CultureInfo.InvariantCulture));
            //var page = Convert.ToInt32(Request.QueryString["page"].ToString(CultureInfo.InvariantCulture));
            //var perpage = Convert.ToInt32(Request.QueryString["perpage"].ToString(CultureInfo.InvariantCulture));

            //var totalMatch = "";
            //if (Request.QueryString["totalMatch"] != null && Request.QueryString["totalMatch"] != "null" && Request.QueryString["totalMatch"] != "undefined")
            //{
            //    totalMatch = Request.QueryString["totalMatch"].ToString(CultureInfo.InvariantCulture);
            //}

            try
            {
                IWorkgraphyService workgraphyService = new WorkgraphyService(solrUserModel, solrWorkgraphyModel, dynamoDbModel, graphDbContractModel);
                response.Payload = workgraphyService.GetParticularWorkgraphyWithVertexId(vertexId);
               
                response.Message = "success";
                response.Status = 200;

            }
            catch (Exception ex)
            {
                string error = ex.Message;
                response.Status = 500;
                response.Message = "Exception occured";
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCompanyCompetitorsDetail()
        {
            var response = new ResponseModel<SolrQueryResults<UnCompanySolr>>();
            var size = Request.QueryString["size"].ToString(CultureInfo.InvariantCulture);
            var rating = Request.QueryString["rating"].ToString(CultureInfo.InvariantCulture);
            var speciality = Request.QueryString["speciality"].ToString(CultureInfo.InvariantCulture);            
            try
            {
                ISolrCompany solrCompanyModel = new SolrCompany();
                response.Payload = solrCompanyModel.GetCompanyCompetitorsDetail(size,rating,speciality);
                response.Status = 200;
                response.Message = "Success";
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                response.Status = 500;
                response.Message = "Exception occured";
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

    }
}
