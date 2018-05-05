using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Text;
using System.IO;
using UnityEngine.Profiling;
using UnityEditor.SceneManagement;

namespace QuickBuild
{
    public class QBBuilder
    {		
        public bool	BuildAndPlayCurrentScenes(QBEditorSettings Settings)
        {
            string[] scenes = GetSceneNames();

            DumpScenesToBuild();

            if (scenes.Length > 0)
            {
                string path = Settings.ExecutablePath;
                ThrowIfBuildPathInvalid(path);

                using (new QBBuildSettingsPreserveContext())
                {
                    PlayerSettings.runInBackground = true;
                    PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.HiddenByDefault;

                    EditorBuildSettings.scenes = GetEditorBuildSettingsScenes();
                    BuildPlayerOptions buildPlayerOptions = GetBuildPlayerOptions(Settings, path, scenes);

                    Profiler.BeginSample("Quick Build - Build operation");
                    string result = BuildPipeline.BuildPlayer(buildPlayerOptions);
                    Profiler.EndSample();

                    return (string.IsNullOrEmpty(result));
                }
            }

            return (false);
        }

        BuildPlayerOptions	GetBuildPlayerOptions(QBEditorSettings settings, string BuildPath, string[] SceneNames)
        {
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions() {
                locationPathName = BuildPath,
                scenes = SceneNames,
                options = BuildOptions.UncompressedAssetBundle,
                target = EditorUserBuildSettings.selectedStandaloneTarget,
                targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup
            };

            if (settings.allowDebugging)
            {
                buildPlayerOptions.options |= BuildOptions.Development | BuildOptions.AllowDebugging;
            }

            if (settings.buildScriptsOnly)
            {
                buildPlayerOptions.options |= BuildOptions.Development | BuildOptions.BuildScriptsOnly;
            }

            return (buildPlayerOptions);
        }

        string[]	GetSceneNames()
        {
            string[] scenes = new string[SceneManager.sceneCount];

            for (int i = 0; i < SceneManager.sceneCount; ++i)
            {
                scenes[i] = SceneManager.GetSceneAt(i).path;
            }

            return (scenes);
        }

        EditorBuildSettingsScene[]	GetEditorBuildSettingsScenes()
        {
            EditorBuildSettingsScene[] editorBuildScenes = new EditorBuildSettingsScene[SceneManager.sceneCount];
            for (int i = 0; i < SceneManager.sceneCount; ++i)
            {
                editorBuildScenes[i] = new EditorBuildSettingsScene() {
                    enabled = true,
                    path = SceneManager.GetSceneAt(i).path
                };
            }
            return (editorBuildScenes);
        }

        void	DumpScenesToBuild()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < SceneManager.sceneCount; ++i)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                sb.AppendLine(scene.name);
            }

            if (sb.Length > 0)
            {
                Debug.Log("Build scenes : " + System.Environment.NewLine + sb.ToString());
            }
            else
            {
                Debug.LogError("No scene to build.");
            }
        }

        void	ThrowIfBuildPathInvalid(string BuildPath)
        {
            if (string.IsNullOrEmpty(BuildPath))
            {
                throw new UnityException("Quick Build need a build location. See the settings.");
            }
        }
    }
}