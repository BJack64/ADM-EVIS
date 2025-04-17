using eFakturADM.Logic.Collections;
using eFakturADM.Logic.Core;
using Hangfire;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace eFakturADM.WebApi
{
    /// <summary>
    /// 
    /// </summary>
    public class WebApiApplication : HttpApplication
    {
        /// <summary>
        /// 
        /// </summary>
        protected void Application_Start()
        {
            //log4net.Config.XmlConfigurator.Configure();
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(Server.MapPath("~/web.config")));

            //// Menginisialisasi Hangfire Dashboard dan Server
            //RouteTable.Routes.MapHttpRoute(
            //    name: "Hangfire",
            //    routeTemplate: "hangfire",
            //    defaults: new { controller = "Hangfire", action = "Dashboard" }
            //);

            AreaRegistration.RegisterAllAreas();
            System.Web.Http.GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            MappingConfig.RegisterMapping();

            // Max Retry Antrian Hangfire jika gagal
            //GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute
            //{
            //    Attempts = 0, // Menonaktifkan retry
            //    OnAttemptsExceeded = AttemptsExceededAction.Delete // Menghapus job setelah gagal sejumlah percobaan
            //});
        }
    }
}
