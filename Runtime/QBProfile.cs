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

        [Tooltip("The path to the build generated by QuickBuild. It is relative to your project path.")]
        public string buildPath = DefaultBuildPath;

        private string _buildDirectoryPath = null;

        public string BuildDirectoryPath
        {
            get
            {
                if (_buildDirectoryPath == null)
                {
                    _buildDirectoryPath = Path.GetDirectoryName(Application.dataPath) + "/" + buildPath;
                }
                return (_buildDirectoryPath);
            }
            set
            {
                _buildDirectoryPath = value;
            }
        }

        public string ExecutablePath
        {
            get
            {
                return (BuildDirectoryPath != null ? BuildDirectoryPath + "/" + ExecutableName : string.Empty);
            }
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

			[Tooltip("Allow to use custom settings for each instance started")]
            public QBInstanceData[] customInstanceDatas;
        }

        [System.Serializable]
        public class AdvancedSettings
        {
            [Tooltip("Should the instance ID displayed in the build?")]
            public bool displayInstanceID = false;

            [Tooltip("Not working for now. You can still connect via the console to get logs.")]
            public bool redirectOutputLog = true;
            public QBScrenSettings screenSettings;

            [Tooltip("Allow to add custom command line arguments to all instances started.")]
            public string commandLineArguments;

            public ExpertSettings expertSettings;
        }

        public AdvancedSettings advancedSettings;

        public ExpertSettings expertSettings
        {
            get { return advancedSettings.expertSettings; }
        }


        public QBInstanceData  GetInstanceData(int instanceID)
        {
            if (instanceID < expertSettings.customInstanceDatas.Length)
            {
                return (expertSettings.customInstanceDatas[instanceID]);
            }
            return (null);
        }
        
        public void GetScreenSizeForInstance(int instanceID, out int width, out int height)
        {
            QBInstanceData instanceData = GetInstanceData(instanceID);
            if (instanceData != null && instanceData.screenSize.Override)
            {
                width = instanceData.screenSize.Width;
                height = instanceData.screenSize.Height;
            }
            else
            {
                width = advancedSettings.screenSettings.screenWidth;
                height = advancedSettings.screenSettings.screenHeight;
            }
        }
    }
}