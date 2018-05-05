﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace QuickBuild
{
    [CreateAssetMenu(fileName = "QBProfile", menuName = "QuickBuild/Profile")]
    public class QBProfile : ScriptableObject
    {
        private const string ExecutableName = "quick_build.exe";
        private const string DefaultBuildPath = "Builds/QuickBuild";

        [System.Serializable]
        public class QBScrenSettings
        {
            public bool isFullScreen = false;
            public int screenWidth = 800;
            public int screenHeight = 600;
        }

        [Tooltip("Number of instance to run")]
        public int numberOfInstances = 1;

        [Tooltip("Build script only (as in build settings). If true, the build will be faster.")]
        public bool buildScriptsOnly = false;

        [System.Serializable]
        public class ExpertSettings
        {
            [Tooltip("Allow debugging (as in build settings). If true, the PDB will be added and the build will take more time")]
            public bool allowDebugging = true;

            [Tooltip("If true, the instance will be in 'batchmode' and won't have any graphics. Equivalent of '-batchmode -nographics'")]
            public bool launchInBatchMode = false;

            public QBInstanceData[] customInstanceDatas;
        }

        [System.Serializable]
        public class AdvancedSettings
        {
            [Tooltip("Should the instance ID displayed in the build?")]
            public bool displayInstanceID = false;
            public bool redirectOutputLog = true;
            public QBScrenSettings screenSettings;

            public string BuildDirectoryPath
            {
                get
                {
                    if (_buildDirectoryPath == null)
                    {
                        _buildDirectoryPath = Path.GetDirectoryName(Application.dataPath) + "/" + DefaultBuildPath;
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

            public ExpertSettings expertSettings;
        }

        public AdvancedSettings advancedSettings;

        public ExpertSettings expertSettings
        {
            get { return advancedSettings.expertSettings; }
        }
    }
}