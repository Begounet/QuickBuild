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
        GUI.Label(new Rect(10, 10, 100, 20), string.Format("QB ID[{0}]", QuickBuild.QBSettingsLoader.settings.InstanceID));
    }
}
