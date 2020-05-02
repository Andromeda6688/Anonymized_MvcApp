using MvcApplication1.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication1.Models
{
    public class PagesListItem
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Address { get; set; }

        public string IsVisible
        {
            get
            {
                return _isVisible ? "Yes" : "No";
            }
        }
        bool _isVisible;

        public string IsInMenu {
            get 
            {
                return _isInMenu ? "Yes" : "No";               
            }
        }
        bool _isInMenu;

        public int DisplayOrder { get; set; }

        public string Parent { get; set; }

        public List<PagesListItem> Children { get; set; }

        public PagesListItem(int p_id, string p_Title, string p_Address, bool p_IsVisible, bool p_IsInMenu, string p_Parent)
        {
            Id = p_id;
            Title = p_Title;
            Address = p_Address;
            _isVisible = p_IsVisible;
            _isInMenu = p_IsInMenu;
            Parent = p_Parent;

            Children = new List<PagesListItem>();
        }

    }

}