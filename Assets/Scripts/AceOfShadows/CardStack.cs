using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace AceOfShadows
{
	public class CardStack : MonoBehaviour
	{
		[SerializeField]
		private Transform cardsContainer = null;
		
		[SerializeField]
		private CardStackPositions positions = null;

		[SerializeField]
		private int maxCards = 12;
		
		[SerializeField]
		private TextMeshProUGUI label = null;

		private Dictionary<int, Card> cards;

		private int createdCardsCount => cardsContainer.childCount;
		private bool isEmpty => createdCardsCount <= 0;
		private bool isFull => createdCardsCount >= maxCards;
		
		public event Action<Card> OnCardEjected;

		public Func<int, Card> CreateNextCard;

		public void Initialise(int cardCount)
		{
			Enumerable.Range(1, cardCount)
				.ToList()
				.ForEach(i => TryCreateCardAtIndex(i));
			
			RefreshCardCountLabel();
		}

		public bool TryCreateCardInStack()
		{
			int? nextCardIndex = (createdCardsCount < cards.Count(kvp => kvp.Value)) 
				? createdCardsCount
				: null;

			if (nextCardIndex.HasValue)
			{
				TryCreateCardAtIndex(nextCardIndex.Value);
			}

			return nextCardIndex.HasValue;
		}
		
		private Card TryCreateCardAtIndex(int cardIndex)
		{
			var card = (!isFull) 
				? CreateNextCard(cardIndex) 
				: null;

			if (card)
			{
				var cardTransform = card.transform;
				
				cardTransform.SetParent(cardsContainer);

				var positionData = positions.GetNextPosition();
				card.transform.position = positionData.WorldPosition;
				card.transform.rotation = positionData.WorldRotation;
				
				cardTransform.SetAsFirstSibling();
			}

			SetCard(cardIndex, card);
			
			return card;
		}
		
		public bool MoveCardFromStack(out Card card)
		{
			if (isEmpty)
			{
				card = null;
				return false;
			}

			var cardTransform = cardsContainer.GetChild(createdCardsCount - 1);
			card = cardTransform.GetComponent<Card>();
			cards.Remove(card.Index);
			
			while (GetNumberOfCardsNeeded() > 0)
			{
				TryCreateCardAtIndex(cards.First(kvp => !kvp.Value).Key);
			}
			
			RefreshCardCountLabel();

			return true;
		}
		
		public void MoveCardToStack(Card card)
		{
			if (!card) return;
			
			card.MoveToAnchor(cardsContainer, positions.GetNextPosition());
			
			SetCard(card.Index, card);
			
			if (GetNumberOfCardsNeeded() < 0 || isFull) EjectLastCard();

			RefreshCardCountLabel();
		}
		
		public void ClearCards()
		{
			if (cards == null) return;
			
			foreach (var card in cards.Values)
			{
				OnCardEjected?.Invoke(card);
			}
			
			cards.Clear();
			RefreshCardCountLabel();

			positions.ResetCounts();
		}

		private void SetCard(int cardIndex, Card card)
		{
			cards ??= new Dictionary<int, Card>();
			cards[cardIndex] = card;
		}

		private int GetNumberOfCardsNeeded()
		{
			return Math.Min(cards.Count, maxCards) - createdCardsCount;	
		}

		private void RefreshCardCountLabel()
		{
			if (label) label.text = cards.Count.ToString();
		}

		private void EjectLastCard()
		{
			// TODO
			
			/*var lastCard = cardsContainer.GetChild(cardsContainer.childCount - 1).GetComponent<Card>();
			OnCardEjected?.Invoke(lastCard);*/
		}
	}
}
