using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

[System.Serializable]
public class QBEditorSettings 
{
	private const string ExecutableName = "quick_build.exe";
	private const string DefaultBuildPath = "Builds/QuickBuild";
	private const string QBEditorSettingsEditorPrefKeyPrefix = "QBEditorSettings_";

	[System.Serializable]
	public class QBScrenSettings
	{
		public bool	IsFullScreen = false;
		public int ScreenWidth = 800;
		public int ScreenHeight = 600;
	}

	private int SettingsVersion = 1;

	public int NumInstances = 1;
	public bool BuildScriptsOnly = false;

	#region Advanced Settings
	public bool AdvancedSettingsFoldout = false;
	public bool DisplayInstanceID = false;
	public bool RedirectOutputLog = true;
	public bool ScreenSettingsFoldout = false;
	public QBScrenSettings ScreenSettings = new QBScrenSettings();

	public string BuildDirectoryPath
	{
		get
		{
			if (_BuildDirectoryPath == null)
			{
				InitializeBuildDirectoryPath();
			}
			return (_BuildDirectoryPath);
		}
		set
		{
			_BuildDirectoryPath = value;
		}
	}
	public string _BuildDirectoryPath = null;

	public string ExecutablePath
	{
		get 
		{
			return (BuildDirectoryPath != null ? BuildDirectoryPath + "/" + ExecutableName : string.Empty);
		}
	}

	#region Expert Settings
	public bool ExpertSettingsFoldout = false;
	public bool AllowDebugging = true;
	public bool LaunchInBatchMode = false;
	#endregion

	#endregion

	#region EditorPref Keys
	private string EKey_SettingsVersion 		{ get { return (EditorPrefsKey("SettingsVersion")); } }
	private string EKey_NumInstances 			{ get { return (EditorPrefsKey("NumInstances")); } }
	private string EKey_BuildScriptsOnly 		{ get { return (EditorPrefsKey("BuildScriptsOnly")); } }
	private string EKey_AdvancedSettingsFoldout { get { return (EditorPrefsKey("AdvancedSettingsFoldout")); } }
	private string EKey_DisplayInstanceID 		{ get { return (EditorPrefsKey("DisplayInstanceID")); } }
	private string EKey_RedirectOutputLog 		{ get { return (EditorPrefsKey("RedirectOutputLog")); } }
	private string EKey_ScreenSettingsFoldout	{ get { return (EditorPrefsKey("ScreenSettingsFoldout")); } }
	private string EKey_IsFullScreen 			{ get { return (EditorPrefsKey("IsFullScreen")); } }
	private string EKey_ScreenWidth 			{ get { return (EditorPrefsKey("ScreenWidth")); } }
	private string EKey_ScreenHeight 			{ get { return (EditorPrefsKey("ScreenHeight")); } }
	private string EKey_BuildDirectoryPath 		{ get { return (EditorPrefsKey("BuildDirectoryPath")); } }
	private string EKey_ExpertSettingsFoldout	{ get { return (EditorPrefsKey("ExpertSettingsFoldout")); } }
	private string EKey_AllowDebugging 			{ get { return (EditorPrefsKey("AllowDebugging")); } }
	private string EKey_LaunchInBatchMode 		{ get { return (EditorPrefsKey("LaunchInBatchMode")); } }
	#endregion

	public void InitializeBuildDirectoryPath()
	{
		_BuildDirectoryPath = Path.GetDirectoryName(Application.dataPath) + "/" + DefaultBuildPath;
	}

	public void	Save()
	{
		EditorPrefs.SetInt(EKey_SettingsVersion, SettingsVersion);

		EditorPrefs.SetInt(EKey_NumInstances, NumInstances);
		EditorPrefs.SetBool(EKey_BuildScriptsOnly, BuildScriptsOnly);

		EditorPrefs.SetBool(EKey_AdvancedSettingsFoldout, AdvancedSettingsFoldout);
		EditorPrefs.SetBool(EKey_DisplayInstanceID, DisplayInstanceID);
		EditorPrefs.SetBool(EKey_RedirectOutputLog, RedirectOutputLog);

		EditorPrefs.SetBool(EKey_ScreenSettingsFoldout, ScreenSettingsFoldout);
		EditorPrefs.SetBool(EKey_IsFullScreen, ScreenSettings.IsFullScreen);
		EditorPrefs.SetInt(EKey_ScreenWidth, ScreenSettings.ScreenWidth);
		EditorPrefs.SetInt(EKey_ScreenHeight, ScreenSettings.ScreenHeight);
		  
		if (BuildDirectoryPath != null)
		{
			EditorPrefs.SetString(EKey_BuildDirectoryPath, BuildDirectoryPath);
		}

		EditorPrefs.SetBool(EKey_ExpertSettingsFoldout, ExpertSettingsFoldout);
		EditorPrefs.SetBool(EKey_AllowDebugging, AllowDebugging);
		EditorPrefs.SetBool(EKey_LaunchInBatchMode, LaunchInBatchMode);
	}

	public void	Load()
	{ 
		SettingsVersion = EditorPrefs.GetInt(EKey_SettingsVersion);

		if (SettingsVersion == 1)
		{
			NumInstances = EditorPrefs.GetInt(EKey_NumInstances);
			BuildScriptsOnly = EditorPrefs.GetBool(EKey_BuildScriptsOnly);

			AdvancedSettingsFoldout = EditorPrefs.GetBool(EKey_AdvancedSettingsFoldout);
			DisplayInstanceID = EditorPrefs.GetBool(EKey_DisplayInstanceID);
			RedirectOutputLog = EditorPrefs.GetBool(EKey_RedirectOutputLog);

			ScreenSettingsFoldout = EditorPrefs.GetBool(EKey_ScreenSettingsFoldout);
			ScreenSettings.IsFullScreen = EditorPrefs.GetBool(EKey_IsFullScreen);
			ScreenSettings.ScreenWidth = EditorPrefs.GetInt(EKey_ScreenWidth);
			ScreenSettings.ScreenHeight = EditorPrefs.GetInt(EKey_ScreenHeight);

			if (EditorPrefs.HasKey(EKey_BuildDirectoryPath))
			{
				BuildDirectoryPath = EditorPrefs.GetString(EKey_BuildDirectoryPath);
			}

			ExpertSettingsFoldout = EditorPrefs.GetBool(EKey_ExpertSettingsFoldout);
			AllowDebugging = EditorPrefs.GetBool(EKey_AllowDebugging);
			LaunchInBatchMode = EditorPrefs.GetBool(EKey_LaunchInBatchMode);
		}
	}

	private string EditorPrefsKey(string KeyName)
	{
		return (QBEditorSettingsEditorPrefKeyPrefix + KeyName);
	}
}
