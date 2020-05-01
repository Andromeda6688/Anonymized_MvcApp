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
    [Authorize]
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
            if (ModelState.IsValid && WebSecurity.Login(model.Email, model.Password, persistCookie: model.RememberMe))
            {
                return Redirect(returnUrl);
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "Неудачная попытка входа");
            return RedirectToAction("Login", "Account");
        }

        //
        // POST: /Account/LogOff
       
        public ActionResult LogOff(string returnUrl)
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


        public ActionResult UserList()
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

            return View("UserList", model); 
        }


        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    WebSecurity.CreateUserAndAccount(model.Email, model.Password,
                        new { Name = model.Name, IsActive = model.IsActive});

                    var roles = (SimpleRoleProvider)Roles.Provider;
                    
                    if (model.IsAdmin)
                    {
                        if (!roles.RoleExists("Admin"))
                        {
                            roles.CreateRole("Admin");
                        }

                        roles.AddUsersToRoles(new [] {model.Email}, new [] {"Admin"});
                    }

                    if (!roles.RoleExists("Author"))
                    {
                        roles.CreateRole("Author");
                    }

                    roles.AddUsersToRoles(new[] { model.Email }, new[] { "Author" }); 
                    
                    return RedirectToAction("Index", "Home");
                    
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }
            return View(model);
        }

        //
        // POST: /Account/Disassociate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // Only disassociate the account if the currently logged in user is the owner
            if (ownerAccount == User.Identity.Name)
            {
                // Use a transaction to prevent the user from deleting their last login credential
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = ManageMessageId.RemoveLoginSuccess;
                    }
                }
            }

            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage

        public ActionResult Manage(string Id, string message)
        {
            UsersContext context = new UsersContext();
            int _id = Convert.ToInt32(Id);
            User response =
                context.Users.FirstOrDefault(u => u.Id == _id );

            
            var RoleProvider = (SimpleRoleProvider)Roles.Provider;
            var roles = RoleProvider.GetRolesForUser(response.Email).ToList();

            ManageModel model = new ManageModel()
            {
                Id = response.Id,
                Email = response.Email,
                Name = response.Name,
                IsActive = response.IsActive,
                IsAdmin = roles.Contains("Admin")
            };
           
            ViewBag.Message = message;
            
            return View("Manage", model);
        }

        //
        // POST: /Account/Manage   

        [HttpPost]
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

                    return RedirectToAction("Manage", new { Id = model.Id, message = "Аккаунт успешно обновлен" });
                }

                ModelState.AddModelError("", "Аккаунт SuperAdmin изменять нельзя"); 
            }
            else
            {
                ModelState.AddModelError("", "Изменения не сохранены");
            }

            return View("Manage", model);
        }

        //GET: /Account/ChangePassword
            
        public ActionResult ChangePassword(string Id,  string message)
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

            ViewBag.Message = message;

            return View("ChangePassword", model);
        }


        //
        // POST: /Account/ChangePassword   

        [HttpPost]
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
                        string _message = "Пароль успешно изменен";

                        return RedirectToAction("ChangePassword", "Account", new { Id = model.Id, message = _message });
                    }
                    else
                    {
                        ModelState.AddModelError("", "Ошибка! Неверный текущий пароль?");  
                    }                

                    return View("ChangePassword", model);
                }
                else
                {
                    ModelState.AddModelError("", "Пароль для SuperAdmin изменять нельзя"); 
                }
            }
            else
            {
                ModelState.AddModelError("", "Пароль не обновлен");
            }
            return View("ChangePassword", model);
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
