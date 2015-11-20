using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using GDYCMS.Models.Global;
using System.IO;
using System.Web.Mvc;

namespace GDYCMS.Models
{
    /// <summary>
    /// Universal editor model
    /// </summary>
    /// It used for data exchange between angularJS model and controller
    /// Depending on the task, some procedure use necessary fields or fill necessary fields.
    /// Universal (two way binding) model was made to simplify code.
    public class EditorModel
    {
        private DB_GDYCMSEntities Context = new DB_GDYCMSEntities();

        /// <summary>
        /// Container for server root path. May be used by file manager
        /// <example>
        ///     http://localhost/
        /// </example>
        /// </summary>
        public string ServerPath { get; set; }
        /// <summary>
        /// New material name
        /// </summary>
        public string NewMaterialName { get; set; }
        /// <summary>
        /// New material group name
        /// </summary>
        public string NewMaterialGroupName { get; set; }
        /// <summary>
        /// Last change date
        /// </summary>
        public DateTime LastChange { get; set; }
        /// <summary>
        /// Selected material group
        /// </summary>
        public string SelectedMaterialGroup { get; set; }
        /// <summary>
        /// Selected material name
        /// </summary>
        public string SelectedMaterial { get; set; }
        /// <summary>
        /// Selected material id
        /// </summary>
        public int SelectedMaterialID { get; set; }

        /// <summary>
        /// Material group list
        /// </summary>
        public List<string> MaterialGroupList { get; set; }
        /// <summary>
        /// Material list
        /// </summary>
        public List<string> MaterialList { get; set; }
        /// <summary>
        /// Selected material group name
        /// </summary>
        public string MaterialGroupName{get;set;}
        /// <summary>
        /// Selected material name
        /// </summary>
        public string MaterialName{get;set;}
        /// <summary>
        /// Html content of selected material
        /// </summary>
        public string HTML{get;set;}
        /// <summary>
        /// First creator name
        /// </summary>
        public string FirstCreator{get;set;}
        /// <summary>
        /// Last modifier name
        /// </summary>
        public string LastModifier{get;set;}
        /// <summary>
        /// Is published=true, not published=false
        /// </summary>
        public bool isPublished { get; set; }
        /// <summary>
        /// Container number(for database and central page editor(
        /// </summary>
        public int Container { get; set; }
        /// <summary>
        /// Command (what to do, loaded by AngularJS code)
        /// </summary>
        public string cmd { get; set; }

        /// <summary>
        /// Result of operation
        /// </summary>
        public bool ActionResult { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public EditorModel() {
            this.Container = 0xFF;
            this.SelectedMaterialGroup = "";
            this.SelectedMaterial = "";
            this.ActionResult = true;
        }


        /// <summary>
        /// Load data for selected container at admin controller
        /// </summary>
        public void LoadData() {
            if (Container != 0xFF) {
                this.HTML = (string)Context.CentralMaterials.Where(p => p.ContainerID == Container).Select(p => p.HTML).SingleOrDefault();
            }
        }
        /// <summary>
        /// Save data for selected container at admin controller
        /// </summary>
        public void SaveData() {
            if (Container != 0xFF) {
                var tmp = Context.CentralMaterials.Where(p => p.ContainerID == Container).Select(p=>p).SingleOrDefault();
                tmp.HTML = this.HTML;
                tmp.isHidden = !isPublished;
                Context.SaveChanges();
            }
        }
    }
}