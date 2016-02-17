using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using urNotice.Common.Infrastructure.Common.Constants;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.GraphModel;
using urNotice.Common.Infrastructure.Model.urNoticeModel.User;
using urNotice.Common.Infrastructure.Session;
using urNotice.Services.DynamoDbService;
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

        
        //[HttpPost]
        //public JsonResult UserPost(UserNewPostRequest req)
        //{
        //    var message = req.Message;
        //    var image = req.Image;
        //    var vertexId = req.VertexId;
        //    var headers = new HeaderManager(Request);
        //    urNoticeSession session = new SessionService().CheckAndValidateSession(headers, authKey, accessKey, secretKey);

        //    var isValidToken = TokenManager.IsValidSession(headers.AuthToken);
        //    if (isValidToken)
        //    {
        //        if (String.IsNullOrWhiteSpace(image) || image == CommonConstants.undefined)
        //        {
        //            image = String.Empty;
        //        }
        //        var newUserPostResponse = new UserService().CreateNewUserPost(session, message, image, vertexId, accessKey, secretKey);
        //        return Json(newUserPostResponse);
        //    }
        //    else
        //    {
        //        var response = new ResponseModel<string>();
        //        response.Status = 401;
        //        response.Message = "Unauthorized";
        //        return Json(response);
        //    }

        //}

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
                if (String.IsNullOrWhiteSpace(image) || image == CommonConstants.undefined)
                {
                    image = String.Empty;
                }
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

        public JsonResult UserCommentOnPost()
        {
            var message = Request.QueryString["message"].ToString(CultureInfo.InvariantCulture);
            var image = Request.QueryString["image"].ToString(CultureInfo.InvariantCulture);
            var vertexId = Request.QueryString["vertexId"].ToString(CultureInfo.InvariantCulture);

            var wallVertexId = Request.QueryString["wallVertexId"].ToString(CultureInfo.InvariantCulture);
            var postPostedByVertexId = Request.QueryString["postPostedByVertexId"].ToString(CultureInfo.InvariantCulture);

            var headers = new HeaderManager(Request);
            urNoticeSession session = new SessionService().CheckAndValidateSession(headers, authKey, accessKey, secretKey);

            var isValidToken = TokenManager.IsValidSession(headers.AuthToken);
            if (isValidToken)
            {
                if (String.IsNullOrWhiteSpace(image) || image == CommonConstants.undefined)
                {
                    image = String.Empty;
                }
                var newUserPostResponse = new UserService().CreateNewCommentOnUserPost(session, message, image, vertexId,wallVertexId,postPostedByVertexId, accessKey, secretKey);
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
