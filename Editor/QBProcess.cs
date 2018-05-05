using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Diagnostics;
using System;
using System.Text;

namespace QuickBuild
{
    public class QBProcess
    {
        public event Action<QBProcess> OnProcessCompleted;

        private Thread _thread;
        private Process _process;
        private ProcessStartInfo _processStartInfo;
        private int _instanceID;

        private const string QBMessageFormatPrefix = "<b>[QB:{0}]</b> {1}";
        
        public QBProcess(string buildPath, QBProfile editorSettings, QBPlayerSettings playerSettings, int instanceID)
        {
            _instanceID = instanceID;

            _processStartInfo = new ProcessStartInfo();
            _processStartInfo.UseShellExecute = false;
//			processStartInfo.RedirectStandardOutput = EditorSettings.RedirectOutputLog;
            _processStartInfo.FileName = buildPath;
            _processStartInfo.Arguments = BuildCommandLineArguments(editorSettings, playerSettings, instanceID);

            UnityEngine.Debug.Log("Command line arguments : " + _processStartInfo.Arguments);
        }

        public void	Start()
        {
            Kill();

            _thread = new Thread(ProcessWorker);
            _thread.Start();
        }

        void ProcessWorker(object param)
        {
            _process = Process.Start(_processStartInfo);
//			process.EnableRaisingEvents = true;
//			process.OutputDataReceived += HandleOutputDataReceived;
//			process.Exited += HandleProcessExited;
//			process.BeginOutputReadLine();
        }

        void HandleOutputDataReceived (object sender, DataReceivedEventArgs e)
        {
            // Should be dispatched on Unity thread !
            string condition;
            string stackTrace;
            LogType logType;
            if (QBMessagePacker.UnpackMessage(e.Data, out condition, out stackTrace, out logType))
            {
                string messageContent = string.Format("{0}{1}<i>{2}</i>", condition, System.Environment.NewLine, stackTrace);
                string message = string.Format(QBMessageFormatPrefix, _instanceID, messageContent);
                switch (logType)
                {
                    case LogType.Log:
                        UnityEngine.Debug.Log(message);
                    break;

                    case LogType.Warning:
                        UnityEngine.Debug.LogWarning(message);
                    break;

                    default:
                        UnityEngine.Debug.LogError(message);
                    break;
                }
            }
            else
            {
                UnityEngine.Debug.Log(string.Format(QBMessageFormatPrefix, _instanceID, e.Data));
            }
        }

        void HandleProcessExited (object sender, EventArgs e)
        {
            _process = null;
        }

        public void Kill()
        {
            if (_thread != null && _thread.IsAlive)
            {
                _thread.Abort();
                _thread = null;
            }

            if (_process != null)
            {
                _process.Kill();
            }
        }

        private string	BuildCommandLineArguments(QBProfile editorProfile, QBPlayerSettings playerSettings, int instanceID)
        {
            StringBuilder sb = new StringBuilder();

            AddCommandLineArgument(sb, QBCommandLineParameters.EnableQuickBuild);
            AddCommandLineArgument(sb, QBCommandLineParameters.InstanceID, instanceID);
             
            AddCommandLineArgument(sb, QBCommandLineParameters.Screen_FullscreenMode, editorProfile.advancedSettings.screenSettings.isFullScreen ? 1 : 0); 
            AddCommandLineArgument(sb, QBCommandLineParameters.Screen_Width, editorProfile.advancedSettings.screenSettings.screenWidth);
            AddCommandLineArgument(sb, QBCommandLineParameters.Screen_Height, editorProfile.advancedSettings.screenSettings.screenHeight);

            string outputLogFileName = string.Empty;
            if (!editorProfile.advancedSettings.redirectOutputLog)
            {
                outputLogFileName = editorProfile.BuildDirectoryPath + "/" + string.Format(QBCommandLineParameters.LogFileFormat, instanceID);
                AddCommandLineArgument(sb, QBCommandLineParameters.LogFile, outputLogFileName);
            }
            else
            {
                AddCommandLineArgument(sb, QBCommandLineParameters.RedirectOutput);
            }

            if (editorProfile.expertSettings.launchInBatchMode)
            {
                AddCommandLineArgument(sb, QBCommandLineParameters.Batchmode);
                AddCommandLineArgument(sb, QBCommandLineParameters.NoGraphics);
            }

            if (editorProfile.advancedSettings.displayInstanceID)
            {
                AddCommandLineArgument(sb, QBCommandLineParameters.DisplayInstanceID);
            }


            if (playerSettings.AdditiveScenes.Length > 0)
            {
                AddCommandLineArgument(sb, QBCommandLineParameters.AdditiveScenes, QBCommandLineHelper.PackStringArray(playerSettings.AdditiveScenes));
            }

            if (instanceID < editorProfile.expertSettings.customInstanceDatas.Length)
            {
                QBInstanceData qbInstanceData = editorProfile.expertSettings.customInstanceDatas[instanceID];
                if (qbInstanceData)
                {
                    AddCommandLineArgument(sb, qbInstanceData.commandLineArguments);

                    if (!string.IsNullOrEmpty(qbInstanceData.customName))
                    {
                        AddCommandLineArgument(sb, QBCommandLineParameters.CustomName, qbInstanceData.customName);
                    }
                }
            }

            AddCommandLineArgument(sb, editorProfile.advancedSettings.commandLineArguments);
            return (sb.ToString());
        }

        private void	AddCommandLineArgument(StringBuilder sb, string argument)
        {
            sb.AppendFormat("{0} ", argument);
        }

        private void	AddCommandLineArgument(StringBuilder sb, string key, object value)
        {
            AddCommandLineArgument(sb, string.Format("{0} {1}", key, value));
        }

    }

}