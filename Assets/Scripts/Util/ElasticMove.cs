using System;
using System.Collections;
using UnityEngine;

namespace Util
{
	public class ElasticMove : MonoBehaviour
	{
		[SerializeField]
		private Transform thisTransform = null;
		
		[SerializeField]
		private float duration = 1f;
		
		[SerializeField]
		private AnimationCurve curve = AnimationCurve.EaseInOut(0, 1, 1, 0);

		private Vector3? initialLocalPosition;
		private Quaternion? initialLocalRotation;
		private Action onMotionEnded;

		public void MoveToNewAnchor(Transform anchor, Action onEnded = null)
		{
			if (!thisTransform) thisTransform = transform;
			
			if (!anchor) return;

			initialLocalPosition = null;
			initialLocalRotation = null;
			onMotionEnded = onEnded;
			
			StopAllCoroutines();
			StartCoroutine(FadeCoroutine(anchor));
		}

		private IEnumerator FadeCoroutine(Transform anchor)
		{
			var elapsed = 0f;
			
			while (elapsed < duration)
			{
				if (elapsed > 0f)
				{
					if (!initialLocalPosition.HasValue || !initialLocalRotation.HasValue)
					{
						thisTransform.SetParent(anchor, true);
						initialLocalPosition = thisTransform.localPosition;
						initialLocalRotation = thisTransform.localRotation;
					}
					
					thisTransform.localPosition = initialLocalPosition.Value * curve.Evaluate(elapsed / duration);
					thisTransform.localRotation = Quaternion.Slerp(Quaternion.identity, initialLocalRotation.Value, curve.Evaluate(elapsed / duration));
				}
				
				elapsed += Time.deltaTime;
				yield return null;
			}
			
			if (initialLocalPosition.HasValue)
				thisTransform.localPosition = initialLocalPosition.Value * curve.Evaluate(1f);
			
			onMotionEnded?.Invoke();
		}
	}
}
