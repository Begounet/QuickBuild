using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuickBuild
{
	
	public class AssetWatcher : UnityEditor.AssetPostprocessor {

		public static void	_OnPostprocessAllAssets(string[] ImportedAssets, string[] DeletedAssets, string[] MovedAssets, string[] MovedFromAssetPaths)
		{
			Debug.Log("OnPostProcessAllAssets called");

			for (int i = 0; i < ImportedAssets.Length; ++i)
			{
				Debug.Log("Imported assets " + ImportedAssets[i]);
			}

			for (int i = 0; i < DeletedAssets.Length; ++i)
			{
				Debug.Log("Deleted assets " + DeletedAssets[i]);
			}

			for (int i = 0; i < MovedAssets.Length; ++i)
			{
				Debug.Log("Moved assets " + MovedAssets[i]);
			}

			for (int i = 0; i < MovedFromAssetPaths.Length; ++i)
			{
				Debug.Log("Moved from asset paths " + MovedFromAssetPaths[i]);
			}
		}

	}

}