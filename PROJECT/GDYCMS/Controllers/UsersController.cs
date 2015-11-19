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
using GDYCMS.Filters;
using GDYCMS.Models;


using GDYCMS.Models.Global;
using GDYCMS.Models.Wrappers;

using CaptchaMvc;
using CaptchaMvc.Attributes;
using CaptchaMvc.HtmlHelpers;
using CaptchaMvc.Interface;

namespace GDYCMS.Controllers
{
    /// <summary>
    /// User authorization controller.
    /// </summary>
    /// This controller provide user authorization mechanism.
    [Authorize]
    [InitializeSimpleMembership]
    public class UsersController : AdminControllerType
    {
        /// <summary>
        /// Login. anonymous allowed
        /// </summary>
        /// <returns>View Login.cshtml</returns>
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Action method
        /// </summary>
        /// Administrator only
        /// <param name="received"> -users model</param>       
        /// <returns>View:AccountManagement.cshtml</returns>
        /// Depending on received.cmd, action method provide next functions:
        /// <list type="Actions">
        /// <item>Add new user</item>
        /// <item>Delete user</item>
        /// <item>Change user password</item>
        /// <item>Get user list</item>
        /// </list>
        /// Administrator can not delete himself.
        [Authorize(Users = "Administrator")]
        public ActionResult AccountManagement(UsrsModel received) {
            if (received.cmd == null)
            {
                UsrsModel usrs = new UsrsModel();
                usrs.UserList = this.UserAdmin.UserList;
                return View(usrs);
            }
            else {
                switch (received.cmd) {
                    case "Add": {
                        this.UserAdmin.AddNewUser(received);
                        break;
                    }
                    case "Delete": {
                        this.UserAdmin.DeleteUser(received);
                        break;
                    }
                    case "ChangePassword": {
                        this.UserAdmin.ChangePassword(received);
                        break;
                    }
                }
                received.UserList = this.UserAdmin.UserList;
                return View(received);
            }           
        }
        /// <summary>
        /// Action method
        /// </summary>
        /// <param name="model">-users model</param>
        /// Provide login mechanism. It use <a href="https://captchamvc.codeplex.com/">CaptchaMVC project</a>
        /// <returns>View:Login.cshtml</returns>
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(UsrsModel model) {
            ViewBag.CaptchaMessage= "";
            if (this.IsCaptchaValid("Captcha is not valid"))
            {
                if (User.Identity.IsAuthenticated == true)
                {
                    return Redirect("/Admin/Index");
                }
                else
                {
                    if (model.UserName != null && model.Password != null)
                    {
                        if (WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
                        {
                            return Redirect("/Admin/Index");
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "User name or password is incorrect";
                            return View();
                        }
                    }
                    else
                    {
                        return View();
                    }
                }
            }
            else {
                ViewBag.CaptchaMessage = "Message: captcha is not valid.";
                return View();
            }
        }

        /// <summary>
        /// Action method.
        /// </summary>
        /// Create password change interface for non-administrator.
        /// <returns>View:ChangeUserPassword.cshtml</returns>
        [Authorize]
        public ActionResult ChangeUserPassword() {
            UsrsModel m = new UsrsModel();
            return View(m);
        }

        /// <summary>
        /// Action method
        /// </summary>
        /// Allow user to change password
        /// <param name="model">-users model</param>
        /// <returns>View:ChangeUserPassword.cshtml</returns>
        [HttpPost]
        public ActionResult ChangeUserPassword(UsrsModel model) {
            if (model.NewPassword != null || model.RetypeNewPassword != null)
            {
                if (model.NewPassword == model.RetypeNewPassword)
                {
                    model.UserName = User.Identity.Name; // Load user from identy                
                    if (model.NewPassword == model.RetypeNewPassword)
                    {
                        model.isResulted = true;
                    }
                    else
                    {
                        model.ErrorState = "Something error, some password is null or empty, or passwords are not equal";
                    }
                }
                return View(model);
            }
            return View(model);
        }

        /// <summary>
        /// Action method. Logout
        /// </summary>
        /// <returns>Redirect to Users/Login</returns>
        [Authorize]
        public ActionResult Logout() {
            WebSecurity.Logout();
            return Redirect("/Users/Login");
        }
    }
}
