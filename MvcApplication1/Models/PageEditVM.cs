using MvcApplication1.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication1.Models
{
    public class PageEditVM
    {
        public Page Page { get; set; }
        public IEnumerable <PagesMenuItem> Parents {get;set;}

        public PageEditVM(Page p_page)
        {
            Page = p_page;

            FillParents();
        }

        public PageEditVM()
        {
            FillParents();
        }

        void FillParents()
        {
            SqlRepository repository = new SqlRepository();

            Parents = repository.GetParentsList();
        }

    }
}