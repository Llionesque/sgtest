using System.Collections;
using UnityEngine;

namespace Util
{
	[RequireComponent(typeof(CanvasGroup))]
	public class CanvasGroupFader : MonoBehaviour
	{
		[SerializeField]
		private float duration = 1f;

		[SerializeField]
		private bool disableOnFadeOut = true;

		private Vector2 fadeValues;
		private CanvasGroup canvasGroup;

		private void Awake()
		{
			canvasGroup = GetComponent<CanvasGroup>();
		}

		public void SetAlpha(float alpha)
		{
			StopAllCoroutines();
			
			canvasGroup.alpha = alpha;
			canvasGroup.gameObject.SetActive(!disableOnFadeOut || alpha > 0f);
		}
		
		public void SetDuration(float duration)
		{
			this.duration = duration;
		}

		public void FadeIn() => Fade(0f, 1f);
		
		public void FadeOut() => Fade(canvasGroup.alpha, 0f);

		public void Fade(float from, float to)
		{
			StopAllCoroutines();
			
			canvasGroup.gameObject.SetActive(true);

			if (Mathf.Approximately(from, to))
			{
				SetAlpha(to);
				return;
			}
			
			fadeValues = new Vector2(from, to);
			StartCoroutine(FadeCoroutine());
		}

		private IEnumerator FadeCoroutine()
		{
			var elapsed = 0f;
			
			while (elapsed < duration)
			{
				elapsed += Time.deltaTime;
				canvasGroup.alpha = Mathf.Lerp(fadeValues.x, fadeValues.y, Mathf.Clamp01(elapsed / duration));
				yield return null;
			}

			SetAlpha(fadeValues.y);
		}
	}
}
