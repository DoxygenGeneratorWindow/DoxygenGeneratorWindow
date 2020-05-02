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
        [MenuItem(DGWConstants.K_MENU_BASE + "/Doxygen© Generator Window", false, 100)]
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
            string tJson = JsonUtility.ToJson(ConfigList);
            File.WriteAllText(PathConfig(), tJson);
        }
        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Load the preferences.
        /// </summary>
        void LoadPreferences()
        {
            //Debug.Log ("LoadPreferences");
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
            // UnityEngine.Debug.Log ("DGWDocumentationGenerator OnGUI()");
            EditorGUI.BeginChangeCheck();
            // set size of window
            this.minSize = new Vector2(300, 300);
            this.maxSize = new Vector2(2048, 2048);
            // set title of window
            Texture2D tImageIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(DGWFindPackage.PathOfPackage("/Editor/Resources/Textures/DGWIcon.psd"));
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
            // Config selector
            //----------

            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            List<string> tToolBarList = new List<string>();
            foreach (DGWConfig tConfig in ConfigList.ConfigList)
            {
                tToolBarList.Add(tConfig.ProjectName);
            }
            ConfigList.Selected = GUILayout.Toolbar(ConfigList.Selected, tToolBarList.ToArray());
            Config = ConfigList.ConfigList[ConfigList.Selected];
            if (GUILayout.Button("Add", GUILayout.Width(80)))
            {
                Config = ConfigList.AddNewConfig();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            //----------
            // Project box
            //----------
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                // form project
                EditorGUILayout.LabelField("Project settings", EditorStyles.boldLabel);
                // indent add
                EditorGUI.indentLevel++;
                Config.ProjectName = EditorGUILayout.TextField("Project Name", Config.ProjectName);
                Config.ProjectSynopsis = EditorGUILayout.TextField("Project Synopsis", Config.ProjectSynopsis);
                Config.ProjectVersion = EditorGUILayout.TextField("Project Version", Config.ProjectVersion);

                Texture2D tTexture = (Texture2D)AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(Config.ProjectLogo);
                tTexture = (Texture2D) EditorGUILayout.ObjectField("Project Logo", tTexture, typeof(Texture2D), false);
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
                // indent remove
                EditorGUI.indentLevel--;
                // finish form project
            }
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
            tFolderObject = EditorGUILayout.ObjectField("Folder sources", tFolderObject, typeof(UnityEngine.Object), false);
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
            if (GUILayout.Button("...", EditorStyles.miniButtonRight, GUILayout.Width(k_PathButtonWidth)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Application.dataPath) + "/Documentation");
                string tFolderOutput = EditorUtility.OpenFolderPanel("Where do you want generate documentation?", Path.GetDirectoryName(Application.dataPath) + "/Documentation", "");
                //UnityEngine.Debug.Log ("tFolderOutput = " + tFolderOutput);
                //UnityEngine.Debug.Log ("Application.dataPath = " + Application.dataPath);
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
            GUILayout.EndVertical();
            //----------

            //----------
            // Tool function
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
                    Config.EmptyFill();
                    SavePreferences();
                }
                if (GUILayout.Button("Remove"))
                {
                    Config = ConfigList.RemoveConfig(Config);
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
            Texture2D tImageLogo = AssetDatabase.LoadAssetAtPath<Texture2D>(DGWFindPackage.PathOfPackage("/Scripts/Editor/Resources/Textures/IdemobiIconAlpha.png"));
            GUILayout.Space(20.0F);
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUIContent tContent = new GUIContent(tImageLogo);
            GUILayout.Label(tContent, tStyleImage, GUILayout.Height(64.0F), GUILayout.Width(64.0F));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Label(new GUIContent("idéMobi ©"), tStyleBold);
            GUILayout.Label(new GUIContent("Copyright 2017"), tStyleBold);
            tStyle.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label(new GUIContent("idéMobi is a french society. All rights based on french law."), tStyle);
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
            // UnityEngine.Debug.Log ("DGWDocumentationGenerator PathConfig()");
            string rReturn = Path.GetDirectoryName(Application.dataPath) + "/DGWConfigListSave.txt";
            return rReturn;
        }
        //-------------------------------------------------------------------------------------------------------------
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
}
//=====================================================================================================================