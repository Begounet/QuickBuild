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
		
		List<QBProcess> processes;

		public QBProcesses()
		{
			processes = new List<QBProcess>();
		}

		public void	StartNewProcess(string BuildPath, QBEditorSettings EditorSettings, QBPlayerSettings PlayerSettings, int NumProcess = 1)
		{
			for (int processIndex = 0; processIndex < NumProcess; ++processIndex)
			{
				StartNewSoloProcess(BuildPath, EditorSettings, PlayerSettings, processIndex);
			}
		}

		void StartNewSoloProcess(string BuildPath, QBEditorSettings EditorSettings, QBPlayerSettings PlayerSettings, int ProcessID)
		{
			QBProcess process = new QBProcess(BuildPath, EditorSettings, PlayerSettings, ProcessID);
			process.OnProcessCompleted += HandleProcessCompleted;;
			processes.Add(process);
			process.Start();
		}

		public void StopAllProcesses()
		{
			for (int i = 0; i < processes.Count; ++i)
			{
				processes[i].Kill();
			}
			processes.Clear();
		}

		public int	GetNumRunningProcesses()
		{
			return (processes.Count);
		}

		void HandleProcessCompleted(QBProcess Process)
		{
			processes.Remove(Process);
		}
	}

}
