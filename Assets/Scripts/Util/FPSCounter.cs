using TMPro;
using UnityEngine;

namespace Util
{
	public class FPSCounter : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI outputText = null;

		[SerializeField]
		private string format = "FPS: {0:0.}";

		private void Awake()
		{
			if (!outputText) outputText = GetComponent<TextMeshProUGUI>();
		}

		private void Update()
		{
			if (!outputText) return;
			
			outputText.text = string.Format(format, 1f / Time.unscaledDeltaTime);
		}
	}	
}

