using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using MonteCarloSim.DAL;
using System.Data.Entity.Infrastructure.Interception;
namespace MonteCarloSim
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Added for Logging
            DbInterception.Add(new OptionInterceptorTransientErrors());
            DbInterception.Add(new OptionInterceptorLogging());
        }
    }
}
