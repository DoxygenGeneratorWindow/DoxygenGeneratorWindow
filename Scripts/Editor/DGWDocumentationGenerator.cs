//=====================================================================================================================
//
// ideMobi copyright 2020 
// All rights reserved by ideMobi
//
//=====================================================================================================================

using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

//=====================================================================================================================
namespace DoxygenGeneratorWindow
{
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    /// <summary>
    /// DGW Documentation generator is a class to generate the documentation by Doxygen automatically
    /// After reading comparison at https://en.wikipedia.org/wiki/Comparison_of_documentation_generators
    /// the challengers for c# with output HTML are ... 
    /// - Document! X (eliminated : no macOS, no linux)
    /// - Doxygen
    /// - Natural Docs (partial PHP ...)
    /// - NDoc (eliminated : no read text as input!)
    /// - Visual Expert (eliminated : no macOS, no linux)
    /// - VSdocman (eliminated : no macOS, no linux)
    /// We decided to use Doxygen beacuse it's the most easy to use for a proof of concept.
    /// We will propose Natural Docs later, because the library is not really usable on macosx
    /// </summary>
    /// 
    public enum DGWTools : int
    {
        Doxygen = 0,
        //NaturalDocs = 1,
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    public partial class DGWDocumentationGenerator : EditorWindow
    {
        //-------------------------------------------------------------------------------------------------------------
        const float k_PathButtonWidth = 20.0F;
        const float k_Marge = 5.0F;
        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// The shared instance.
        /// </summary>
        private static DGWDocumentationGenerator kSharedInstance;
        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// The config of this project.
        /// </summary>
        private DGWConfig Config;
        /// <summary>
        /// The list config
        /// </summary>
        private DGWConfigList ConfigList;
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
        /// <summary>
        /// Check if generation can be execute
        /// </summary>
        private bool OutIsOk = true;
        /// <summary>
        /// Idemobi Logo content
        /// </summary>
        GUIContent IdemobiContent;
        /// <summary>
        /// Doxygen Generator Window Logo
        /// </summary>
        GUIContent DGWContent;
        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Ascencor to shared instance.
        /// </summary>
        /// <returns>The shared instance.</returns>
        public static DGWDocumentationGenerator SharedInstance()
        {
            if (kSharedInstance == null)
            {
                kSharedInstance = EditorWindow.GetWindow(typeof(DGWDocumentationGenerator)) as DGWDocumentationGenerator;
                kSharedInstance.position = new Rect(10, 10, 300, 500);
            }
            return kSharedInstance;
        }
        //-------------------------------------------------------------------------------------------------------------
        [MenuItem(DGWConstants.K_MENU_BASE + "/Doxygen© Generator Window", false, 100)]
        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Prepare the window.
        /// </summary>
        static void PrepareWindow()
        {
            SharedInstance().ShowUtility();
            SharedInstance().Focus();
        }
        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Raises the enable event.
        /// </summary>
        public void OnEnable()
        {
            LoadPreferences();
            Texture2D tImageLogo = AssetDatabase.LoadAssetAtPath<Texture2D>(DGWFindPackage.PathOfPackage("/Scripts/Editor/Idemobi.png"));
            IdemobiContent = new GUIContent(tImageLogo);
            Texture2D tImageLogoDGW = AssetDatabase.LoadAssetAtPath<Texture2D>(DGWFindPackage.PathOfPackage("/Scripts/Editor/DoxygenGeneratorWindow_Icon.png"));
            DGWContent = new GUIContent(tImageLogoDGW);
            titleContent = new GUIContent("DoxyGW", tImageLogoDGW);
            // set size of window
            this.minSize = new Vector2(300, 300);
            this.maxSize = new Vector2(2048, 2048);
        }
        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Save the preferences.
        /// </summary>
        void SavePreferences()
        {
            string tJson = JsonUtility.ToJson(ConfigList);
            File.WriteAllText(PathConfig(), tJson);
        }
        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Load the preferences.
        /// </summary>
        void LoadPreferences()
        {
            string tPath = PathConfig();
            if (File.Exists(tPath))
            {
                string tJSONString = File.ReadAllText(tPath);
                ConfigList = JsonUtility.FromJson<DGWConfigList>(tJSONString);
                if (ConfigList != null)
                {
                    if (ConfigList.ConfigList.Count > 0)
                    {
                        Config = ConfigList.ConfigList[ConfigList.Selected];
                    }
                    else
                    {
                        ConfigList = new DGWConfigList();
                        Config = ConfigList.AddNewConfig();
                    }
                }
                else
                {
                    ConfigList = new DGWConfigList();
                    Config = ConfigList.AddNewConfig();
                }
            }
            else
            {
                ConfigList = new DGWConfigList();
                Config = ConfigList.AddNewConfig();
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
            EditorGUI.BeginChangeCheck();
            //----------
            // SCROLL ZONE START
            //----------
            ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition);
            if (WorkInProgress)
            {
                EditorGUILayout.HelpBox("Work in progress!", MessageType.Info);
            }
            //----------
            // Choose box
            //----------
            GUILayout.Space(k_Marge);
            GUILayout.Label("Choose your configuration", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            List<string> tToolBarList = new List<string>();
            List<string> tConfigNameList = new List<string>();
            int tN = 0;
            foreach (DGWConfig tConfig in ConfigList.ConfigList)
            {
                tN++;
                tToolBarList.Add(tN.ToString() + " - " + tConfig.ProjectName);
                tConfigNameList.Add(tConfig.ProjectName);
            }
            ConfigList.Selected = EditorGUILayout.Popup(ConfigList.Selected, tToolBarList.ToArray());
            Config = ConfigList.ConfigList[ConfigList.Selected];
            if (GUILayout.Button("Add", GUILayout.Width(80)))
            {
                Config = ConfigList.AddNewConfig();
                tToolBarList.Add(tN.ToString() + " - " + Config.ProjectName);
                tConfigNameList.Add(Config.ProjectName);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            //----------
            // Edit box
            //----------
            GUILayout.Space(k_Marge);
            GUILayout.Label("Edit \"" + tConfigNameList[ConfigList.Selected]+"\"", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.LabelField("Project settings", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                Config.ProjectName = EditorGUILayout.TextField("Project Name", Config.ProjectName);
                Config.ProjectSynopsis = EditorGUILayout.TextField("Project Synopsis", Config.ProjectSynopsis);
                Config.ProjectVersion = EditorGUILayout.TextField("Project Version", Config.ProjectVersion);
                Texture2D tTexture = (Texture2D)AssetDatabase.LoadAssetAtPath<Object>(Config.ProjectLogo);
                tTexture = (Texture2D)EditorGUILayout.ObjectField("Project Logo", tTexture, typeof(Texture2D), false);
                if (tTexture != null)
                {
                    string tTexturePath = AssetDatabase.GetAssetPath(tTexture);
                    if (System.IO.File.Exists(tTexturePath) == true)
                    {
                        Config.ProjectLogo = tTexturePath;
                    }
                }
                else
                {
                    Config.ProjectLogo = "";
                }

                Config.UseTools = (DGWTools)EditorGUILayout.EnumPopup("Use tool", Config.UseTools);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.LabelField("Input", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            Object tFolderObject = AssetDatabase.LoadAssetAtPath<Object>(Config.FolderName);
            tFolderObject = EditorGUILayout.ObjectField("Folder sources", tFolderObject, typeof(UnityEngine.Object), false);
            if (tFolderObject != null)
            {
                string tFolderPath = AssetDatabase.GetAssetPath(tFolderObject);
                if (Directory.Exists(tFolderPath) == true)
                {
                    Config.FolderName = tFolderPath;
                }
            }
            else
            {
                Config.FolderName = "";
                EditorGUILayout.HelpBox("Drag and drop folder from your project!", MessageType.Warning);
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.LabelField("Output", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Folder output path", Config.FolderOutput);
            if (GUILayout.Button("...", EditorStyles.miniButtonRight, GUILayout.Width(k_PathButtonWidth)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Application.dataPath) + "/Documentation");
                string tFolderOutput = EditorUtility.OpenFolderPanel("Where do you want generate documentation?", Path.GetDirectoryName(Application.dataPath) + "/Documentation", "");
                Config.SetFolderOutputAbsolute(tFolderOutput);
            }
            EditorGUILayout.EndHorizontal();
            OutIsOk = true;
            if (Config.FolderOutput == null || Config.FolderOutput == "")
            {
                OutIsOk = false;
            }
            else
            {
                DirectoryInfo tOutPath = new DirectoryInfo(Config.FolderOutputAbsolute());
                OutIsOk = tOutPath.Exists;
            }
            if (OutIsOk == false)
            {
                EditorGUILayout.HelpBox("Select output folder", MessageType.Warning);
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.LabelField("Delete action", EditorStyles.boldLabel);
            kGroupResetState = EditorGUILayout.Foldout(kGroupResetState, "Show reset action");
            if (kGroupResetState == true)
            {
                Color tBackgroundColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(1.0F, 0.7F, 0.7F, 1.0F);
                if (GUILayout.Button("Reset"))
                {
                    if (EditorUtility.DisplayDialog("Alert", "You will reset this configuration", "Reset", "Cancel"))
                    {
                        Config.EmptyFill();
                        SavePreferences();
                        GUI.FocusControl(null);
                    }
                }
                if (GUILayout.Button("Delete"))
                {
                    if (EditorUtility.DisplayDialog("Alert", "You will delete this configuration", "Delete", "Cancel"))
                    {
                        Config = ConfigList.RemoveConfig(Config);
                        SavePreferences();
                        GUI.FocusControl(null);
                    }
                }
                GUI.backgroundColor = tBackgroundColor;
            }
            EditorGUI.indentLevel--;
            GUILayout.EndVertical();
            //----------
            // Tool box
            //----------
            switch (Config.UseTools)
            {
                case DGWTools.Doxygen:
                    {
                        OnGUI_Doxygen();
                    }
                    break;
                default:
                    {
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        EditorGUILayout.LabelField("in dev", EditorStyles.boldLabel);
                        GUILayout.EndVertical();
                    }
                    break;
            }
            //----------
            // Thanks box
            //----------
            GUILayout.Space(10.0F);
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
            GUILayout.Space(20.0F);
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(DGWContent, tStyleImage, GUILayout.Height(64.0F), GUILayout.Width(64.0F));
            GUILayout.Label(IdemobiContent, tStyleImage, GUILayout.Height(64.0F), GUILayout.Width(64.0F));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Label(new GUIContent("DoxygenGenratorWindow© by idéMobi©"), tStyleBold);
            tStyle.alignment = TextAnchor.MiddleCenter;
            if (GUILayout.Button("Go to GitHub repository"))
            {
                Application.OpenURL("https://github.com/idemobi/DoxygenGeneratorWindow");
            }
            GUILayout.Space(20.0F);
            GUILayout.Label(new GUIContent("Thanks to Doxygen©!"), tStyleBold);
            GUILayout.Label(new GUIContent("Copyright 1997-2018 by Dimitri van Heesch."), tStyleBold);
            if (GUILayout.Button(K_TEXTL_DOXYGEN))
            {
                Application.OpenURL(K_URL_DOXYGEN);
            }
            GUILayout.Space(20.0F);
            //----------
            // SCROLL ZONE END
            //----------
            EditorGUILayout.EndScrollView();
            //----------
            // Check Action
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
            // UnityEngine.Debug.Log ("DGWDocumentationGenerator PathConfig()");
            string rReturn = Path.GetDirectoryName(Application.dataPath) + "/DGWConfigListSave.txt";
            return rReturn;
        }
        //-------------------------------------------------------------------------------------------------------------
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
}
//=====================================================================================================================