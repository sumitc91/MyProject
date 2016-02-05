using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
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
    }
}
