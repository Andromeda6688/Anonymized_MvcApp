using MvcApplication1.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using WebMatrix.WebData;

namespace MvcApplication1.Models
{
    public class UserListItem
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public string IsActive
        {
            get
            {
                return _isActive ? "Yes" : "No";
            }
        }
        public bool _isActive;

        public string IsAdmin
        {
            get 
            {
                var RoleProvider = (SimpleRoleProvider)Roles.Provider;

                var roles = RoleProvider.GetRolesForUser(Email).ToList();

                return roles.Contains("Admin") ? "Yes" : "No";                
            }
        }

        public UserListItem(int p_id, string p_Email, string p_Name, bool p_IsActive)
        {
            Id = p_id;
            Email = p_Email;
            Name = p_Name;
            _isActive = p_IsActive;            
        }

    }

}