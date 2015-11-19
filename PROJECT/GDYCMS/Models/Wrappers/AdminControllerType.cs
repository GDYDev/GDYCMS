using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.Entity;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using GDYCMS.Filters;
using GDYCMS.Models;
using System.IO;


namespace GDYCMS.Models.Wrappers
{
    /// <summary>
    /// Admin controller base class. All administrator controllers would be inherited from this class.
    /// <para>Contain all backend functions for material and content edition</para>
    /// </summary>
    public class AdminControllerType:CMSController
    {
        /// <summary>
        /// This internal class contain all functions for working with users
        /// </summary>
        public class UsersAdminType : CMSController
        {
            /// <summary>
            /// Entity framework context
            /// </summary>
            private DB_GDYCMSEntities Context = new DB_GDYCMSEntities();
            /// <summary>
            /// Registered user list. This property get user list from database
            /// </summary>
            public List<string> UserList {
                get { 
                    List<string> ret = new List<string>();
                    var users = Context.UserProfile;
                    foreach (var user in users) {
                        ret.Add(user.UserName);
                    }
                    return ret;
                }
            }
            /// <summary>
            /// Register new user, use WebSecurity
            /// </summary>
            /// <param name="InputModel">-users model(universal model for working with users)</param>
            /// <returns>users model</returns>
            public UsrsModel AddNewUser(UsrsModel InputModel)
            {
                if (InputModel.UserName == null)
                {
                    InputModel.ErrorState = "User name is empty";
                }
                else {
                    if (Global.GlobalThings.isUserAdministrator(InputModel.UserName))
                    {
                        InputModel.ErrorState = "Administrator already exists";
                    }
                    else { // Check free name in user list
                        bool SameUserFound = false;
                        foreach (var item in UserList) {
                            if (Global.GlobalThings.AreNamesEqual(item, InputModel.UserName)) {
                                SameUserFound = true;
                                break;
                            }
                        }
                        if (SameUserFound == true)
                        {
                            InputModel.ErrorState = "This user already exists";
                        }
                        else { // User name is free
                            if ((InputModel.Password != InputModel.RetypePassword) || InputModel.Password==null)
                            {
                                InputModel.ErrorState = "Some password is empty or passwords are not equal";
                            }
                            else { // Passwords are equal
                                WebSecurity.CreateUserAndAccount(InputModel.UserName, InputModel.Password);
                                InputModel.ErrorState = "";
                            }
                        }
                    }
                }
                return InputModel;
            }
            /// <summary>
            /// Delete user. Use websecurity.
            /// </summary>
            /// <param name="InputModel">-users model(universal model for working with users)</param>
            /// <returns>-users model</returns>
            public UsrsModel DeleteUser(UsrsModel InputModel) {
                if (Global.GlobalThings.isUserAdministrator(InputModel.UserName) == false)
                {
                    if (InputModel.UserName != null || InputModel.UserName!="")
                    {
                        ((SimpleMembershipProvider)Membership.Provider).DeleteAccount(InputModel.UserName);
                        ((SimpleMembershipProvider)Membership.Provider).DeleteUser(InputModel.UserName, true);
                        InputModel.ErrorState = "";
                    }
                    else {
                        InputModel.ErrorState = "Sorry, but user name is empty";
                    }
                }
                else {
                    InputModel.ErrorState = "Sorry, but administrator can not delete himself";
                }
                return InputModel;
            }
            /// <summary>
            /// Change password
            /// </summary>
            /// <param name="InputModel">-users model(universal model for working with users)</param>
            /// <returns>-users model</returns>
            public UsrsModel ChangePassword(UsrsModel InputModel) {
                if (InputModel.UserName != null || InputModel.UserName != "")
                {
                    if (InputModel.NewPassword != InputModel.RetypeNewPassword)
                    {
                        InputModel.ErrorState = "Sorry, but passwords are not equal";
                    }
                    else {
                        if (InputModel.NewPassword == null || InputModel.RetypeNewPassword == null) {
                            InputModel.ErrorState = "Some password is empty";
                        }
                        else {
                            var token = WebSecurity.GeneratePasswordResetToken(InputModel.UserName);
                            WebSecurity.ResetPassword(token, InputModel.NewPassword);
                            InputModel.ErrorState = "Password has been changed successfully";
                        }
                    }
                }
                else {
                    InputModel.ErrorState = "Sorry, but user name is empty";
                }
                return InputModel;
            }
        }
        /// <summary>
        /// example of UsersAdminType
        /// </summary>
        private UsersAdminType _usersCntrl = new UsersAdminType();
        /// <summary>
        /// Users admin type access
        /// </summary>
        public UsersAdminType UserAdmin {
            get {
                return _usersCntrl;
            }
            set {
                _usersCntrl = value;
            }
        }
        /// <summary>
        /// Add material group operation
        /// </summary>
        /// <param name="Model">-editor model(universal model for data exchange)</param>
        /// <returns>true- operation has done successful</returns>        
        public bool ADDMaterialGroup(EditorModel Model){
            bool ret = true;
            MaterialGroups creation = new MaterialGroups{Name=Model.NewMaterialGroupName};
            try {
                DBBase.Context.MaterialGroups.Add(creation);
                DBBase.Context.SaveChanges();
            }
            catch{
                ret = false;
            }
            return ret;
        }
        /// <summary>
        /// Delete material group operation
        /// </summary>
        /// Remove material group and all materials that has same MaterialGroupID
        /// <param name="Model">-editor model(universal model for data exchange)</param>
        /// <returns>true- operation has done successful</returns>
        public bool DeleteMaterialGroup(EditorModel Model) {
            bool ret = false;
            var itemToRemove = DBBase.Context.MaterialGroups.SingleOrDefault(p => p.Name == Model.SelectedMaterialGroup);
            if (itemToRemove != null) {
                // Delete materials before delete material group
                var materialsToRemove = DBBase.Context.Materials.Where(u => u.MaterialGroupID == itemToRemove.MaterialGroupID);
                DBBase.Context.Materials.RemoveRange(materialsToRemove);
                // Delete material group
                DBBase.Context.MaterialGroups.Remove(itemToRemove);
                DBBase.Context.SaveChanges();
                ret = true;
            }
            return ret;
        }
        /// <summary>
        /// Rename material group operation
        /// </summary>
        /// <param name="Model">-editor model(universal model for data exchange)</param>
        /// <returns>true- operation has done successful</returns>
        public bool RenameMaterialGroup(EditorModel Model) {
            bool ret = false;
            var foundedMaterialGroup = DBBase.Context.MaterialGroups.SingleOrDefault(p => p.Name == Model.SelectedMaterialGroup);
            if (foundedMaterialGroup != null) {
                foundedMaterialGroup.Name = Model.NewMaterialGroupName;
                DBBase.Context.Entry(foundedMaterialGroup).State = System.Data.Entity.EntityState.Modified;
                DBBase.Context.SaveChanges();
            }
            return ret;
        }
        /// <summary>
        /// Rename material operation
        /// </summary>
        /// <param name="Model">editor model(universal model for data exchange)</param>
        /// <returns>true- operation has done successful</returns>
        public bool RenameMaterial(EditorModel Model) {
            bool ret = false;
            var foundedMaterialGroup = DBBase.Context.MaterialGroups.SingleOrDefault(p => p.Name == Model.SelectedMaterialGroup);
            if (foundedMaterialGroup != null) {
                var foundedMaterial = DBBase.Context.Materials.SingleOrDefault(p => p.Name == Model.SelectedMaterial && p.MaterialGroupID == foundedMaterialGroup.MaterialGroupID); //Выбираем материал по Имени и ID мат.групппы
                if (foundedMaterial != null) {
                    foundedMaterial.Name = Model.NewMaterialName;
                    DBBase.Context.Entry(foundedMaterial).State = System.Data.Entity.EntityState.Modified;
                    DBBase.Context.SaveChanges();
                }
            }
            return ret;
        }
        /// <summary>
        /// Add or modify material group
        /// <para>If there are no material with selected name, material would be created</para>
        /// <para>If material with selected name exist, it would be updated</para>
        /// </summary>
        /// <param name="Model">editor model(universal model for data exchange)</param>
        /// <returns>true- operation has done successful</returns>
        public bool AddOrModifyMaterial(EditorModel Model) {
            bool ret = true;
            var foundedMaterialGroup = DBBase.Context.MaterialGroups.SingleOrDefault(p => p.Name == Model.SelectedMaterialGroup);
            int DetectedMaterialGroupID;
            int DetectedUserID = GetUserIDByName(User.Identity.Name);
            if (foundedMaterialGroup != null)
            {
                DetectedMaterialGroupID = foundedMaterialGroup.MaterialGroupID;
                var foundedMaterial = DBBase.Context.Materials.SingleOrDefault(p => p.Name == Model.SelectedMaterial && p.MaterialGroupID == DetectedMaterialGroupID); //Выбираем материал по Имени и ID мат.групппы
                if (foundedMaterial == null)
                {
                    Materials creation = new Materials { MaterialGroupID = DetectedMaterialGroupID, Name = Model.NewMaterialName, HTML = Model.HTML, FirstCreatorID = DetectedUserID, LastModifierID = DetectedUserID, LastChange = DateTime.Now, isPublished = Model.isPublished };
                    DBBase.Context.Materials.Add(creation);
                    DBBase.Context.SaveChanges();
                }
                else { // Founded material
                    Materials Modification = DBBase.Context.Materials.SingleOrDefault(p => p.Name == Model.SelectedMaterial && p.MaterialGroupID == DetectedMaterialGroupID);
                    Modification.HTML = Model.HTML;
                    Modification.LastModifierID = DetectedUserID;
                    Modification.LastChange = DateTime.Now;
                    Modification.isPublished = Model.isPublished;
                    DBBase.Context.Entry(Modification).State = EntityState.Modified;
                    DBBase.Context.SaveChanges();
                }

            }            
            return ret;
        }
        /// <summary>
        /// Delete material operation
        /// </summary>
        /// <param name="Model">editor model(universal model for data exchange)</param>
        /// <returns>true- operation has done successful</returns>
        public bool DeleteMaterial(EditorModel Model) {
            bool ret = false;
            int DetectedMaterialGroup=DBBase.Context.MaterialGroups.SingleOrDefault(p=>p.Name==Model.SelectedMaterialGroup).MaterialGroupID;
            Materials foundedMaterial = DBBase.Context.Materials.SingleOrDefault(p => p.Name == Model.SelectedMaterial && p.MaterialGroupID == DetectedMaterialGroup);
            if (foundedMaterial != null) {
                DBBase.Context.Materials.Remove(foundedMaterial);
                DBBase.Context.SaveChanges();
            }
            return ret;
        }
        /// <summary>
        /// Load material group list operation
        /// <para>You give class example to this procedure and your class example would be filled by list of material groups</para>
        /// </summary>
        /// <param name="model">editor model(universal model for data exchange). Use this class "as pointer".</param>
        public void LoadMaterialGroupsToModel(EditorModel model) {
            model.MaterialGroupList = new List<string>();
            foreach (var item in DBBase.Context.MaterialGroups)
            {
                model.MaterialGroupList.Add(item.Name);
            }
        }
        /// <summary>
        /// Load material list for selected material group
        /// <para>You give class example to this procedure(with filled SelectedMaterialGroup) and your class example would be filled by list of materials for your group</para>
        /// </summary>
        /// <param name="model">editor model(universal model for data exchange). Use this class "as pointer"</param>
        public void LoadMaterialListForSelectedGroup(EditorModel model) {
            model.MaterialList = new List<string>();
            int DetectedMaterialGroup=DBBase.Context.MaterialGroups.SingleOrDefault(p=>p.Name==model.SelectedMaterialGroup).MaterialGroupID;
            var mat = DBBase.Context.Materials.Where(p => p.MaterialGroupID == DetectedMaterialGroup);
            foreach (var item in mat) {
                model.MaterialList.Add(item.Name);
            }
        }
        /// <summary>
        /// Load selected material.
        /// <para>Load selected material by selected material group name and material name.</para>
        /// </summary>
        /// <param name="Model">editor model(universal model for data exchange)</param>
        public void LoadSelectedMaterial(EditorModel Model) {
            int DetectedMaterialGroup = DBBase.Context.MaterialGroups.SingleOrDefault(p => p.Name == Model.SelectedMaterialGroup).MaterialGroupID;
            var mat = DBBase.Context.Materials.SingleOrDefault(p => p.Name == Model.SelectedMaterial && p.MaterialGroupID == DetectedMaterialGroup);

            Model.HTML = mat.HTML;
            Model.FirstCreator = DBBase.Context.UserProfile.SingleOrDefault(p => p.UserId == mat.FirstCreatorID).UserName;
            Model.isPublished=mat.isPublished;
            if (mat.LastChange != null)
            {
                Model.LastChange = (DateTime)mat.LastChange;
            }
            Model.LastModifier=DBBase.Context.UserProfile.SingleOrDefault(p => p.UserId == mat.LastModifierID).UserName;
            Model.MaterialName = mat.Name;
            Model.SelectedMaterialID = mat.MaterialID;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public AdminControllerType() {
            
        }
        /// <summary>
        /// Supply file manager with file and folder list
        /// </summary>
        /// <param name="ptrList">-pointer to List iterated by FileManagerModel.ItemType</param>
        /// <param name="ServerPath">- server path to target folder. Not mapped path. It has Server.MapPath inside.</param>
        public void SupplyFileManagerPanel(List<FileManagerModel.ItemType> ptrList, string ServerPath)
        {
            var Dirs = Directory.GetDirectories(Server.MapPath(ServerPath)).Select(f => new DirectoryInfo(f).Name);
            foreach (var item in Dirs)
            {
                ptrList.Add(new FileManagerModel.ItemType { Name = item.ToString(), Type = "folder" });
            }

            var Files = Directory.EnumerateFiles(Server.MapPath(ServerPath));
            foreach (var item in Files)
            {
                ptrList.Add(new FileManagerModel.ItemType { Name = Path.GetFileName(item), Type = "file" });
            }
        }
    }
}