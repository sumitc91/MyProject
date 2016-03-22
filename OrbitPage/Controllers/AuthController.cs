using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using urNotice.Common.Infrastructure.Encryption;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper;
using urNotice.Common.Infrastructure.Session;
using urNotice.Services.AuthService;
using System.Web.Mvc;
namespace OrbitPage.Controllers
{
    public class AuthController : Controller
    {
        private static string accessKey = ConfigurationManager.AppSettings["AWSAccessKey"];
        private static string secretKey = ConfigurationManager.AppSettings["AWSSecretKey"];

        [System.Web.Mvc.HttpPost]
        public JsonResult Login(LoginRequest req)
        {
            var returnUrl = "/";
            //var referral = Request.QueryString["ref"];
            //var isMobileFacebookLogin = Request.QueryString["isMobileFacebookLogin"];
            var response = new ResponseModel<LoginResponse>();
            if (req.Type == "web")
            {
                response = new AuthService().WebLogin(req.UserName, EncryptionClass.Md5Hash(req.Password), returnUrl, req.KeepMeSignedInCheckBox, accessKey, secretKey);
            }

            if (response.Payload.Code == "200")
            {
                var session = new urNoticeSession(req.UserName, response.Payload.VertexId);
                TokenManager.CreateSession(session);
                response.Payload.UTMZT = session.SessionId;
            }
            
            return Json(response);
        }

        [System.Web.Mvc.HttpPost]
        public JsonResult CreateAccount(RegisterationRequest req)
        {
            //var returnUrl = "/";
            return Json(new AuthService().UserRegistration(req, Request, accessKey, secretKey));
        }
    }
}
