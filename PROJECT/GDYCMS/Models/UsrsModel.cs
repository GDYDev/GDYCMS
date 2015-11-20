using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace GDYCMS.Models
{
    /// <summary>
    /// Users management model(two way binding model).
    /// Some procedures fill necessary fields.
    /// <para>Single model was written to simplify data exchange</para>
    /// </summary>
    public class UsrsModel
    {
        /// <summary>
        /// User list container
        /// </summary>
        public List<string> UserList { get; set; }
        
        /// <summary>
        /// Login with validator
        /// </summary>
        [Required(ErrorMessage = "Please enter user login")]
        public string UserName { get; set; }
        /// <summary>
        /// Password with validator
        /// </summary>
        [Required(ErrorMessage = "Please enter user password")]
        public string Password { get; set; }
        /// <summary>
        /// Retypa password with validator
        /// </summary>
        [Required(ErrorMessage = "Please retype password")]
        public string RetypePassword { get; set; }
        /// <summary>
        /// New password field
        /// </summary>
        public string NewPassword { get; set; }
        /// <summary>
        /// Retype new password field
        /// </summary>
        public string RetypeNewPassword { get; set; }
        /// <summary>
        /// Remember me ? Yes=true
        /// </summary>
        public bool RememberMe { get; set; }
        /// <summary>
        /// Command. What to do.
        /// </summary>
        public string cmd { get; set; }
        /// <summary>
        /// Error message, filled by controller
        /// </summary>
        public string ErrorState { get; set; }
        /// <summary>
        /// Result. true=ok, false=error
        /// </summary>
        public bool isResulted { get; set; }
        /// <summary>
        /// Constructor
        /// </summary>
        public UsrsModel() {
            ErrorState = "";
            isResulted = false;
        }
    }
}