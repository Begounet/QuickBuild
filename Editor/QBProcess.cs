using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Diagnostics;
using System;
using System.Text;
using System.Runtime.InteropServices;

namespace QuickBuild
{
    public class QBProcess
    {
#if UNITY_EDITOR_WIN
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool SetWindowPos(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, int flags);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern IntPtr GetForegroundWindow();
#endif

        public event Action<QBProcess> OnProcessCompleted;

        private Thread _thread;
        private Process _process;
        private ProcessStartInfo _processStartInfo;
        private int _instanceID;
        protected QBProfile _profile;

        private const string QBMessageFormatPrefix = "<b>[QB:{0}]</b> {1}";
        
        public QBProcess(string buildPath, QBProfile editorProfile, QBPlayerSettings playerSettings, int instanceID)
        {
            _instanceID = instanceID;
            _profile = editorProfile;

            _processStartInfo = new ProcessStartInfo();
            //_processStartInfo.UseShellExecute = false;
			_processStartInfo.WindowStyle = ProcessWindowStyle.Normal;
//			processStartInfo.RedirectStandardOutput = EditorSettings.RedirectOutputLog;
            _processStartInfo.FileName = buildPath;
            _processStartInfo.Arguments = BuildCommandLineArguments(editorProfile, playerSettings, instanceID);

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

			// Does not work for now... _process.MainWindowTitle is always invalid
			_process.WaitForInputIdle();
			_process.Refresh();
            MoveProcessWindowIfRequired(_process);
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

            int width, height;
            editorProfile.GetScreenSizeForInstance(instanceID, out width, out height);
            AddCommandLineArgument(sb, QBCommandLineParameters.Screen_Width, width);
            AddCommandLineArgument(sb, QBCommandLineParameters.Screen_Height, height);

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
        
        private void MoveProcessWindowIfRequired(Process process)
        {
			if (process.MainWindowHandle.ToInt32() == 0)
			{
				return;
			}

            QBInstanceData instanceData = _profile.GetInstanceData(_instanceID);
            if (instanceData != null && instanceData.screenPosition.Override)
            {
#if UNITY_EDITOR_WIN
                int width, height;
                _profile.GetScreenSizeForInstance(_instanceID, out width, out height);
				IntPtr id = GetForegroundWindow();
				bool repaint = true;

				UnityEngine.Debug.LogFormat("Move window [{0}, {1}, {2}, {3}]", instanceData.screenPosition.X, instanceData.screenPosition.Y, width, height);
//                QBProcess.MoveWindow(
//                    id, 
//                    instanceData.screenPosition.X, instanceData.screenPosition.Y,
//                    width, height,
//                    repaint
//                    );
				bool result = QBProcess.SetWindowPos(
					id,
					instanceData.screenPosition.X, instanceData.screenPosition.Y,
					width, height,
					0x0010 | 0x0200 | 0x0001 | 0x0004
				);

				UnityEngine.Debug.LogFormat("Move success : {0}", result);
				if (!result)
				{
					int errorCode = Marshal.GetLastWin32Error();
					UnityEngine.Debug.LogFormat("Error code : {0}", errorCode);
				}
#endif
            }
        }
    }

}