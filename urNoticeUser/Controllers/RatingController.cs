using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper;
using urNotice.Common.Infrastructure.Model.urNoticeRatingContext;
using urNotice.Common.Infrastructure.Session;
using urNotice.Services.RatingService;

namespace urNoticeUser.Controllers
{
    public class RatingController : Controller
    {
        //
        // GET: /Rating/

        private static string accessKey = ConfigurationManager.AppSettings["AWSAccessKey"];
        private static string secretKey = ConfigurationManager.AppSettings["AWSSecretKey"];

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetUserRatingStatus()
        {
            var cid = Request.QueryString["cid"];

            

            return Json("Not Implemented", JsonRequestBehavior.AllowGet);
        }
    }
}
