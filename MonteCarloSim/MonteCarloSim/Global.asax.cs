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
            // Can be added in the OptionConfiguration, but only be added once as it will get two logs for every SQL query
            DbInterception.Add(new OptionInterceptorTransientErrors());
            DbInterception.Add(new OptionInterceptorLogging());

            // These lines of code are what causes the interceptor code to be run when EF sends queries to the database. 
            // Created separate interceptor classes for transient error simulation and logging, so can independently enable and disable them.
        }
    }
}
