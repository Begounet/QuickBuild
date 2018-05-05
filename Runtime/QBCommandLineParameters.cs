using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuickBuild
{

	public class QBCommandLineParameters
	{
		public const string EnableQuickBuild = "-qb";
		public const string RedirectOutput = "-qbRRedirectOutput";
		public const string InstanceID = "-qbInstanceID";
		public const string DisplayInstanceID = "-qbDisplayInstanceID";
		public const string AdditiveScenes = "-qbAdditiveScenes";

		public const string Screen_FullscreenMode = "-screen-fullscreen";
		public const string Screen_Width = "-screen-width";
		public const string Screen_Height = "-screen-height";

		public const string LogFile = "-logfile";
		public const string LogFileFormat = "output_log_id{0}.txt";

		public const string Batchmode = "-batchmode";
		public const string NoGraphics = "-nographics";

	}

}