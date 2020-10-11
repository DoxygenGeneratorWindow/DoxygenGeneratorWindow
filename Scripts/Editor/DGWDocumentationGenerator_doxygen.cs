//=====================================================================================================================
//
// ideMobi copyright 2020 
// All rights reserved by ideMobi
//
//=====================================================================================================================

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEditor;

//=====================================================================================================================
namespace DoxygenGeneratorWindow
{
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    public partial class DGWDocumentationGenerator : EditorWindow
    {
        //-------------------------------------------------------------------------------------------------------------
        /// https://mcss.mosra.cz/documentation/doxygen/
        //-------------------------------------------------------------------------------------------------------------
        const string K_PathToDOxygenOSX_key = "OSX_key";
        //-------------------------------------------------------------------------------------------------------------
        const string K_PathToDOxygenWIN_key = "WIN_key";
        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// The DOXYGEN URL.
        /// </summary>
        //const string K_URL_DOXYGEN = "http://www.stack.nl/~dimitri/doxygen/download.html";
        const string K_URL_DOXYGEN = "https://www.doxygen.nl/download.html";
        const string K_TEXTL_DOXYGEN = "Go to Doxygen© website";
        //-------------------------------------------------------------------------------------------------------------
        void OnGUI_Doxygen()
        {
            //----------
            // exe box
            //----------
            GUILayout.Space(k_Marge);
#if UNITY_EDITOR_OSX
            // form to .app 
            EditorGUILayout.LabelField("Path to Doxygen.app © application", EditorStyles.boldLabel);
#else
            // form to .exe 
            GUILayout.Label("Path to Doxygen.exe © application", EditorStyles.boldLabel);
#endif
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;
            FileInfo tExePath = null;
#if UNITY_EDITOR_OSX
            EditorGUILayout.LabelField("Path to Doxygen", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            // select Doxygen.app
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Path to Doxygen.app", EditorPrefs.GetString(K_PathToDOxygenOSX_key, "/Applications/Doxygen.app"));
            if (GUILayout.Button("...", EditorStyles.miniButtonRight, GUILayout.Width(k_PathButtonWidth)))
            {
                EditorPrefs.SetString(K_PathToDOxygenOSX_key, EditorUtility.OpenFilePanel("Where is Doxygen.app installed?", "/Applications/", ""));
                GUI.FocusControl(null);
            }
            EditorGUILayout.EndHorizontal();
            // test selection and show donwload instruction
            tExePath = new FileInfo(EditorPrefs.GetString(K_PathToDOxygenOSX_key) + "/Contents/Resources/doxygen");
            if (!tExePath.Exists)
            {
            }
            // indent remove
            EditorGUI.indentLevel--;
            // finsih form to .app
#else
            EditorGUILayout.LabelField("Path to Doxygen", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            // select Doxygen.exe
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Path to Doxygen.exe", EditorPrefs.GetString(K_PathToDOxygenWIN_key, "C:/Program Files/doxygen/bin/doxygen.exe"));
            if (GUILayout.Button("...", EditorStyles.miniButtonRight, GUILayout.Width(tPathButtonWidth)))
            {
                EditorPrefs.SetString(K_PathToDOxygenWIN_key, EditorUtility.OpenFilePanel("Where is Doxygen.exe installed?", "", ""));
                GUI.FocusControl(null);
            }
            EditorGUILayout.EndHorizontal();
            // test selection and show donwload instruction
            if (!EditorPrefs.GetString(K_PathToDOxygenWIN_key).Equals(""))
            {
                tExePath = new FileInfo(EditorPrefs.GetString(K_PathToDOxygenWIN_key));
                if (!tExePath.Exists)
                {
                }
            }
            // indent remove
            EditorGUI.indentLevel--;
            // finsih form to .exe
#endif
            // If no app/exe
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
                if (GUILayout.Button(K_TEXTL_DOXYGEN))
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
			    if (GUILayout.Button(K_TEXTL_DOXYGEN))
			    {
				    Application.OpenURL(K_URL_DOXYGEN);
			    }
#endif
            }
            //----------
            EditorGUI.BeginDisabledGroup(!tExePathIsValid);
            //----------
            // Output box
            //----------
            EditorGUILayout.LabelField("Output settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            Config.GenerateHTML = EditorGUILayout.Toggle("Generate HTML", Config.GenerateHTML);
            EditorGUI.indentLevel++;
            EditorGUI.BeginDisabledGroup(!Config.GenerateHTML);
            Config.HTMLColor = EditorGUILayout.ColorField("Tint Color", Config.HTMLColor);
            Config.GenerateDocSet = EditorGUILayout.Toggle("Generate DocSet", Config.GenerateDocSet);
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
            if (OutIsOk == true)
            {
                if (Config.GenerateHTML == false && Config.GenerateLatex == false && Config.GenerateXML == false && Config.GenerateRTF == false)
                {
                    OutIsOk = false;
                }
            }
            EditorGUI.indentLevel--;
            //----------
            // Analyze box
            //----------
            EditorGUILayout.LabelField("Analyze settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            Config.Recursive = EditorGUILayout.Toggle("Recursive", Config.Recursive);
            Config.ALPHABETICAL_INDEX = EditorGUILayout.Toggle("Alphabetic index", Config.ALPHABETICAL_INDEX);
            // indent remove
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
            // finish form output 
            GUILayout.EndVertical();
            //----------
            EditorGUI.EndDisabledGroup();
            EditorGUI.EndDisabledGroup();
            //----------
            // Analyze box
            //----------
            GUILayout.Space(k_Marge);
            EditorGUILayout.LabelField("Generate documentation", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.BeginDisabledGroup(!OutIsOk || !tExePathIsValid);
            if (GUILayout.Button("Generate"))
            {
                RunDoxygen();
            }
            EditorGUI.EndDisabledGroup();
            GUILayout.EndVertical();
            //----------
        }
        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Return  the path of doxygen config.
        /// </summary>
        /// <returns>The doxygen Path.</returns>
        private string PathDoxygenConfig()
        {
            // UnityEngine.Debug.Log ("DGWDocumentationGenerator PathDoxygenConfig()");
            string rReturn = Path.GetDirectoryName(Application.dataPath) + "/DGWDoxygenConfig.txt";
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

            string tImage = "";
            Texture2D tTextureOrign = (Texture2D)AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(Config.ProjectLogo);
            if (tTextureOrign != null)
            {
                int tLogoSize = 96;
                bool tBilinear = true;
                Texture2D tTexture = DeCompress(tTextureOrign);
                if (tTexture.width > tLogoSize || tTexture.height > tLogoSize)
                {
                    if (tTexture.width > tTexture.height)
                    {
                        DGWToolbox.ThreadedScale(tTexture, tLogoSize, tLogoSize * Mathf.CeilToInt(tTexture.height / tTexture.width), tBilinear);
                    }
                    else if (tTexture.width < tTexture.height)
                    {
                        DGWToolbox.ThreadedScale(tTexture, tLogoSize, tLogoSize * Mathf.CeilToInt(tTexture.width / tTexture.height), tBilinear);
                    }
                    else
                    {
                        DGWToolbox.ThreadedScale(tTexture, tLogoSize, tLogoSize, tBilinear);
                    }
                }
                tImage = Path.GetDirectoryName(Application.dataPath) + "/DGWDoxygenConfig.png";
                byte[] tBytes = tTexture.EncodeToPNG();
                System.IO.File.WriteAllBytes(tImage, tBytes);

                //var tImporter = AssetImporter.GetAtPath(Config.ProjectLogo) as TextureImporter;
                //if (tImporter != null)
                //{
                //TextureImporterType tOriginalType = tImporter.textureType;
                //tImporter.textureType = TextureImporterType.Advanced;
                //bool tOriginalReadeable = tImporter.isReadable;
                //tImporter.isReadable = true;
                //AssetDatabase.ImportAsset(Config.ProjectLogo);
                //AssetDatabase.Refresh();
                //tImage = Path.GetDirectoryName(Application.dataPath) + "/DGWDoxygenConfig.png";
                //    byte[] tBytes = tTexture.EncodeToPNG();
                //    System.IO.File.WriteAllBytes(tImage, tBytes);
                //tImporter.textureType = tOriginalType;
                //tImporter.isReadable = tOriginalReadeable;
                //AssetDatabase.ImportAsset(Config.ProjectLogo);
                //AssetDatabase.Refresh();
                //}
            }
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
                         "PROJECT_LOGO           = \"" + tImage + "\"" + "\n" +
                         "OUTPUT_DIRECTORY       = \"" + Config.FolderOutputAbsolute().Replace(" ", " ") + "/" + Config.ProjectName + "\"\n" +
                         "CREATE_SUBDIRS         = NO" + "\n" +
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
                         "ALPHABETICAL_INDEX     = " + YesOrNo(Config.ALPHABETICAL_INDEX) + "\n" +
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
                         "GENERATE_DOCSET        = " + YesOrNo(Config.GenerateDocSet) + "\n" +
                         "DOCSET_FEEDNAME        = \"" + Config.ProjectName + " generated docs\"" + "\n" +
                         "DOCSET_BUNDLE_ID       = org." + Config.ProjectName.ToLower() + ".Project" + "\n" +
                         "DOCSET_PUBLISHER_ID    = org." + Config.ProjectName.ToLower() + ".Publisher" + "\n" +
                         "DOCSET_PUBLISHER_NAME  = " + Config.ProjectName + "" + "\n" +
                         "GENERATE_HTMLHELP      = NO" + "\n" +
                         "CHM_FILE               = " + "\n" +
                         "HHC_LOCATION           = " + "\n" +
                         "GENERATE_CHI           = NO" + "\n" +
                         "CHM_INDEX_ENCODING     = " + "\n" +
                         "BINARY_TOC             = NO" + "\n" +
                         "TOC_EXPAND             = NO" + "\n" +
                         "GENERATE_QHP           = NO" + "\n" +
                         "QCH_FILE               = " + "\n" +
                         "QHP_NAMESPACE          = org." + Config.ProjectName.ToLower() + ".Project" + "\n" +
                         "QHP_VIRTUAL_FOLDER     = doc" + "\n" +
                         "QHP_CUST_FILTER_NAME   = " + "\n" +
                         "QHP_CUST_FILTER_ATTRS  = " + "\n" +
                         "QHP_SECT_FILTER_ATTRS  = " + "\n" +
                         "QHG_LOCATION           = " + "\n" +
                         "GENERATE_ECLIPSEHELP   = NO" + "\n" +
                         "ECLIPSE_DOC_ID         = org." + Config.ProjectName.ToLower() + ".Project" + "\n" +
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
            tProcessInfo.FileName = EditorPrefs.GetString(K_PathToDOxygenOSX_key) + "/Contents/Resources/doxygen";
#else
			tProcessInfo.FileName = EditorPrefs.GetString(K_PathToDOxygenWIN_key);
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
            WorkInProgress = false;
            Repaint();
            Application.OpenURL("file://" + Config.FolderOutputAbsolute().Replace(" ", "%20") + "/");
            if (Config.GenerateHTML == true)
            {
                Application.OpenURL("file://" + Config.FolderOutputAbsolute().Replace(" ", "%20") + "/" + Config.ProjectName + "/html/index.html");
            }
        }
        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// decompress texture in new texture to save in png file
        /// </summary>
        /// <param name="sSource"></param>
        /// <returns></returns>
        public static Texture2D DeCompress(Texture2D sSource)
        {
            RenderTexture tRenderTexture = RenderTexture.GetTemporary(sSource.width,sSource.height,0,RenderTextureFormat.Default,RenderTextureReadWrite.Linear);
            Graphics.Blit(sSource, tRenderTexture);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = tRenderTexture;
            Texture2D readableText = new Texture2D(sSource.width, sSource.height);
            readableText.ReadPixels(new Rect(0, 0, tRenderTexture.width, tRenderTexture.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(tRenderTexture);
            return readableText;
        }
        //-------------------------------------------------------------------------------------------------------------
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
}
//=====================================================================================================================