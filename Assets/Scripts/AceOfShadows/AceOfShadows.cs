using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Configuration;
using UnityEngine;
using UnityEngine.UI;

namespace AceOfShadows
{
	public class AceOfShadows : ExerciseController<Configuration.AceOfShadowsConfig>
	{
		private const int MAX_CARDS = 144;
		
		[Header("Scene references")]
		[SerializeField]
		private CardPool cardPool;
		
		[SerializeField]
		private CardStack sourceCardStack = null;
		
		[SerializeField]
		private GameObject destinationCardStacksRoot = null;
		
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
		
		private Dictionary<int, CardStack> destinationCardStacks = null;
		private AceOfShadowsSession session;
		private float cardInterval;

		protected override async Task InitialiseAsyncInternal(Configuration.AceOfShadowsConfig config)
		{
			if (session != null)
			{
				session.OnCardMoved -= HandleCardMoved;
				session.OnComplete -= HandleCardTransitionsEnded;
				session = null;
			}
			
			session = new AceOfShadowsSession(config);
			session.OnCardMoved += HandleCardMoved;
			session.OnComplete += HandleCardTransitionsEnded;
			
			replayButton.onClick.AddListener(Begin);
		}

		public override void Begin()
		{
			base.Begin();

			dealingDisplay.SetActive(false);
			completionDisplay.SetActive(false);
			replayButton.gameObject.SetActive(false);
			
			InitialiseSpeedButtons();
			InitialiseCardStacks();

			StopAllCoroutines();
			SetSingleButtonDisabled(null);
			StartCoroutine(CardTransitionCoroutine());
		}
		
		private void InitialiseCardStacks()
		{
			foreach (Transform child in destinationCardStacksRoot.transform)
				Destroy(child.gameObject);

			destinationCardStacks = session.GetStackInfos()
				.Where(info => info.Position > AceOfShadowsSession.CardStackInfo.SOURCE_POSITION_ID)
				.ToDictionary(info => info.Position,
				info => CreateCardStack(info.StackType));
			
			sourceCardStack.CreateNextCard = cardPool.GetCard;
			sourceCardStack.CreateCards(config.GetClampedProperty(config.CardCount, nameof(config.CardCount), 0, MAX_CARDS));
		}

		private CardStack CreateCardStack(Configuration.AceOfShadowsConfig.CardStackType stackType)
		{
			var prefab = config.GetCardStackPrefab(stackType);
			var cardStack = Instantiate(prefab, destinationCardStacksRoot.transform, false);
			cardStack.OnCardEjected += cardPool.AddCardToPool;
			return cardStack;
		}

		private IEnumerator CardTransitionCoroutine()
		{
			yield return new WaitForSeconds(1f);
			
			dealingDisplay.SetActive(true);
			SetSingleButtonDisabled(playButton);
			cardInterval = config.NormalCardInterval;
			
			while (!session.IsComplete)
			{
				if (cardInterval > 0f)
				{
					session.MoveNextCard();
					yield return new WaitForSeconds(cardInterval);
				}
				else yield return null;
			}
		}

		private void HandleCardTransitionsEnded()
		{
			dealingDisplay.SetActive(false);
			completionDisplay.SetActive(true);
			replayButton.gameObject.SetActive(true);
			SetSingleButtonDisabled(null);
		}

		private void HandleCardMoved(int sourcePosition, int destinationPosition)
		{
			var sourceIndexId = AceOfShadowsSession.CardStackInfo.SOURCE_POSITION_ID;

			if (sourcePosition != sourceIndexId)
				throw new Exception($"Moving cards from non-source index ({sourceIndexId}): {sourcePosition} is not yet supported.");

			if (sourcePosition == destinationPosition)
				throw new Exception($"Cannot move a card from the same stack to itself");
			
			if (sourceCardStack.MoveCardFromStack(out var card))
			{
				destinationCardStacks[destinationPosition].MoveCardToStack(card);
				sourceCardStack.TryCreateCardInStack();
			}
		}
		
		private void InitialiseSpeedButtons()
		{
			playButton.onClick.AddListener(() =>
			{
				cardInterval = config.NormalCardInterval;
				SetSingleButtonDisabled(playButton);
			});
			
			fastButton.onClick.AddListener(() =>
			{
				cardInterval = config.FastCardInterval;
				SetSingleButtonDisabled(fastButton);
			});

			pauseButton.onClick.AddListener(() =>
			{
				cardInterval = 0f;
				SetSingleButtonDisabled(pauseButton);
			});

			SetSingleButtonDisabled(null);
		}
		
		private void SetSingleButtonDisabled(Button activeButton)
		{
			void UpdateButton(Button button)
				=> button.interactable = activeButton && (activeButton != button);
			
			UpdateButton(playButton);
			UpdateButton(pauseButton);
			UpdateButton(fastButton);
		}
	}
}
