using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using SolrNet;
using urNotice.Common.Infrastructure.Model.urNoticeModel.Solr;


namespace OrbitPage
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //RouteTable.Routes.MapHubs();
            
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            //Startup.Init<UnCompanySolr>(ConfigurationManager.AppSettings["unCompanySolr"]);
            //Startup.Init<UnDesignationSolr>(ConfigurationManager.AppSettings["UnDesignationSolr"]);
            SolrNet.Startup.Init<UnUserSolr>(ConfigurationManager.AppSettings["UnUserSolr"]);
            SolrNet.Startup.Init<UnVirtualFriendSolr>(ConfigurationManager.AppSettings["UnVirtualFriendSolr"]);
        }
    }
}