using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace AceOfShadows
{
    public class Card : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI label = null;
        
        [SerializeField]
        private ElasticMove elasticMove = null;
        
        [SerializeField]
        private Image cardTexture = null;
        
        [SerializeField]
        private Sprite[] cardBacks = null;
        
        public int Index { get; private set; }

        private void Awake()
        {
            if (!elasticMove) elasticMove = GetComponent<ElasticMove>();
        }

        public Card Initialise(int cardIndex)
        {
            Index = cardIndex;
            cardTexture.sprite = cardBacks[Index % cardBacks.Length];
            gameObject.name = $"Card{Index}";
            
            if (label) label.SetText(Index.ToString());

            return this;
        }
        
        public void MoveToAnchor(Transform anchor, Action onComplete = null)
        {
            elasticMove.MoveToNewAnchor(anchor, onComplete);
            transform.SetAsLastSibling();
        }
    }
}
