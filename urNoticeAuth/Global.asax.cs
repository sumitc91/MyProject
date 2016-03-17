﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using SolrNet;
using urNotice.Common.Infrastructure.Model.urNoticeModel.Solr;

namespace urNoticeAuth
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            
            Startup.Init<UnCompanySolr>(ConfigurationManager.AppSettings["unCompanySolr"]);
            Startup.Init<UnDesignationSolr>(ConfigurationManager.AppSettings["UnDesignationSolr"]);
            Startup.Init<UnUserSolr>(ConfigurationManager.AppSettings["UnUserSolr"]);
            Startup.Init<UnVirtualFriendSolr>(ConfigurationManager.AppSettings["UnVirtualFriendSolr"]);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");

            //HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache");

            if (HttpContext.Current.Request.HttpMethod == "OPTIONS")
            {
                //HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache");
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept, tokenId,UTMZT,UTMZK,UTMZV,_ga");
                HttpContext.Current.Response.AddHeader("Access-Control-Max-Age", "1728000");
                HttpContext.Current.Response.End();
            }
        }
    }
}