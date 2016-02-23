using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using GDYCMS.Models;
using GDYCMS.Models.Global;
using GDYCMS.Models.Wrappers;

namespace GDYCMS.Controllers
{
    #region Comments
    /// <summary>
    /// This controller create site interface.
    /// </summary>
       
    /// 
    /// View:Index.cshtml (empty view that receive model (ViewerModel class (<see cref="GDYCMS.Models.ViewerModel"/>) ) and use layout:_BaseLayout.cshtml, which also use same model. 
    /// _BaseLayout.cshtml use partial views and take them model.
    /// Partial views are: 
    /// <list type="Partial views">
    /// <item>
    /// _CentralContentPartial.cshtml
    /// <description>Main content</description>
    /// </item>
    /// <item>
    /// _LeftMenuDownPartial.cshtml
    /// <description>
    ///  Left down menu
    /// </description>
    /// </item>
    /// <item>
    /// _LeftMenuUpPartial.cshtml
    /// <description>
    /// Left up menu
    /// </description>
    /// </item>
    /// <item>
    /// _RightMenuDownPartial.cshtml
    /// <description>
    /// Right menu down
    /// </description>
    /// </item>
    /// <item>
    /// _RightMenuUpPartial.cshtml
    /// <description>
    /// Right up menu
    /// </description>
    /// </item>
    /// <item>
    /// _UpMenuPartial.cshtml
    /// <description>
    /// Up menu
    /// </description>
    /// </item>
    /// </list>
    #endregion


    public class HomeController : CMSController
    {
        private ViewerModel m; 
        #region Method Index

           
        /// <summary>
        /// Action method.
        /// </summary>
        /// <param name="id">is article identifier in database. See table:materials</param>
        /// <returns>View as HTML result</returns>
    
        public ActionResult Index(int? id)
        {
            this.BaseThings.CountThisUserInStatistics();

            m = new ViewerModel();
            FoundedMaterialQueryResult CentralContent = this.GetHtmlByID(new FoundedMaterialQueryResult { id = id, IsAuthenticated = User.Identity.IsAuthenticated });            
            if (CentralContent.Result == FoundedMaterialQueryResult.QueryResults.NotFound)
            {
                m.CentralContent = this.BaseThings.NotFoundPage;
            }
            else {
                if (CentralContent.Result == FoundedMaterialQueryResult.QueryResults.Published)
                {
                    m.CentralContent = CentralContent.Container;
                }
                else {
                    if (CentralContent.IsAuthenticated == true)
                    {
                        m.CentralContent = CentralContent.Container;
                    }
                    else {
                        m.CentralContent = this.BaseThings.AccessLimitedPage;
                    }
                }
            }
            var AnotherMainPageParts = DBBase.Context.CentralMaterials;
            foreach (var item in AnotherMainPageParts) {
                string ErrMsg = this.BaseThings.AccessLimitedPage; ;
                if (item.isHidden == true)
                {
                    if (User.Identity.IsAuthenticated == false) {
                        item.HTML = ErrMsg;
                    }
                }
                    switch (item.ContainerID)
                    {
                        case (int)GlobalThings.ContainerTypes.Footer: {
                            
                            m.Footer = item.HTML;
                            break;
                        }
                        case (int)GlobalThings.ContainerTypes.LeftMenuDown: {
                            m.LeftMenuDown = item.HTML;
                            break;
                        }
                        case (int)GlobalThings.ContainerTypes.LeftMenuUp: {
                            m.LeftMenuUp = item.HTML;
                            break;
                        }
                        case (int)GlobalThings.ContainerTypes.RightMenuDown: {
                            m.RightMenuDown = item.HTML;
                            break;
                        }
                        case (int)GlobalThings.ContainerTypes.RightMenuUp: {
                            m.RightMenuUp = item.HTML;
                            break;
                        }
                        case (int)GlobalThings.ContainerTypes.UpperMenu: {
                            m.UpperMenu = item.HTML;
                            break;
                        }
                }
            }
            return View(m);
        }
        #endregion
    }
}


