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

namespace QuickBuild
{

	public class QBWindow : EditorWindow {

		QBBuilder builder;
		QBPlayer player;
		QBEditorSettings settings;

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
			mainScrollViewPosition = GUILayout.BeginScrollView(mainScrollViewPosition);
			{
				DrawBuildSettings();
			}
			GUILayout.EndScrollView();
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
			settings.NumInstances = EditorGUILayout.IntField("Number of instances", settings.NumInstances);
			settings.BuildScriptsOnly = EditorGUILayout.Toggle("Build Scripts Only", settings.BuildScriptsOnly);
			
			settings.AdvancedSettingsFoldout = EditorGUILayout.Foldout(settings.AdvancedSettingsFoldout, "Advanced Settings");
			if (settings.AdvancedSettingsFoldout)
			{
				using (new QBEditorLayoutIndent())
				{
					DrawAdvancedSettings();
				}
			}
		}

		void	DrawAdvancedSettings()
		{
			settings.ScreenSettingsFoldout = EditorGUILayout.Foldout(settings.ScreenSettingsFoldout, "Screen");
			if (settings.ScreenSettingsFoldout)
			{
				using (new QBEditorLayoutIndent())
				{
					settings.ScreenSettings.IsFullScreen = EditorGUILayout.Toggle("Is Fullscreen ?", settings.ScreenSettings.IsFullScreen);
					settings.ScreenSettings.ScreenWidth = EditorGUILayout.IntField("Width", settings.ScreenSettings.ScreenWidth);
					settings.ScreenSettings.ScreenHeight = EditorGUILayout.IntField("Height", settings.ScreenSettings.ScreenHeight);
				}
			}

			settings.DisplayInstanceID = EditorGUILayout.Toggle(new GUIContent("Display Instance ID", "Display the instance ID on the opened window so you can identify the player"), settings.DisplayInstanceID);

			GUI.enabled = false;
			settings.RedirectOutputLog = EditorGUILayout.Toggle(new GUIContent("Redirect output log", "!! Not working now !! - If true, redirect the output to the opened Unity Editor. Otherwise, log into separate files in the build directory."), false/*settings.RedirectOutputLog*/);
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

			settings.ExpertSettingsFoldout = EditorGUILayout.Foldout(settings.ExpertSettingsFoldout, "Expert Settings");
			if (settings.ExpertSettingsFoldout)
			{
				using (new QBEditorLayoutIndent())
				{
					DrawExpertSettings();
				}
			}
		}

		void	DrawExpertSettings()
		{
			settings.AllowDebugging = EditorGUILayout.Toggle("Allow debugging", settings.AllowDebugging);
			settings.LaunchInBatchMode = EditorGUILayout.Toggle(new GUIContent("Batchmode", "Launch in batchmode so there is no rendering (see command line arguments manual). Implies -nographics. Useful for server."), settings.LaunchInBatchMode);
		}
	}

}