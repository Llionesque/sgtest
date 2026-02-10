using UnityEngine;
using Util;

namespace MagicWords
{
	[RequireComponent(typeof(TextureFetcher))]
	public class DialogueAvatar : MonoBehaviour
	{
		[SerializeField]
		private DialogueAvatarPosition position = default;
		public DialogueAvatarPosition Position => position;
		
		[SerializeField]
		private TextureFetcher textureFetcher = null;

		private void Awake()
		{
			textureFetcher ??= GetComponent<TextureFetcher>();
		}
		
		public void Show(DialogueAvatarData avatarData)
		{
			gameObject.SetActive(true);
			
			textureFetcher.FetchTextureAsync(avatarData.url);
		}

		public void Hide()
		{
			gameObject.SetActive(false);

			textureFetcher.StopFetching();
		}
	}
}
