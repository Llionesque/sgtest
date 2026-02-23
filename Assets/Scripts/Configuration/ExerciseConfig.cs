using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Configuration
{
	public abstract class ExerciseConfig : ScriptableObject
	{
		const string resourcesPath = "Configs/";
		
		public static IEnumerable<ExerciseConfig> LoadAll()
		{
			return Resources.LoadAll<ExerciseConfig>(resourcesPath);
		}
		
		public abstract string Title { get; }
		public string FullTitle => $"{Title} - {variantTitle}";

		[Header("Exercise metaData")]
		[SerializeField]
		private string variantTitle = null;
		
		[SerializeField]
		private Sprite icon = null;
		public Sprite Icon => icon;

		[SerializeField]
		private Sprite background = null;
		public Sprite Background => background;
	
		[SerializeField]
		private Color color = Color.white;
		public Color Color => color;

		[Header("Unit testing settings")]
		[SerializeField]
		private bool supportsUnitTesting = false;
		public bool SupportsUnitTesting => supportsUnitTesting;
		
		public string GetSceneName() => Title.Replace(" ", "") + "Scene";

		public int GetClampedProperty(int propertyValue, string propertyName,
			int min = int.MinValue, int max = int.MaxValue)
		{
			return GetClampedPropertyValueInternal(propertyValue, min, max, propertyName);
		}

		public float GetClampedProperty(float propertyValue, string propertyName,
			float min = float.MinValue, float max = float.MaxValue)
		{
			return GetClampedPropertyValueInternal(propertyValue, min, max, propertyName);
		}

		private T GetClampedPropertyValueInternal<T>(T value, T min, T max, string propertyName) where T : IComparable<T>
		{
			if (value.CompareTo(min) < 0)
			{
				Debug.LogWarning($"Config property '{propertyName}' on config '{name}' was below minimum: {min}");
				return min;
			}

			if (value.CompareTo(max) > 0)
			{
				Debug.LogWarning($"Config property '{propertyName}' on config '{name}' was above maximum: {max}");
				return max;
			}

			return value;
		}
		
#region Unit testing
		
		public virtual void RunUnitTest(Action onStarted, Action<bool> onEnded)
		{
			if (!SupportsUnitTesting) throw new Exception("This config doesn't support unit testing.");
			
			Debug.Log($"Running unit test on Config: '{name}'...");
			onStarted?.Invoke();
		}
		
#if UNITY_EDITOR
		[ContextMenu("Run unit test")]
		private void RunUnitTestFromEditor() => RunUnitTest(null, null);
#endif

#endregion
	}
}
