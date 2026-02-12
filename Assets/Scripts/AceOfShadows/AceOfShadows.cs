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

		private const float DEFAULT_CARD_INTERVAL = 0.25f;
		private const float FAST_INTERVAL_MODIFIER = 0.2f;

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

		[Header("Displays")]
		[SerializeField]
		[Tooltip("GameObject to show when dealing starts")]
		private GameObject dealingDisplay = null;
		
		[SerializeField]
		[Tooltip("GameObject to show when dealing finishes")]
		private GameObject completionDisplay = null;
		
		[Header("Buttons")]
		[SerializeField]
		private Button playButton = null;
		
		[SerializeField]
		private Button pauseButton = null;
		
		[SerializeField]
		private Button fastButton;

		[SerializeField]
		private Button replayButton = null;
		
		private int movedCardsCount;
		private int destinationStackIndex;
		
		private float normalCardInterval;
		private float fastCardInterval;
		
		private float cardInterval;
		
		private void Awake()
		{
			foreach (var cardStack in destinationCardStacks)
			{
				cardStack.OnCardEjected += cardPool.AddCardToPool;
			}
			
			replayButton.onClick.AddListener(Begin);

			InitialiseSpeedButtons();
		}

		public override void Begin()
		{
			base.Begin();
			
			InitialiseCardStacks();
			
			movedCardsCount = 0;
			destinationStackIndex = 0;
			
			normalCardInterval = DEFAULT_CARD_INTERVAL;
			fastCardInterval = DEFAULT_CARD_INTERVAL * FAST_INTERVAL_MODIFIER;
			
			cardInterval = normalCardInterval;
			UpdateButtonStates(null);

			StartCoroutine(CardTransitionCoroutine());
		}
		
		private void InitialiseCardStacks()
		{
			dealingDisplay.SetActive(false);
			completionDisplay.SetActive(false);
			replayButton.gameObject.SetActive(false);
			
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
			UpdateButtonStates(playButton);
			
			while (movedCardsCount < cardCount)
			{
				if (cardInterval > 0f)
				{
					MoveNextCard();

					movedCardsCount++;
					destinationStackIndex = (destinationStackIndex + 1) % destinationCardStacks.Length;

					yield return new WaitForSeconds(cardInterval);
				}
				else yield return null;
			}
			
			HandleCardTransitionsEnded();
		}

		private void HandleCardTransitionsEnded()
		{
			dealingDisplay.SetActive(false);
			completionDisplay.SetActive(true);
			replayButton.gameObject.SetActive(true);
		}

		private void MoveNextCard()
		{
			if (sourceCardStack.MoveCardFromStack(out var card))
			{
				destinationCardStacks[destinationStackIndex].MoveCardToStack(card);
				sourceCardStack.TryCreateCardInStack();
			}
		}
		
		private void InitialiseSpeedButtons()
		{
			playButton.onClick.AddListener(() =>
			{
				cardInterval = normalCardInterval;
				UpdateButtonStates(playButton);
			});
			
			fastButton.onClick.AddListener(() =>
			{
				cardInterval = fastCardInterval;
				UpdateButtonStates(fastButton);
			});

			pauseButton.onClick.AddListener(() =>
			{
				cardInterval = 0f;
				UpdateButtonStates(pauseButton);
			});

			UpdateButtonStates(null);
		}
		
		private void UpdateButtonStates(Button activeButton)
		{
			void UpdateButton(Button button)
				=> button.interactable = activeButton && (activeButton != button);
			
			UpdateButton(playButton);
			UpdateButton(pauseButton);
			UpdateButton(fastButton);
		}
	}
}
