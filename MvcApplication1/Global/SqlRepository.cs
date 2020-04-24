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
            return DB.Pages.Where(p => (string.Compare(p.Address, p_PageName, true) == 0) &&
                                        p.IsVisible == true )
                                        .FirstOrDefault();
        }

        public Page GetPage(int p_PageId)
        {
            return DB.Pages.Where(p => (p.Id==p_PageId) &&
                                        p.IsVisible == true )
                                        .FirstOrDefault();
        }

        public List<NavMenuItem> GetNavMenu()
        {            
            List<NavMenuItem> result = (from page in DB.Pages.Where(p => p.ParentId == DB.Pages.FirstOrDefault(m => m.Address == "Index").Id &&
                                                                         p.IsVisible == true &&
                                                                         p.IsInMenu == true)
                                                             .ToList()
                                        orderby page.DisplayOrder

                                        select new NavMenuItem() { Title = page.Title, Address = page.Address }  //we should not load all page fields due to performance reasons
                                        ).ToList();

            return result;
        }

        public List<PagesListItem> GetPagesList()
        {            
            List<PagesListItem> result =    (from page in DB.Pages
                                            join parentPage in DB.Pages on page.ParentId equals parentPage.Id into outer
                                             from joinedPage in outer.DefaultIfEmpty()  //LEFT JOIN
                                             select new PagesListItem(page.Id, page.Title, page.Address, page.IsVisible, page.IsInMenu, joinedPage.Title))
                                            .ToList();
            return result;
        }

        public List<PagesMenuItem> GetParentsList()
        {
            List<PagesMenuItem> result =
                DB.Pages.Where(p=>p.ParentId==0) //only one level is acceptable
                .Select(p => new PagesMenuItem() { Title = p.Title, Id = p.Id })
                .ToList();

            return result;
        }

        public int CreatePage(
            string p_Title,
            string p_Description,
            string p_Keywords,
            string p_Content,
            string p_Address,
            int p_ParentId = 0,
            bool p_IsVisible = true,
            bool p_IsInMenu = false
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
                IsVisible = p_IsVisible,
                IsInMenu = p_IsInMenu
            };

            return CreatePage(_page);
        }

        public int CreatePage(Page p_Page)
        {
            if (!(String.IsNullOrEmpty(p_Page.Title) &&
                String.IsNullOrEmpty(p_Page.Address)))
            {
                if (IsAddressUnique(p_Page))
	            {
                    if (p_Page.ParentId == null)
                    {
                        int IndexId = DB.Pages.Where(p => (p.Address == "Index"))
                                              .Select(p => p.Id)
                                              .FirstOrDefault();
                        p_Page.ParentId = IndexId;
                    }

                    DB.Pages.InsertOnSubmit(p_Page);
                    DB.Pages.Context.SubmitChanges();

                    return p_Page.Id;
                }            
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        public bool UpdatePage(Page p_Page)
        {
            Page changingPage = DB.Pages.FirstOrDefault(p => p.Id == p_Page.Id);

            if (changingPage!=null)
            {
                if (string.Compare(p_Page.Address, "Index", true)!=0 )
                {
                    if (IsAddressUnique(p_Page))
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
                else
                {
                    return false;
                }  
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
            bool p_IsVisible = true,
            bool p_IsInMenu = false
            )
        {
            Page _page = new Page()
            {
                Id = p_Id,
                Title = p_Title,
                Description = p_Description,
                Keywords = p_Keywords,
                Content = p_Content,
                Address = p_Address,
                ParentId = p_ParentId,
                IsVisible = p_IsVisible,
                IsInMenu = p_IsInMenu
            };

            return UpdatePage(_page);           
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

        bool IsAddressUnique(Page p_Page)
        {
            int count = DB.Pages.Where(p => string.Compare(p.Address, p_Page.Address, true) == 0 &&
                                            p.ParentId == p_Page.ParentId &&
                                            p.Id != p_Page.Id )
                                .Count();
            
            return (count > 0) ? false : true;            
        }







    }
}