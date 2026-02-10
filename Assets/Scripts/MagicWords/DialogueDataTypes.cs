using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace MagicWords
{
	public class DialogueData
	{
		public List<DialogueLineData> dialogue;
		public List<DialogueAvatarData> avatars;
		
		public int LineCount => dialogue.Count;
		public DialogueLineData GetLine(int lineIndex) => dialogue[lineIndex];
		public DialogueAvatarData GetAvatar(string name) => avatars.FirstOrDefault(a => a.name == name 
																						&& DialogueAvatarData.IsValidAvatarUrl(a.url));
	}

	public class DialogueLineData
	{
		public string name;
		public string text;
	}

	public class DialogueAvatarData
	{
		public static bool IsValidAvatarUrl(string url) => url.StartsWith("https://") && url.Contains("png");
		
		public string name;
		public string url;
		public string position;
		
		[JsonIgnore]
		public DialogueAvatarPosition Position
		{
			get
			{
				var positionCamelCase = char.ToUpper(position[0]) + position[1..].ToLower();

				return (Enum.TryParse<DialogueAvatarPosition>(positionCamelCase, out var pos))
					? pos
					: throw new JsonSerializationException($"Unknown position: {position}");
			}
		}
	}

	public enum DialogueAvatarPosition
	{
		Left,
		Right
	}
}
