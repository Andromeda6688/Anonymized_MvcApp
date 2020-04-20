using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.Linq;
using MvcApplication1.Models;
using MvcApplication1.Global;

namespace MvcApplication1.Models
{
    public class SqlRepository
    {
        WebSiteDBDataContext DB = new WebSiteDBDataContext(ConfigurationManager.ConnectionStrings["WebSiteDBConnectionString"].ConnectionString);

        public IQueryable<Page> Pages
        {
            get
            {
                return DB.Pages;
            }
        }

        public Page GetPage(string p_PageName)
        {
            return DB.Pages.Where(p => (string.Compare(p.Address, p_PageName, true) == 0)).FirstOrDefault();
        }

        public Page GetPage(int p_PageId)
        {
            return DB.Pages.Where(p => (p.Id==p_PageId)).FirstOrDefault();
        }

        public List<NavMenuItem> GetNavMenu()
        {
            //we should not load all page fields due to performance reasons
            List<NavMenuItem> result = (from page in DB.Pages.Where(p => p.ParentId == DB.Pages.FirstOrDefault(m => m.Address == "Index").Id
                                                                            && p.IsVisible == true).ToList()
                                       select new NavMenuItem() { Title = page.Title, Address = page.Address }).ToList();

            return result;
        }

        public List<PagesMenuItem> GetPagesList()
        {
            List<PagesMenuItem> result =
                DB.Pages.Select(p => new PagesMenuItem() { Title = p.Title, Id = p.Id }).ToList();

            return result;
        }

        public bool CreatePage(
            string p_Title,
            string p_Description,
            string p_Keywords,
            string p_Content,
            string p_Address,
            int p_ParentId = 0,
            bool p_IsVisible = true
            )
        {
            Page _page = new Page
            {
                Title = p_Title,
                Description = p_Description,
                Keywords = p_Keywords,
                Content = p_Content,
                Address = p_Address,
                ParentId = p_ParentId,
                IsVisible = p_IsVisible
            };

            return CreatePage(_page);
        }

        public bool CreatePage(Page p_Page)
        {
            if (!(String.IsNullOrEmpty(p_Page.Title) &&
                String.IsNullOrEmpty(p_Page.Address)))
            {
                DB.Pages.InsertOnSubmit(p_Page);
                DB.Pages.Context.SubmitChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool UpdatePage(Page p_Page)
        {
            Page changingPage = DB.Pages.FirstOrDefault(p => p.Id == p_Page.Id);

            if (changingPage!=null)
            {
                changingPage.Title = p_Page.Title;
                changingPage.Description = p_Page.Description;
                changingPage.Keywords = p_Page.Keywords;
                changingPage.Content = p_Page.Content;
                changingPage.ParentId = p_Page.ParentId;
                changingPage.Address = p_Page.Address;
                changingPage.IsVisible = p_Page.IsVisible;

                DB.SubmitChanges();

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool UpdatePage(
            int p_Id,
            string p_Title,
            string p_Description,
            string p_Keywords,
            string p_Content,
            string p_Address,
            int p_ParentId = 0,
            bool p_IsVisible = true
            )
        {
            Page changingPage = DB.Pages.FirstOrDefault(p => p.Id == p_Id);

            if (changingPage != null)
            {
                changingPage.Title = p_Title;
                changingPage.Description = p_Description;
                changingPage.Keywords = p_Keywords;
                changingPage.Content = p_Content;
                changingPage.ParentId = p_ParentId;
                changingPage.Address = p_Address;
                changingPage.IsVisible = p_IsVisible;

                DB.SubmitChanges();

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RemovePage(int p_Id)
        {
            Page _page = DB.Pages.FirstOrDefault(p => p.Id == p_Id);

            bool _result = false;

            if (_page != null)
            {
                if (!_page.IsVisible)
                {
                    DB.Pages.DeleteOnSubmit(_page);
                    DB.Pages.Context.SubmitChanges();
                    _result = true;
                }
            }

            return _result;
        }







    }
}