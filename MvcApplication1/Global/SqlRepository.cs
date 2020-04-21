﻿using System;
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
            List<NavMenuItem> result = (from page in DB.Pages.Where(p => p.ParentId == DB.Pages.FirstOrDefault(m => m.Address == "Index").Id &&
                                                                         p.IsVisible == true &&
                                                                         p.IsInMenu).ToList()
                                        orderby page.DisplayOrder
                                                                            
                                        select new NavMenuItem() { Title = page.Title, Address = page.Address }
                                        ).ToList();

            return result;
        }

        public List<PagesMenuItem> GetPagesList()
        {
            List<PagesMenuItem> result =
                DB.Pages.Select(p => new PagesMenuItem() { Title = p.Title, Id = p.Id }).ToList();

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

        public int CreatePage(Page p_Page)
        {
            if (!(String.IsNullOrEmpty(p_Page.Title) &&
                String.IsNullOrEmpty(p_Page.Address)))
            {
                DB.Pages.InsertOnSubmit(p_Page);
                DB.Pages.Context.SubmitChanges();

                return p_Page.Id;
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
                UpdatePage(p_Page.Id, p_Page.Title, p_Page.Description, p_Page.Keywords, 
                    p_Page.Content, p_Page.Address, p_Page.ParentId, p_Page.IsVisible);

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