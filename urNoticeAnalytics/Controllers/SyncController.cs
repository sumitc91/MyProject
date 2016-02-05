using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using urNoticeAnalytics.Services;

namespace urNoticeAnalytics.Controllers
{
    public class SyncController : Controller
    {
        //
        // GET: /Sync/

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult SyncGoogleApiContactList()
        {
            var id = Request.QueryString["id"];
            return Json(new SyncService().SyncGoogleApiContactList(id, Request), JsonRequestBehavior.AllowGet);
            //return Json(new DynamoDbSyncService().SyncGoogleApiContactList(id, Request), JsonRequestBehavior.AllowGet);
        }
    }
}
