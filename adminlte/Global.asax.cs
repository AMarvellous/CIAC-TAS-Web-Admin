using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace adminlte
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        //protected void Application_BeginRequest(Object sender, EventArgs e)
        //{
        //    System.Globalization.CultureInfo newCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
        //    newCulture.DateTimeFormat.ShortDatePattern = "dd/mm/yyyy";
        //    newCulture.DateTimeFormat.DateSeparator = "/";
        //    System.Threading.Thread.CurrentThread.CurrentCulture = newCulture;
        //}
    }
}
