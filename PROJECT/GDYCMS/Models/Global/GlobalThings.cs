using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace GDYCMS.Models.Global
{
    /// <summary>
    /// Global static class with constants and global functions.
    /// </summary>
    public static class GlobalThings
    {
        /// <summary>
        /// Administrator name. Now- default name is administrator
        /// </summary>
        public static string AdministratorName = "administrator";

        public static string[] MainPageEditorTarget = new string[] { "UpperMenu", "LeftMenuUp", "LeftMenuDown", "RightMenuUp", "RightMenuDown", "CentralContent", "Footer" };


        /// <summary>
        /// Container type list
        /// </summary>
        public enum ContainerTypes {UpperMenu=(int)0,LeftMenuUp,LeftMenuDown,RightMenuUp,RightMenuDown,CentralContent,Footer }
        /// <summary>
        /// This function check- is user name=administrator
        /// </summary>
        /// <param name="Name">- user name</param>
        /// <returns>true if it's administrator, false otherwise</returns>
        public static bool isUserAdministrator(string Name) {
            bool ret = false;
            if (Name.ToLower() == AdministratorName) {
                ret = true;
            }
            return ret;
        }
        /// <summary>
        /// Compare two user names
        /// </summary>
        /// <param name="Name1">-user name 1</param>
        /// <param name="Name2">-user name 2</param>
        /// <returns>true if names are equal, false otherwise</returns>
        public static bool AreNamesEqual(string Name1, string Name2) {
            bool ret = false;
            if (Name1.ToLower() == Name2.ToLower()) {
                ret = true;
            }
            return ret;
        }
    }
}