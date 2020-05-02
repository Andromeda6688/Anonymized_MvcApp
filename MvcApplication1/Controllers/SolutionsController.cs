using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication1.Controllers
{
    //reserved for more complex logics
    public class SolutionsController : Controller
    {
        //
        // GET: /Solutions/
        
        public ActionResult Index()
        {
            return View();
        }
    }
}