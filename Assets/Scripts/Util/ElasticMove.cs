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

		public void MoveToNewAnchor(Transform anchor,
			Vector3? targetLocalPosition = null, Quaternion? targetLocalRotation = null,
			Action onEnded = null)
		{
			if (!thisTransform) thisTransform = transform;
			
			if (!anchor) return;

			initialLocalPosition = null;
			initialLocalRotation = null;
			onMotionEnded = onEnded;
			
			StopAllCoroutines();
			StartCoroutine(MoveCoroutine(anchor, targetLocalPosition, targetLocalRotation));
		}

		private IEnumerator MoveCoroutine(Transform anchor,
			Vector3? targetLocalPosition = null, Quaternion? targetLocalRotation = null)
		{
			var elapsed = 0f;
			
			targetLocalPosition ??= Vector3.zero;
			targetLocalRotation = Quaternion.Inverse(targetLocalRotation ?? Quaternion.identity);

			while (elapsed < duration)
			{
				if (elapsed > 0f)
				{
					if (!initialLocalPosition.HasValue || !initialLocalRotation.HasValue)
					{
						CalculateOffsetsBeforeMotion(anchor);	
					}

					if (initialLocalPosition.HasValue && initialLocalRotation.HasValue)
					{
						var t = curve.Evaluate(elapsed / duration);

						thisTransform.localPosition = Vector3.Lerp(initialLocalPosition.Value, targetLocalPosition.Value, t);
						thisTransform.localRotation = Quaternion.Slerp(Quaternion.identity, targetLocalRotation.Value, t);
					}
				}
				
				elapsed += Time.deltaTime;
				yield return null;
			}
			
			thisTransform.localPosition = targetLocalPosition.Value;
			thisTransform.localRotation = targetLocalRotation.Value;
			
			onMotionEnded?.Invoke();
		}

		private void CalculateOffsetsBeforeMotion(Transform anchor)
		{
			thisTransform.SetParent(anchor, true);
			initialLocalPosition = thisTransform.localPosition;
			initialLocalRotation = thisTransform.localRotation;
		}
	}
}
