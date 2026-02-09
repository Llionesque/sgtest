using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Util
{
	public class FadeBehaviour : MonoBehaviour
	{
		[SerializeField]
		private Graphic graphic = null;
		
		[SerializeField]
		private Color startColor = Color.white;
		
		[SerializeField]
		private Color endColor = Color.red;
		
		[SerializeField]
		private float duration = 1f;
		
		[SerializeField]
		private bool setOnAwake = true;

		private void Awake()
		{
			if (!graphic) graphic = GetComponent<Graphic>();

			if (setOnAwake) SetToStartColor();
		}

		public void Fade()
		{
			StopAllCoroutines();
			StartCoroutine(FadeCoroutine());
		}

		private void SetToStartColor()
		{
			graphic.color = startColor;
		}

		private void SetToEndColor()
		{
			graphic.color = endColor;
		}

		private IEnumerator FadeCoroutine()
		{
			var elapsed = 0f;
			
			while (elapsed < duration)
			{
				graphic.color = Color.Lerp(startColor, endColor, Mathf.Clamp01(elapsed / duration));
				elapsed += Time.deltaTime;
				yield return null;
			}
			
			SetToEndColor();
		}
	}
}
