using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

/// <summary>
/// Save and restore settings modified for the QB build.
/// Should be used with : using (new QBBuildSettingsPreserveContext) 
/// { 
///  // change settings 
///  // start build
/// } 
/// // settings are automatically restored
/// </summary>
public class QBBuildSettingsPreserveContext : IDisposable
{
    bool	playerSettings_RunInBackground;

    ResolutionDialogSetting playerSettings_DisplayResolutionDialog;

    EditorBuildSettingsScene[]	editorBuildSettingsScenes;

    public QBBuildSettingsPreserveContext()
    {
        playerSettings_RunInBackground = PlayerSettings.runInBackground;
        playerSettings_DisplayResolutionDialog = PlayerSettings.displayResolutionDialog;
        editorBuildSettingsScenes = EditorBuildSettings.scenes;
    }

    #region IDisposable implementation
    public void Dispose()
    {
        PlayerSettings.runInBackground = playerSettings_RunInBackground;
        PlayerSettings.displayResolutionDialog = playerSettings_DisplayResolutionDialog;
        EditorBuildSettings.scenes = editorBuildSettingsScenes;
    }
    #endregion
}
