//=====================================================================================================================
//
// ideMobi copyright 2017 
// All rights reserved by ideMobi
//
//=====================================================================================================================

using System;
#if UNITY_EDITOR
//=====================================================================================================================
namespace DoxygenGeneratorWindow
{
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    public partial class DGWConstants
    {
        //-------------------------------------------------------------------------------------------------------------
        // Menu Strings
        //#if UNITY_MENU_IDEMOBI
        //public const string K_MENU_BASE = UMIConstants.K_MENU_IDEMOBI;
        //#else
        //public const string K_MENU_BASE = "Window";
        //#endif
        public const string K_MENU_BASE = "Window";
        //-------------------------------------------------------------------------------------------------------------
        // Idemobi alert Strings
        public const string K_ALERT_IDEMOBI_MENU = "/Developed by idéMobi©";
        public const string K_ALERT_IDEMOBI_TITLE = "idéMobi";
        public const string K_ALERT_IDEMOBI_MESSAGE = "idéMobi Doxygen Generator Window.";
        public const string K_ALERT_IDEMOBI_OK = "Thanks!";
        public const string K_ALERT_IDEMOBI_SEE_DOC = "See online docs";
        public const string K_ALERT_IDEMOBI_DOC_HTTP = "http://www.idemobi.com/DoxygenGeneratorWindow/";
        //-------------------------------------------------------------------------------------------------------------
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
}
//=====================================================================================================================
#endif