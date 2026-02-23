using UnityEngine;
using Util;

namespace Configuration
{
	[CreateAssetMenu(fileName = "MagicWords_", menuName = "Configs/Magic Words", order = 1)]
	public partial class MagicWordsConfig : ExerciseConfig
	{
		public override string Title => "Magic Words";
		
		[Header("Magic Words")]
		[SerializeField]
		private string url = null;
		public string Url => url;

		[SerializeField]
		private StringReplacementMap replacementMap = null;
		public StringReplacementMap ReplacementMap => replacementMap;
	}
}
