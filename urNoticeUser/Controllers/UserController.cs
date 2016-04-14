using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using urNotice.Common.Infrastructure.Common.Constants;
using urNotice.Common.Infrastructure.Model.Person;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.GraphModel;
using urNotice.Common.Infrastructure.Model.urNoticeModel.User;
using urNotice.Common.Infrastructure.Session;
using urNotice.Services.GraphDb.GraphDbContract;
using urNotice.Services.Person;
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
                IPerson clientModel = new Consumer();
                var clientDetailResponse = clientModel.GetPersonDetails(session.UserName);//new UserService().GetClientDetails(session.UserName,accessKey,secretKey);              
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

        public JsonResult GetFullUserDetails()
        {
            var email = Request.QueryString["email"].ToString(CultureInfo.InvariantCulture);

            IPerson adminModel = new Admin();
            if (!string.IsNullOrEmpty(email))
            {
                var clientDetailResponse = adminModel.GetFullUserDetail(email);
                return Json(clientDetailResponse, JsonRequestBehavior.AllowGet);
            }

            return Json("Email address cann't be empty.", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNotificationDetails()
        {
            var headers = new HeaderManager(Request);
            var from = Request.QueryString["from"].ToString(CultureInfo.InvariantCulture);
            var to = Request.QueryString["to"].ToString(CultureInfo.InvariantCulture);

            urNoticeSession session = new SessionService().CheckAndValidateSession(headers, authKey, accessKey, secretKey);

            var isValidToken = TokenManager.IsValidSession(headers.AuthToken);
            if (isValidToken)
            {
                
                var clientNotificationDetailResponse = new UserService().GetUserNotification(session.UserVertexId,from,to, accessKey, secretKey);
                var clientNotificationDetailResponseDeserialized =
                        JsonConvert.DeserializeObject<UserNotificationVertexModelResponse>(clientNotificationDetailResponse);
                return Json(clientNotificationDetailResponseDeserialized, JsonRequestBehavior.AllowGet);
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

        //public JsonResult UserPost()
        //{
        //    var message = Request.QueryString["message"].ToString(CultureInfo.InvariantCulture);
        //    var image = Request.QueryString["image"].ToString(CultureInfo.InvariantCulture);
        //    var userWallVertexId = Request.QueryString["vertexId"].ToString(CultureInfo.InvariantCulture);
        //    var headers = new HeaderManager(Request);
        //    urNoticeSession session = new SessionService().CheckAndValidateSession(headers, authKey, accessKey, secretKey);

        //    var isValidToken = TokenManager.IsValidSession(headers.AuthToken);
        //    if (isValidToken)
        //    {
        //        if (String.IsNullOrWhiteSpace(image) || image == CommonConstants.undefined)
        //        {
        //            image = String.Empty;
        //        }
        //        var newUserPostResponse = new UserService().CreateNewUserPost(session, message, image, userWallVertexId, accessKey, secretKey);
        //        return Json(newUserPostResponse, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        var response = new ResponseModel<string>();
        //        response.Status = 401;
        //        response.Message = "Unauthorized";
        //        return Json(response, JsonRequestBehavior.AllowGet);
        //    }

        //}

        //public JsonResult UserCommentOnPost()
        //{
        //    var message = Request.QueryString["message"].ToString(CultureInfo.InvariantCulture);
        //    var image = Request.QueryString["image"].ToString(CultureInfo.InvariantCulture);
        //    var postVertexId = Request.QueryString["vertexId"].ToString(CultureInfo.InvariantCulture);

        //    var userWallVertexId = Request.QueryString["wallVertexId"].ToString(CultureInfo.InvariantCulture);
        //    var postPostedByVertexId = Request.QueryString["postPostedByVertexId"].ToString(CultureInfo.InvariantCulture);

        //    var headers = new HeaderManager(Request);
        //    urNoticeSession session = new SessionService().CheckAndValidateSession(headers, authKey, accessKey, secretKey);

        //    var isValidToken = TokenManager.IsValidSession(headers.AuthToken);
        //    if (isValidToken)
        //    {
        //        if (String.IsNullOrWhiteSpace(image) || image == CommonConstants.undefined)
        //        {
        //            image = String.Empty;
        //        }
        //        var newUserPostResponse = new UserService().CreateNewCommentOnUserPost(session, message, image, postVertexId, userWallVertexId, postPostedByVertexId, accessKey, secretKey);
        //        return Json(newUserPostResponse, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        var response = new ResponseModel<string>();
        //        response.Status = 401;
        //        response.Message = "Unauthorized";
        //        return Json(response, JsonRequestBehavior.AllowGet);
        //    }

        //}

        public JsonResult GetUserPost()
        {
            var from = Request.QueryString["from"].ToString(CultureInfo.InvariantCulture);
            var to = Request.QueryString["to"].ToString(CultureInfo.InvariantCulture);
            var vertexId = Request.QueryString["vertexId"].ToString(CultureInfo.InvariantCulture);
            var userEmail = string.Empty;
            var headers = new HeaderManager(Request);
            urNoticeSession session = new SessionService().CheckAndValidateSession(headers, authKey, accessKey, secretKey);

            if (session != null)
                userEmail = session.UserName;

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
                    var getUserPostResponse = new UserService().GetUserPost(vertexId, from, to, userEmail);
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

        public JsonResult GetUserPostMessages()
        {
            var from = Request.QueryString["from"].ToString(CultureInfo.InvariantCulture);
            var to = Request.QueryString["to"].ToString(CultureInfo.InvariantCulture);
            var vertexId = Request.QueryString["vertexId"].ToString(CultureInfo.InvariantCulture);
            var userEmail = string.Empty;
            var headers = new HeaderManager(Request);
            urNoticeSession session = new SessionService().CheckAndValidateSession(headers, authKey, accessKey, secretKey);

            if (session != null)
                userEmail = session.UserName;

            var isValidToken = TokenManager.IsValidSession(headers.AuthToken);
            isValidToken = true;//TODO: currently hard coded.
            if (isValidToken)
            {
                
                if (!String.IsNullOrWhiteSpace(vertexId) && !vertexId.Equals("undefined"))
                {
                    var getUserPostMessagesResponse = new UserService().GetUserPostMessages(vertexId, from, to);
                    var getUserPostMessagesResponseDeserialized =
                        JsonConvert.DeserializeObject<UserPostMessagesVertexModelResponse>(getUserPostMessagesResponse);


                    return Json(getUserPostMessagesResponseDeserialized, JsonRequestBehavior.AllowGet);
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

        public JsonResult GetUserPostLikes()
        {
            var from = Request.QueryString["from"].ToString(CultureInfo.InvariantCulture);
            var to = Request.QueryString["to"].ToString(CultureInfo.InvariantCulture);
            var vertexId = Request.QueryString["vertexId"].ToString(CultureInfo.InvariantCulture);
            var userEmail = string.Empty;
            var headers = new HeaderManager(Request);
            urNoticeSession session = new SessionService().CheckAndValidateSession(headers, authKey, accessKey, secretKey);

            if (session != null)
                userEmail = session.UserName;

            var isValidToken = TokenManager.IsValidSession(headers.AuthToken);
            isValidToken = true;//TODO: currently hard coded.
            if (isValidToken)
            {

                if (!String.IsNullOrWhiteSpace(vertexId) && !vertexId.Equals("undefined"))
                {
                    var getUserPostMessagesResponse = new UserService().GetUserPostLikes(vertexId, from, to);
                    var getUserPostMessagesResponseDeserialized =
                        JsonConvert.DeserializeObject<UserPostLikesVertexModelResponse>(getUserPostMessagesResponse);


                    return Json(getUserPostMessagesResponseDeserialized, JsonRequestBehavior.AllowGet);
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

        public JsonResult GetCompanySalaryInfo()
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
                if (String.IsNullOrWhiteSpace(vertexId) || vertexId.Equals("undefined"))
                {
                    if (session != null)
                        vertexId = session.UserVertexId;
                    else
                        isRequestValid = false;
                }

                if (isRequestValid)
                {
                    IGraphDbContract graphDbContract = new GraphDbContract();
                    var getCompanySalaryInfoResponse = graphDbContract.CompanySalaryInfo(vertexId,from,to);//new UserService().GetUserPost(vertexId, from, to, accessKey, secretKey);
                    var getCompanySalaryInfoResponseDeserialized =
                        JsonConvert.DeserializeObject<CompanySalaryVertexModelResponse>(getCompanySalaryInfoResponse);
                    return Json(getCompanySalaryInfoResponseDeserialized, JsonRequestBehavior.AllowGet);
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

        public JsonResult GetCompanyNoticePeriodInfo()
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
                if (String.IsNullOrWhiteSpace(vertexId) || vertexId.Equals("undefined"))
                {
                    if (session != null)
                        vertexId = session.UserVertexId;
                    else
                        isRequestValid = false;
                }

                if (isRequestValid)
                {
                    IGraphDbContract graphDbContract = new GraphDbContract();
                    var getCompanyNoticePeriodInfoResponse = graphDbContract.CompanyNoticePeriodInfo(vertexId, from, to);//new UserService().GetUserPost(vertexId, from, to, accessKey, secretKey);
                    var getCompanyNoticePeriodInfoResponseDeserialized =
                        JsonConvert.DeserializeObject<CompanyNoticePeriodVertexModelResponse>(getCompanyNoticePeriodInfoResponse);
                    return Json(getCompanyNoticePeriodInfoResponseDeserialized, JsonRequestBehavior.AllowGet);
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

        public JsonResult GetCompanyWorkgraphyInfo()
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
                if (String.IsNullOrWhiteSpace(vertexId) || vertexId.Equals("undefined"))
                {
                    if (session != null)
                        vertexId = session.UserVertexId;
                    else
                        isRequestValid = false;
                }

                if (isRequestValid)
                {
                    IGraphDbContract graphDbContract = new GraphDbContract();
                    var getCompanyWorkgraphyInfoResponse = graphDbContract.CompanyWorkgraphyInfo(vertexId, from, to);
                    var getCompanyWorkgraphyInfoResponseDeserialized =
                        JsonConvert.DeserializeObject<CompanyWorkgraphyVertexModelResponse>(getCompanyWorkgraphyInfoResponse);
                    return Json(getCompanyWorkgraphyInfoResponseDeserialized, JsonRequestBehavior.AllowGet);
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

        public JsonResult GetPostByVertexId()
        {            
            var vertexId = Request.QueryString["vertexId"].ToString(CultureInfo.InvariantCulture);

            var headers = new HeaderManager(Request);
            urNoticeSession session = new SessionService().CheckAndValidateSession(headers, authKey, accessKey, secretKey);
            var userEmail = string.Empty;
            if (session != null)
                userEmail = session.UserName;
            var isValidToken = TokenManager.IsValidSession(headers.AuthToken);
            isValidToken = true;//TODO: currently hard coded.
            if (isValidToken)
            {
                Boolean isRequestValid = true;
                if (String.IsNullOrWhiteSpace(vertexId) || vertexId.Equals("undefined"))
                {
                    if (session != null)
                        vertexId = session.UserVertexId;
                    else
                        isRequestValid = false;
                }

                if (isRequestValid)
                {
                    var getUserPostResponse = new UserService().GetPostByVertexId(vertexId, userEmail);
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
