using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace adminlte.Controllers
{
    public class IndexController : BaseController
    {
        // GET: Index
        public ActionResult Index()
        {
            //List<AXFWebMenuEntity> ltAXFWebMenuLic = new List<AXFWebMenuEntity>();

            return View();
        }
    }
}