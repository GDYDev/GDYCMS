using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.IO;

using GDYCMS.Models;
using GDYCMS.Models.Global;
using GDYCMS.Models.Wrappers;
using System.Text.RegularExpressions;

namespace GDYCMS.Controllers
{
    /// <summary>
    /// Administrator controller
    /// </summary>
    /// It's used for material and central page content edition.
    public class AdminController : AdminControllerType
    {
        /// <summary>
        /// Action method
        /// </summary>
        /// <returns>View:Index.cshtml</returns>
        [Authorize]
        public ActionResult Index(StatisticsModel model)
        {
            if (model.Action == "ClearStatistics")
            {
                this.ClearStatistics();
                model.Action = "";
                return View(CurrentStatistics());
            }
            else
            {
                if (model.From != null)
                {
                    StatisticsModel m = CurrentStatistics(model.From, model.To);
                    return View(m);
                }
                else
                {
                    return View(CurrentStatistics());
                }
            }
        }
        /// <summary>
        /// Material editor action method
        /// </summary>
        /// Work with json only. Front end code work with it.
        /// <param name="ReceiverModel">Editor model</param>
        /// <returns>Editor model as json result</returns>
        /// Editor model is universal data structure that is used for data exchange.
        /// Controller provide data exchange with angularJS module.
        /// Operations:
        /// <list>
        ///     <item>Load material group list</item>
        ///     <item>Load material list for selected material group</item>
        ///     <item>Load selected material</item>
        ///     <item>Add new material group</item>
        ///     <item>Rename material group</item>
        ///     <item>Delete material group</item>
        ///     <item>Add new material</item>
        ///     <item>Rename material</item>
        ///     <item>Delete material</item>
        ///     <item>Update material</item>
        ///     <item>Get server path</item>
        /// </list>
        /// 
        [Authorize]
        public JsonResult MaterialsEditorJSON(EditorModel ReceiverModel) {
            EditorModel model = ReceiverModel;
            JsonResult ret = Json("ok");
            switch (ReceiverModel.cmd) {
                case "GetMaterialGroups": { /// Load material group list
                    LoadMaterialGroupsToModel(model);
                    ret = Json(model);
                    break;
                }
                case "LoadMaterialListForSelectedGroup": { /// Load material list for selected material group
                    LoadMaterialListForSelectedGroup(model);
                    ret = Json(model);
                    break;
                }
                case "LoadSelectedMaterial": { /// Load selected material
                    LoadSelectedMaterial(model);
                    ret = Json(model);
                    break;
                }
                case "AddNewMaterialGroup": { /// Add new material group
                    ADDMaterialGroup(model);
                    ret = Json(model);
                    break;
                }
                case "RenameMaterialGroup":{ /// Rename material group
                    RenameMaterialGroup(model);
                    ret = Json(model);
                    break;
                }
                case "DeleteMaterialGroup":{ // Delete material group
                    DeleteMaterialGroup(model);
                    ret=Json(model);
                    break;
                }
                case "AddNewMaterial":{ /// Add new material
                    AddOrModifyMaterial(model);
                    ret=Json(model);
                    break;
                }
                case "RenameMaterial":{ /// Rename material
                    RenameMaterial(model);
                    ret=Json(model);
                    break;
                }
                case "DeleteMaterial":{ /// Delete material
                    DeleteMaterial(model);
                    ret = Json(model);
                    break;
                }
                case "UpdateMaterial": { /// Update material
                    AddOrModifyMaterial(model);
                    ret = Json(model);
                    break;
                }
                case "GetServerPath":{ /// Get server path
                    model.ServerPath = Server.MapPath("");
                    ret = Json(model);
                    break;
                }
            }            
            return ret;
        }

        /// <summary>
        /// Action method.
        /// </summary>
        /// This action method return view only(user interface creation).
        /// <returns>View:MaterialsEditor.cshtml</returns>
        [Authorize]
        public ActionResult MaterialsEditor() {
            return View();
        }
        /// <summary>
        /// Action method
        /// </summary>
        /// This action method provide central page edition.
        /// AJAX is not used.
        /// <param name="id">-Page part id. string parameter</param>
        /// <param name="ReceiveModel">Model that contain data</param>
        /// <returns>View:CentralPageEditor.cshtml</returns>
        [ValidateInput(false)]
        [Authorize]
        public ActionResult CentralPageEditor(string id, EditorModel ReceiveModel)
        {
            if (ReceiveModel != null) {
                ReceiveModel.SaveData();                                                
            }
            EditorModel model = new EditorModel();
            switch (id) { 
                case "UpperMenu":{                                                     
                    model.Container = (int)GlobalThings.ContainerTypes.UpperMenu;
                    break;
                }
                case "LeftMenuUp":{                                                     
                    model.Container = (int)GlobalThings.ContainerTypes.LeftMenuUp;
                    break;
                }
                case "LeftMenuDown":{                                                   
                    model.Container = (int)GlobalThings.ContainerTypes.LeftMenuDown;
                    break;
                }
                case "RightMenuUp":{                                                    
                    model.Container = (int)GlobalThings.ContainerTypes.RightMenuUp;
                    break;
                }
                case "RightMenuDown":{                                                 
                    model.Container = (int)GlobalThings.ContainerTypes.RightMenuDown;
                    break;
                }
                case "CentralContent":{                                                 
                    model.Container = (int)GlobalThings.ContainerTypes.CentralContent;
                    break;
                }
                case "Footer":{                                                         
                    model.Container = (int)GlobalThings.ContainerTypes.Footer;
                    break;
                }
                    
            }
            model.LoadData();
            return View(model);
        }

        /// <summary>
        /// Action method.
        /// </summary>
        /// <param name="Incoming">- file manager model, it's used for data exchange with AngularJS code</param>
        /// <param name="file">- uploaded file</param>
        /// <returns>View(FileManager.cshtml) or JSON.</returns>
        /// If Incoming has not null content attribute, this action will return json, otherwise -view.
        /// 

        [Authorize]
        public ActionResult FileManager(FileManagerModel Incoming,HttpPostedFileBase file) {            
            if (file != null) {
                if (Request.Cookies["GDYSimpleCMS_LAST_UPLOAD_PATH"] != null) { /// Transform JSON.stringify structure to server path
                    var tmp=Request.Cookies["GDYSimpleCMS_LAST_UPLOAD_PATH"].Value.ToString();
                    tmp=Regex.Replace(tmp, "%22~%2F", "");
                    tmp = Regex.Replace(tmp, "%22%2F", "");
                    tmp = Regex.Replace(tmp, "%2F%22", "\\");
                    tmp = Regex.Replace(tmp, "%2F", "\\");
                    tmp = Regex.Replace(tmp, "%22", "\\");
                    tmp = Regex.Replace(tmp, "%20", " "); // Space symbol
                    tmp = AppDomain.CurrentDomain.BaseDirectory+tmp;
                    tmp += file.FileName;
                    tmp = Regex.Replace(tmp, "\"", "");
                    file.SaveAs(tmp);
                }
            }
            bool JsonNeed = false;
            FileManagerModel OutputModel = new FileManagerModel(true);
            if ((Incoming.CurrentAction != null) && (Incoming.CurrentAction != "")) {
                JsonNeed = true;                
                switch (Incoming.CurrentAction) {
                    case "read": {
                        SupplyFileManagerPanel(OutputModel.LeftPanelList, Incoming.LeftPanelAbsolutePath);
                        SupplyFileManagerPanel(OutputModel.RightPanelList, Incoming.RightPanelAbsolutePath);
                        break;
                    }
                    case "move": {
                        if (Incoming.ActivePanel=="left"){
                            foreach (var item in Incoming.LeftPanelList) {
                                if (item.isChecked == true) {
                                    if (item.Type == "file")
                                    {
                                        this.BaseThings.MoveFile(Incoming.LeftPanelAbsolutePath, Incoming.RightPanelAbsolutePath, item.Name);
                                    }
                                    else {
                                        if (item.Type == "folder") {
                                            this.BaseThings.MoveDirectory(Incoming.LeftPanelAbsolutePath, Incoming.RightPanelAbsolutePath, item.Name);
                                        }
                                    }
                                }
                            }
                        }
                        else {
                            if (Incoming.ActivePanel == "right") {
                                foreach (var item in Incoming.RightPanelList)
                                {
                                    if (item.isChecked == true)
                                    {
                                        if (item.Type == "file")
                                        {
                                            this.BaseThings.MoveFile(Incoming.RightPanelAbsolutePath, Incoming.LeftPanelAbsolutePath, item.Name);
                                        }
                                        else
                                        {
                                            if (item.Type == "folder")
                                            {
                                                this.BaseThings.MoveDirectory(Incoming.RightPanelAbsolutePath, Incoming.LeftPanelAbsolutePath, item.Name);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }
                    case "newfolder": {
                        if (Incoming.ActivePanel == "left")
                        {
                            this.BaseThings.CreateDirectory(Incoming.LeftPanelAbsolutePath, Incoming.NewDirectoryName);
                        }
                        else {
                            if (Incoming.ActivePanel == "right") {
                                this.BaseThings.CreateDirectory(Incoming.RightPanelAbsolutePath, Incoming.NewDirectoryName);
                            }
                        }
                        break;
                    }
                    case "delete": {
                        if (Incoming.ActivePanel == "left")
                        {
                            foreach (var item in Incoming.LeftPanelList) {
                                if (item.isChecked == true) {
                                    if (item.Type == "file")
                                    {
                                        this.BaseThings.DeleteFile(Incoming.LeftPanelAbsolutePath, item.Name);
                                    }
                                    else {
                                        if (item.Type == "folder") {
                                            this.BaseThings.DeleteDirectory(Incoming.LeftPanelAbsolutePath, item.Name, true);
                                        }
                                    }
                                }
                            }
                        }
                        else {
                            if (Incoming.ActivePanel == "right") {
                                foreach (var item in Incoming.RightPanelList)
                                {
                                    if (item.isChecked == true)
                                    {
                                        if (item.Type == "file")
                                        {
                                            this.BaseThings.DeleteFile(Incoming.RightPanelAbsolutePath, item.Name);
                                        }
                                        else
                                        {
                                            if (item.Type == "folder")
                                            {
                                                this.BaseThings.DeleteDirectory(Incoming.RightPanelAbsolutePath, item.Name, true);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }
                    case "rename": {
                        if (Incoming.ActivePanel == "left")
                        {
                            if (Incoming.SelectedObject.Type == "file")
                            {
                                this.BaseThings.RenameFile(Incoming.LeftPanelAbsolutePath, Incoming.SelectedObject.Name, Incoming.NewObjectName);
                            }
                            else
                            {
                                if (Incoming.SelectedObject.Type == "folder")
                                {
                                    this.BaseThings.RenameDirectory(Incoming.LeftPanelAbsolutePath, Incoming.SelectedObject.Name, Incoming.NewObjectName);
                                }
                            }
                        }
                        else {
                            if (Incoming.ActivePanel == "right") {
                                if (Incoming.SelectedObject.Type == "file")
                                {
                                    this.BaseThings.RenameFile(Incoming.RightPanelAbsolutePath, Incoming.SelectedObject.Name, Incoming.NewObjectName);
                                }
                                else
                                {
                                    if (Incoming.SelectedObject.Type == "folder")
                                    {
                                        this.BaseThings.RenameDirectory(Incoming.RightPanelAbsolutePath, Incoming.SelectedObject.Name, Incoming.NewObjectName);
                                    }
                                }
                            }
                        }
                        break;
                    }
                };
            }
            if (JsonNeed == true)
            {
                return Json(OutputModel);
            }
            else {
                return View();
            }            
        }
    }
}
