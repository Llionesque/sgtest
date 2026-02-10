using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Util
{
	[RequireComponent(typeof(TMP_Text))]
	public class TextTypewriterEffect : MonoBehaviour
	{
		private const char tmproEmojiTagStart = '<';
		private const char tmproEmojiTagEnd = '>';
		
		[SerializeField]
		private int charactersPerSecond = 25;

		[SerializeField]
		private float delayBeforeTyping = 0.5f;
		
		[SerializeField]
		private float delayAfterTyping = 0f;
		
		private string fullMessage;
		private TMP_Text text;
		private Coroutine typingCoroutine;
		private bool skipCurrentLine;
		private Action<string> onComplete;
		
		public bool IsTyping { get; private set; }

		public event Action<bool> OnTypingStateChanged;

		private void Awake()
		{
			text = GetComponent<TMP_Text>();
		}

		public void StartTyping(string message, Action<string> onComplete = null)
		{
			Clear();

			this.onComplete = onComplete;
			
			fullMessage = message;
			typingCoroutine = StartCoroutine(TypingCoroutine(message));
		}
		
		public void SkipToEnd()
		{
			skipCurrentLine = true;
		}

		private IEnumerator TypingCoroutine(string message)
		{
			text.text = "";
			skipCurrentLine = false;
			
			if (delayBeforeTyping > 0)
				yield return new WaitForSeconds(delayBeforeTyping);
			
			IsTyping = true;
			OnTypingStateChanged?.Invoke(true);
			
			for (var i = 0; i < message.Length; i++)
			{
				bool PrintEntireTag(char c)
				{
					if (c != tmproEmojiTagStart) return false;
			
					var tagEnd = message.IndexOf(tmproEmojiTagEnd, i);
					if (tagEnd == -1) return false;
					
					text.text += fullMessage.Substring(i, tagEnd - i + 1);
					i = tagEnd;
					
					return true;
				}
				
				if (skipCurrentLine)
				{
					text.text = fullMessage;
					break;
				}
				
				var character = message[i];

				if (PrintEntireTag(character)) continue;
				
				text.text += character;

				if (charactersPerSecond > 0)
				{
					yield return new WaitForSeconds(1f / charactersPerSecond);
				}
				else yield return null;
			}
			
			IsTyping = false;
			OnTypingStateChanged?.Invoke(false);
			
			if (delayAfterTyping > 0)
				yield return new WaitForSeconds(delayAfterTyping);

			StopTyping();
			onComplete?.Invoke(message);
		}

		private void StopTyping()
		{
			if (!IsTyping) return;
			
			StopCoroutine(typingCoroutine);
			typingCoroutine = null;
			skipCurrentLine = false;
		}

		private void Clear()
		{
			StopTyping();
			text.text = string.Empty;
		}
	}
}

