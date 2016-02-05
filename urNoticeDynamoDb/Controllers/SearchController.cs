using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using urNoticeDynamoDb.Models;
using urNoticeDynamoDb.Service;

namespace urNoticeDynamoDb.Controllers
{
    public class SearchController : Controller
    {
        //
        // GET: /Search/

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetDetails()
        {
            //new MidlevelItemCRUD().MainMethod();
            //new HighLevelQueryAndScan().MainMethod();
            new UserAnalyticsService().MainMethod();
            return Json("", JsonRequestBehavior.AllowGet);
        }
    }
}
