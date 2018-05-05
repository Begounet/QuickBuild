using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace QuickBuild
{

    public class QBProcesses
    {
//		public event Action<QBProcess> OnProcessCompleted;
//		public event Action	OnAllProcessesCompleted;
        
        private List<QBProcess> _processes;

        public QBProcesses()
        {
            _processes = new List<QBProcess>();
        }

        public void	StartNewProcess(string buildPath, QBProfile editorProfile, QBPlayerSettings playerSettings, int numProcess = 1)
        {
            for (int processIndex = 0; processIndex < numProcess; ++processIndex)
            {
                StartNewSoloProcess(buildPath, editorProfile, playerSettings, processIndex);
            }
        }

        void StartNewSoloProcess(string BuildPath, QBProfile editorSettings, QBPlayerSettings PlayerSettings, int ProcessID)
        {
            QBProcess process = new QBProcess(BuildPath, editorSettings, PlayerSettings, ProcessID);
            process.OnProcessCompleted += HandleProcessCompleted;;
            _processes.Add(process);
            process.Start();
        }

        public void StopAllProcesses()
        {
            for (int i = 0; i < _processes.Count; ++i)
            {
                _processes[i].Kill();
            }
            _processes.Clear();
        }

        public int	GetNumRunningProcesses()
        {
            return (_processes.Count);
        }

        void HandleProcessCompleted(QBProcess Process)
        {
            _processes.Remove(Process);
        }
    }

}
