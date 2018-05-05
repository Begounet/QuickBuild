using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QBInstanceData : ScriptableObject
{
    [Tooltip("Allow to replace the displayed ID by a custom name")]
    public string customName;

    [Tooltip("Allow to add command line arguments at start")]
    public string commandLineArguments;
}
