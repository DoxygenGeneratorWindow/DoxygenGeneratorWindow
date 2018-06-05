//=====================================================================================================================
//
// ideMobi copyright 2017 
// All rights reserved by ideMobi
//
//=====================================================================================================================

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
//=====================================================================================================================
namespace DoxygenGeneratorWindow
{
    /// <summary>
    /// DGW editor menu.
    /// </summary>
    public class DGWEditorMenu
    {
        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="DoxygenGeneratorWindow.DGWEditorMenu"/> class.
        /// </summary>
        public DGWEditorMenu()
        {
        }
        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Idemobis the editor info show.
        /// </summary>
        //		[MenuItem (DGWConstants.K_MENU_BASE+DGWConstants.K_ALERT_IDEMOBI_MENU,false, 0)]
        //		public static void IdemobiInfoShow()
        //		{
        //			if (EditorUtility.DisplayDialog (DGWConstants.K_ALERT_IDEMOBI_TITLE,
        //				DGWConstants.K_ALERT_IDEMOBI_MESSAGE,
        //				DGWConstants.K_ALERT_IDEMOBI_OK,
        //				DGWConstants.K_ALERT_IDEMOBI_SEE_DOC)) {
        //			} else {
        //				Application.OpenURL (DGWConstants.K_ALERT_IDEMOBI_DOC_HTTP);
        //			}
        //		}
        //-------------------------------------------------------------------------------------------------------------
    }
}
//=====================================================================================================================
#endif