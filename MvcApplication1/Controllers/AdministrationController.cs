using MvcApplication1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcApplication1.Global;

namespace MvcApplication1.Controllers
{
    public class AdministrationController : Controller
    {
        //
        // GET: /Administration/

        public ActionResult Index()
        {
            return Redirect("Admin/Page");
        }


        public ActionResult Page(string pageId)
        {
            if (string.IsNullOrEmpty(pageId))
            {
                SqlRepository repository = new SqlRepository();

                List<PagesMenuItem> model = repository.GetPagesList();

                return View("PageList", model);
            }
            else if (string.Compare(pageId, "new", true) == 0)
            {
                Page model = new Page();

                return View("PageEdit", model);
            }
            else
            {
                int pageID = Convert.ToInt32(pageId);

                SqlRepository repository = new SqlRepository();

                Page model = repository.GetPage(pageID);

                return View("PageEdit", model);
            }

        }

        [HttpPost]
        public ActionResult Page(string pageId, Page model)
        {
            
            if (ModelState.IsValid &&
                    !string.IsNullOrEmpty(model.Title) &&
                    !string.IsNullOrEmpty(model.Address))
                {

                    if (string.Compare(pageId, "new", true) == 0)
                    {
                        
                        SqlRepository repository = new SqlRepository();
                        int newPageId = repository.CreatePage(model);
                        ViewBag.Message = "Страница добавлена";

                        return RedirectToAction("Page", "Administration", new { pageId = newPageId });
                       
                    }

                    else // modifying existing page
                    {
                        SqlRepository repository = new SqlRepository();
                        repository.UpdatePage(model);
                        ViewBag.Message = "Изменения успешно сохранены";
                    }
            }

            else
            {
                ModelState.AddModelError("", "Изменения не сохранены");
            }
            
            return View("PageEdit", model);

            
        }





        public ActionResult AdminMenuPartial()
        {
            List<NavMenuItem> model = new List<NavMenuItem>()
            { new NavMenuItem() { Title = "Страницы", Address = "Admin/Page" },
              new NavMenuItem() { Title = "Пользователи", Address = "Admin/User"}
            };

            return PartialView("_AdminMenuPartial", model);
        }

    }
}
