using System;
using UnityEngine;
using UnityEngine.UI;

namespace PhoenixFlame
{
	public class PhoenixFlameColourButton : MonoBehaviour
	{
		[SerializeField]
		private Button button = null;
		
		[SerializeField]
		private Image image = null;
		
		private Color color;
		private Action onPressed;

		private void Awake()
		{
			button = GetComponentInChildren<Button>();
			button.onClick.AddListener(() => onPressed?.Invoke());
		}
		
		public PhoenixFlameColourButton Initialise(Color color, Action onPressed)
		{
			this.color = color;
			this.onPressed = onPressed;
			
			image.color = new Color(color.r, color.g, color.b, image.color.a);

			return this;
		}
	}
}
