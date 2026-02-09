using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace AceOfShadows
{
	public class AceOfShadows : ExerciseController
	{
		public override string Title => "Ace of Shadows";

		[Header("Cards")]
		[SerializeField]
		private CardPool cardPool;
		
		[SerializeField]
		private CardStack sourceCardStack = null;
		
		[SerializeField]
		private CardStack[] destinationCardStacks = null;

		[Header("Settings")]
		[Tooltip("Number of cards in the source deck")]
		[SerializeField]
		private int cardCount = 144;
		
		[SerializeField]
		[Tooltip("Number of cards in the source deck")]
		private float cardInterval = 0.2f;

		[Header("Displays")]
		[SerializeField]
		[Tooltip("GameObject to show when dealing starts")]
		private GameObject dealingDisplay = null;
		
		[SerializeField]
		[Tooltip("GameObject to show when dealing finishes")]
		private GameObject completionDisplay = null;

		[SerializeField]
		private Button quitButton = null;
		
		private int movedCardsCount;
		private int destinationStackIndex;
		
		private void Awake()
		{
			foreach (var cardStack in destinationCardStacks)
			{
				cardStack.OnCardEjected += cardPool.AddCardToPool;
			}
			
			quitButton.onClick.RemoveAllListeners();
			quitButton.onClick.AddListener(End);
		}
		
		public override void Begin()
		{
			base.Begin();
			
			InitialiseCardStacks();
			
			movedCardsCount = 0;
			destinationStackIndex = 0;
			
			StartCoroutine(CardTransitionCoroutine());
		}
		
		private void InitialiseCardStacks()
		{
			dealingDisplay.SetActive(false);
			completionDisplay.SetActive(false);
			quitButton.gameObject.SetActive(false);
			
			StopAllCoroutines();
			
			foreach (var stack in destinationCardStacks.Concat(new[]{ sourceCardStack }))
			{
				stack.ClearCards();
				stack.CreateNextCard = cardPool.GetCard;
			}

			sourceCardStack.Initialise(cardCount);
		}

		private IEnumerator CardTransitionCoroutine()
		{
			yield return new WaitForSeconds(1f);
			
			dealingDisplay.SetActive(true);
			
			while (movedCardsCount < cardCount)
			{
				MoveNextCard();
				
				movedCardsCount++;
				destinationStackIndex = (destinationStackIndex + 1) % destinationCardStacks.Length;
				
				yield return new WaitForSeconds(cardInterval);
			}

			HandleCardTransitionsEnded();
		}

		private void HandleCardTransitionsEnded()
		{
			dealingDisplay.SetActive(false);
			completionDisplay.SetActive(true);
			quitButton.gameObject.SetActive(true);
		}

		private void MoveNextCard()
		{
			if (sourceCardStack.MoveCardFromStack(out var card))
			{
				destinationCardStacks[destinationStackIndex].MoveCardToStack(card);
				sourceCardStack.TryCreateCardInStack();
			}
		}
	}
}
