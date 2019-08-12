//=====================================================================================================================
//
// ideMobi copyright 2017 
// All rights reserved by ideMobi
//
//=====================================================================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;

using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;

//=====================================================================================================================
namespace DoxygenGeneratorWindow
{
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    /// <summary>
    /// DGW Documentation generator is a class to generate the documentation by Doxygen automatically
    /// </summary>
    /// 
    public class DGWDocumentationGenerator : EditorWindow
    {
        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// The DOXYGEN URL.
        /// </summary>
        public const string K_URL_DOXYGEN = "http://www.stack.nl/~dimitri/doxygen/download.html";
        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// The shared instance.
        /// </summary>
        private static DGWDocumentationGenerator kSharedInstance;
        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// The config of this project.
        /// </summary>
        public DGWConfig Config;
        /// <summary>
        /// The scroll position of scrollview.
        /// </summary>
        private Vector2 ScrollPosition;
        /// <summary>
        /// Work in progress or not.
        /// </summary>
        public bool WorkInProgress = false;
        /// <summary>
        /// The state of the group reset.
        /// </summary>
        private static bool kGroupResetState = false;
        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Ascencor to shared instance.
        /// </summary>
        /// <returns>The shared instance.</returns>
        public static DGWDocumentationGenerator SharedInstance()
        {
            //			UnityEngine.Debug.Log ("STATIC DGWDocumentationGenerator SharedInstance()");
            if (kSharedInstance == null)
            {
                kSharedInstance = EditorWindow.GetWindow(typeof(DGWDocumentationGenerator)) as DGWDocumentationGenerator;
                kSharedInstance.position = new Rect(10, 10, 300, 500);
            }
            return kSharedInstance;
        }
        //-------------------------------------------------------------------------------------------------------------
#if UNITY_MENU_IDEMOBI
        [MenuItem(DGWConstants.K_MENU_BASE + "/Doxygen© Generator Window", false, 900)]
#else
		[MenuItem (DGWConstants.K_MENU_BASE + "/Doxygen© Generator Window", false, 100)]
#endif
        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Prepare the window.
        /// </summary>
        static void PrepareWindow()
        {
            //			UnityEngine.Debug.Log ("STATIC DGWDocumentationGenerator PrepareWindow()");
            SharedInstance().ShowUtility();
            SharedInstance().Focus();
        }
        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Raises the enable event.
        /// </summary>
        public void OnEnable()
        {
            //			UnityEngine.Debug.Log ("DGWDocumentationGenerator OnEnable()");
            LoadPreferences();
        }
        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Save the preferences.
        /// </summary>
        void SavePreferences()
        {
            //			UnityEngine.Debug.Log ("DGWDocumentationGenerator SavePreferences()");
            string tJson = JsonUtility.ToJson(Config);
            File.WriteAllText(PathConfig(), tJson);
        }
        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Load the preferences.
        /// </summary>
        void LoadPreferences()
        {
            //			UnityEngine.Debug.Log ("DGWDocumentationGenerator LoadPreferences()");
            //Debug.Log ("LoadPreferences");
            string tPath = PathConfig();
            if (File.Exists(tPath))
            {
                string tJSONString = File.ReadAllText(tPath);
                Config = JsonUtility.FromJson<DGWConfig>(tJSONString);
            }
            else
            {
                Config = new DGWConfig();
                Config.EmptyFill();
            }
        }
        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Conversion bool to string YES or NO for config fill.
        /// </summary>
        /// <returns>The or no.</returns>
        /// <param name="sValue">If set to <c>true</c> value.</param>
        public string YesOrNo(bool sValue)
        {
            //			UnityEngine.Debug.Log ("DGWDocumentationGenerator YesOrNo()");
            if (sValue == true)
            {
                return "YES";
            }
            else
            {
                return "NO";
            }
        }
        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Raises the GUI event.
        /// </summary>
        void OnGUI()
        {
            //			UnityEngine.Debug.Log ("DGWDocumentationGenerator OnGUI()");
            EditorGUI.BeginChangeCheck();
            // set size of window
            this.minSize = new Vector2(300, 300);
            this.maxSize = new Vector2(500, 1024);
            // set title of window
            Texture2D tImageIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(DGWFindPackage.PathOfPackage("/Editor/DGWIcon.psd"));
            titleContent = new GUIContent("Doc", tImageIcon);

            //----------
            //SCROLL ZONE START
            //----------
            ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition);

            if (WorkInProgress)
            {
                EditorGUILayout.HelpBox("Work in progress!", MessageType.Info);
            }
            //----------


            //----------
            // exe box
            //----------
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            float tPathButtonWidth = 20.0F;

            FileInfo tExePath = null;
#if UNITY_EDITOR_OSX
            // form to .app 
            // title
            GUILayout.Label("Path to Doxygen.app © application", EditorStyles.boldLabel);
            // indent add
            EditorGUI.indentLevel++;
            // select Doxygen.app
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Path to Doxygen.app", Config.PathToDOxygenOSX);
            if (GUILayout.Button("...", EditorStyles.miniButtonRight, GUILayout.Width(tPathButtonWidth)))
            {
                Config.PathToDOxygenOSX = EditorUtility.OpenFilePanel("Where is Doxygen.app installed?", "/Applications/", "");
                GUI.FocusControl(null);
            }
            EditorGUILayout.EndHorizontal();
            // test selection and show donwload instruction
            tExePath = new FileInfo(Config.PathToDOxygenOSX + "/Contents/Resources/doxygen");
            if (!tExePath.Exists)
            {
            }
            // indent remove
            EditorGUI.indentLevel--;
            // finsih form to .app
#else
			
// form to .exe 
// title
GUILayout.Label ("Path to Doxygen.exe © application", EditorStyles.boldLabel);
// indent add
EditorGUI.indentLevel ++;
// select Doxygen.app
EditorGUILayout.BeginHorizontal ();
Config.PathToDOxygenWIN = EditorGUILayout.TextField ("Path to Doxygen.exe", Config.PathToDOxygenWIN);
if (GUILayout.Button ("...", EditorStyles.miniButtonRight, GUILayout.Width (tPathButtonWidth)))
{
Config.PathToDOxygenWIN = EditorUtility.OpenFilePanel ("Where is Doxygen.exe installed?", "", "");
GUI.FocusControl(null);
}
EditorGUILayout.EndHorizontal ();
            // test selection and show donwload instruction
            if (!Config.PathToDOxygenWIN.Equals(""))
            {
                tExePath = new FileInfo(Config.PathToDOxygenWIN);
                if (!tExePath.Exists)
                {
                }
            }
// indent remove
EditorGUI.indentLevel --;
            // finsih form to .app
#endif
            // If no exe disable
            bool tExePathIsValid = true;
            if (tExePath == null)
            {
                tExePathIsValid = false;
            }
            else if (tExePath.Exists == false)
            {
                tExePathIsValid = false;
            }


            if (tExePathIsValid == false)
            {
#if UNITY_EDITOR_OSX
                EditorGUILayout.HelpBox("Download MacOSX© version at " + K_URL_DOXYGEN + "!", MessageType.Warning);
                Rect tLabelRect = GUILayoutUtility.GetLastRect();

                if (Event.current.type == EventType.MouseUp && tLabelRect.Contains(Event.current.mousePosition))
                {
                    Application.OpenURL(K_URL_DOXYGEN);
                }
                if (GUILayout.Button("Go to website !"))
                {
                    Application.OpenURL(K_URL_DOXYGEN);
                }
#else
			EditorGUILayout.HelpBox("Download Windows© version at " + K_URL_DOXYGEN + "!", MessageType.Warning);
			Rect tLabelRect = GUILayoutUtility.GetLastRect();
			if (Event.current.type == EventType.MouseUp && tLabelRect.Contains(Event.current.mousePosition))
			{
				Application.OpenURL(K_URL_DOXYGEN);
			}
			if (GUILayout.Button("Go to website !"))
			{
				Application.OpenURL(K_URL_DOXYGEN);
			}

#endif
            }
            GUILayout.EndVertical();
            //----------

            EditorGUI.BeginDisabledGroup(!tExePathIsValid);

            //----------
            // Project box
            //----------
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            // form project
            EditorGUILayout.LabelField("Project settings", EditorStyles.boldLabel);
            // indent add
            EditorGUI.indentLevel++;
            Config.ProjectName = EditorGUILayout.TextField("Project Name", Config.ProjectName);
            Config.ProjectSynopsis = EditorGUILayout.TextField("Project Synopsis", Config.ProjectSynopsis);
            Config.ProjectVersion = EditorGUILayout.TextField("Project Version", Config.ProjectVersion);
            // indent remove
            EditorGUI.indentLevel--;
            // finish form project


            GUILayout.EndVertical();
            //----------

            //----------
            // InPut box
            //----------
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            // form source code
            EditorGUILayout.LabelField("Input", EditorStyles.boldLabel);
            // indent add
            EditorGUI.indentLevel++;
            UnityEngine.Object tFolderObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(Config.FolderName);
            tFolderObject = EditorGUILayout.ObjectField("Folder sources assets", tFolderObject, typeof(UnityEngine.Object), false);
            if (tFolderObject != null)
            {
                string tFolderPath = AssetDatabase.GetAssetPath(tFolderObject);
                if (System.IO.Directory.Exists(tFolderPath) == true)
                {
                    Config.FolderName = tFolderPath;
                }
            }
            else
            {
                Config.FolderName = "";
                EditorGUILayout.HelpBox("Drag and drop folder from your project!", MessageType.Warning);
            }
            // indent remove
            EditorGUI.indentLevel--;
            // finish form source code 

            GUILayout.EndVertical();
            //----------

            //----------
            // Outut  box
            //----------
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            // form ouput
            EditorGUILayout.LabelField("Output", EditorStyles.boldLabel);
            // indent add
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Folder output path", Config.FolderOutput);
            if (GUILayout.Button("...", EditorStyles.miniButtonRight, GUILayout.Width(tPathButtonWidth)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Application.dataPath) + "/Documentation");
                string tFolderOutput = EditorUtility.OpenFolderPanel("Where do you want generate documentation?", Path.GetDirectoryName(Application.dataPath) + "/Documentation", "");
                //UnityEngine.Debug.Log ("tFolderOutput = " + tFolderOutput);
                //UnityEngine.Debug.Log ("Application.dataPath = " + Application.dataPath);
                Config.SetFolderOutputAbsolute(tFolderOutput);
            }
            EditorGUILayout.EndHorizontal();
            bool tOutIsOk = true;
            if (Config.FolderOutput == null || Config.FolderOutput == "")
            {
                tOutIsOk = false;
            }
            else
            {
                DirectoryInfo tOutPath = new DirectoryInfo(Config.FolderOutputAbsolute());
                tOutIsOk = tOutPath.Exists;
            }
            if (tOutIsOk == false)
            {
                EditorGUILayout.HelpBox("Select output folder", MessageType.Warning);
            }


            EditorGUI.indentLevel--;
            GUILayout.EndVertical();
            //----------


            //----------
            // Output box
            //----------
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Output settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            Config.GenerateHTML = EditorGUILayout.Toggle("Generate HTML", Config.GenerateHTML);
            EditorGUI.indentLevel++;
            EditorGUI.BeginDisabledGroup(!Config.GenerateHTML);
            Config.HTMLColor = EditorGUILayout.ColorField("Tint Color", Config.HTMLColor);
            EditorGUI.EndDisabledGroup();
            EditorGUI.indentLevel--;
            Config.GenerateXML = EditorGUILayout.Toggle("Generate XML", Config.GenerateXML);
            EditorGUI.indentLevel++;
            EditorGUI.BeginDisabledGroup(!Config.GenerateXML);
            EditorGUI.EndDisabledGroup();
            EditorGUI.indentLevel--;
            Config.GenerateRTF = EditorGUILayout.Toggle("Generate RTF", Config.GenerateRTF);
            EditorGUI.indentLevel++;
            EditorGUI.BeginDisabledGroup(!Config.GenerateRTF);
            EditorGUI.EndDisabledGroup();
            EditorGUI.indentLevel--;
            Config.GenerateLatex = EditorGUILayout.Toggle("Generate Latex", Config.GenerateLatex);
            EditorGUI.indentLevel++;
            EditorGUI.BeginDisabledGroup(!Config.GenerateLatex);
            EditorGUI.EndDisabledGroup();
            EditorGUI.indentLevel--;

            if (tOutIsOk == true)
            {
                if (Config.GenerateHTML == false && Config.GenerateLatex == false && Config.GenerateXML == false && Config.GenerateRTF == false)
                {
                    tOutIsOk = false;
                }
            }
            // indent remove
            EditorGUI.indentLevel--;
            // finish form output 
            GUILayout.EndVertical();
            //----------

            //----------
            // Analyze box
            //----------
            // form analyze
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Analyze settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            Config.Recursive = EditorGUILayout.Toggle("Recursive", Config.Recursive);
            // indent remove
            EditorGUI.indentLevel--;
            // finish form output 
            GUILayout.EndVertical();
            //----------

            //----------
            // save box
            //----------
            //			EditorGUILayout.BeginVertical (EditorStyles.helpBox);
            //			EditorGUILayout.LabelField ("Save configurations", EditorStyles.boldLabel);
            //			if (GUILayout.Button ("Save")) {
            //				SavePreferences ();
            //			}
            //			GUILayout.EndVertical ();
            //----------

            //----------
            // Analyze box
            //----------
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.BeginDisabledGroup(!tOutIsOk);
            EditorGUILayout.LabelField("Generate documentation", EditorStyles.boldLabel);
            if (GUILayout.Button("Generate"))
            {
                RunDoxygen();
            }
            EditorGUI.EndDisabledGroup();
            GUILayout.EndVertical();

            //----------

            EditorGUI.EndDisabledGroup();

            EditorGUI.EndDisabledGroup();

            //----------
            // Reset box
            //----------
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            kGroupResetState = EditorGUILayout.Foldout(kGroupResetState, "Reset configurations");
            //EditorGUILayout.LabelField ("Reset configurations", EditorStyles.boldLabel);
            if (kGroupResetState == true)
            {
                Color tBackgroundColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(1.0F, 0.7F, 0.7F, 1.0F);
                if (GUILayout.Button("Reset"))
                {
                    Config = new DGWConfig();
                    Config.EmptyFill();
                    SavePreferences();
                }
                GUI.backgroundColor = tBackgroundColor;
            }
            GUILayout.EndVertical();
            GUILayout.Space(10.0F);
            //----------

            //----------
            // idemobi box
            //----------
            GUIStyle tStyle = new GUIStyle(GUI.skin.label);
            tStyle.richText = true;
            tStyle.wordWrap = true;

            GUIStyle tStyleBold = new GUIStyle(GUI.skin.label);
            tStyleBold.richText = true;
            tStyleBold.wordWrap = true;
            tStyleBold.fontStyle = FontStyle.Bold;
            tStyleBold.alignment = TextAnchor.MiddleCenter;

            GUIStyle tStyleItalic = new GUIStyle(GUI.skin.label);
            tStyleItalic.richText = true;
            tStyleItalic.wordWrap = true;
            tStyleItalic.fontStyle = FontStyle.Italic;

            GUIStyle tStyleImage = new GUIStyle(GUI.skin.label);
            tStyleImage.imagePosition = ImagePosition.ImageOnly;
            tStyleImage.alignment = TextAnchor.MiddleCenter;

#if UNITY_MENU_IDEMOBI
#else
			Texture2D tImageLogo = AssetDatabase.LoadAssetAtPath<Texture2D> (DGWFindPackage.PathOfPackage ("/Editor/IdemobiIconAlpha.png"));
			GUILayout.Space (20.0F);
			GUILayout.FlexibleSpace ();
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			GUIContent tContent = new GUIContent (tImageLogo);
			GUILayout.Label (tContent, tStyleImage, GUILayout.Height (64.0F), GUILayout.Width (64.0F));
			GUILayout.FlexibleSpace ();
			GUILayout.EndHorizontal ();
			GUILayout.Label (new GUIContent ("idéMobi ©"), tStyleBold);
			GUILayout.Label (new GUIContent ("Copyright 2017"), tStyleBold);
			tStyle.alignment = TextAnchor.MiddleCenter;
			GUILayout.Label (new GUIContent ("idéMobi is a french society. All rights based on french law."), tStyle);
#endif
            GUILayout.Space(20.0F);
            GUILayout.Label(new GUIContent("Thanks to Doxygen ©!"), tStyleBold);
            GUILayout.Label(new GUIContent("Copyright 1997-2016 by Dimitri van Heesch."), tStyleBold);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Go to Doxygen© website !"))
            {
                Application.OpenURL(K_URL_DOXYGEN);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(20.0F);
            //----------


            EditorGUILayout.EndScrollView();
            //----------
            //SCROLL ZONE END
            //----------

            if (EditorGUI.EndChangeCheck())
            {
                SavePreferences();
            }
        }
        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Return the path of the config.
        /// </summary>
        /// <returns>The Path.</returns>
        private string PathConfig()
        {
            //			UnityEngine.Debug.Log ("DGWDocumentationGenerator PathConfig()");
            string rReturn = Application.dataPath + "/DGWConfig.txt";
            return rReturn;
        }
        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Return  the path of doxygen config.
        /// </summary>
        /// <returns>The doxygen Path.</returns>
        private string PathDoxygenConfig()
        {
            //			UnityEngine.Debug.Log ("DGWDocumentationGenerator PathDoxygenConfig()");
            string rReturn = Application.dataPath + "/DGWDoxygenConfig.txt";
            return rReturn;
        }
        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Run doxygen!
        /// </summary>
        void RunDoxygen()
        {
            //			UnityEngine.Debug.Log ("DGWDocumentationGenerator RunDoxygen()");
            // Create a config file 
            float tHue, tSaturation, tVa;
            int tHueInt, tSaturationInt, tVaInt;
            Color.RGBToHSV(Config.HTMLColor, out tHue, out tSaturation, out tVa);
            tHueInt = (int)Mathf.Floor(tHue * 360.0F);
            tSaturationInt = (int)Mathf.Floor(tSaturation * 100.0F);
            tVaInt = (int)Mathf.Floor(tVa * 100.0F);

            //UnityEngine.Debug.Log (" tHue : " + tHue.ToString () + "   => " + tHueInt.ToString ());
            //UnityEngine.Debug.Log (" tSaturation : " + tSaturation.ToString () + "   => " + tSaturationInt.ToString ());
            //UnityEngine.Debug.Log (" tVa : " + tVa.ToString () + "   => " + tVaInt.ToString ());

            string tConfig = "# Doxyfile 1.8.13" + "\n" +
                             "" + "\n" +
                             "#---------------------------------------------------------------------------" + "\n" +
                             "# Project related configuration options" + "\n" +
                             "#---------------------------------------------------------------------------" + "\n" +
                             "DOXYFILE_ENCODING      = UTF-8" + "\n" +
                             "PROJECT_NAME           = \"" + Config.ProjectName + "\"" + "\n" +
                             "PROJECT_NUMBER         = \"" + Config.ProjectVersion + "\"" + "\n" +
                             "PROJECT_BRIEF          = \"" + Config.ProjectSynopsis + "\"" + "\n" +
                             "PROJECT_LOGO           = " + "\n" +
                            "OUTPUT_DIRECTORY       = \"" + Config.FolderOutputAbsolute().Replace(" ", " ") + "\"\n" +
                             "CREATE_SUBDIRS         = YES" + "\n" +
                             "ALLOW_UNICODE_NAMES    = YES" + "\n" +
                             "OUTPUT_LANGUAGE        = English" + "\n" +
                             "BRIEF_MEMBER_DESC      = YES" + "\n" +
                             "REPEAT_BRIEF           = YES" + "\n" +
                             "ABBREVIATE_BRIEF       = \"The $name class\" \\" + "\n" +
                             "                         \"The $name widget\" \\" + "\n" +
                             "                         \"The $name file\" \\" + "\n" +
                             "                         is \\" + "\n" +
                             "                         provides \\" + "\n" +
                             "                         specifies \\" + "\n" +
                             "                         contains \\" + "\n" +
                             "                         represents \\" + "\n" +
                             "                         a \\" + "\n" +
                             "                         an \\" + "\n" +
                             "                         the" + "\n" +
                             "ALWAYS_DETAILED_SEC    = NO" + "\n" +
                             "INLINE_INHERITED_MEMB  = NO" + "\n" +
                             "FULL_PATH_NAMES        = YES" + "\n" +
                             "STRIP_FROM_PATH        = " + "\n" +
                             "STRIP_FROM_INC_PATH    = " + "\n" +
                             "SHORT_NAMES            = NO" + "\n" +
                             "JAVADOC_AUTOBRIEF      = NO" + "\n" +
                             "QT_AUTOBRIEF           = NO" + "\n" +
                             "MULTILINE_CPP_IS_BRIEF = NO" + "\n" +
                             "INHERIT_DOCS           = YES" + "\n" +
                             "SEPARATE_MEMBER_PAGES  = NO" + "\n" +
                             "TAB_SIZE               = 4" + "\n" +
                             "ALIASES                = " + "\n" +
                             "TCL_SUBST              = " + "\n" +
                             "OPTIMIZE_OUTPUT_FOR_C  = YES" + "\n" +
                             "OPTIMIZE_OUTPUT_JAVA   = YES" + "\n" +
                             "OPTIMIZE_FOR_FORTRAN   = YES" + "\n" +
                             "OPTIMIZE_OUTPUT_VHDL   = YES" + "\n" +
                             "EXTENSION_MAPPING      = " + "\n" +
                             "MARKDOWN_SUPPORT       = YES" + "\n" +
                             "TOC_INCLUDE_HEADINGS   = 0" + "\n" +
                             "AUTOLINK_SUPPORT       = YES" + "\n" +
                             "BUILTIN_STL_SUPPORT    = NO" + "\n" +
                             "CPP_CLI_SUPPORT        = NO" + "\n" +
                             "SIP_SUPPORT            = NO" + "\n" +
                             "IDL_PROPERTY_SUPPORT   = YES" + "\n" +
                             "DISTRIBUTE_GROUP_DOC   = NO" + "\n" +
                             "GROUP_NESTED_COMPOUNDS = NO" + "\n" +
                             "SUBGROUPING            = YES" + "\n" +
                             "INLINE_GROUPED_CLASSES = NO" + "\n" +
                             "INLINE_SIMPLE_STRUCTS  = NO" + "\n" +
                             "TYPEDEF_HIDES_STRUCT   = NO" + "\n" +
                             "LOOKUP_CACHE_SIZE      = 0" + "\n" +
                             "#---------------------------------------------------------------------------" + "\n" +
                             "# Build related configuration options" + "\n" +
                             "#---------------------------------------------------------------------------" + "\n" +
                             "EXTRACT_ALL            = YES" + "\n" +
                             "EXTRACT_PRIVATE        = YES" + "\n" +
                             "EXTRACT_PACKAGE        = YES" + "\n" +
                             "EXTRACT_STATIC         = YES" + "\n" +
                             "EXTRACT_LOCAL_CLASSES  = YES" + "\n" +
                             "EXTRACT_LOCAL_METHODS  = YES" + "\n" +
                             "EXTRACT_ANON_NSPACES   = YES" + "\n" +
                             "HIDE_UNDOC_MEMBERS     = YES" + "\n" +
                             "HIDE_UNDOC_CLASSES     = YES" + "\n" +
                             "HIDE_FRIEND_COMPOUNDS  = YES" + "\n" +
                             "HIDE_IN_BODY_DOCS      = YES" + "\n" +
                             "INTERNAL_DOCS          = YES" + "\n" +
                             "CASE_SENSE_NAMES       = YES" + "\n" +
                             "HIDE_SCOPE_NAMES       = NO" + "\n" +
                             "HIDE_COMPOUND_REFERENCE= NO" + "\n" +
                             "SHOW_INCLUDE_FILES     = YES" + "\n" +
                             "SHOW_GROUPED_MEMB_INC  = YES" + "\n" +
                             "FORCE_LOCAL_INCLUDES   = YES" + "\n" +
                             "INLINE_INFO            = YES" + "\n" +
                             "SORT_MEMBER_DOCS       = YES" + "\n" +
                             "SORT_BRIEF_DOCS        = YES" + "\n" +
                             "SORT_MEMBERS_CTORS_1ST = YES" + "\n" +
                             "SORT_GROUP_NAMES       = YES" + "\n" +
                             "SORT_BY_SCOPE_NAME     = YES" + "\n" +
                             "STRICT_PROTO_MATCHING  = NO" + "\n" +
                             "GENERATE_TODOLIST      = YES" + "\n" +
                             "GENERATE_TESTLIST      = YES" + "\n" +
                             "GENERATE_BUGLIST       = YES" + "\n" +
                             "GENERATE_DEPRECATEDLIST= YES" + "\n" +
                             "ENABLED_SECTIONS       = " + "\n" +
                             "MAX_INITIALIZER_LINES  = 30" + "\n" +
                             "SHOW_USED_FILES        = YES" + "\n" +
                             "SHOW_FILES             = YES" + "\n" +
                             "SHOW_NAMESPACES        = YES" + "\n" +
                             "FILE_VERSION_FILTER    = " + "\n" +
                             "LAYOUT_FILE            = " + "\n" +
                             "CITE_BIB_FILES         = " + "\n" +
                             "#---------------------------------------------------------------------------" + "\n" +
                             "# Configuration options related to warning and progress messages" + "\n" +
                             "#---------------------------------------------------------------------------" + "\n" +
                             "QUIET                  = NO" + "\n" +
                             "WARNINGS               = YES" + "\n" +
                             "WARN_IF_UNDOCUMENTED   = YES" + "\n" +
                             "WARN_IF_DOC_ERROR      = YES" + "\n" +
                             "WARN_NO_PARAMDOC       = NO" + "\n" +
                             "WARN_AS_ERROR          = NO" + "\n" +
                             "WARN_FORMAT            = \"$file:$line: $text\"" + "\n" +
                             "WARN_LOGFILE           = " + "\n" +
                             "#---------------------------------------------------------------------------" + "\n" +
                             "# Configuration options related to the input files" + "\n" +
                             "#---------------------------------------------------------------------------" + "\n" +
                             "INPUT                  = \"" + Directory.GetParent(Application.dataPath) + "/" + Config.FolderName + "\"\n" +
                             "INPUT_ENCODING         = UTF-8" + "\n" +
                             "FILE_PATTERNS          = *.c \\" + "\n" +
                             "                         *.cs \\" + "\n" +
                             "                         *.cc \\" + "\n" +
                             "                         *.cxx \\" + "\n" +
                             "                         *.cpp \\" + "\n" +
                             "                         *.c++ \\" + "\n" +
                             "                         *.java \\" + "\n" +
                             "                         *.ii \\" + "\n" +
                             "                         *.ixx \\" + "\n" +
                             "                         *.ipp \\" + "\n" +
                             "                         *.i++ \\" + "\n" +
                             "                         *.inl \\" + "\n" +
                             "                         *.idl \\" + "\n" +
                             "                         *.ddl \\" + "\n" +
                             "                         *.odl \\" + "\n" +
                             "                         *.h \\" + "\n" +
                             "                         *.hh \\" + "\n" +
                             "                         *.hxx \\" + "\n" +
                             "                         *.hpp \\" + "\n" +
                             "                         *.h++ \\" + "\n" +
                             "                         *.cs \\" + "\n" +
                             "                         *.d \\" + "\n" +
                             "                         *.php \\" + "\n" +
                             "                         *.php4 \\" + "\n" +
                             "                         *.php5 \\" + "\n" +
                             "                         *.phtml \\" + "\n" +
                             "                         *.inc \\" + "\n" +
                             "                         *.m \\" + "\n" +
                             "                         *.markdown \\" + "\n" +
                             "                         *.md \\" + "\n" +
                             "                         *.mm \\" + "\n" +
                             "                         *.dox \\" + "\n" +
                             "                         *.py \\" + "\n" +
                             "                         *.pyw \\" + "\n" +
                             "                         *.f90 \\" + "\n" +
                             "                         *.f95 \\" + "\n" +
                             "                         *.f03 \\" + "\n" +
                             "                         *.f08 \\" + "\n" +
                             "                         *.f \\" + "\n" +
                             "                         *.for \\" + "\n" +
                             "                         *.tcl \\" + "\n" +
                             "                         *.vhd \\" + "\n" +
                             "                         *.vhdl \\" + "\n" +
                             "                         *.ucf \\" + "\n" +
                             "                         *.qsf" + "\n" +
                             "RECURSIVE              = " + YesOrNo(Config.Recursive) + "\n" +
                             "EXCLUDE                = " + "\n" +
                             "EXCLUDE_SYMLINKS       = NO" + "\n" +
                             "EXCLUDE_PATTERNS       = " + "\n" +
                             "EXCLUDE_SYMBOLS        = " + "\n" +
                             "EXAMPLE_PATH           = " + "\n" +
                             "EXAMPLE_PATTERNS       = *" + "\n" +
                             "EXAMPLE_RECURSIVE      = NO" + "\n" +
                             "IMAGE_PATH             = " + "\n" +
                             "INPUT_FILTER           = " + "\n" +
                             "FILTER_PATTERNS        = " + "\n" +
                             "FILTER_SOURCE_FILES    = NO" + "\n" +
                             "FILTER_SOURCE_PATTERNS = " + "\n" +
                             "USE_MDFILE_AS_MAINPAGE = " + "\n" +
                             "#---------------------------------------------------------------------------" + "\n" +
                             "# Configuration options related to source browsing" + "\n" +
                             "#---------------------------------------------------------------------------" + "\n" +
                             "SOURCE_BROWSER         = NO" + "\n" +
                             "INLINE_SOURCES         = NO" + "\n" +
                             "STRIP_CODE_COMMENTS    = YES" + "\n" +
                             "REFERENCED_BY_RELATION = NO" + "\n" +
                             "REFERENCES_RELATION    = NO" + "\n" +
                             "REFERENCES_LINK_SOURCE = YES" + "\n" +
                             "SOURCE_TOOLTIPS        = YES" + "\n" +
                             "USE_HTAGS              = NO" + "\n" +
                             "VERBATIM_HEADERS       = YES" + "\n" +
                             "CLANG_ASSISTED_PARSING = NO" + "\n" +
                             "CLANG_OPTIONS          = " + "\n" +
                             "#---------------------------------------------------------------------------" + "\n" +
                             "# Configuration options related to the alphabetical class index" + "\n" +
                             "#---------------------------------------------------------------------------" + "\n" +
                             "ALPHABETICAL_INDEX     = YES" + "\n" +
                             "COLS_IN_ALPHA_INDEX    = 5" + "\n" +
                             "IGNORE_PREFIX          = " + "\n" +
                             "#---------------------------------------------------------------------------" + "\n" +
                             "# Configuration options related to the HTML output" + "\n" +
                             "#---------------------------------------------------------------------------" + "\n" +
                             "GENERATE_HTML          = " + YesOrNo(Config.GenerateHTML) + "\n" +
                             "HTML_OUTPUT            = html" + "\n" +
                             "HTML_FILE_EXTENSION    = .html" + "\n" +
                             "HTML_HEADER            = " + "\n" +
                             "HTML_FOOTER            = " + "\n" +
                             "HTML_STYLESHEET        = " + "\n" +
                             "HTML_EXTRA_STYLESHEET  = " + "\n" +
                             "HTML_EXTRA_FILES       = " + "\n" +
                             "HTML_COLORSTYLE_HUE    =  " + tHueInt.ToString() + "\n" +
                             "HTML_COLORSTYLE_SAT    = " + tSaturationInt.ToString() + "\n" +
                             "HTML_COLORSTYLE_GAMMA  = " + tVaInt.ToString() + "\n" +
                             "HTML_TIMESTAMP         = NO" + "\n" +
                             "HTML_DYNAMIC_SECTIONS  = NO" + "\n" +
                             "HTML_INDEX_NUM_ENTRIES = 100" + "\n" +
                             "GENERATE_DOCSET        = NO" + "\n" +
                             "DOCSET_FEEDNAME        = \"Doxygen generated docs\"" + "\n" +
                             "DOCSET_BUNDLE_ID       = org.doxygen.Project" + "\n" +
                             "DOCSET_PUBLISHER_ID    = org.doxygen.Publisher" + "\n" +
                             "DOCSET_PUBLISHER_NAME  = Publisher" + "\n" +
                             "GENERATE_HTMLHELP      = NO" + "\n" +
                             "CHM_FILE               = " + "\n" +
                             "HHC_LOCATION           = " + "\n" +
                             "GENERATE_CHI           = NO" + "\n" +
                             "CHM_INDEX_ENCODING     = " + "\n" +
                             "BINARY_TOC             = NO" + "\n" +
                             "TOC_EXPAND             = NO" + "\n" +
                             "GENERATE_QHP           = NO" + "\n" +
                             "QCH_FILE               = " + "\n" +
                             "QHP_NAMESPACE          = org.doxygen.Project" + "\n" +
                             "QHP_VIRTUAL_FOLDER     = doc" + "\n" +
                             "QHP_CUST_FILTER_NAME   = " + "\n" +
                             "QHP_CUST_FILTER_ATTRS  = " + "\n" +
                             "QHP_SECT_FILTER_ATTRS  = " + "\n" +
                             "QHG_LOCATION           = " + "\n" +
                             "GENERATE_ECLIPSEHELP   = NO" + "\n" +
                             "ECLIPSE_DOC_ID         = org.doxygen.Project" + "\n" +
                             "DISABLE_INDEX          = NO" + "\n" +
                             "GENERATE_TREEVIEW      = NO" + "\n" +
                             "ENUM_VALUES_PER_LINE   = 4" + "\n" +
                             "TREEVIEW_WIDTH         = 250" + "\n" +
                             "EXT_LINKS_IN_WINDOW    = NO" + "\n" +
                             "FORMULA_FONTSIZE       = 10" + "\n" +
                             "FORMULA_TRANSPARENT    = YES" + "\n" +
                             "USE_MATHJAX            = NO" + "\n" +
                             "MATHJAX_FORMAT         = HTML-CSS" + "\n" +
                             "MATHJAX_RELPATH        = http://cdn.mathjax.org/mathjax/latest" + "\n" +
                             "MATHJAX_EXTENSIONS     = " + "\n" +
                             "MATHJAX_CODEFILE       = " + "\n" +
                             "SEARCHENGINE           = YES" + "\n" +
                             "SERVER_BASED_SEARCH    = NO" + "\n" +
                             "EXTERNAL_SEARCH        = NO" + "\n" +
                             "SEARCHENGINE_URL       = " + "\n" +
                             "SEARCHDATA_FILE        = searchdata.xml" + "\n" +
                             "EXTERNAL_SEARCH_ID     = " + "\n" +
                             "EXTRA_SEARCH_MAPPINGS  = " + "\n" +
                             "#---------------------------------------------------------------------------" + "\n" +
                             "# Configuration options related to the LaTeX output" + "\n" +
                             "#---------------------------------------------------------------------------" + "\n" +
                             "GENERATE_LATEX         = " + YesOrNo(Config.GenerateLatex) + "\n" +
                             "LATEX_OUTPUT           = latex" + "\n" +
                             "LATEX_CMD_NAME         = latex" + "\n" +
                             "MAKEINDEX_CMD_NAME     = makeindex" + "\n" +
                             "COMPACT_LATEX          = NO" + "\n" +
                             "PAPER_TYPE             = a4" + "\n" +
                             "EXTRA_PACKAGES         = " + "\n" +
                             "LATEX_HEADER           = " + "\n" +
                             "LATEX_FOOTER           = " + "\n" +
                             "LATEX_EXTRA_STYLESHEET = " + "\n" +
                             "LATEX_EXTRA_FILES      = " + "\n" +
                             "PDF_HYPERLINKS         = YES" + "\n" +
                             "USE_PDFLATEX           = YES" + "\n" +
                             "LATEX_BATCHMODE        = NO" + "\n" +
                             "LATEX_HIDE_INDICES     = NO" + "\n" +
                             "LATEX_SOURCE_CODE      = NO" + "\n" +
                             "LATEX_BIB_STYLE        = plain" + "\n" +
                             "LATEX_TIMESTAMP        = NO" + "\n" +
                             "#---------------------------------------------------------------------------" + "\n" +
                             "# Configuration options related to the RTF output" + "\n" +
                             "#---------------------------------------------------------------------------" + "\n" +
                             "GENERATE_RTF           = " + YesOrNo(Config.GenerateRTF) + "\n" +
                             "RTF_OUTPUT             = rtf" + "\n" +
                             "COMPACT_RTF            = NO" + "\n" +
                             "RTF_HYPERLINKS         = NO" + "\n" +
                             "RTF_STYLESHEET_FILE    = " + "\n" +
                             "RTF_EXTENSIONS_FILE    = " + "\n" +
                             "RTF_SOURCE_CODE        = NO" + "\n" +
                             "#---------------------------------------------------------------------------" + "\n" +
                             "# Configuration options related to the man page output" + "\n" +
                             "#---------------------------------------------------------------------------" + "\n" +
                             "GENERATE_MAN           = NO" + "\n" +
                             "MAN_OUTPUT             = man" + "\n" +
                             "MAN_EXTENSION          = .3" + "\n" +
                             "MAN_SUBDIR             = " + "\n" +
                             "MAN_LINKS              = NO" + "\n" +
                             "#---------------------------------------------------------------------------" + "\n" +
                             "# Configuration options related to the XML output" + "\n" +
                             "#---------------------------------------------------------------------------" + "\n" +
                             "GENERATE_XML           = " + YesOrNo(Config.GenerateXML) + "\n" +
                             "XML_OUTPUT             = xml" + "\n" +
                             "XML_PROGRAMLISTING     = YES" + "\n" +
                             "#---------------------------------------------------------------------------" + "\n" +
                             "# Configuration options related to the DOCBOOK output" + "\n" +
                             "#---------------------------------------------------------------------------" + "\n" +
                             "GENERATE_DOCBOOK       = NO" + "\n" +
                             "DOCBOOK_OUTPUT         = docbook" + "\n" +
                             "DOCBOOK_PROGRAMLISTING = NO" + "\n" +
                             "#---------------------------------------------------------------------------" + "\n" +
                             "# Configuration options for the AutoGen Definitions output" + "\n" +
                             "#---------------------------------------------------------------------------" + "\n" +
                             "GENERATE_AUTOGEN_DEF   = NO" + "\n" +
                             "#---------------------------------------------------------------------------" + "\n" +
                             "# Configuration options related to the Perl module output" + "\n" +
                             "#---------------------------------------------------------------------------" + "\n" +
                             "GENERATE_PERLMOD       = NO" + "\n" +
                             "PERLMOD_LATEX          = NO" + "\n" +
                             "PERLMOD_PRETTY         = YES" + "\n" +
                             "PERLMOD_MAKEVAR_PREFIX = " + "\n" +
                             "#---------------------------------------------------------------------------" + "\n" +
                             "# Configuration options related to the preprocessor" + "\n" +
                             "#---------------------------------------------------------------------------" + "\n" +
                             "ENABLE_PREPROCESSING   = YES" + "\n" +
                             "MACRO_EXPANSION        = NO" + "\n" +
                             "EXPAND_ONLY_PREDEF     = NO" + "\n" +
                             "SEARCH_INCLUDES        = YES" + "\n" +
                             "INCLUDE_PATH           = " + "\n" +
                             "INCLUDE_FILE_PATTERNS  = " + "\n" +
                             "PREDEFINED             = " + "\n" +
                             "EXPAND_AS_DEFINED      = " + "\n" +
                             "SKIP_FUNCTION_MACROS   = YES" + "\n" +
                             "#---------------------------------------------------------------------------" + "\n" +
                             "# Configuration options related to external references" + "\n" +
                             "#---------------------------------------------------------------------------" + "\n" +
                             "TAGFILES               = " + "\n" +
                             "GENERATE_TAGFILE       = " + "\n" +
                             "ALLEXTERNALS           = NO" + "\n" +
                             "EXTERNAL_GROUPS        = YES" + "\n" +
                             "EXTERNAL_PAGES         = YES" + "\n" +
                             "PERL_PATH              = /usr/bin/perl" + "\n" +
                             "#---------------------------------------------------------------------------" + "\n" +
                             "# Configuration options related to the dot tool" + "\n" +
                             "#---------------------------------------------------------------------------" + "\n" +
                             "CLASS_DIAGRAMS         = YES" + "\n" +
                             "MSCGEN_PATH            = " + "\n" +
                             "DIA_PATH               = " + "\n" +
                             "HIDE_UNDOC_RELATIONS   = YES" + "\n" +
                             "HAVE_DOT               = NO" + "\n" +
                             "DOT_NUM_THREADS        = 0" + "\n" +
                             "DOT_FONTNAME           = Helvetica" + "\n" +
                             "DOT_FONTSIZE           = 10" + "\n" +
                             "DOT_FONTPATH           = " + "\n" +
                             "CLASS_GRAPH            = YES" + "\n" +
                             "COLLABORATION_GRAPH    = YES" + "\n" +
                             "GROUP_GRAPHS           = YES" + "\n" +
                             "UML_LOOK               = NO" + "\n" +
                             "UML_LIMIT_NUM_FIELDS   = 10" + "\n" +
                             "TEMPLATE_RELATIONS     = NO" + "\n" +
                             "INCLUDE_GRAPH          = YES" + "\n" +
                             "INCLUDED_BY_GRAPH      = YES" + "\n" +
                             "CALL_GRAPH             = NO" + "\n" +
                             "CALLER_GRAPH           = NO" + "\n" +
                             "GRAPHICAL_HIERARCHY    = YES" + "\n" +
                             "DIRECTORY_GRAPH        = YES" + "\n" +
                             "DOT_IMAGE_FORMAT       = png" + "\n" +
                             "INTERACTIVE_SVG        = NO" + "\n" +
                             "DOT_PATH               = " + "\n" +
                             "DOTFILE_DIRS           = " + "\n" +
                             "MSCFILE_DIRS           = " + "\n" +
                             "DIAFILE_DIRS           = " + "\n" +
                             "PLANTUML_JAR_PATH      = " + "\n" +
                             "PLANTUML_CFG_FILE      = " + "\n" +
                             "PLANTUML_INCLUDE_PATH  = " + "\n" +
                             "DOT_GRAPH_MAX_NODES    = 50" + "\n" +
                             "MAX_DOT_GRAPH_DEPTH    = 0" + "\n" +
                             "DOT_TRANSPARENT        = NO" + "\n" +
                             "DOT_MULTI_TARGETS      = NO" + "\n" +
                             "GENERATE_LEGEND        = YES" + "\n" +
                             "DOT_CLEANUP            = YES";
            File.WriteAllText(PathDoxygenConfig(), tConfig);
            ProcessStartInfo tProcessInfo = new ProcessStartInfo();
            // Enter in the command line arguments, everything you would enter after the executable name itself
            tProcessInfo.Arguments = "'" + PathDoxygenConfig() + "'";
            // Enter the executable to run, including the complete path
#if UNITY_EDITOR_OSX
            tProcessInfo.FileName = Config.PathToDOxygenOSX + "/Contents/Resources/doxygen";
#else
			tProcessInfo.FileName = Config.PathToDOxygenWIN;
#endif

            // Do you want to show a console window?
            tProcessInfo.WindowStyle = ProcessWindowStyle.Hidden;
            tProcessInfo.CreateNoWindow = true;
            tProcessInfo.UseShellExecute = true;
            Process tProcessExec = new Process();
            tProcessExec.StartInfo = tProcessInfo;
            WorkInProgress = true;
            Repaint();
            //UnityEngine.Debug.Log ("tProcessExec  => " + tProcessExec.StartInfo.FileName + " " + tProcessExec.StartInfo.Arguments);
            tProcessExec.Start();
            tProcessExec.WaitForExit();
            // Retrieve the app's exit code
            FinishDoxygen();
        }
        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Finish generation.
        /// </summary>
        void FinishDoxygen()
        {
            //			UnityEngine.Debug.Log ("DGWDocumentationGenerator FinishDoxygen()");
            WorkInProgress = false;
            Repaint();
            Application.OpenURL("file://" + Config.FolderOutputAbsolute().Replace(" ", "%20") + "/");
            if (Config.GenerateHTML == true)
            {
                Application.OpenURL("file://" + Config.FolderOutputAbsolute().Replace(" ", "%20") + "/html/index.html");
            }
        }
        //-------------------------------------------------------------------------------------------------------------
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
}
//=====================================================================================================================
#endif