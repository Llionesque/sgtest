using System;
using System.Linq;
using AceOfShadows;
using UnityEngine;

namespace Configuration
{
	public partial class AceOfShadowsConfig
	{
		public override void RunUnitTest(Action onStarted, Action<bool> onEnded)
		{
			base.RunUnitTest(onStarted, onEnded);
			
			var moveCount = 0;
			var session = new AceOfShadowsSession(this);
			while (!session.IsComplete && moveCount < CardCount)
			{
				session.MoveNextCard();
				moveCount++;
			}

			var correctNumberOfMoves = (moveCount == CardCount);
			Debug.Log($"Moves taken: {moveCount}, to move {CardCount} cards");

			var sourceIsEmpty = session.SourceCardCount == 0;
			Debug.Log($"Source card stack has emptied: {sourceIsEmpty}");
			
			var destinationsMatchCardTotal = (session.DestinationCardCount == CardCount);
			Debug.Log($"All cards moved to destinations: {destinationsMatchCardTotal}");
			
			var correctNumberOfCardStacks = (session.GetStackInfos().Count() == cardStacks.Length);
			Debug.Log($"Correct number of destinations: {correctNumberOfCardStacks}");

			var didPass = correctNumberOfMoves
						&& sourceIsEmpty
						&& destinationsMatchCardTotal
						&& correctNumberOfCardStacks;
			
			Debug.Log($"Result: {didPass}");
			
			onEnded?.Invoke(didPass);
		}
	}
}
