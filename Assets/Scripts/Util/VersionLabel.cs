using TMPro;
using UnityEngine;

namespace Util
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class VersionLabel : MonoBehaviour
	{
		[SerializeField]
		private string prefix = "v.";
		
		private void Awake()
		{
			var text = GetComponent<TextMeshProUGUI>();
			if (text) text.text = prefix + Application.version;
		}
	}
}
