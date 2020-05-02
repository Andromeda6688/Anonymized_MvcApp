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

    [Authorize(Roles = "Author")]
    public class AdministrationController : Controller
    {
        //
        // GET: /Administration/

        public ActionResult Index()
        {
            return RedirectToAction("PageList", "Administration"); //Admin/PageList
        }

        public ActionResult PageList(string Message)
        {
            SqlRepository repository = new SqlRepository();

            List<PagesListItem> model = repository.GetPagesList();

            ViewBag.Message = Message;

            return View("PageList", model);
        }

        public ActionResult Page(string Id, string message)
        {
            Page _page;

            if (string.IsNullOrEmpty(Id))//new
            {
                 _page = new Page();

                 ViewBag.IsIndex = false;
            }
            else
            {
                int pageID = Convert.ToInt32(Id);

                SqlRepository repository = new SqlRepository();

                _page = repository.GetPage(pageID);

                ViewBag.Message = message;
            }

            if (string.Compare(_page.Address, "Index", true)==0)
            {
                return RedirectToAction("IndexEdit", "Administration"); //Admin/IndexEdit
            }
            else
            {
                PageEditVM _VM = new PageEditVM(_page);

                return View("PageEdit", _VM);
            }            
        }

        [Authorize(Roles = "Admin")]
        public ActionResult IndexEdit(string Message)
        {
            Page _page;

            SqlRepository repository = new SqlRepository();

            _page = repository.GetPage("Index", null);

            ViewBag.Message = Message;

            return View("IndexEdit", _page);           
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

                        if (newPageId!=0)
                        {
                            return RedirectToAction("Page", "Administration", new { Id = newPageId, Message = "Страница создана" });
                        }
                        else
                        {
                            ModelState.AddModelError("", "Ошибка. Возможно, адрес страницы не уникальный");
                        }
                    }

                    else // modifying existing page
                    {
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

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult IndexEdit(Page model)
        {
           if (string.IsNullOrEmpty(model.Title))
	       {
               ModelState.AddModelError("Title", "Пустой заголовок");
	       }
           else if (model.Title.Length>60)
	       {
               ModelState.AddModelError("Title", "Длина не более 60 символов");
	       }
            if (!string.IsNullOrEmpty(model.Keywords) )
	        {
                if (model.Keywords.Length<160)
	            {
		            ModelState.AddModelError("Keywords", "Длина не более 160 символов");
	            }                
	        }
            if (!string.IsNullOrEmpty(model.Description))
	        {
                if (model.Description.Length<160)
	            {
		            ModelState.AddModelError("Keywords", "Длина не более 160 символов");
	            }                
	        }

            ModelState["Address"].Errors.Clear();

            if (ModelState.IsValid)
            {
                model.Address = "Index";
                model.ParentId = 0;
                model.DisplayOrder = 0;

                SqlRepository repository = new SqlRepository();
                bool result = repository.UpdatePage(model);

                if (result)
                {
                    ViewBag.Message = "Изменения успешно сохранены";
                }
                else
                {
                    ModelState.AddModelError("", "Ошибка сохранения");
                }
            }
            else
            {
                ModelState.AddModelError("", "Изменения не сохранены");
            }

            return View("IndexEdit", model);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult DeletePage(bool confirm, string Id)
        {
            SqlRepository repository = new SqlRepository();

            int _id = Convert.ToInt32(Id);

            if (confirm)
            {
                bool result = repository.RemovePage(_id);

                if (result) //deleted
                {
                    return RedirectToAction("PageList", "Administration", new { Message = "Страница успешно удалена" });
                }
                else //error
                {
                    return RedirectToAction("Page", "Administration", new { Id = _id, Message = "Нельзя удалить страницу, которая отображается на сайте. Сначала ее нужно скрыть." });
                }
            }

            return RedirectToAction("Page", "Administration", new { Id = _id });                       
        }

        public ActionResult AdminMenuPartial()
        {
            List<NavMenuItem> model = new List<NavMenuItem>()
            { new NavMenuItem() { Title = "Страницы", Address = "Admin/PageList" },
              new NavMenuItem() { Title = "Пользователи", Address = "Account/UserList"}
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
