using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.IO;
using System.Text;
using UnityEngine.Profiling;
using System.Threading;

using SDiag = System.Diagnostics;
using System;

namespace QuickBuild
{

    public class QBWindow : EditorWindow {

        QBBuilder builder;
        QBPlayer player;
        QBEditorSettings settings;
        QBProfile profile;

        Vector2 mainScrollViewPosition;

        [MenuItem("Window/Quick Build")]
        static void	Init()
        {
            QBWindow quickBuildWindow = (QBWindow)EditorWindow.GetWindow(typeof(QBWindow));
            quickBuildWindow.Show();
        } 

        void	OnEnable()
        {
            titleContent = new GUIContent("Quick Build");
            builder = new QBBuilder();
            player = new QBPlayer();
            settings = new QBEditorSettings();
            settings.Load();
        }

        void	OnDisable()
        {
            settings.Save();
        }

        void	OnGUI()
        {
            DrawBuildButtons();
            GUILayout.Space(10);
            DrawQBProfile();
        }

        private void DrawQBProfile()
        {
            profile = (QBProfile)EditorGUILayout.ObjectField(profile, typeof(QBProfile), false);

            if (profile != null)
            {
                SerializedObject serializedProfile = new SerializedObject(profile);

                SerializedProperty props = serializedProfile.GetIterator();
                props.NextVisible(true);
                while (props.NextVisible(false))
                {
                    EditorGUILayout.PropertyField(props, true);
                }

                if (serializedProfile.ApplyModifiedProperties())
                {
                    serializedProfile.UpdateIfRequiredOrScript();
                }
            }
        }

        void	DrawBuildButtons()
        {
            if (GUILayout.Button("Build & Start current scenes"))
            {
                if (builder.BuildAndPlayCurrentScenes(settings))
                {
                    player.RunBuilds(settings);	
                }
            }
        }
        void	DrawBuildSettings()
        {
            settings.numInstances = EditorGUILayout.IntField("Number of instances", settings.numInstances);
            settings.buildScriptsOnly = EditorGUILayout.Toggle("Build Scripts Only", settings.buildScriptsOnly);
            
            settings.advancedSettingsFoldout = EditorGUILayout.Foldout(settings.advancedSettingsFoldout, "Advanced Settings");
            if (settings.advancedSettingsFoldout)
            {
                using (new QBEditorLayoutIndent())
                {
                    DrawAdvancedSettings();
                }
            }
        }

        void	DrawAdvancedSettings()
        {
            settings.screenSettingsFoldout = EditorGUILayout.Foldout(settings.screenSettingsFoldout, "Screen");
            if (settings.screenSettingsFoldout)
            {
                using (new QBEditorLayoutIndent())
                {
                    settings.screenSettings.isFullScreen = EditorGUILayout.Toggle("Is Fullscreen ?", settings.screenSettings.isFullScreen);
                    settings.screenSettings.screenWidth = EditorGUILayout.IntField("Width", settings.screenSettings.screenWidth);
                    settings.screenSettings.screenHeight = EditorGUILayout.IntField("Height", settings.screenSettings.screenHeight);
                }
            }

            settings.displayInstanceID = EditorGUILayout.Toggle(new GUIContent("Display Instance ID", "Display the instance ID on the opened window so you can identify the player"), settings.displayInstanceID);

            GUI.enabled = false;
            settings.redirectOutputLog = EditorGUILayout.Toggle(new GUIContent("Redirect output log", "!! Not working now !! - If true, redirect the output to the opened Unity Editor. Otherwise, log into separate files in the build directory."), false/*settings.RedirectOutputLog*/);
            GUI.enabled = true;

            EditorGUILayout.BeginHorizontal();
            {
                settings.BuildDirectoryPath = EditorGUILayout.DelayedTextField("Build directory", settings.BuildDirectoryPath);
                if (GUILayout.Button("Browse...", GUILayout.MaxWidth(100)))
                {
                    string newBuildDirectoryPath = EditorUtility.SaveFolderPanel("Choose your build location...", settings.BuildDirectoryPath, "");
                    if (!string.IsNullOrEmpty(newBuildDirectoryPath))
                    {
                        settings.BuildDirectoryPath = newBuildDirectoryPath;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            settings.expertSettingsFoldout = EditorGUILayout.Foldout(settings.expertSettingsFoldout, "Expert Settings");
            if (settings.expertSettingsFoldout)
            {
                using (new QBEditorLayoutIndent())
                {
                    DrawExpertSettings();
                }
            }
        }

        void	DrawExpertSettings()
        {
            settings.allowDebugging = EditorGUILayout.Toggle("Allow debugging", settings.allowDebugging);
            settings.launchInBatchMode = EditorGUILayout.Toggle(new GUIContent("Batchmode", "Launch in batchmode so there is no rendering (see command line arguments manual). Implies -nographics. Useful for server."), settings.launchInBatchMode);
        }
    }

}