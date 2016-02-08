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
using urNotice.Common.Infrastructure.Common.Logger;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper;
using urNotice.Common.Infrastructure.Model.urNoticeModel.Solr;
using urNotice.Common.Infrastructure.Session;
using urNotice.Services.DynamoDbService;
using urNotice.Services.SessionService;
using urNotice.Services.SolrService;

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
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<UnDesignationSolr>>();
            var unDesignationSolrList = new List<UnDesignationSolr>();
            var designationObj = new UnDesignationSolr
            {
                id = GuidsPredefined.guids[0],
                designation = "Associate Software Engineer"
            };

            unDesignationSolrList.Add(designationObj);
            solr.AddRange(unDesignationSolrList);
            solr.Commit();

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
                queryText = queryText.Replace(" ", "*");
                //queryText = queryText.ToLower();
                var solr = ServiceLocator.Current.GetInstance<ISolrReadOnlyOperations<UnDesignationSolr>>();
                var solrQuery = new SolrQuery("designation:*" + queryText + "*");
                var solrQueryExecute = solr.Query(solrQuery, new QueryOptions
                {
                    Rows = 15,
                    Start = 0,
                    Fields = new[] { "designation", "id" }
                });
                response.Payload = solrQueryExecute;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
            
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDetails()
        {
            var userType = Request.QueryString["userType"].ToString(CultureInfo.InvariantCulture);
            var headers = new HeaderManager(Request);
            urNoticeSession session = new SessionService().CheckAndValidateSession(headers, authKey, accessKey, secretKey);

            var isValidToken = TokenManager.IsValidSession(headers.AuthToken);
            if (isValidToken)
            {
                var clientDetailResponse = new SolrService().GetClientDetails(session.UserName);
                return Json(clientDetailResponse, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var response = new ResponseModel<string>();
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
                                
                var solr = ServiceLocator.Current.GetInstance<ISolrReadOnlyOperations<UnUserSolr>>();
                var solrQuery = new SolrQuery("(id:" + queryText + ") OR (email:"+queryText+") OR (phone:"+queryText+")");
                var solrQueryExecute = solr.Query(solrQuery, new QueryOptions
                {
                    Rows = 1,
                    Start = 0,
                    Fields = new[] {"id" }
                });

                if (solrQueryExecute == null || solrQueryExecute.Count == 0)
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
                queryText = queryText.Replace(" ", "*");
                //queryText = queryText.ToLower();
                var solr = ServiceLocator.Current.GetInstance<ISolrReadOnlyOperations<UnCompanySolr>>();
                var solrQuery = new SolrQuery("companyname:*" + queryText + "*");
                var solrQueryExecute = solr.Query(solrQuery, new QueryOptions
                {
                    Rows = 15,
                    Start = 0,
                    Fields = new[] { "guid", "companyname", "companyid", "isprimary", "squarelogourl" }
                });
                response.Payload = solrQueryExecute;
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
                queryText = queryText.Replace(" ", "*");
                //queryText = queryText.ToLower();
                var solr = ServiceLocator.Current.GetInstance<ISolrReadOnlyOperations<UnUserSolr>>();

                String solrQueryString = "(name:" + queryText + "*) || (lastname:" + queryText + "*) || (email:" + queryText + ") || (username:" + queryText + ") || (phone:" + queryText + ")";
                var solrQuery = new SolrQuery(solrQueryString);
                var solrQueryExecute = solr.Query(solrQuery, new QueryOptions
                {
                    Rows = 15,
                    Start = 0,
                    Fields = new[] { "email","name", "profilepic", "vertexId" }
                });
                response.Payload = solrQueryExecute;
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
                //var headers = new HeaderManager(Request);
                //new TokenManager().getSessionInfo(headers.AuthToken, headers);
                //urNoticeSession session = new TokenManager().getSessionInfo(headers.AuthToken, headers);

                //if(session != null)
                //    DynamoDbService.IncrementUserAnalyticsCounter(accessKey, secretKey, session.UserName, cid);

                var solr = ServiceLocator.Current.GetInstance<ISolrReadOnlyOperations<UnCompanySolr>>();
                var solrQuery = new SolrQuery("guid:" + cid);
                var solrQueryExecute = solr.Query(solrQuery, new QueryOptions
                {
                    Rows = 10,
                    Start = 0,
                    Fields = new[] { "guid","companyid","companyname","rating","website","size","description","averagerating","totalratingcount","totalreviews","isprimary",
                        "logourl", "squarelogourl", "speciality", "telephone","avgnoticeperiod","buyoutpercentage","maxnoticeperiod","minnoticeperiod","avghikeperct","perclookingforchange",
                        "sublocality","city","district","state","country","postal_code","latitude","longitude","geo" }
                });
                response.Payload = solrQueryExecute;
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

                var solr = ServiceLocator.Current.GetInstance<ISolrReadOnlyOperations<UnUserSolr>>();
                var solrQuery = new SolrQuery("username:" + uid);
                var solrQueryExecute = solr.Query(solrQuery, new QueryOptions
                {
                    Rows = 10,
                    Start = 0,
                    Fields = new[] { "id", "firstname", "lastname", "gender", "profilepic", "isactive", "source", "email", "phone", "username", "coverpic" }
                });
                response.Payload = solrQueryExecute;
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
            var q = Request.QueryString["q"].ToString(CultureInfo.InvariantCulture);
            var page = Request.QueryString["page"].ToString(CultureInfo.InvariantCulture);
            var perpage = Request.QueryString["perpage"].ToString(CultureInfo.InvariantCulture);

            var totalMatch = "";
            if (Request.QueryString["totalMatch"] != null && Request.QueryString["totalMatch"] != "null" && Request.QueryString["totalMatch"] != "undefined")
            {
                totalMatch = Request.QueryString["totalMatch"].ToString(CultureInfo.InvariantCulture);
            }
            if (q == null || q == "")
                q = "*";
            else
                q = q.Replace(" ", "*");

            if (page == null || page == "")
                page = "0";
            else
                page = (Convert.ToInt32(page) - 1).ToString();

            if (perpage == null || perpage == "")
                perpage = "10";
            try
            {

                var solr = ServiceLocator.Current.GetInstance<ISolrReadOnlyOperations<UnCompanySolr>>();
                var solrQuery = new SolrQuery("companyname:*" + q + "*");
                if (totalMatch == null || totalMatch == "")
                    totalMatch = solr.Query(solrQuery).Count().ToString();

                var solrQueryExecute = solr.Query(solrQuery, new QueryOptions
                {
                    Rows = Convert.ToInt32(perpage),
                    Start = Convert.ToInt32(page),
                    Fields = new[] { "guid","companyid","companyname","rating","website","size","description","averagerating","totalratingcount","totalreviews","isprimary",
                        "logourl", "squarelogourl", "speciality", "telephone","avgnoticeperiod","buyoutpercentage","maxnoticeperiod","minnoticeperiod","avghikeperct","perclookingforchange",
                        "sublocality","city","district","state","country","postal_code","latitude","longitude","geo" }
                });

                var queryResponse = new Dictionary<String, Object>();
                queryResponse["result"] = solrQueryExecute;
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

        public JsonResult GetCompanyCompetitorsDetail()
        {
            var response = new ResponseModel<SolrQueryResults<UnCompanySolr>>();
            var size = Request.QueryString["size"].ToString(CultureInfo.InvariantCulture);
            var rating = Request.QueryString["rating"].ToString(CultureInfo.InvariantCulture);
            var speciality = Request.QueryString["speciality"].ToString(CultureInfo.InvariantCulture);
            if (speciality != null)
                speciality = speciality.Replace("(", "*").Replace(")", "*").Replace(" ", "*").Replace(":", "");

            try
            {

                var solr = ServiceLocator.Current.GetInstance<ISolrReadOnlyOperations<UnCompanySolr>>();
                String query = CompanyCompetitorQueryBuilder(size, rating, speciality);
                var solrQuery = new SolrQuery(query);

                var solrQueryExecute = solr.Query(solrQuery, new QueryOptions
                {
                    Rows = 10,
                    Start = 0,
                    Fields = new[] { "guid", "rating","website","size", "companyname","isprimary",
                        "squarelogourl"}
                });

                if (solrQueryExecute.Count < 2)
                {
                    query = CompanyCompetitorQueryBuilder(null, rating, speciality);
                    solrQuery = new SolrQuery(query);
                    solrQueryExecute = solr.Query(solrQuery, new QueryOptions
                    {
                        Rows = 10,
                        Start = 0,
                        Fields = new[] { "guid", "rating","website","size", "companyname","isprimary",
                        "squarelogourl"}
                    });
                }
                response.Payload = solrQueryExecute;
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

        private String CompanyCompetitorQueryBuilder(String size, String rating, String speciality)
        {
            StringBuilder query = new StringBuilder();

            query.Append("(");

            if (size != null)
            {
                query.Append("(");
                query.Append("size:" + size);
                query.Append(")");
            }



            if (speciality != null && speciality != " " && speciality != "")
            {
                if (size != null)
                    query.Append(" AND ");

                query.Append("(");
                foreach (var companySpeciality in speciality.Split(','))
                {
                    var companySpecialityLocal = companySpeciality.Replace(" ", "*");
                    query.Append("(");
                    query.Append("speciality:" + companySpecialityLocal.Trim());
                    query.Append(")");

                    query.Append(" OR ");

                    query.Append("(");
                    query.Append("speciality: " + companySpecialityLocal.Trim());
                    query.Append(")");

                    query.Append(" OR ");
                }

                //queryString = query.ToString();
                query.Remove(query.Length - 4, 4);

                query.Append(")");
            }
            query.Append(")");

            return query.ToString();
        }

    }
}
