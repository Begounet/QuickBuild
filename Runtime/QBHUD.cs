using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QBHUD : MonoBehaviour
{

    public static QBHUD CreateHUD()
    {
        return (new GameObject().AddComponent<QBHUD>());
    }

    void OnGUI()
    {
        GUI.color = Color.red;

        string instanceName = string.Format("QB ID[{0}]", QuickBuild.QBSettingsLoader.settings.instanceID);
        string customName = QuickBuild.QBSettingsLoader.settings.customName;
        if (!string.IsNullOrEmpty(customName))
        {
            instanceName += " " + customName;
        }

        GUI.Label(new Rect(10, 10, Screen.width, 20), instanceName);
    }
}
