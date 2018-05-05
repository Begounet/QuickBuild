using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace QuickBuild
{

    public class QBPlayer
    {
        QBProcesses processes;

        public QBPlayer()
        {
            processes = new QBProcesses();
        }

        public void	RunBuilds(QBEditorSettings Settings)
        {
            processes.StartNewProcess(Settings.ExecutablePath, Settings, GeneratePlayerSettings(), Settings.NumInstances);
        }

        QBPlayerSettings	GeneratePlayerSettings()
        {
            QBPlayerSettings qbPlayerSettings = new QBPlayerSettings();

            qbPlayerSettings.AdditiveScenes = new string[SceneManager.sceneCount - 1];
            int sceneIndex = 0;
            for (int i = 0; i < SceneManager.sceneCount; ++i)
            {
                Scene currentScene = SceneManager.GetSceneAt(i);
                // Add only additives!
                if (currentScene != SceneManager.GetActiveScene())
                {
                    qbPlayerSettings.AdditiveScenes[sceneIndex++] = SceneManager.GetSceneAt(i).name;
                }
            }

            return (qbPlayerSettings);
        }
    }

}