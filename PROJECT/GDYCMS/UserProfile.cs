//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GDYCMS
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserProfile
    {
        public UserProfile()
        {
            this.Materials = new HashSet<Materials>();
            this.Materials1 = new HashSet<Materials>();
            this.webpages_Roles = new HashSet<webpages_Roles>();
        }
    
        public int UserId { get; set; }
        public string UserName { get; set; }
    
        public virtual ICollection<Materials> Materials { get; set; }
        public virtual ICollection<Materials> Materials1 { get; set; }
        public virtual ICollection<webpages_Roles> webpages_Roles { get; set; }
    }
}