using UnityEngine;
using UnityEngine.Serialization;

namespace AceOfShadows
{
	public class CardStackPositions : MonoBehaviour
	{
		public struct Position
		{
			public Vector3 WorldPosition;
			public Quaternion WorldRotation;
		}
		
		[SerializeField]
		private Transform[] positions;
		
		[Header("Offsets")]
		[SerializeField]
		[FormerlySerializedAs("stackingOffsetPosition")]
		private Vector3 groupOffsetPosition = Vector3.zero;
		
		[SerializeField]
		[FormerlySerializedAs("stackingOffsetRotation")]
		private Vector3 groupOffsetRotation = Vector3.zero;

		private int positionIndex = 0;
		private int groupIndex = 0;
		
		public Position GetNextPosition()
		{
			var targetTransform = positions[positionIndex];
			var nextPosition = new Position()
			{
				WorldPosition = targetTransform.position + groupOffsetPosition * groupIndex,
				WorldRotation = targetTransform.rotation * Quaternion.Euler(groupOffsetRotation * groupIndex),
			};

			if ((++positionIndex) >= positions.Length)
			{
				positionIndex = 0;
				groupIndex++;
			}
			
			return nextPosition;
		}
		
		public void ResetCounts()
		{
			positionIndex = 0;
			groupIndex = 0;
		}
	}
}
