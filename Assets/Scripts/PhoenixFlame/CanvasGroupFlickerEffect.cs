using UnityEngine;
using Util;

namespace PhoenixFlame
{
    [RequireComponent(typeof(CanvasGroupFader))]
    public class CanvasGroupFlickerEffect : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroupFader canvasGroupFader = null;
        
        [SerializeField]
        private Vector2 minMaxDelay = Vector2.zero;
        
        [SerializeField]
        private Vector2 minMaxAlpha = Vector2.zero;
        
        [SerializeField]
        private Vector2 minMaxDuration = Vector2.zero;

        private float currentDelay;

        private void Awake()
        {
            canvasGroupFader ??= GetComponent<CanvasGroupFader>();
        }

        private void Start() => StartNextFlicker();

        private void Update()
        {
            currentDelay -= Time.deltaTime;

            if (currentDelay <= 0)
            {
                StartNextFlicker();
            }
        }

        private void StartNextFlicker()
        {
            currentDelay = Random.Range(minMaxDelay.x, minMaxDelay.y);
            canvasGroupFader.Fade(Random.Range(minMaxAlpha.x, minMaxAlpha.y), 0f);
            canvasGroupFader.SetDuration(Random.Range(minMaxDuration.x, minMaxDuration.y));
        }
    }
}
