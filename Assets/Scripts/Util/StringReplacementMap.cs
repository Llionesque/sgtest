using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Util
{
	[CreateAssetMenu(fileName = "StringReplacementMap", menuName = "Utils/StringReplacementMap")]
	public class StringReplacementMap : ScriptableObject
	{
		[Serializable]
		public class Entry
		{
			[SerializeField]
			private string tag = null;
			
			[SerializeField]
			private string replacement = null;

			public string Tag => tag;
			public string Replacement => replacement;
		}

		[SerializeField]
		private Entry[] entries = null;

		[Tooltip("Regex pattern with one capture group for the tag format")]
		[SerializeField]
		private string format = @"\{(\w+)\}";

		[Tooltip("What to replace tags with, if no matching entry was found")]
		[SerializeField]
		private string fallbackReplacement = "";

		private Regex compiledRegex;

		public string ApplyTo(string input)
		{
			if (string.IsNullOrEmpty(input) || entries.Length == 0) return input;

			compiledRegex ??= compiledRegex = new Regex(format, RegexOptions.Compiled);

			return compiledRegex.Replace(input, match =>
			{
				var key = match.Groups[1].Value;
				
				foreach (var entry in entries)
				{
					if (string.Equals(entry.Tag, key))
						return entry.Replacement ?? fallbackReplacement;
				}

				return match.Value;
			});
		}
	}

}
