using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.GraphModel;
using urNotice.Common.Infrastructure.Session;
using urNotice.Services.SessionService;
using urNotice.Services.UserService;

namespace urNoticeUser.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /User/
        private static string accessKey = ConfigurationManager.AppSettings["AWSAccessKey"];
        private static string secretKey = ConfigurationManager.AppSettings["AWSSecretKey"];
        private static string authKey = ConfigurationManager.AppSettings["AuthKey"];


        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetDetails()
        {
            var userType = Request.QueryString["userType"].ToString(CultureInfo.InvariantCulture);
            var headers = new HeaderManager(Request);
            urNoticeSession session = new SessionService().CheckAndValidateSession(headers, authKey, accessKey, secretKey);

            var isValidToken = TokenManager.IsValidSession(headers.AuthToken);
            if (isValidToken)
            {
                var clientDetailResponse = new UserService().GetClientDetails(session.UserName,accessKey,secretKey);              
                return Json(clientDetailResponse,JsonRequestBehavior.AllowGet);
            }
            else
            {
                var response = new ResponseModel<string>();
                response.Status = 401;
                response.Message = "Unauthorized";
                return Json(response, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult UserPost()
        {
            var message = Request.QueryString["message"].ToString(CultureInfo.InvariantCulture);
            var image = Request.QueryString["image"].ToString(CultureInfo.InvariantCulture);
            var vertexId = Request.QueryString["vertexId"].ToString(CultureInfo.InvariantCulture);
            var headers = new HeaderManager(Request);
            urNoticeSession session = new SessionService().CheckAndValidateSession(headers, authKey, accessKey, secretKey);

            var isValidToken = TokenManager.IsValidSession(headers.AuthToken);
            if (isValidToken)
            {
                var newUserPostResponse = new UserService().CreateNewUserPost(session, message,image,vertexId, accessKey, secretKey);
                return Json(newUserPostResponse, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var response = new ResponseModel<string>();
                response.Status = 401;
                response.Message = "Unauthorized";
                return Json(response, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult GetUserPost()
        {
            var from = Request.QueryString["from"].ToString(CultureInfo.InvariantCulture);
            var to = Request.QueryString["to"].ToString(CultureInfo.InvariantCulture);
            var vertexId = Request.QueryString["vertexId"].ToString(CultureInfo.InvariantCulture);

            var headers = new HeaderManager(Request);
            urNoticeSession session = new SessionService().CheckAndValidateSession(headers, authKey, accessKey, secretKey);

            var isValidToken = TokenManager.IsValidSession(headers.AuthToken);
            isValidToken = true;//TODO: currently hard coded.
            if (isValidToken)
            {
                Boolean isRequestValid = true;
                if(String.IsNullOrWhiteSpace(vertexId) || vertexId.Equals("undefined"))
                {
                    if (session != null)
                        vertexId = session.UserVertexId;
                    else
                        isRequestValid = false;
                }

                if (isRequestValid)
                {
                    var getUserPostResponse = new UserService().GetUserPost(vertexId, from, to, accessKey, secretKey);
                    var getUserPostResponseDeserialized =
                        JsonConvert.DeserializeObject<UserPostVertexModelResponse>(getUserPostResponse);
                    return Json(getUserPostResponseDeserialized, JsonRequestBehavior.AllowGet);
                }
                else
                {                    
                    return Json("not a valid request", JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var response = new ResponseModel<string>();
                response.Status = 401;
                response.Message = "Unauthorized";
                return Json(response, JsonRequestBehavior.AllowGet);
            }

        }
    }
}
