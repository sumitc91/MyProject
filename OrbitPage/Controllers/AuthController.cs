using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using urNotice.Common.Infrastructure.Encryption;
using urNotice.Common.Infrastructure.Model.Person;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper;
using urNotice.Common.Infrastructure.Session;
using System.Web.Mvc;
using urNotice.Services.Person;
using urNotice.Services.Person.PersonContract.LoginOperation;

namespace OrbitPage.Controllers
{
    public class AuthController : Controller
    {
        
        [System.Web.Mvc.HttpPost]
        public JsonResult Login(LoginRequest req)
        {
            var returnUrl = "/";
            //var referral = Request.QueryString["ref"];
            //var isMobileFacebookLogin = Request.QueryString["isMobileFacebookLogin"];
            var response = new ResponseModel<LoginResponse>();
            if (req.Type == "web")
            {
                IOrbitPageLogin loginModel = new OrbitPageLogin();
                response = loginModel.Login(req.UserName, EncryptionClass.Md5Hash(req.Password), returnUrl, req.KeepMeSignedInCheckBox, false);
            }

            if (response.Payload.Code == "200")
            {
                string displayName = response.Payload.FirstName + " " + response.Payload.LastName; 
                var session = new urNoticeSession(req.UserName,displayName, response.Payload.VertexId,response.Payload.imageUrl);
                TokenManager.CreateSession(session);
                response.Payload.UTMZT = session.SessionId;
            }
            
            return Json(response);
        }

        [System.Web.Mvc.HttpPost]
        public JsonResult CreateAccount(RegisterationRequest req)
        {
            //var returnUrl = "/";
            IPerson person = new Consumer();
            var response = person.RegisterMe(req, Request);
            return Json(response);
        }

        [System.Web.Mvc.HttpPost]
        public JsonResult ResendValidationCode(ValidateAccountRequest req)
        {
            IPerson consumerModel = new Consumer();
            var response = consumerModel.ResendValidationCodeService(req, Request);
            return Json(response);
        }
    }
}
