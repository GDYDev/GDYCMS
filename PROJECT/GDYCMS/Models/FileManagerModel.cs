using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GDYCMS.Models
{
    /// <summary>
    /// File manager model. This is universal model for data exchange between controller and angularJS code (by AJAX)
    /// <para>Used for two model binding by filling necessary fields</para>
    /// <para>Was written to simplify code.</para>
    /// </summary>
    public class FileManagerModel
    {
        /// <summary>
        /// File or folder text container
        /// </summary>
        public class ItemType {
            /// <summary>
            /// Name
            /// <example>NewFolder1</example>
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// file or folder
            /// </summary>
            public string Type { get; set; }
            /// <summary>
            /// boolean(is checked or not)
            /// </summary>
            public bool isChecked { get; set; }
            /// <summary>
            /// Constructor
            /// </summary>
            public ItemType() {
                this.isChecked = false;
            }
        }
        /// <summary>
        /// Left panel items
        /// </summary>
        public List<ItemType> LeftPanelList { get; set; }
        /// <summary>
        /// Right panel items
        /// </summary>
        public List<ItemType> RightPanelList { get; set; }
        /// <summary>
        /// Left panel absolute path(current).
        /// </summary>
        public string LeftPanelAbsolutePath { get; set; }
        /// <summary>
        /// Right panel absolute path(current).
        /// </summary>
        public string RightPanelAbsolutePath { get; set; }
        /// <summary>
        /// Current action
        /// </summary>
        public string CurrentAction{get;set;}
        /// <summary>
        /// Selected object(for rename operation only)
        /// </summary>
        public ItemType SelectedObject { get; set; } 
        /// <summary>
        /// New object name(for rename operation)
        /// </summary>
        public string NewObjectName { get; set; }
        /// <summary>
        /// New directory name (for create directory or rename operation)
        /// </summary>
        public string NewDirectoryName { get; set; }
        /// <summary>
        /// Selected active panel(left or right)
        /// </summary>
        public string ActivePanel { get; set; }
        /// <summary>
        /// Empty constructor
        /// </summary>
        public FileManagerModel() { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="CreatePanelLists">-true - create examples of panel lists</param>
        public FileManagerModel(bool CreatePanelLists){
            if (CreatePanelLists == true) {
                LeftPanelList = new List<ItemType>();
                RightPanelList = new List<ItemType>();
            }
        }
    }
}