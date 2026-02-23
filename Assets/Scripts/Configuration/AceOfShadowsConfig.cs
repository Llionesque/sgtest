using System;
using System.Collections.Generic;
using System.Linq;
using AceOfShadows;
using UnityEngine;

namespace Configuration
{
	[CreateAssetMenu(fileName = "AceOfShadows_", menuName = "Configs/Ace Of Shadows", order = 0)]
	public partial class AceOfShadowsConfig : ExerciseConfig
	{
		public enum CardStackType
		{
			Source = 0,
			
			Vertical = 1,
			Rows = 2,
			Fan = 3,
			Circle = 4
		}

		[Serializable]
		public class CardStackPrefabEntry
		{
			private const string prefabPathHeader = "AceOfShadows/";
			
			public CardStackType StackType;
			public string PrefabName;

			private CardStack loadedPrefab;

			public CardStack Prefab
				=> loadedPrefab ??= Resources.Load<CardStack>(prefabPathHeader + PrefabName);

			public void UnloadPrefab() => loadedPrefab = null;
		}
		
		private const float DEFAULT_CARD_INTERVAL = 0.25f;
		private const float DEFAULT_FAST_INTERVAL_MODIFIER = 2f;
		
		public override string Title => "Ace of Shadows";
		
		[Header("Ace of Shadows")]
		[Tooltip("Number of cards to create in the source deck")]
		[SerializeField]
		private int cardCount = 144;
		public int CardCount => cardCount;
		
		[Tooltip("Whether cards move randomly (true), or between stacks in order (false)")]
		[SerializeField]
		private bool randomCardMoves = false;
		public bool RandomCardMoves => randomCardMoves;

		[Tooltip("Interval between card moves, at normal speed")]
		[SerializeField]
		private float normalCardInterval = DEFAULT_CARD_INTERVAL;
		public float NormalCardInterval => normalCardInterval;
		
		[Tooltip("Multiplier to interval, at fast speed (e.g. 2 -> double)")]
		[SerializeField]
		private float fastCardIntervalModifier = DEFAULT_FAST_INTERVAL_MODIFIER;
		public float FastCardInterval => NormalCardInterval / ((fastCardIntervalModifier > 0) ? fastCardIntervalModifier : 1f);

		[SerializeField]
		private CardStackType[] cardStacks = null;
		public IEnumerable<CardStackType> CardStacks => cardStacks.Length > 0 ? cardStacks : new[] { CardStackType.Fan };

		[Tooltip("References to card stack prefabs per type")]
		[SerializeField]
		private CardStackPrefabEntry[] prefabEntries = null;
		
		public CardStack GetCardStackPrefab(CardStackType stackType)
		{
			// Less overhead than a dictionary, will only be accessed [cardStacks.Length] times
			for (var i = 0; i < prefabEntries.Length; i++)
			{
				if (prefabEntries[i].StackType.Equals(stackType)) return prefabEntries[i].Prefab;
			}

			throw new Exception($"No prefab defined for Card Stack type: '{stackType}'");
		}
		
		public void UnloadAllPrefabs()
		{
			foreach (var entry in prefabEntries) entry.UnloadPrefab();
		}
	}
}
