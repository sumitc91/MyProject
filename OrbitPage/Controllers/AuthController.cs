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
            var responseData = new LoginResponse();
            if (req.Type == "web")
            {
                responseData = new AuthService().WebLogin(req.UserName, EncryptionClass.Md5Hash(req.Password), returnUrl, req.KeepMeSignedInCheckBox, accessKey, secretKey);
            }

            if (responseData.Code == "200")
            {
                var session = new urNoticeSession(req.UserName, responseData.VertexId);
                TokenManager.CreateSession(session);
                responseData.UTMZT = session.SessionId;
            }
            var response = new ResponseModel<LoginResponse> { Status = Convert.ToInt32(responseData.Code), Message = "success", Payload = responseData };
            return Json(response);
        }
    }
}
