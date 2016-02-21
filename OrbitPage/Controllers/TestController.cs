using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using OrbitPage.Hubs;

namespace OrbitPage.Controllers
{
    public class TestController : Controller
    {
        //
        // GET: /Test/

        public ActionResult Index()
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            context.Clients.All.receiveMessage("Sumit", "hello everyone");
            //new ChatHub().BroadCastMessage("Sumit","hello everyone");
            return View();
        }

    }
}
