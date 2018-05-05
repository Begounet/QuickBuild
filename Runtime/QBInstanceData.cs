using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuickBuild
{
    [CreateAssetMenu(fileName = "QBInstanceData", menuName = "QuickBuild/Custom Instance Data")]
    public class QBInstanceData : ScriptableObject
    {
        [Tooltip("Allow to replace the displayed ID by a custom name.\nDisplayInstanceID must be checked to be displayed.")]
        public string customName;

        [Tooltip("Allow to add command line arguments at start")]
        public string commandLineArguments;

        [System.Serializable]
        public class ScreenPosition
        {
            public bool Override;
            public int X;
            public int Y;
        }

		[System.Serializable]
        public class ScreenSize
        {
            public bool Override;
            public int Width;
            public int Height;
        }

		// Disabled for now because no implementation succeed to move a window for now
        [Tooltip("Allow to set a specific position on the screen (in pixel units).\nOnly works on Windows!")]
		[HideInInspector]
        public ScreenPosition screenPosition;

        [Tooltip("Allow to set a specific size on the screen (in pixel units).")]
        public ScreenSize screenSize;
    }
}