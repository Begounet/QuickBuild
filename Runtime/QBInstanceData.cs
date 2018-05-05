using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QBInstanceData", menuName = "QuickBuild/Custom Instance Data")]
public class QBInstanceData : ScriptableObject
{
    [Tooltip("Allow to replace the displayed ID by a custom name.\nDisplayInstanceID must be checked to be displayed.")]
    public string customName;

    [Tooltip("Allow to add command line arguments at start")]
    public string commandLineArguments;
}
