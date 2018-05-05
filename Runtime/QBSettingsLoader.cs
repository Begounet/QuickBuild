using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

namespace QuickBuild
{

    public class QBSettingsLoader
    {
        public class Settings
        {
            public int instanceID;
            public string customName;
        }


        public static Settings settings;

        
        [RuntimeInitializeOnLoadMethod]
        static void OnGameInitialized()
        {
            Debug.Log("Quick build settings loader init");

            if (IsQuickBuildEnabled())
            {
                settings = new Settings();
                ProcessCommandLineArguments();
            }
        }

        static void	ProcessCommandLineArguments()
        {	
            ParseInstanceIDCommandArgs();
            ParseRedirectOutputCommandArgs();
            ParseDisplayInstanceID();
            ParseCustomName();
            ParseAdditiveScenes();
        }

        private static void ParseCustomName()
        {
            string customName;
            if (FindCommandArgument(QBCommandLineParameters.CustomName, out customName))
            {
                settings.customName = customName;
            }
        }

        static void ParseInstanceIDCommandArgs()
        {
            // Get instance ID
            int instanceID;
            if (FindCommandArgumentAsInt(QBCommandLineParameters.InstanceID, out instanceID))
            {
                settings.instanceID = instanceID;
            }
        }

        static void ParseRedirectOutputCommandArgs()
        {
            // Redirect output
            if (DoesCommandLineContainsKey(QBCommandLineParameters.RedirectOutput))
            {
                RedirectOutput();
            }
        }

        static void RedirectOutput()
        {
            Application.logMessageReceived += HandleLogMessageReceived;
        }

        static void HandleLogMessageReceived (string condition, string stackTrace, LogType type)
        {
            string formattedMessage = QBMessagePacker.PackMessage(condition, stackTrace, type);

            // Write to stdout so it can be get by the editor
            Console.WriteLine(formattedMessage);
        }

        static void ParseDisplayInstanceID()
        {
            if (DoesCommandLineContainsKey(QBCommandLineParameters.DisplayInstanceID))
            {
                QBHUD.CreateHUD();
            }
        }

        static void ParseAdditiveScenes()
        {
            string scenesPack;
            if (FindCommandArgument(QBCommandLineParameters.AdditiveScenes, out scenesPack))
            {
                string[] scenes = QBCommandLineHelper.UnpackStringArray(scenesPack);
                LoadAdditiveScenes(scenes);
            }
        }

        static bool	IsQuickBuildEnabled()
        {
            return (DoesCommandLineContainsKey(QBCommandLineParameters.EnableQuickBuild));
        }
        static bool DoesCommandLineContainsKey(string Key)
        {
            string[] commandLineArguments = System.Environment.GetCommandLineArgs();

            for (int i = 0; i < commandLineArguments.Length; ++i)
            {
                if (IsKeyCommand(commandLineArguments[i]) && commandLineArguments[i] == Key)
                {
                    return (true);
                }
            }

            return (false);
        }

        static bool	FindCommandArgument(string Key, out string Argument)
        {
            string[] commandLineArguments = System.Environment.GetCommandLineArgs();

            for (int i = 0; i < commandLineArguments.Length; ++i)
            {
                if (IsKeyCommand(commandLineArguments[i]) && 
                    commandLineArguments[i] == Key &&
                    !IsKeyCommand(commandLineArguments[i + 1]))
                {
                    Argument = commandLineArguments[i + 1];
                    return (true);
                }
            }

            Argument = string.Empty;
            return (false);
        }

        static bool FindCommandArgumentAsInt(string Key, out int ArgumentInteger)
        {
            string outValue;
            if (FindCommandArgument(Key, out outValue))
            {
                return (int.TryParse(outValue, out ArgumentInteger));
            }

            ArgumentInteger = 0;
            return (false);
        }

        static bool IsKeyCommand(string commandArgument)
        {
            return (commandArgument[0] == '-');
        }

        static void LoadAdditiveScenes(string[] scenes)
        {
            for (int i = 0 ; i < scenes.Length ; ++i)
            {
                SceneManager.LoadScene(scenes[i], LoadSceneMode.Additive);
            }
        }
    }

}