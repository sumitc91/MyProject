using System;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using urNotice.Common.Infrastructure.Common.Constants;
using urNotice.Common.Infrastructure.commonMethods;
using urNotice.Common.Infrastructure.Encryption;
using urNotice.Common.Infrastructure.Model.Person;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper;
using urNotice.Common.Infrastructure.Session;
using urNotice.Services.DataImport;
using urNotice.Services.DataImport.ImportCompanies;
using urNotice.Services.DataImport.ImportCompanyDesignationSalaries;
using urNotice.Services.DataImport.ImportDesignations;
using urNotice.Services.DataImport.ImportNoticePeriods;
using urNotice.Services.ErrorLogger;
using urNotice.Services.Person;
using urNoticeAuth.App_Start;

namespace urNoticeAuth.Controllers
{


    [AllowCrossSiteJson]
    public class AuthController : Controller
    {
        //
        // GET: /Auth/
       
        public delegate void LogoutDelegate(HttpRequestBase requestData);
        
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult CreateAccount(RegisterationRequest req)
        {
            //var returnUrl = "/";
            IPerson person = new Consumer();
            var response = person.RegisterMe(req, Request);
            return Json(response);
        }

        //[HttpPost]
        //public JsonResult Login(LoginRequest req)
        //{
        //    var returnUrl = "/";
        //    //var referral = Request.QueryString["ref"];
        //    //var isMobileFacebookLogin = Request.QueryString["isMobileFacebookLogin"];
        //    var responseData = new LoginResponse();
        //    if (req.Type == "web")
        //    {
        //        responseData = _authService.WebLogin(req.UserName, EncryptionClass.Md5Hash(req.Password), returnUrl, req.KeepMeSignedInCheckBox, accessKey, secretKey);
        //    }

        //    if (responseData.Code == "200")
        //    {
        //        var session = new urNoticeSession(req.UserName, responseData.VertexId);
        //        TokenManager.CreateSession(session);
        //        responseData.UTMZT = session.SessionId;
        //    }
        //    var response = new ResponseModel<LoginResponse> { Status = Convert.ToInt32(responseData.Code), Message = "success", Payload = responseData };
        //    return Json(response);
        //}

        [HttpPost]
        public JsonResult ValidateAccount(ValidateAccountRequest req)
        {
            IPerson consumerModel = new Consumer();
            var response = consumerModel.ValidateAccountService(req);
            return Json(response);
        }

        [HttpPost]
        public JsonResult IsValidSession()
        {
            var headers = new HeaderManager(Request);
            new TokenManager().getSessionInfo(headers.AuthToken, headers);
            var isValidToken = TokenManager.IsValidSession(headers.AuthToken);
            return Json(isValidToken);
        }

        [HttpPost]
        public JsonResult Logout()
        {
            //var isValidToken = ThreadPool.QueueUserWorkItem(new WaitCallback(asyncLogoutThread), Request);
            var logoutServiceDelegate = new LogoutDelegate(AsyncLogoutDelegate);
            logoutServiceDelegate.BeginInvoke(Request, null, null); //invoking the method
            return Json("success");
        }

        public JsonResult ForgetPassword()
        {
            var id = Request.QueryString["id"];

            IPerson consumerModel = new Consumer();
            var response = consumerModel.ForgetPasswordService(id, Request);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateDesignations()
        {
            IImportDesignations importDesignationsModel = new ImportDesinationsFromCsv();
            bool response = false;
            //response = importDesignationsModel.ImportAllDesignations();
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateCompanyDesignationSalaries()
        {
            IImportCompanyDesignationSalaries importCompanyDesignationSalariesModel = new ImportCompanyDesignationSalariesFromCsv();
            bool response = false;
            //response = importCompanyDesignationSalariesModel.ImportCompanyDesignationAllSalaries();
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateCompanyDesignationNoticePeriod()
        {
            IImportNoticePeriods importNoticePeriodsModel = new ImportNoticePeriods();
            bool response = false;
            response = importNoticePeriodsModel.ImportCompanyAllDesignationNoticePeriods();
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateCompaniesFromCsv()
        {
            IImportCompanies importCompaniesModel = new ImportCompaniesFromCsv();
            string serverMapPath = @Server.MapPath("~/Downloads/");
            bool response = false;
            //response = importCompaniesModel.ImportAllCompanies(serverMapPath);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ContactUs(ContactUsRequest req)
        {
            IPerson consumerModel = new Consumer();
            var response = consumerModel.ContactUsService(req);
            return Json(response);
        }

        public void AsyncLogoutDelegate(HttpRequestBase requestData)
        {
            var headers = new HeaderManager(requestData);
            urNoticeSession session = TokenManager.getLogoutSessionInfo(headers.AuthToken);
            //if (session != null)
            //{
            //    var user = _db.Users.SingleOrDefault(x => x.username == session.UserName);
            //    if (user != null) user.keepMeSignedIn = "false";
            //    try
            //    {
            //        _db.SaveChanges();
            //    }
            //    catch (DbEntityValidationException e)
            //    {
            //        DbContextException.LogDbContextException(e);
            //    }
            //}
            new TokenManager().Logout(headers.AuthToken);
        }


        //[HttpPost]
        //public JsonResult ResendValidationCode(ValidateAccountRequest req)
        //{
        //    IPerson consumerModel = new Consumer();
        //    var response = consumerModel.ResendValidationCodeService(req, Request);
        //    return Json(response);
        //}

        //swagger done
        [HttpPost]
        public JsonResult ResetPassword(ResetPasswordRequest req)
        {
            IPerson consumerModel = new Consumer();
            var response = consumerModel.ResetPasswordService(req);
            return Json(response);
        }
    }
}
