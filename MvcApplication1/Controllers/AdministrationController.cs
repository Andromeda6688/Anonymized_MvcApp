using MvcApplication1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcApplication1.Global;
using System.Text.RegularExpressions;

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
        public ActionResult Page(string Id, PageEditVM model)
        {
            AmendPageVM(model);
            
            if (ModelState.IsValid &&
                    !string.IsNullOrEmpty(model.Page.Title) &&
                    !string.IsNullOrEmpty(model.Page.Address))
            {

                if (!(string.Compare(model.Page.Address, "Index", true) == 0
                    ^ model.Page.ParentId == 0))
                {
                    if (string.IsNullOrEmpty(Id))//new
                    {

                        SqlRepository repository = new SqlRepository();
                        int newPageId = repository.CreatePage(model.Page);
                        ViewBag.Message = "Страница добавлена";

                        if (newPageId!=0)
                        {
                            return RedirectToAction("Page", "Administration", new { Id = newPageId });
                        }
                        else
                        {
                            ModelState.AddModelError("", "Ошибка. Возможно, адрес страницы не уникальный");
                        }
                    }

                    else // modifying existing page
                    {
                        model.Page.Id = Convert.ToInt32(Id);
                        SqlRepository repository = new SqlRepository();
                        bool result = repository.UpdatePage(model.Page);

                        if (result)
                        {
                            ViewBag.Message = "Изменения успешно сохранены";
                        }
                        else
                        {
                            ModelState.AddModelError("", "Ошибка. Возможно, адрес страницы не уникальный");
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Может быть только одна страница с адресом 'Index'");
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

        void AmendPageVM(PageEditVM model)
        {
            if (!string.IsNullOrEmpty(model.Page.Title))
            {
                 model.Page.Title.Trim();
            }
            if (!string.IsNullOrEmpty(model.Page.Description))
            {
                model.Page.Description.Trim();
            }
            if (!string.IsNullOrEmpty(model.Page.Keywords))
            {
                model.Page.Keywords.Trim();
            }
            if (!string.IsNullOrEmpty(model.Page.Address))
            {
                model.Page.Address.Trim();
            }

            //model.Page.Address = Regex.Replace(model.Page.Address, @"[^A-Za-z0-9$]", "", RegexOptions.ECMAScript);


        }

    }
}
