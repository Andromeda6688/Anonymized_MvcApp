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
            return Redirect("Admin/PageList");
        }

        public ActionResult PageList(string Message)
        {
            SqlRepository repository = new SqlRepository();

            List<PagesListItem> model = repository.GetPagesList();

            ViewBag.Message = Message;

            return View("PageList", model);
        }

        public ActionResult Page(string Id, string Message)
        {
            Page _page;

            if (string.IsNullOrEmpty(Id))//new
            {
                 _page = new Page();
            }
            else
            {
                int pageID = Convert.ToInt32(Id);

                SqlRepository repository = new SqlRepository();

                _page = repository.GetPage(pageID);                

                ViewBag.Message = Message;
                
            }

            PageEditVM _VM = new PageEditVM(_page);

            return View("PageEdit", _VM);
        }

        [HttpPost]
        public ActionResult Page(string Id, Page model)
        {
            
            if (ModelState.IsValid &&
                    !string.IsNullOrEmpty(model.Title) &&
                    !string.IsNullOrEmpty(model.Address))
                {

                    if (string.IsNullOrEmpty(Id))//new
                    {
                        
                        SqlRepository repository = new SqlRepository();
                        int newPageId = repository.CreatePage(model);
                        ViewBag.Message = "Страница добавлена";

                        return RedirectToAction("Page", "Administration", new { Id = newPageId });
                       
                    }

                    else // modifying existing page
                    {
                        //model.Id = Convert.ToInt32(Id);
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

        public ActionResult DeletePage(bool confirm, int pageId)
        {
            SqlRepository repository = new SqlRepository();

            if (confirm)
            {
                bool result = repository.RemovePage(pageId);

                if (result) //deleted
                {
                    return RedirectToAction("PageList", "Administration", new { Message = "Страница успешно удалена" });
                }
                else //error
                {
                    return RedirectToAction("Page", "Administration", new { Id = pageId, Message = "Нельзя удалить страницу, которая отображается на сайте. Сначала ее нужно скрыть." });
                }
            }

            return RedirectToAction("Page", "Administration", new { Id = pageId });                       
        }






        public ActionResult AdminMenuPartial()
        {
            List<NavMenuItem> model = new List<NavMenuItem>()
            { new NavMenuItem() { Title = "Страницы", Address = "Admin/PageList" },
              new NavMenuItem() { Title = "Пользователи", Address = "Admin/User"}
            };

            return PartialView("_AdminMenuPartial", model);
        }

    }
}
