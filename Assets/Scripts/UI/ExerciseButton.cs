using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Configuration;

namespace UI
{
	[RequireComponent(typeof(Button))]
	public class ExerciseButton : MonoBehaviour
	{
		[SerializeField]
		private Button button = null;
		
		[SerializeField] 
		private Image background = null;
		
		[SerializeField] 
		private Image icon = null;
		
		[SerializeField] 
		private TextMeshProUGUI label = null;

		[Header("Unit test")]
		[SerializeField]
		private UnitTestButton unitTestButton = null;
		
		public void Configure(ExerciseConfig config, Action<ExerciseConfig> onPressed = null,
			Action onUnitTestStarted = null, Action<bool> onUnitTestEnded = null)
		{
			if (label) label.text = config.FullTitle;
			if (icon && config.Icon) icon.sprite = config.Icon;
			
			if (background)
			{
				background.sprite = config.Background;
				background.color = config.Color;
			}

			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(() => onPressed?.Invoke(config));

			if (unitTestButton)
				unitTestButton.Configure(config, onUnitTestStarted, onUnitTestEnded);
		}
	}
}
