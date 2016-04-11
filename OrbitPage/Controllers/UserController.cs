using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using OrbitPage.Hubs;
using urNotice.Common.Infrastructure.Common.Constants;
using urNotice.Common.Infrastructure.Model.Person;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper;
using urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper.EditProfile;
using urNotice.Common.Infrastructure.Model.urNoticeModel.User;
using urNotice.Common.Infrastructure.Session;
using urNotice.Services.Person;
using urNotice.Services.SessionService;
using urNotice.Services.UserService;

namespace OrbitPage.Controllers
{
    public class UserController : Controller
    {
        private static string accessKey = ConfigurationManager.AppSettings["AWSAccessKey"];
        private static string secretKey = ConfigurationManager.AppSettings["AWSSecretKey"];
        private static string authKey = ConfigurationManager.AppSettings["AuthKey"];

        [System.Web.Mvc.HttpPost]
        public JsonResult UserPost(UserNewPostRequest userPostData)
        {
            var message = userPostData.Message;//Request.QueryString["message"].ToString(CultureInfo.InvariantCulture);
            var image = userPostData.Image;//Request.QueryString["image"].ToString(CultureInfo.InvariantCulture);
            var userWallVertexId = userPostData.VertexId;//Request.QueryString["vertexId"].ToString(CultureInfo.InvariantCulture);
            var headers = new HeaderManager(Request);
            urNoticeSession session = new SessionService().CheckAndValidateSession(headers, authKey, accessKey, secretKey);

            var isValidToken = TokenManager.IsValidSession(headers.AuthToken);
            if (isValidToken)
            {
                if (String.IsNullOrWhiteSpace(image) || image == CommonConstants.undefined)
                {
                    image = String.Empty;
                }

                Dictionary<string, string> sendNotificationResponse = null;
                IPerson clientModel = new Consumer();
                var newUserPostResponse = clientModel.CreateNewUserPost(session, message, image, userWallVertexId, out sendNotificationResponse);
                if (sendNotificationResponse.ContainsKey(CommonConstants.PushNotificationArray))
                {
                    new SignalRNotification().SendNewPostNotification(sendNotificationResponse.FirstOrDefault(x => x.Key == CommonConstants.PushNotificationArray).Value);                     
                }
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

        [System.Web.Mvc.HttpPost]
        public JsonResult UserCommentOnPost(UserNewCommentOnPostRequest userNewCommentOnPostRequest)
        {
            var message = userNewCommentOnPostRequest.Message;//Request.QueryString["message"].ToString(CultureInfo.InvariantCulture);
            var image = userNewCommentOnPostRequest.Image;//Request.QueryString["image"].ToString(CultureInfo.InvariantCulture);
            var postVertexId = userNewCommentOnPostRequest.VertexId;//Request.QueryString["vertexId"].ToString(CultureInfo.InvariantCulture);

            var userWallVertexId = userNewCommentOnPostRequest.WallVertexId;//Request.QueryString["wallVertexId"].ToString(CultureInfo.InvariantCulture);
            var postPostedByVertexId = userNewCommentOnPostRequest.PostPostedByVertexId;//Request.QueryString["postPostedByVertexId"].ToString(CultureInfo.InvariantCulture);

            var headers = new HeaderManager(Request);
            urNoticeSession session = new SessionService().CheckAndValidateSession(headers, authKey, accessKey, secretKey);

            var isValidToken = TokenManager.IsValidSession(headers.AuthToken);
            if (isValidToken)
            {
                if (String.IsNullOrWhiteSpace(image) || image == CommonConstants.undefined)
                {
                    image = String.Empty;
                }
                Dictionary<string, string> sendNotificationResponse = null;
                var newUserPostCommentResponse = new UserService().CreateNewCommentOnUserPost(session, message, image, postVertexId, userWallVertexId, postPostedByVertexId, out sendNotificationResponse);
                if (sendNotificationResponse.ContainsKey(CommonConstants.PushNotificationArray))
                {
                    new SignalRNotification().SendNewPostNotification(sendNotificationResponse.FirstOrDefault(x => x.Key == CommonConstants.PushNotificationArray).Value);
                }
                return Json(newUserPostCommentResponse, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var response = new ResponseModel<string>();
                response.Status = 401;
                response.Message = "Unauthorized";
                return Json(response, JsonRequestBehavior.AllowGet);
            }

        }

        [System.Web.Mvc.HttpPost]
        public JsonResult UserReactionOnPost(UserNewReactionRequest userNewReactionRequest)
        {
            var reaction = userNewReactionRequest.Reaction;

            var vertexId = userNewReactionRequest.VertexId;

            var userWallVertexId = userNewReactionRequest.WallVertexId;
            var postPostedByVertexId = userNewReactionRequest.PostPostedByVertexId;

            var headers = new HeaderManager(Request);
            urNoticeSession session = new SessionService().CheckAndValidateSession(headers, authKey, accessKey, secretKey);

            var isValidToken = TokenManager.IsValidSession(headers.AuthToken);
            if (isValidToken)
            {                
                Dictionary<string, string> sendNotificationResponse = null;
                var newUserPostCommentResponse = new UserService().CreateNewReactionOnUserPost(session, reaction, vertexId, userWallVertexId, postPostedByVertexId, out sendNotificationResponse);
                if (sendNotificationResponse.ContainsKey(CommonConstants.PushNotificationArray))
                {
                    new SignalRNotification().SendNewPostNotification(sendNotificationResponse.FirstOrDefault(x => x.Key == CommonConstants.PushNotificationArray).Value);
                }
                return Json(newUserPostCommentResponse, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var response = new ResponseModel<string>();
                response.Status = 401;
                response.Message = "Unauthorized";
                return Json(response, JsonRequestBehavior.AllowGet);
            }

        }
        

        [System.Web.Mvc.HttpPost]
        public JsonResult EditPersonDetails(EditPersonModel editPersonModel)
        {
            
            var headers = new HeaderManager(Request);
            urNoticeSession session = new SessionService().CheckAndValidateSession(headers, authKey, accessKey, secretKey);

            var isValidToken = TokenManager.IsValidSession(headers.AuthToken);
            if (isValidToken && session != null)
            {
                
                IPerson personModel = new Consumer();
                var response = personModel.EditPersonDetails(session,editPersonModel);

                return Json(response, JsonRequestBehavior.AllowGet);
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
