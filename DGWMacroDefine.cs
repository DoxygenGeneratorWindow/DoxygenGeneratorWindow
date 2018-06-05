using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;

//=====================================================================================================================
namespace DoxygenGeneratorWindow
{
	[InitializeOnLoad]
	/// <summary>
	/// Macro define can find if kMacro is set in the settings and add it if necessary.
	/// This class auto run at build project.
	/// You can use kMacro in precompile definition.
	/// </summary>
	public class DGWMacroDefine : IActiveBuildTargetChanged
	{
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// The macro to check in this project. It's tag in settings. The you can use #if define xxxxx #endif in your 
		/// code.
		/// </summary>
		const string kMacro = "DOXYGEN_GENERATOR_WINDOW";
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// The shared instance used for this class.
		/// </summary>
		static DGWMacroDefine kSharedInstance;
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes the DGWMacroDefine class. 
		/// Instance kSharedInstance to use it when method OnActiveBuildTargetChanged must be invoked
		/// </summary>
		static DGWMacroDefine ()
		{
			if (kSharedInstance == null) {
				kSharedInstance = new DGWMacroDefine ();
				kSharedInstance.OnChangedPlatform ();
			}
		}
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the callback order for IActiveBuildTargetChanged
		/// </summary>
		/// <value>The callback order.</value>
		public int callbackOrder { get { return 0; } }
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Raises the active build target changed event.
		/// </summary>
		/// <param name="previousTarget">Previous target.</param>
		/// <param name="newTarget">New target.</param>
		public void OnActiveBuildTargetChanged (BuildTarget previousTarget, BuildTarget newTarget)
		{
			OnChangedPlatform ();
		}
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Raises the changed platform event.
		/// </summary>
		public void OnChangedPlatform ()
		{
			InstallMacro (EditorUserBuildSettings.selectedBuildTargetGroup);
		}
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Installs the macro.
		/// </summary>
		/// <param name="sBuildTarget">S build target.</param>
		public void InstallMacro (BuildTargetGroup sBuildTarget)
		{
			if (PlayerSettings.GetScriptingDefineSymbolsForGroup (sBuildTarget).Contains (kMacro) == false) {
				//Debug.Log ("Install macro " + kMacro + " in " + sBuildTarget + " player settings");
				PlayerSettings.SetScriptingDefineSymbolsForGroup (sBuildTarget, PlayerSettings.GetScriptingDefineSymbolsForGroup (sBuildTarget) + ";" + kMacro);
				if (PlayerSettings.GetScriptingDefineSymbolsForGroup (sBuildTarget).Contains (kMacro) == false) {
					//Debug.LogError ("Fail to install macro " + kMacro + " in " + sBuildTarget + " player settings!");
				}
			}
		}
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Installs the macro in all build target.
		/// </summary>
		public void InstallMacroAll ()
		{
			Array BuildTargetGroupsArray = Enum.GetValues (typeof(BuildTargetGroup));
			foreach (BuildTargetGroup tBuildTarget in BuildTargetGroupsArray) {
				if (tBuildTarget != BuildTargetGroup.Unknown) {
					InstallMacro (tBuildTarget);
				}
			}
		}
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================
#endif
