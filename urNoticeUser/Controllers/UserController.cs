using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using urNotice.Common.Infrastructure.Common.Config;
using urNotice.Common.Infrastructure.Common.Constants;
using urNotice.Common.Infrastructure.commonMethods;
using urNotice.Common.Infrastructure.Model.Person;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.GraphModel;
using urNotice.Common.Infrastructure.Model.urNoticeModel.GraphModel.V1;
using urNotice.Common.Infrastructure.Model.urNoticeModel.User;
using urNotice.Common.Infrastructure.Session;
using urNotice.Services.ErrorLogger;
using urNotice.Services.GraphDb.GraphDbContract;
using urNotice.Services.Person;
using urNotice.Services.SessionService;

namespace urNoticeUser.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /User/
        private static string accessKey = AwsConfig._awsAccessKey;
        private static string secretKey = AwsConfig._awsSecretKey;
        private static string authKey = OrbitPageConfig.AuthKey;

        private ILogger _logger = new Logger(Convert.ToString(MethodBase.GetCurrentMethod().DeclaringType));

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
                try
                {
                    IPerson clientModel = new Consumer();
                    var clientNotificationDetailResponse = clientModel.GetUserNotification(session, from, to);
                    var clientNotificationDetailResponseDeserialized =
                            JsonConvert.DeserializeObject<UserNotificationVertexV1ModelResponse>(clientNotificationDetailResponse);
                    UserNotificationVertexModelResponse response = ModelAdapterUtil.GetUserNotificationVertexModelResponse(clientNotificationDetailResponseDeserialized);

                    //TODO: need to write gremlin query.
                    //if (response != null)
                    //    response.unread = clientModel.GetUserUnreadNotificationCount(session);
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    _logger.Error("Graph Db Exception", ex);
                    return Json("Exception Occured.", JsonRequestBehavior.AllowGet);
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

        public JsonResult GetFriendRequestNotificationDetails()
        {
            var headers = new HeaderManager(Request);
            var from = Request.QueryString["from"].ToString(CultureInfo.InvariantCulture);
            var to = Request.QueryString["to"].ToString(CultureInfo.InvariantCulture);

            urNoticeSession session = new SessionService().CheckAndValidateSession(headers, authKey, accessKey, secretKey);

            var isValidToken = TokenManager.IsValidSession(headers.AuthToken);
            if (isValidToken)
            {
                try
                {
                    IPerson clientModel = new Consumer();
                    var clientFriendRequestNotificationDetailResponse = clientModel.GetUserFriendRequestNotification(session, from, to);
                    var clientFriendRequestNotificationDetailResponseDeserialized =
                            JsonConvert.DeserializeObject<UserFriendRequestNotificationVertexV1ModelResponse>(clientFriendRequestNotificationDetailResponse);
                    UserFriendRequestNotificationVertexModelResponse response = ModelAdapterUtil.GetUserFriendRequestNotificationVertexModelResponse(clientFriendRequestNotificationDetailResponseDeserialized);


                    //if (response != null)
                    //    response.unread = clientModel.GetUserUnreadFriendRequestNotificationCount(session);
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    _logger.Error("Graph Db Exception", ex);
                    return Json("Exception Occured.", JsonRequestBehavior.AllowGet);
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

        public JsonResult SeenNotification()
        {
            var headers = new HeaderManager(Request);            
            urNoticeSession session = new SessionService().CheckAndValidateSession(headers, authKey, accessKey, secretKey);

            var isValidToken = TokenManager.IsValidSession(headers.AuthToken);
            if (isValidToken)
            {
                IPerson consumerModel = new Consumer();
                var seenNotificationResponse = consumerModel.SeenNotification(session.UserName);
                return Json(seenNotificationResponse, JsonRequestBehavior.AllowGet);
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
                    IPerson clientModel = new Consumer();
                    var getUserPostResponse = clientModel.GetUserPost(vertexId, from, to, userEmail);
                    var getUserPostResponseDeserialized =
                        JsonConvert.DeserializeObject<UserPostVertexModelV1Response>(getUserPostResponse);

                    //var getCompanySalaryInfoResponseDeserialized =
                    //    JsonConvert.DeserializeObject<CompanySalaryVertexModelV1Response>(getCompanySalaryInfoResponse);

                    UserPostVertexModelResponse response = ModelAdapterUtil.GetUserPostInfoResponse(getUserPostResponseDeserialized);
                    return Json(response, JsonRequestBehavior.AllowGet);
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

        public JsonResult GetUserOrbitFeedPost()
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
                if (String.IsNullOrWhiteSpace(vertexId) || vertexId.Equals("undefined"))
                {
                    if (session != null)
                        vertexId = session.UserVertexId;
                    else
                        isRequestValid = false;
                }

                if (isRequestValid)
                {
                    IPerson clientModel = new Consumer();
                    var getUserPostResponse = clientModel.GetUserOrbitFeedPost(vertexId, from, to, userEmail);
                    var getUserPostResponseDeserialized =
                        JsonConvert.DeserializeObject<UserPostVertexModelV1Response>(getUserPostResponse);

                    UserPostVertexModelResponse response = ModelAdapterUtil.GetUserPostInfoResponse(getUserPostResponseDeserialized);
                    return Json(response, JsonRequestBehavior.AllowGet);
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
                    IPerson clientModel = new Consumer();
                    var getUserPostMessagesResponse = clientModel.GetUserPostMessages(vertexId, from, to, userEmail);
                    var getUserPostMessagesResponseDeserialized =
                        JsonConvert.DeserializeObject<UserPostMessagesVertexModelV1Response>(getUserPostMessagesResponse);

                    UserPostMessagesVertexModelResponse response = ModelAdapterUtil.GetUserPostMessagesVertexModelResponse(getUserPostMessagesResponseDeserialized);
                    return Json(response, JsonRequestBehavior.AllowGet);
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
                    IPerson clientModel = new Consumer();
                    var getUserPostMessagesResponse = clientModel.GetUserPostLikes(vertexId, from, to);
                    var getUserPostMessagesResponseDeserialized =
                        JsonConvert.DeserializeObject<UserPostLikesVertexModelV1Response>(getUserPostMessagesResponse);

                    UserPostLikesVertexModelResponse response = ModelAdapterUtil.GetUserPostLikesVertexModelResponse(getUserPostMessagesResponseDeserialized);
                    return Json(response, JsonRequestBehavior.AllowGet);
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
                    //IGraphDbContract graphDbContract = new GraphDbContract();
                    IGraphDbContract graphDbContractModel = new GremlinServerGraphDbContract();
                    var getCompanySalaryInfoResponse = graphDbContractModel.CompanySalaryInfo(vertexId,from,to);//new UserService().GetUserPost(vertexId, from, to, accessKey, secretKey);
                    var getCompanySalaryInfoResponseDeserialized =
                        JsonConvert.DeserializeObject<CompanySalaryVertexModelV1Response>(getCompanySalaryInfoResponse);

                    CompanySalaryVertexModelResponse response = ModelAdapterUtil.GetCompanySalaryInfoResponse(getCompanySalaryInfoResponseDeserialized);
                    return Json(response, JsonRequestBehavior.AllowGet);
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
                    //IGraphDbContract graphDbContract = new GraphDbContract();
                    //var getCompanyNoticePeriodInfoResponse = graphDbContract.CompanyNoticePeriodInfo(vertexId, from, to);//new UserService().GetUserPost(vertexId, from, to, accessKey, secretKey);
                    //var getCompanyNoticePeriodInfoResponseDeserialized =
                    //    JsonConvert.DeserializeObject<CompanyNoticePeriodVertexModelResponse>(getCompanyNoticePeriodInfoResponse);
                    //return Json(getCompanyNoticePeriodInfoResponseDeserialized, JsonRequestBehavior.AllowGet);

                    IGraphDbContract graphDbContract = new GremlinServerGraphDbContract();
                    var getCompanyNoticePeriodInfoResponse = graphDbContract.CompanyNoticePeriodInfo(vertexId, from, to);//new UserService().GetUserPost(vertexId, from, to, accessKey, secretKey);
                    
                    var getCompanyNoticePeriodInfoResponseDeserialized =
                        JsonConvert.DeserializeObject<CompanyNoticePeriodVertexModelV1Response>(getCompanyNoticePeriodInfoResponse);
                    
                    CompanyNoticePeriodVertexModelResponse response = ModelAdapterUtil.GetCompanyNoticePeriodInfoResponse(getCompanyNoticePeriodInfoResponseDeserialized);
                    return Json(response, JsonRequestBehavior.AllowGet);
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
            var username = string.Empty;
            var headers = new HeaderManager(Request);
            urNoticeSession session = new SessionService().CheckAndValidateSession(headers, authKey, accessKey, secretKey);
            
            var isValidToken = TokenManager.IsValidSession(headers.AuthToken);
            if (isValidToken)
                username = session.UserName;
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
                    //IGraphDbContract graphDbContract = new GraphDbContract();
                    IGraphDbContract graphDbContractModel = new GremlinServerGraphDbContract();
                    var getCompanyWorkgraphyInfoResponse = graphDbContractModel.CompanyWorkgraphyInfo(vertexId,username, from, to);
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
                    IPerson clientModel = new Consumer();
                    var getUserPostResponse = clientModel.GetPostByVertexId(vertexId, userEmail);
                    var getUserPostResponseDeserialized =
                        JsonConvert.DeserializeObject<UserPostVertexModelV1Response>(getUserPostResponse);
                    UserPostVertexModelResponse response = ModelAdapterUtil.GetUserPostInfoResponse(getUserPostResponseDeserialized);

                    return Json(response, JsonRequestBehavior.AllowGet);
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

        public JsonResult GetUserNetworkDetail()
        {
            var from = Request.QueryString["from"].ToString(CultureInfo.InvariantCulture);
            var to = Request.QueryString["to"].ToString(CultureInfo.InvariantCulture);
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
                    IPerson clientModel = new Consumer();
                    var getUserNetworkDetailResponse = clientModel.GetUserNetworkDetail(session,vertexId, from,to);
                    var getUserNetworkDetailResponseDeserialized =
                        JsonConvert.DeserializeObject<UserPostNetworkDetailModelV1Response>(getUserNetworkDetailResponse);
                    UserPostNetworkDetailModelResponse response = ModelAdapterUtil.UserPostNetworkDetailModelResponse(getUserNetworkDetailResponseDeserialized);


                    return Json(response, JsonRequestBehavior.AllowGet);
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
