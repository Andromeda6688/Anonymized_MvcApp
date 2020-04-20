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

        public bool CreatePage(
            string p_Title,
            string p_Description,
            string p_Keywords,
            string p_Content,
            string p_Address,
            int p_ParentId = 0
            )
        {
            if (
                !(String.IsNullOrEmpty(p_Title) &&
                String.IsNullOrEmpty(p_Address))
                )
            {
                Page _page = new Page
                {
                    Title = p_Title,
                    Description = p_Description,
                    Keywords = p_Keywords,
                    Content = p_Content,
                    Address = p_Address,
                    ParentId = p_ParentId
                };

                DB.Pages.InsertOnSubmit(_page);
                DB.Pages.Context.SubmitChanges();
                return true;
            }

            return false;
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