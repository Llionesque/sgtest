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
		private Transform offset = null;

		[SerializeField]
		private int maxCards = 12;
		
		[SerializeField]
		private TextMeshProUGUI label = null;

		private Dictionary<int, Card> cards;

		public int CreatedCardsCount => cardsContainer.childCount;
		public bool IsEmpty => CreatedCardsCount <= 0;
		public bool IsFull => CreatedCardsCount >= maxCards;
		
		public event Action<Card> OnCardEjected;

		public Func<int, Card> CreateNextCard;

		public void Initialise(int cardCount)
		{
			Enumerable.Range(1, cardCount)
				.ToList()
				.ForEach(i => AddCardIndex(i));
			
			RefreshCardCountLabel();
			ResolveChildPositions();
		}

		public bool TryCreateCardInStack()
		{
			int? nextCardIndex = (CreatedCardsCount < cards.Count(kvp => kvp.Value)) 
				? CreatedCardsCount
				: null;

			if (nextCardIndex.HasValue)
			{
				AddCardIndex(nextCardIndex.Value);
				ResolveChildPositions();	
			}

			return nextCardIndex.HasValue;
		}
		
		public bool MoveCardFromStack(out Card card)
		{
			if (IsEmpty)
			{
				card = null;
				return false;
			}

			var cardTransform = cardsContainer.GetChild(CreatedCardsCount - 1);
			cardTransform.SetParent(null);
			card = cardTransform.GetComponent<Card>();
			cards.Remove(card.Index);
			
			while (GetNumberOfCardsNeeded() > 0)
			{
				AddCardIndex(cards.First(kvp => !kvp.Value).Key);
			}
			
			RefreshCardCountLabel();
			ResolveChildPositions();
			
			return true;
		}
		
		public void MoveCardToStack(Card card)
		{
			if (!card) return;
			
			if (IsFull) EjectLastCard();
			
			card.MoveToAnchor(cardsContainer); //, ResolveChildPositions); // Post-move resolve
			
			SetCard(card.Index, card);
			
			if (GetNumberOfCardsNeeded() < 0) EjectLastCard();

			ResolveChildPositions();
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
		}
		
		private Card AddCardIndex(int cardIndex)
		{
			var card = (!IsFull) 
				? CreateNextCard(cardIndex) 
				: null;

			if (card != null)
			{
				var cardTransform = card.transform;
				
				cardTransform.SetParent(cardsContainer);
				cardTransform.SetAsFirstSibling();
			}

			SetCard(cardIndex, card);
			
			return card;
		}

		private void SetCard(int cardIndex, Card card)
		{
			cards ??= new Dictionary<int, Card>();
			cards[cardIndex] = card;
		}

		private int GetNumberOfCardsNeeded()
		{
			return Math.Min(cards.Count, maxCards) - CreatedCardsCount;	
		}

		private void RefreshCardCountLabel()
		{
			if (label) label.text = cards.Count.ToString();
		}
		
		private void ResolveChildPositions()
		{
			foreach (Transform child in cardsContainer)
			{
				child.localPosition = offset.localPosition * (cardsContainer.childCount - child.GetSiblingIndex());
				child.gameObject.SetActive(true);
			}
		}

		private void EjectLastCard()
		{
			// TODO
			
			/*var lastCard = cardsContainer.GetChild(cardsContainer.childCount - 1).GetComponent<Card>();
			OnCardEjected?.Invoke(lastCard);*/
		}
	}
}
