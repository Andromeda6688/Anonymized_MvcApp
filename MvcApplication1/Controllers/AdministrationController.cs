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

        
        public ActionResult Page(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                SqlRepository repository = new SqlRepository();

                List<PagesMenuItem> model = repository.GetPagesList();

                return View("PageList", model);
            }
            else
            {
                int pageID = Convert.ToInt32(Id);

                SqlRepository repository = new SqlRepository();

                Page model = repository.GetPage(pageID);

                return View("PageEdit", model);
            }

        }

        [HttpPost]
        public ActionResult Page(Page model)
        {
            if (ModelState.IsValid &&
                !string.IsNullOrEmpty(model.Title) &&
                !string.IsNullOrEmpty(model.Address) 
                )
            {
                SqlRepository repository = new SqlRepository();
                repository.UpdatePage(model);
            }
            else
            {
                ModelState.AddModelError("", "Ошибка редактирования");
            }
            ViewBag.Message = "Изменения успешно сохранены";
            return View("PageEdit", model);

            
        }





        public ActionResult AdminMenuPartial()
        {
            List<NavMenuItem> model = new List<NavMenuItem>()
            { new NavMenuItem() { Title = "Страницы", Address = "~/Admin/Page" },
              new NavMenuItem() { Title = "Пользователи", Address = "~/Admin/User"}
            };

            return PartialView("_AdminMenuPartial", model);
        }

    }
}
