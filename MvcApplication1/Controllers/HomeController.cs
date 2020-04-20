using MvcApplication1.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcApplication1.Global;


namespace MvcApplication1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string PageName)
        {
            if (String.IsNullOrEmpty(PageName))
            {
                return View("Index");
            }
            else
            {
                if (string.Compare(PageName, "Index", true) == 0)
                {
                    return View("Index");
                }
                else
                {
                    SqlRepository repository = new SqlRepository();

                    Page pageModel = repository.GetPage(PageName);

                    if (pageModel != null)
                    {                       
                        return View("Page", pageModel);
                    }
                    else
                    {
                        Response.StatusCode = 404;
                        return View ("Error");
                    }
                }
            }
        }


        public ActionResult NavMenuPartial()
        {
            SqlRepository repository = new SqlRepository();

            //now we need only 1-level menu. perhaps we will change it later
            //TODO
            // n-level menu
            
            List<NavMenuItem> model = repository.GetNavMenu();            

            return PartialView("_NavMenuPartial", model);  
        }
       

        public ActionResult Test()
        {
            ViewBag.Message = "My test text";

            return View();
        }

    }
}