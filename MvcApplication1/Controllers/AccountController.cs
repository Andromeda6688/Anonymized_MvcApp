using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using MvcApplication1.Filters;
using MvcApplication1.Models;

namespace MvcApplication1.Controllers
{
    [Authorize(Roles = "Admin")]
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {
        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            string message;

            if (ModelState.IsValid) 
            {                
                if (Roles.GetRolesForUser(model.Email).Contains("Author"))
                {
                    if (WebSecurity.Login(model.Email, model.Password, persistCookie: model.RememberMe))
	                {
                        if (string.IsNullOrEmpty(returnUrl))
                        {
                            return RedirectToAction("PageList", "Administration"); //Admin/PageList
                        }

                        return Redirect(returnUrl);
                    }
                    else
                    {
                        message = "Unsuccessfull attempt";
                    }		 
	            }
                else
                {                    
                    message = "The account is inactive";
                }                
            }
            else
            {
                message = "Unsuccessfull attempt";
            }

            ModelState.AddModelError("", message);
            return RedirectToAction("Login", "Account");
        }

        //
        // POST: /Account/LogOff
        [AllowAnonymous]
        public ActionResult LogOff(string returnUrl)
        {
            if (WebSecurity.IsAuthenticated)
            {
                WebSecurity.Logout();

                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return Redirect(returnUrl);            
        }

        [AllowAnonymous]
        public ActionResult CurrentUserPartial()
        { 
            if (HttpContext.User.Identity.IsAuthenticated)
	        {                
                UsersContext context = new UsersContext();
                User model = context.Users.FirstOrDefault(u => u.Id == WebSecurity.CurrentUserId);
                return PartialView("_LogOffPartial", model);  
	        }

            return PartialView("_LogOffPartial", null);             
        }

        
        public ActionResult UserList(string message)
        {
            UsersContext context = new UsersContext();

            List<User> response =
                context.Users.Where(u => true).ToList();
            List<UserListItem> model = new List<UserListItem>();

            foreach (var user in response)
	        {
                model.Add(
                    new UserListItem(user.Id, user.Email, user.Name, user.IsActive)
                );
	        }

            ViewBag.Message = message;

            return View("UserList", model); 
        }

        //
        // GET: /Account/Register
        
        public ActionResult Register()
        {
            return View("Register");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    WebSecurity.CreateUserAndAccount(model.Email, model.Password,
                        new { Name = model.Name, IsActive = model.IsActive});                   
                    
                    if (model.IsAdmin)
                    {
                        if (!Roles.RoleExists("Admin"))
                        {
                            Roles.CreateRole("Admin");
                        }

                        Roles.AddUsersToRoles(new[] { model.Email }, new[] { "Admin" });
                    }

                    if (model.IsActive)
                    {
                        if (!Roles.RoleExists("Author"))
                        {
                            Roles.CreateRole("Author");
                        }

                        Roles.AddUsersToRoles(new[] { model.Email }, new[] { "Author" });

                    }
                   
                    UsersContext context = new UsersContext();
                    User response =
                        context.Users.FirstOrDefault(u => u.Email == model.Email);

                    return RedirectToAction("Manage", "Account", new { Id = response.Id, message="Пользователь успешно создан" });
                    
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }
            return View(model);
        }

        //
        // GET: /Account/Manage

        public ActionResult Manage(string Id, string Message)
        {
            UsersContext context = new UsersContext();
            int _id = Convert.ToInt32(Id);
            User response =
                context.Users.FirstOrDefault(u => u.Id == _id );

            var roles = Roles.GetRolesForUser(response.Email).ToList();

            ManageModel model = new ManageModel()
            {
                Id = response.Id,
                Email = response.Email,
                Name = response.Name,
                IsActive = response.IsActive,
                IsAdmin = roles.Contains("Admin")
            };

            ViewBag.Message = Message;
            
            return View("Manage", model);
        }

        //
        // POST: /Account/Manage   

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(string Id, ManageModel model)
        {
            if (ModelState.IsValid)
            {
                UsersContext context = new UsersContext();
                int _id = Convert.ToInt32(Id);
                User response =
                    context.Users.FirstOrDefault(u => u.Id == _id);

                if (string.Compare(response.Email, "SuperAdmin", true) != 0)
                {

                    //update data
                    if (string.Compare(response.Name, model.Name, false) != 0
                        || response.IsActive != model.IsActive)
                    {
                        response.Name = model.Name;
                        response.IsActive = model.IsActive;

                        context.SaveChanges();
                    }

                    //update roles
                    var roles = (SimpleRoleProvider)Roles.Provider;
                    var membership = (SimpleMembershipProvider)Membership.Provider;

                    if (model.IsAdmin)
                    {
                        if (!roles.GetRolesForUser(response.Email).Contains("Admin"))
                        {
                            Roles.AddUserToRole(response.Email, "Admin");
                        }
                    }
                    else
                    {
                        if (roles.GetRolesForUser(response.Email).Contains("Admin"))
                        {
                            Roles.RemoveUserFromRole(response.Email, "Admin");
                        }
                    }

                    if (model.IsActive)
                    {
                        if (!roles.GetRolesForUser(response.Email).Contains("Author"))
                        {
                            Roles.AddUserToRole(response.Email, "Author");
                        }
                    }
                    else
                    {
                        if (roles.GetRolesForUser(response.Email).Contains("Author"))
                        {
                            Roles.RemoveUserFromRole(response.Email, "Author");
                        }
                    }

                    return RedirectToAction("Manage", new { Id = model.Id, Message = "Аккаунт успешно обновлен" });
                }

                ModelState.AddModelError("", "The account of SuperAdmin cannot be changed in administration panel"); 
            }
            else
            {
                ModelState.AddModelError("", "The changes are not saved");
            }

            return View("Manage", model);
        }

        //GET: /Account/ChangePassword

        public ActionResult ChangePassword(string Id, string Message)
        {
            UsersContext context = new UsersContext();
            int _id = Convert.ToInt32(Id);
            User response =
                context.Users.FirstOrDefault(u => u.Id == _id);

            PasswordChangeModel model = new PasswordChangeModel()
            {
                Id = Convert.ToInt32(Id),
                Email = response.Email
            };

            ViewBag.Message = Message;

            return View("ChangePassword", model);
        }


        //
        // POST: /Account/ChangePassword   

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(PasswordChangeModel model)
        {
            if (ModelState.IsValid)
            {
                UsersContext context = new UsersContext();
                int _id = Convert.ToInt32(model.Id);
                User response =
                    context.Users.FirstOrDefault(u => u.Id == _id);

                if (string.Compare(response.Email, "SuperAdmin", true) != 0)
                {

                    bool IsChanged = WebSecurity.ChangePassword(model.Email, model.OldPassword, model.NewPassword);
            
            
                    if (IsChanged)
                    {
                        string _message = "The password was successfully changed";

                        return RedirectToAction("ChangePassword", "Account", new { Id = model.Id, Message = _message });
                    }
                    else
                    {
                        ModelState.AddModelError("", "Error! Wrong password?");  
                    }                

                    return View("ChangePassword", model);
                }
                else
                {
                    ModelState.AddModelError("", "The password of SuperAdmin can not be changed in adminstration panel"); 
                }
            }
            else
            {
                ModelState.AddModelError("", "The password was not changed");
            }
            return View("ChangePassword", model);
        }


        public ActionResult DeleteUser(bool confirm, string Id)
        {
            if (confirm)
            {
                UsersContext context = new UsersContext();
                int _id = Convert.ToInt32(Id);
                User response =
                    context.Users.FirstOrDefault(u => u.Id == _id);

                if (Roles.GetRolesForUser(response.Email).Count() == 0)
                {
                    try
                    {
                        // TODO: Add delete logic here
                        if (Roles.GetRolesForUser(response.Email).Count() > 0)
                        {
                            Roles.RemoveUserFromRoles(response.Email, Roles.GetRolesForUser(response.Email));
                        }
                        ((SimpleMembershipProvider)Membership.Provider).DeleteAccount(response.Email); // deletes record from webpages_Membership table
                        ((SimpleMembershipProvider)Membership.Provider).DeleteUser(response.Email, true); // deletes record from UserProfile table

                        return RedirectToAction("UserList", "Account", new { Message = "Пользователь успешно удален" });
                    }
                    catch
                    {
                        return RedirectToAction("UserList", "Account", new { Message = "Ошибка удаления" });
                    }
                }
                else
                {
                    return RedirectToAction("Manage", "Account", new { Id = Id, Message = "Нельзя удалить активного пользователя и/или админа" });
                }
            }

            return RedirectToAction("Manage", "Account", new { Id = Id });
        }

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
