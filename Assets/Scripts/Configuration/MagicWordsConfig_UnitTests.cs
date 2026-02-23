using System;
using MagicWords;
using UnityEngine;

namespace Configuration
{
	public partial class MagicWordsConfig
	{
		public override void RunUnitTest(Action onStarted, Action<bool> onEnded)
		{
			base.RunUnitTest(onStarted, onEnded);

			var session = new MagicWordsSession(this);

			try
			{
				Debug.Log($"Fetching jSon from: {Url}...");
				
#pragma warning disable CS4014
				session.FetchDialogueData(() =>
				{
					Debug.Log($"jSon deserialized: {session.HasDialogueLines}");
					
					if (!session.HasDialogueLines)
					{
						onEnded?.Invoke(false);
						return;
					}
					
					var lineIndex = 0;
					MagicWordsSession.DisplayLine line = default;
					do
					{
						try
						{
							line = session.GetDisplayLine(lineIndex);
							Debug.Log($"{lineIndex}: {line.ToString()}, is end? {line.IsEnd}");

							lineIndex++;
						}
						catch (Exception e)
						{
							Debug.LogException(e);
							onEnded?.Invoke(false);
							throw;
						}
					} 
					while (!line.IsEnd);
					
					Debug.Log($"Dialogue ended in {lineIndex - 1} lines.");
					
					onEnded?.Invoke(true);
				});
#pragma warning restore CS4014
			}
			catch (Exception e)
			{
				Debug.LogException(e);
				onEnded?.Invoke(false);
			}
		}
	}
}
