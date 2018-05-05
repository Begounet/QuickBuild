using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace QuickBuild
{

	public class QBCommandLineHelper
	{
		private const string StringArrayConcatenor = "+";

		public static string	PackStringArray(string[] array)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < array.Length; ++i)
			{
				sb.Append(array[i]);
				if (i + 1 < array.Length)
				{
					sb.Append(StringArrayConcatenor);
				}
			}
			return (sb.ToString());
		}

		public static string[]	UnpackStringArray(string str, System.StringSplitOptions StringSplitOptions = System.StringSplitOptions.RemoveEmptyEntries)
		{
			return (str.Split(new string[] { StringArrayConcatenor }, StringSplitOptions));
		}
	}

}