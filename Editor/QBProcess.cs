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

        Thread thread;
        Process process;
        ProcessStartInfo processStartInfo;
        int instanceID;

        private const string QBMessageFormatPrefix = "<b>[QB:{0}]</b> {1}";
        
        public QBProcess(string BuildPath, QBEditorSettings EditorSettings, QBPlayerSettings PlayerSettings, int InstanceID)
        {
            instanceID = InstanceID;

            processStartInfo = new ProcessStartInfo();
            processStartInfo.UseShellExecute = false;
//			processStartInfo.RedirectStandardOutput = EditorSettings.RedirectOutputLog;
            processStartInfo.FileName = BuildPath;
            processStartInfo.Arguments = BuildCommandLineArguments(EditorSettings, PlayerSettings, InstanceID);

            UnityEngine.Debug.Log("Command line arguments : " + processStartInfo.Arguments);
        }

        public void	Start()
        {
            Kill();

            thread = new Thread(ProcessWorker);
            thread.Start();
        }

        void ProcessWorker(object param)
        {
            process = Process.Start(processStartInfo);
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
                string message = string.Format(QBMessageFormatPrefix, instanceID, messageContent);
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
                UnityEngine.Debug.Log(string.Format(QBMessageFormatPrefix, instanceID, e.Data));
            }
        }

        void HandleProcessExited (object sender, EventArgs e)
        {
            process = null;
        }

        public void Kill()
        {
            if (thread != null && thread.IsAlive)
            {
                thread.Abort();
                thread = null;
            }

            if (process != null)
            {
                process.Kill();
            }
        }

        private string	BuildCommandLineArguments(QBEditorSettings EditorSettings, QBPlayerSettings PlayerSettings, int InstanceID)
        {
            StringBuilder sb = new StringBuilder();

            AddCommandLineArgument(sb, QBCommandLineParameters.EnableQuickBuild);
            AddCommandLineArgument(sb, QBCommandLineParameters.InstanceID, InstanceID);
             
            AddCommandLineArgument(sb, QBCommandLineParameters.Screen_FullscreenMode, EditorSettings.screenSettings.isFullScreen ? 1 : 0); 
            AddCommandLineArgument(sb, QBCommandLineParameters.Screen_Width, EditorSettings.screenSettings.screenWidth);
            AddCommandLineArgument(sb, QBCommandLineParameters.Screen_Height, EditorSettings.screenSettings.screenHeight);

            string outputLogFileName = string.Empty;
            if (!EditorSettings.redirectOutputLog)
            {
                outputLogFileName = EditorSettings.BuildDirectoryPath + "/" + string.Format(QBCommandLineParameters.LogFileFormat, InstanceID);
                AddCommandLineArgument(sb, QBCommandLineParameters.LogFile, outputLogFileName);
            }
            else
            {
                AddCommandLineArgument(sb, QBCommandLineParameters.RedirectOutput);
            }

            if (EditorSettings.launchInBatchMode)
            {
                AddCommandLineArgument(sb, QBCommandLineParameters.Batchmode);
                AddCommandLineArgument(sb, QBCommandLineParameters.NoGraphics);
            }

            if (EditorSettings.displayInstanceID)
            {
                AddCommandLineArgument(sb, QBCommandLineParameters.DisplayInstanceID);
            }

            if (PlayerSettings.AdditiveScenes.Length > 0)
            {
                AddCommandLineArgument(sb, QBCommandLineParameters.AdditiveScenes, QBCommandLineHelper.PackStringArray(PlayerSettings.AdditiveScenes));
            }

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