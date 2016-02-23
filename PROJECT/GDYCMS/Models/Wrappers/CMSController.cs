using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using GDYCMS.Models;
using GDYCMS.Models.Global;
using GDYCMS.Models.Wrappers;



namespace GDYCMS.Models.Wrappers
{
    /// <summary>
    /// CMS controller base class. All controllers would be inherited from this class
    /// </summary>
    public class CMSController:Controller
    {
        /// <summary>
        /// Base things class.
        /// </summary>
        /// <para>Contain page not found html location. When material is not found, this page would be loaded to central content block.</para>
        /// <para>Contain access limited page. When material is not published, this page would be loaded to central page content(for non-registered users only)</para>
        /// <para></para>
        public class CMSConfigurationsClass{

            /// <summary>
            /// Statistics info
            /// </summary>
            public StatisticsInfo StatInfo { get; set; } 

            /// <summary>
            /// Link to the controller, that inherit this class and has real http context.
            /// </summary>
            private Controller BaseController;

            /// <summary>
            /// Access limited page HTML content
            /// </summary>
            public string AccessLimitedPage {
                get {
                    return System.IO.File.ReadAllText(BaseController.Server.MapPath("~/Content/Warnings/NotFound.html"));
                }
            }

            /// <summary>
            /// Not found page HTML content
            /// </summary>
            public string NotFoundPage {
                get {
                    return System.IO.File.ReadAllText(BaseController.Server.MapPath("~/Content/Warnings/NotFound.html"));
                }
            }

            public DBBaseFunctions DBBaseSample;
            public CMSConfigurationsClass(Controller ControllerBase,DBBaseFunctions DBBase) {
                this.BaseController = ControllerBase;
                this.StatInfo = new StatisticsInfo(); // Create statistics info class
                this.DBBaseSample=DBBase;
            }

            public void CountThisUserInStatistics() {
                if (BaseController.Session["StatInfo"] == null)                
                {
                    StatInfo.UserIP = BaseController.HttpContext.Request.UserHostAddress;
                    StatInfo.UserAgent = BaseController.HttpContext.Request.UserAgent;
                    BaseController.Session["StatInfo"] = StatInfo; // Create first Instance
                    // ADD statistics register operations

                    Statistics creation = new Statistics();
                    creation.IP = StatInfo.UserIP;
                    creation.UserAgent = StatInfo.UserAgent;
                    creation.TimeStamp = System.DateTime.Now;
                    DBBaseSample.Context.Statistics.Add(creation);
                    //DBBaseSample.Context.Entry(creation).State = System.Data.Entity.EntityState.Modified;
                    DBBaseSample.Context.SaveChanges();
                }
            }

            /// <summary>
            /// Rename directory operation
            /// </summary>
            /// <param name="AbsolutePath">-absolute path to directory</param>
            /// <param name="OldDirectoryName">- old directory name</param>
            /// <param name="NewDirectoryName">- new directory name</param>
            /// <returns>true if operation is successful</returns>
            public bool RenameDirectory(string AbsolutePath, string OldDirectoryName, string NewDirectoryName)
            {
                bool ret = false;
                try
                {
                    System.IO.Directory.Move(BaseController.Server.MapPath(AbsolutePath + "/" + OldDirectoryName), BaseController.Server.MapPath(AbsolutePath + "/" + NewDirectoryName));
                    ret = true;
                }
                catch { }
                return ret;
            }

            /// <summary>
            /// Rename file operation
            /// </summary>
            /// <param name="AbsolutePath">-absolute path to file</param>
            /// <param name="OldFileName">-old file name</param>
            /// <param name="NewFileName">-new file name</param>
            /// <returns>true if operation is successful</returns>
            public bool RenameFile(string AbsolutePath, string OldFileName, string NewFileName) {
                bool ret = false;
                try
                {
                    System.IO.File.Move(BaseController.Server.MapPath(AbsolutePath + "/" + OldFileName), BaseController.Server.MapPath(AbsolutePath + "/" + NewFileName));
                    ret = true;
                }
                catch { }
                return ret;
            }
            /// <summary>
            /// Delete directory operation
            /// </summary>
            /// <param name="AbsolutePath">- absolute path to directory</param>
            /// <param name="DirectoryName">- directory name</param>
            /// <param name="DeleteIncludedFilesAndSubfolders">-delete all subfolders and files ?</param>
            /// <returns>true if operation is successful</returns>
            public bool DeleteDirectory(string AbsolutePath, string DirectoryName,bool DeleteIncludedFilesAndSubfolders)
            {
                bool ret = false;
                try
                {
                    System.IO.Directory.Delete(BaseController.Server.MapPath(AbsolutePath + "/" + DirectoryName), DeleteIncludedFilesAndSubfolders);
                }
                catch
                {
                }
                return ret;
            }

            /// <summary>
            /// Delete file operation 
            /// </summary>
            /// <param name="AbsolutePath">-absolute path to file</param>
            /// <param name="FileName">-file name</param>
            /// <returns>true if operation is successful</returns>
            public bool DeleteFile(string AbsolutePath, string FileName) {
                bool ret = false;
                try
                {
                    System.IO.File.Delete(BaseController.Server.MapPath(AbsolutePath + "/" + FileName));
                }
                catch { 
                }
                return ret;
            }
            /// <summary>
            /// Create directory operation
            /// </summary>
            /// <param name="AbsolutePath">- absolute path</param>
            /// <param name="DirectoryName">- directory name</param>
            /// <returns>true if operation is successful</returns>
            public bool CreateDirectory(string AbsolutePath, string DirectoryName) {
                bool ret = false;
                try
                {
                    System.IO.Directory.CreateDirectory(BaseController.Server.MapPath(AbsolutePath + "/" + DirectoryName));
                    ret = true;
                }
                catch { 

                }
                return ret;
            }

            /// <summary>
            /// Mode directory operation
            /// </summary>
            /// <param name="FromAbsolutePath">-absolute path to directory(from)</param>
            /// <param name="ToAbsolutePath">-absolute path to directory(to)</param>
            /// <param name="DirectoryName">- directory name</param>
            /// <returns>-true if operation is successful</returns>
            public bool MoveDirectory(string FromAbsolutePath, string ToAbsolutePath, string DirectoryName)
            {
                bool ret = false;
                try
                {
                    System.IO.Directory.Move(BaseController.Server.MapPath(FromAbsolutePath + "/" + DirectoryName), BaseController.Server.MapPath(ToAbsolutePath + "/" + DirectoryName));
                    ret = true;
                }
                catch
                {
                    ret = false;
                }
                return ret;
            } 
            /// <summary>
            /// Move file operation
            /// </summary>
            /// <param name="FromAbsolutePath">-absolute path to directory(from)</param>
            /// <param name="ToAbsolutePath">-absolute path to directory(to)</param>
            /// <param name="FileName">-file name</param>
            /// <returns></returns>
            public bool MoveFile(string FromAbsolutePath, string ToAbsolutePath, string FileName){
                bool ret = false;
                try
                {
                    System.IO.File.Move(BaseController.Server.MapPath(FromAbsolutePath + "/" + FileName), BaseController.Server.MapPath(ToAbsolutePath + "/" + FileName));
                    ret = true;
                }
                catch {
                    ret = false;
                }
                return ret;
            }           
        }
        /// <summary>
        /// Private definition of CMSConfigurationsClass
        /// </summary>
        private CMSConfigurationsClass _cmsConfiguration;
        /// <summary>
        /// Access to CMSConfigurationsClass named as BaseThings
        /// </summary>
        public CMSConfigurationsClass BaseThings {
            get {
                return _cmsConfiguration;
            }
        }

        /// <summary>
        /// Class that provide database functions
        /// </summary>
        public class DBBaseFunctions {
            public DB_GDYCMSEntities Context = new DB_GDYCMSEntities(); 
        }
        /// <summary>
        /// Property(DBBaseFunctions)
        /// </summary>
        public DBBaseFunctions DBBase { get; set; }

        /// <summary>
        /// Get user id by name
        /// </summary>
        /// <param name="UserName">-user name</param>
        /// <returns>-user id</returns>
        public int GetUserIDByName(string UserName) {
            return DBBase.Context.UserProfile.SingleOrDefault(p => p.UserName == UserName).UserId;
        }
        /// <summary>
        /// Get HTML content of material by id.
        /// </summary>
        /// <param name="model">- FoundedMaterialQueryResult model</param>
        /// <returns>- FoundedMaterialQueryResult with filled content</returns>
        public FoundedMaterialQueryResult GetHtmlByID(FoundedMaterialQueryResult model) {
            FoundedMaterialQueryResult ret = new FoundedMaterialQueryResult();
            if (model.id != null)
            {
                var cnt = DBBase.Context.Materials.Count(p => p.MaterialID == model.id);
                if (cnt > 0)
                {
                    var tmp = DBBase.Context.Materials.SingleOrDefault(p => p.MaterialID == model.id);
                    if (tmp.isPublished == true)
                    {
                        model.Result = FoundedMaterialQueryResult.QueryResults.Published;
                        model.Container = tmp.HTML;
                    }
                    else
                    {
                        if (model.IsAuthenticated == true)
                        {
                            model.Result = FoundedMaterialQueryResult.QueryResults.NotPublished;
                            model.Container = tmp.HTML;
                        }
                        else
                        {
                            model.Container = null;
                            model.Result = FoundedMaterialQueryResult.QueryResults.NotPublished;
                        }
                    }
                }
                else
                { // Material queried by id is not found
                    model.Result = FoundedMaterialQueryResult.QueryResults.NotFound;
                    model.Container = null;
                }
            }
            else { // id is null
                var tmp = DBBase.Context.CentralMaterials.SingleOrDefault(p => p.ContainerID == (int)GlobalThings.ContainerTypes.CentralContent);
                if (tmp.isHidden == true)
                {
                    model.Result = FoundedMaterialQueryResult.QueryResults.NotPublished;
                    if (model.IsAuthenticated == true)
                    {
                        model.Container = tmp.HTML;
                    }
                    else
                    {
                        model.Container = null;
                    }
                }
                else {
                    model.Result = FoundedMaterialQueryResult.QueryResults.Published;
                    model.Container = tmp.HTML;
                }
            }
            return model;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public CMSController() {
            DBBase = new DBBaseFunctions();
            _cmsConfiguration = new CMSConfigurationsClass(this,DBBase);

        }
    }
}