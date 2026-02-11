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
            gameObject.SetActive(true);
            
            if (label) label.SetText(Index.ToString());

            return this;
        }
        
        public void MoveToAnchor(Transform anchor, CardStackPositions.Position position, Action onComplete = null)
        {
            elasticMove.MoveToNewAnchor(anchor,
                position.WorldPosition - anchor.position,
                Quaternion.Inverse(position.WorldRotation) * anchor.rotation,
                onComplete);
            
            transform.SetAsLastSibling();
        }
    }
}
