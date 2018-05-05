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
                if (builder.BuildAndPlayCurrentScenes(profile))
                {
                    player.RunBuilds(profile);	
                }
            }
        }
    }

}