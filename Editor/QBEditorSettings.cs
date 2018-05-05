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
        public bool	isFullScreen = false;
        public int screenWidth = 800;
        public int screenHeight = 600;
    }

    private int _settingsVersion = 1;

    public int numInstances = 1;
    public bool buildScriptsOnly = false;

    #region Advanced Settings
    public bool advancedSettingsFoldout = false;
    public bool displayInstanceID = false;
    public bool redirectOutputLog = true;
    public bool screenSettingsFoldout = false;
    public QBScrenSettings screenSettings = new QBScrenSettings();

    public string BuildDirectoryPath
    {
        get
        {
            if (_buildDirectoryPath == null)
            {
                InitializeBuildDirectoryPath();
            }
            return (_buildDirectoryPath);
        }
        set
        {
            _buildDirectoryPath = value;
        }
    }
    private string _buildDirectoryPath = null;

    public string ExecutablePath
    {
        get 
        {
            return (BuildDirectoryPath != null ? BuildDirectoryPath + "/" + ExecutableName : string.Empty);
        }
    }
    
    public QBInstanceData[] customInstanceDatas;

    #region Expert Settings
    public bool expertSettingsFoldout = false;
    public bool allowDebugging = true;
    public bool launchInBatchMode = false;
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
        _buildDirectoryPath = Path.GetDirectoryName(Application.dataPath) + "/" + DefaultBuildPath;
    }

    public void	Save()
    {
        EditorPrefs.SetInt(EKey_SettingsVersion, _settingsVersion);

        EditorPrefs.SetInt(EKey_NumInstances, numInstances);
        EditorPrefs.SetBool(EKey_BuildScriptsOnly, buildScriptsOnly);

        EditorPrefs.SetBool(EKey_AdvancedSettingsFoldout, advancedSettingsFoldout);
        EditorPrefs.SetBool(EKey_DisplayInstanceID, displayInstanceID);
        EditorPrefs.SetBool(EKey_RedirectOutputLog, redirectOutputLog);

        EditorPrefs.SetBool(EKey_ScreenSettingsFoldout, screenSettingsFoldout);
        EditorPrefs.SetBool(EKey_IsFullScreen, screenSettings.isFullScreen);
        EditorPrefs.SetInt(EKey_ScreenWidth, screenSettings.screenWidth);
        EditorPrefs.SetInt(EKey_ScreenHeight, screenSettings.screenHeight);
          
        if (BuildDirectoryPath != null)
        {
            EditorPrefs.SetString(EKey_BuildDirectoryPath, BuildDirectoryPath);
        }

        EditorPrefs.SetBool(EKey_ExpertSettingsFoldout, expertSettingsFoldout);
        EditorPrefs.SetBool(EKey_AllowDebugging, allowDebugging);
        EditorPrefs.SetBool(EKey_LaunchInBatchMode, launchInBatchMode);
    }

    public void	Load()
    { 
        _settingsVersion = EditorPrefs.GetInt(EKey_SettingsVersion);

        if (_settingsVersion == 1)
        {
            numInstances = EditorPrefs.GetInt(EKey_NumInstances);
            buildScriptsOnly = EditorPrefs.GetBool(EKey_BuildScriptsOnly);

            advancedSettingsFoldout = EditorPrefs.GetBool(EKey_AdvancedSettingsFoldout);
            displayInstanceID = EditorPrefs.GetBool(EKey_DisplayInstanceID);
            redirectOutputLog = EditorPrefs.GetBool(EKey_RedirectOutputLog);

            screenSettingsFoldout = EditorPrefs.GetBool(EKey_ScreenSettingsFoldout);
            screenSettings.isFullScreen = EditorPrefs.GetBool(EKey_IsFullScreen);
            screenSettings.screenWidth = EditorPrefs.GetInt(EKey_ScreenWidth);
            screenSettings.screenHeight = EditorPrefs.GetInt(EKey_ScreenHeight);

            if (EditorPrefs.HasKey(EKey_BuildDirectoryPath))
            {
                BuildDirectoryPath = EditorPrefs.GetString(EKey_BuildDirectoryPath);
            }

            expertSettingsFoldout = EditorPrefs.GetBool(EKey_ExpertSettingsFoldout);
            allowDebugging = EditorPrefs.GetBool(EKey_AllowDebugging);
            launchInBatchMode = EditorPrefs.GetBool(EKey_LaunchInBatchMode);
        }
    }

    private string EditorPrefsKey(string KeyName)
    {
        return (QBEditorSettingsEditorPrefKeyPrefix + KeyName);
    }
}
