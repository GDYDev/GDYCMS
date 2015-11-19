using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GDYCMS.Models.Global;
using System.IO;
namespace GDYCMS.Models
{

    /// <summary>
    /// This model is used for page creation for visitor(registered or non registered)
    /// </summary>
    public class ViewerModel
    {
        #region HTML CONTAINERS
        /// <summary>
        /// Upper container HTML code
        /// </summary>
        public string UpperMenu { get; set; }
        /// <summary>
        /// Left up menu HTML code
        /// </summary>
        public string LeftMenuUp { get; set; }
        /// <summary>
        /// Left down menu HTML code
        /// </summary>
        public string LeftMenuDown { get; set; }
        /// <summary>
        /// Right up menu HTML code
        /// </summary>
        public string RightMenuUp { get; set; }
        /// <summary>
        /// Right down menu HTML code
        /// </summary>
        public string RightMenuDown { get; set; }
        /// <summary>
        /// Central content HTML code
        /// </summary>
        public string CentralContent { get; set; }
        /// <summary>
        /// Footer HTML code
        /// </summary>
        public string Footer { get; set; }

        #endregion        
    }
}