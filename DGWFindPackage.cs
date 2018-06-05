//=====================================================================================================================
//
// ideMobi copyright 2017 
// All rights reserved by ideMobi
//
//=====================================================================================================================

using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

//=====================================================================================================================
namespace DoxygenGeneratorWindow
{
	//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
	/// <summary>
	/// Find package path class.
	/// Use the ScriptableObject to find the path of this package
	/// </summary>
	public class DGWFindPackage : ScriptableObject
	{
		//-------------------------------------------------------------------------------------------------------------
		#region Const
		//-------------------------------------------------------------------------------------------------------------
		// Insert constants here
		//-------------------------------------------------------------------------------------------------------------
		#endregion
		//-------------------------------------------------------------------------------------------------------------
		#region Class properties
		//-------------------------------------------------------------------------------------------------------------
		// Insert static properties here
		/// <summary>
		/// The shared instance.
		/// </summary>
		private static DGWFindPackage kSharedInstance;
		//-------------------------------------------------------------------------------------------------------------
		#endregion
		//-------------------------------------------------------------------------------------------------------------
		#region Instance properties
		//-------------------------------------------------------------------------------------------------------------
		// Insert instance properties here
		/// <summary>
		/// The script file path.
		/// </summary>
		public string ScriptFilePath;
		/// <summary>
		/// The script folder.
		/// </summary>
		public string ScriptFolder;
		/// <summary>
		/// The script folder from assets.
		/// </summary>
		public string ScriptFolderFromAssets;
		//-------------------------------------------------------------------------------------------------------------
		#endregion
		//-------------------------------------------------------------------------------------------------------------
		#region Class methods
		//-------------------------------------------------------------------------------------------------------------
		// Insert static methods here
		/// <summary>
		/// Ascensor to shared instance.
		/// </summary>
		/// <returns>The shared instance.</returns>
		public static DGWFindPackage SharedInstance ()
		{
			if (kSharedInstance == null) {
				kSharedInstance = ScriptableObject.CreateInstance ("DGWFindPackage") as DGWFindPackage;
				kSharedInstance.ReadPaths ();
			}
			return kSharedInstance; 
		}
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Packages the path.
		/// </summary>
		/// <returns>The path.</returns>
		/// <param name="sAddPath">add path.</param>
		public static string PathOfPackage (string sAddPath = "")
		{
			return SharedInstance ().ScriptFolderFromAssets + sAddPath;
		}
		//-------------------------------------------------------------------------------------------------------------
		#endregion
		//-------------------------------------------------------------------------------------------------------------
		#region Instance methods
		//-------------------------------------------------------------------------------------------------------------
		// Insert instance methods here
		/// <summary>
		/// Reads the paths.
		/// </summary>
		public void ReadPaths ()
		{
			MonoScript tMonoScript = MonoScript.FromScriptableObject (this);
			ScriptFilePath = AssetDatabase.GetAssetPath (tMonoScript);
			FileInfo tFileInfo = new FileInfo (ScriptFilePath);
			ScriptFolder = tFileInfo.Directory.ToString ();
			ScriptFolder = ScriptFolder.Replace ("\\", "/");
			ScriptFolderFromAssets = "Assets" + ScriptFolder.Replace (Application.dataPath, "");
		}
		//-------------------------------------------------------------------------------------------------------------
		#endregion
		//-------------------------------------------------------------------------------------------------------------
	}
	//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
}
//=====================================================================================================================
#endif