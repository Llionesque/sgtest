using System;
using System.Collections.Generic;
using System.Linq;

namespace AceOfShadows
{
	public class AceOfShadowsSession
	{
		public struct CardStackInfo
		{
			public const int SOURCE_POSITION_ID = 0;
			
			public int Position;
			public Configuration.AceOfShadowsConfig.CardStackType StackType;
		}
		
		private class CardStackEntry
		{
			public bool IsEmpty => (CardCount == 0);
			public int CardCount { get; private set; }
			public CardStackInfo StackInfo { get; private set; }

			public CardStackEntry(int position, Configuration.AceOfShadowsConfig.CardStackType stackType, int startingCardCount = 0)
			{
				StackInfo = new CardStackInfo()
				{
					Position = position,
					StackType = stackType,
				};
				
				CardCount = startingCardCount;
			}

			public void RemoveCard()
			{
				if (--CardCount < 0)
					throw new ArgumentOutOfRangeException($"Card stack data '{StackInfo.Position}' is out of cards, cannot remove");
			}

			public void AddCard()
			{
				CardCount++;
			}
		}

		private int currentStackIndex;
		private readonly Configuration.AceOfShadowsConfig config;
		private readonly CardStackEntry[] cardStacks;
		private readonly CardStackEntry sourceStackEntry;

		public bool IsComplete => sourceStackEntry.IsEmpty;
		public int SourceCardCount => sourceStackEntry.CardCount;
		public int DestinationCardCount => cardStacks.Sum(s => s.CardCount);
		public event Action<int, int> OnCardMoved;
		public event Action OnComplete;
		
		public AceOfShadowsSession(Configuration.AceOfShadowsConfig config)
		{
			this.config = config;
			
			var stackIndex = CardStackInfo.SOURCE_POSITION_ID;
			sourceStackEntry = new CardStackEntry(stackIndex, Configuration.AceOfShadowsConfig.CardStackType.Source, config.CardCount);
			stackIndex++;
			
			cardStacks = config.CardStacks
				.Select(stackType => new CardStackEntry(stackIndex++, stackType))
				.ToArray();

			currentStackIndex = 0;
		}

		public IEnumerable<CardStackInfo> GetStackInfos() => cardStacks.Select(stack => stack.StackInfo);
		
		public void MoveNextCard()
		{
			if (IsComplete) throw new Exception($"Cannot move next card - session is complete");
			
			if (config.RandomCardMoves)
			{
				currentStackIndex = UnityEngine.Random.Range(0, cardStacks.Length);
			}
			else
			{
				currentStackIndex = (++currentStackIndex % cardStacks.Length);
			}
			
			sourceStackEntry.RemoveCard();
			cardStacks[currentStackIndex].AddCard();

			OnCardMoved?.Invoke(sourceStackEntry.StackInfo.Position, cardStacks[currentStackIndex].StackInfo.Position);

			if (IsComplete)
			{
				OnComplete?.Invoke();
			}
		}
	}
}
