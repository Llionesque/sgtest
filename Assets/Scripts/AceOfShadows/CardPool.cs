using System.Collections.Generic;
using UnityEngine;

namespace AceOfShadows
{
	public class CardPool : MonoBehaviour
	{
		[SerializeField]
		private Card cardPrefab = null;

		private readonly Stack<Card> pooledCards = new();

		private void Awake()
		{
			if (cardPrefab) cardPrefab.gameObject.SetActive(false);
		}
		
		public Card GetCard(int index)
		{
			return (pooledCards.TryPop(out var card))
				? card.Initialise(index)
				: Instantiate(cardPrefab.gameObject).GetComponent<Card>().Initialise(index);
		}

		public void AddCardToPool(Card card)
		{
			card.transform.SetParent(transform, false);
			card.transform.localRotation = Quaternion.identity;
			card.gameObject.SetActive(false);
			
			pooledCards.Push(card);
		}
	}
}
