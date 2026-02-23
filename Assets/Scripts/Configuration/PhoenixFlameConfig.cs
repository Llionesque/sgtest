using UnityEngine;
using Util;

namespace Configuration
{
	[CreateAssetMenu(fileName = "PhoenixFlame_", menuName = "Configs/Phoenix Flame", order = 2)]
	public class PhoenixFlameConfig : ExerciseConfig
	{
		public override string Title => "Phoenix Flame";

		[Header("Magic Words")]
		[SerializeField]
		private int numberOfColourButtons = 12;
		public int NumberOfColourButtons => numberOfColourButtons;
        
		[SerializeField]
		private float animationSpeed = 0.06f;
		public float AnimationSpeed => animationSpeed;
	}
}
