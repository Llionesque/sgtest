using UnityEngine;

namespace Util
{
	public class RotationBehaviour : MonoBehaviour
	{
		[SerializeField]
		private Vector3 rotationAngles = Vector3.back * 45;
		
		private void Update()
		{
			transform.Rotate(rotationAngles * Time.deltaTime);
		}
	}
}
