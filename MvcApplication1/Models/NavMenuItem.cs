using MvcApplication1.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication1.Models
{
    public class NavMenuItem
    {
        public int Id { get; set; }
        
        public string Title { get; set; }

        public string Address { get; set; }

        public List<NavMenuItem> Children { get; set; }

        public NavMenuItem()
        {
            Children = new List<NavMenuItem>();
        }
    }
}