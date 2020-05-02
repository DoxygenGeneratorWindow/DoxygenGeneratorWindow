//=====================================================================================================================
//
// ideMobi copyright 2020 
// All rights reserved by ideMobi
//
//=====================================================================================================================

using System;
using System.IO;
using UnityEngine;
using UnityEditor;

//=====================================================================================================================
namespace DoxygenGeneratorWindow
{
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    /// <summary>
    /// DGW config reccord your config... it's enough!
    /// </summary>
    [Serializable]
    public class DGWConfig
    {
        //-------------------------------------------------------------------------------------------------------------
        public DGWTools UseTools = DGWTools.Doxygen;
        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// The name of the folder to analyze.
        /// </summary>
        public string FolderName = "";
        /// <summary>
        /// The folder to output the documentation.
        /// </summary>
        public string FolderOutput = "";
        /// <summary>
        /// The name of the project.
        /// </summary>
        public string ProjectName = "";
        /// <summary>
        /// The logo for project.
        /// </summary>
        public string ProjectLogo = "";
        /// <summary>
        /// The project synopsis.
        /// </summary>
        public string ProjectSynopsis = "";
        /// <summary>
        /// The project version.
        /// </summary>
        public string ProjectVersion = "";
        /// <summary>
        /// Is analyze is recursive or not?.
        /// </summary>
        public bool Recursive = true;
        /// <summary>
        /// The generate HTML Doc.
        /// </summary>
        public bool GenerateHTML = true;
        /// <summary>
        /// The tint color of the HTML doc.
        /// </summary>
        public Color HTMLColor = Color.gray;
        /// <summary>
        /// The generate latex Doc.
        /// </summary>
        public bool GenerateLatex = false;
        /// <summary>
        /// The generate XML Doc.
        /// </summary>
        public bool GenerateXML = false;
        /// <summary>
        /// The generate RTF Doc.
        /// </summary>
		public bool GenerateRTF = false;
        //-------------------------------------------------------------------------------------------------------------
        public DGWConfig()
        {
        }
        //-------------------------------------------------------------------------------------------------------------
        public void EmptyFill()
        {
            ProjectName = PlayerSettings.productName;
            ProjectSynopsis = "Documentation for " + PlayerSettings.productName + " source code";
            ProjectVersion = PlayerSettings.bundleVersion;
            Directory.CreateDirectory(Path.GetDirectoryName(Application.dataPath) + "/Documentation");
            FolderOutput = "/Documentation";
            FolderName = "Assets";
        }
        //-------------------------------------------------------------------------------------------------------------
        public string FolderOutputAbsolute()
        {
            string tProjectPath = Path.GetDirectoryName(Application.dataPath);
            string tAbsolutePath = tProjectPath + FolderOutput;
            return tAbsolutePath;
        }
        //-------------------------------------------------------------------------------------------------------------
        public void SetFolderOutputAbsolute(string sAbsolutePath)
        {
            string tProjectPath = Path.GetDirectoryName(Application.dataPath);
            if (sAbsolutePath.StartsWith(tProjectPath))
            {
                FolderOutput = sAbsolutePath.Substring(tProjectPath.Length);
            }
            else
            {
                FolderOutput = "...folder must be inside project directory...";
            }
        }
        //-------------------------------------------------------------------------------------------------------------
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
}
//=====================================================================================================================