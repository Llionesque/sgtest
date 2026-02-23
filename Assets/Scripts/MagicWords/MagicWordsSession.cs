using System;
using System.Linq;
using System.Threading.Tasks;
using Configuration;
using Util;

namespace MagicWords
{
	public class MagicWordsSession
	{
		public struct DisplayLine
		{
			public DialogueLineData Line;
			public DialogueAvatarData Avatar;
			public bool IsEnd;

			public override string ToString()
			{
				return $"{Line?.name}: \"{Line?.text}\", ({Avatar?.name}, {Avatar?.position})";
			}
		}
		
		private readonly Configuration.MagicWordsConfig config;
		private AsyncJsonFetch<DialogueData> fetchOperation;
		private DisplayLine[] lines;
		
		public bool HasDialogueLines => (lines?.Length ?? 0) > 0;
		
		public MagicWordsSession(Configuration.MagicWordsConfig config)
		{
			this.config = config;
		}

		public async Task FetchDialogueData(Action onComplete,
			Action<float> onProgress = null, Action<Exception> onError = null)
		{
			try
			{
				var dialogueData = await (new AsyncJsonFetch<DialogueData>(config.Url)).Fetch(onProgress);
				lines = ConvertDialogueDataToDisplayLines(dialogueData);
				onComplete?.Invoke();
			}
			catch (Exception e)
			{
				onError?.Invoke(e);
			}
		}

		public DisplayLine GetDisplayLine(int index)
		{
			return (index >= 0 && index < lines.Length)
				? lines[index]
				: new DisplayLine() { IsEnd = true };
		}

		private static DisplayLine[] ConvertDialogueDataToDisplayLines(DialogueData dialogueData)
		{
			return dialogueData.dialogue
				.Select(line => new DisplayLine()
				{
					Line = line,
					Avatar = dialogueData.GetAvatar(line.name) 
				})
				.ToArray();
		}
	}
}
