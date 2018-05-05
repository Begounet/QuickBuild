using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace QuickBuild
{

	[InitializeOnLoad]
	public class QBSettingsLoaderSettingsInitializer
	{
		static readonly int DefaultQuickBuildSettingsLoaderScriptExecutionOrder = -1000;

		static QBSettingsLoaderSettingsInitializer()
		{
			if (!EditorApplication.isPlayingOrWillChangePlaymode)
			{
				EditorApplication.update += OnEditorInitialized;
			}
		}

		static void OnEditorInitialized()
		{
			EditorApplication.update -= OnEditorInitialized;
			AssureQuickBuildSettingsLoaderExecuteEarly();
		}

		static void AssureQuickBuildSettingsLoaderExecuteEarly()
		{ 
			MonoScript qbSettingsLoaderScript = FindMonoScriptByType<QBSettingsLoader>();
			int executionOrder = MonoImporter.GetExecutionOrder(qbSettingsLoaderScript);
			if (executionOrder == 0)
			{
				MonoImporter.SetExecutionOrder(qbSettingsLoaderScript, DefaultQuickBuildSettingsLoaderScriptExecutionOrder);
			}
		}

		static MonoScript	FindMonoScriptByType<MonoScriptType>()
		{
			MonoScript[] monoScripts = MonoImporter.GetAllRuntimeMonoScripts();
			for (int i = 0; i < monoScripts.Length; ++i)
			{
				if (monoScripts[i].GetClass() == typeof(MonoScriptType))
				{
					return (monoScripts[i]);
				}
			}

			return (null);
		}
	}

}