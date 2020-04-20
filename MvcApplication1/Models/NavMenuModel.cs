using MvcApplication1.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication1.Models
{
    public class NavMenuModel
    {
        public List<NavMenuItem> Menu;

        public NavMenuModel()
        {
            SqlRepository repository = new SqlRepository();

            //now we need only 1-level menu. perhaps we will change it later
            //we should not load all page field due to performance reasons
            Menu = (from page in repository.Pages.Where(p => p.ParentId == repository.Pages.FirstOrDefault(m => m.Address == "Index").Id
                                                                            && p.IsVisible == true).ToList()
                                      select new NavMenuItem() { Title = page.Title, Address = page.Address }).ToList();
            //TODO
            // n-level menu
        }        
    }


    public class NavMenuItem
    {
        public string Title;
        public string Address;
    }

}