using System;
using Configuration;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	[RequireComponent(typeof(Button))]
	public class UnitTestButton : MonoBehaviour
	{
		[SerializeField]
		private Button button = null;

		[SerializeField]
		private GameObject spinner = null;
		
		[SerializeField] 
		private GameObject resultPass = null;
		
		[SerializeField] 
		private GameObject resultFail = null;
		
		[SerializeField] 
		private TextMeshProUGUI label = null;

		public void Configure(ExerciseConfig config, Action onStarted = null, Action<bool> onEnded = null)
		{
			var supported = (config?.SupportsUnitTesting ?? false);
			gameObject.SetActive(supported);
			if (!supported) return;
			
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(() =>
			{
				ShowUnitTestResult(null);
				if (spinner) spinner.SetActive(true);
				
				onStarted?.Invoke();
				
				config.RunUnitTest(onStarted,
					(pass) =>
					{
						ShowUnitTestResult(pass);
						onEnded?.Invoke(pass);
					});
			});
		}

		private void ShowUnitTestResult(bool? pass)
		{
			if (spinner) spinner.SetActive(false);
			
			resultPass.SetActive(pass.HasValue && pass.Value);
			resultFail.SetActive(pass.HasValue && !pass.Value);
			
			label.text = pass.HasValue 
				? (pass.Value ? "Pass" : "Fail")
				: "...";
		}
	}
}
