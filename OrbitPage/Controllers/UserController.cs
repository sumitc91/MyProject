﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using OrbitPage.Hubs;
using urNotice.Common.Infrastructure.Common.Config;
using urNotice.Common.Infrastructure.Common.Constants;
using urNotice.Common.Infrastructure.Model.Person;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper;
using urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper.EditProfile;
using urNotice.Common.Infrastructure.Model.urNoticeModel.User;
using urNotice.Common.Infrastructure.Session;
using urNotice.Services.Person;
using urNotice.Services.SessionService;

namespace OrbitPage.Controllers
{
    public class UserController : Controller
    {
        private static string accessKey = AwsConfig._awsAccessKey;
        private static string secretKey = AwsConfig._awsSecretKey;
        private static string authKey = OrbitPageConfig.AuthKey;

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

                HashSet<string> sendNotificationHashSetResponse = null;
                IPerson clientModel = new Consumer();
                var newUserPostResponse = clientModel.CreateNewUserPost(session, message, image, userWallVertexId, out sendNotificationHashSetResponse);
                if (sendNotificationHashSetResponse.Count > 0)
                {
                    new SignalRNotification().SendNewPostNotification(sendNotificationHashSetResponse);                     
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
            var message = userNewCommentOnPostRequest.Message;
            var image = userNewCommentOnPostRequest.Image;
            var postVertexId = userNewCommentOnPostRequest.VertexId;

            var userWallVertexId = userNewCommentOnPostRequest.WallVertexId;
            var postPostedByVertexId = userNewCommentOnPostRequest.PostPostedByVertexId;

            var headers = new HeaderManager(Request);
            urNoticeSession session = new SessionService().CheckAndValidateSession(headers, authKey, accessKey, secretKey);

            var isValidToken = TokenManager.IsValidSession(headers.AuthToken);
            if (isValidToken)
            {
                if (String.IsNullOrWhiteSpace(image) || image == CommonConstants.undefined)
                {
                    image = String.Empty;
                }
                HashSet<string> sendNotificationHashSetResponse = null;

                IPerson clientModel = new Consumer();
                var newUserPostCommentResponse = clientModel.CreateNewCommentOnUserPost(session, message, image, postVertexId, userWallVertexId, postPostedByVertexId, out sendNotificationHashSetResponse);
                
                if (sendNotificationHashSetResponse.Count>0)
                {
                    new SignalRNotification().SendNewPostNotification(sendNotificationHashSetResponse);
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
            

            var headers = new HeaderManager(Request);
            urNoticeSession session = new SessionService().CheckAndValidateSession(headers, authKey, accessKey, secretKey);

            var isValidToken = TokenManager.IsValidSession(headers.AuthToken);
            if (isValidToken)
            {                
                HashSet<string> sendNotificationHashSetResponse = null;
                IPerson clientModel = new Consumer();
                var newUserPostCommentResponse = clientModel.CreateNewReactionOnUserPost(session, userNewReactionRequest, out sendNotificationHashSetResponse);
                if (sendNotificationHashSetResponse.Count>0)
                {
                    new SignalRNotification().SendNewPostNotification(sendNotificationHashSetResponse);
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
        public JsonResult RemoveReactionOnPost()
        {
            var vertexId = Request.QueryString["vertexId"].ToString(CultureInfo.InvariantCulture);
            
            var headers = new HeaderManager(Request);
            urNoticeSession session = new SessionService().CheckAndValidateSession(headers, authKey, accessKey, secretKey);

            var isValidToken = TokenManager.IsValidSession(headers.AuthToken);
            if (isValidToken)
            {
                IPerson clientModel = new Consumer();
                var newUserPostCommentResponse = clientModel.RemoveReactionOnUserPost(session, vertexId);                
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

        [System.Web.Mvc.HttpPost]
        public JsonResult EditMessageDetails(EditMessageRequest messageReq)
        {

            var headers = new HeaderManager(Request);
            urNoticeSession session = new SessionService().CheckAndValidateSession(headers, authKey, accessKey, secretKey);

            var isValidToken = TokenManager.IsValidSession(headers.AuthToken);
            if (isValidToken && session != null)
            {

                IPerson personModel = new Consumer();
                if (string.IsNullOrEmpty(messageReq.imageUrl))
                    messageReq.deletePreviousImage = true;
                var response = personModel.EditMessageDetails(session, messageReq);

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

        public JsonResult DeleteCommentOnPost()
        {
            var vertexId = Request.QueryString["vertexId"].ToString(CultureInfo.InvariantCulture);

            var headers = new HeaderManager(Request);
            urNoticeSession session = new SessionService().CheckAndValidateSession(headers, authKey, accessKey, secretKey);

            var isValidToken = TokenManager.IsValidSession(headers.AuthToken);
            if (isValidToken)
            {
                IPerson clientModel = new Consumer();
                var newUserPostCommentResponse = clientModel.DeleteCommentOnPost(session, vertexId);
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
    }
}
