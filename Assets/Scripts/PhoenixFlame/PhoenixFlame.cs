using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace PhoenixFlame
{
    public class PhoenixFlame : ExerciseController
    {
        public override string Title => "Phoenix Flame";
        
        private static readonly int propertyValueHash = Animator.StringToHash("Value");

        private static Color HueToColour(float f) => Color.HSVToRGB(f, 1f, 1f);

        [Header("Phoenix Flame")]
        [SerializeField]
        private ParticleSystem[] particleSystems = null;

        [SerializeField]
        private int numberOfColourButtons = 12;
        
        [SerializeField]
        private Animator animator = null;

        [SerializeField]
        private float animationSpeed = 0.06f;

        [Header("UI Elements")]
        [SerializeField]
        private PhoenixFlameColourButton colourButtonPrefab = null;

        [SerializeField]
        private GameObject indicator = null;

        [SerializeField]
        private Button animatorToggleButton = null;
        
        [SerializeField]
        private Graphic[] additionalGraphics = null;
        
        private ParticleSystem.ColorOverLifetimeModule[] colorModules;

        private void Awake()
        {
            colorModules = particleSystems
                .Select(k => k.GetComponent<ParticleSystem>().colorOverLifetime)
                .ToArray();
            
            colourButtonPrefab.gameObject.SetActive(false);
            animatorToggleButton.onClick.AddListener(() => EnableAnimator(!animator.enabled));
            animator.speed = animationSpeed;

            GenerateColourButtons();
        }

        public override void Begin()
        {
            base.Begin();
        
            animator.SetFloat(propertyValueHash, 0f);
            SyncAnimatorColour();
            EnableAnimator(false);
        }
        
        private void GenerateColourButtons()
        {
            for (var i = 0; i < numberOfColourButtons; i++)
            {
                var hue = (float)i / numberOfColourButtons;
                
                GenerateColourButton(hue, Quaternion.AngleAxis(hue * 360f, Vector3.forward));
            }
        }

        private void GenerateColourButton(float hue, Quaternion localRotation)
        {
            var newButton = Instantiate(colourButtonPrefab.gameObject, colourButtonPrefab.transform.parent)
                .GetComponentInChildren<PhoenixFlameColourButton>()
                .Initialise(HueToColour(hue), () =>
                {
                    animator.SetFloat(propertyValueHash, hue);
                    ApplyHueToParticleSystems(hue);
                    
                    EnableAnimator(false);
                });
            
            newButton.transform.localRotation = localRotation;
            newButton.gameObject.SetActive(true);
        }

        private void ApplyHueToParticleSystems(float hue)
        {
            var colour = HueToColour(hue);
            
            for (var i = 0; i < colorModules.Length; i++)
            {
                ApplyColorToModule(colorModules[i], colour);
            }
            
            indicator.transform.localRotation = Quaternion.AngleAxis(hue * 360f, Vector3.forward);

            if (additionalGraphics.Length > 0)
            {
                foreach (var graphic in additionalGraphics)
                    if (graphic) graphic.color = new Color(colour.r, colour.g, colour.b, graphic.color.a);
            }
        }
        
        private void ApplyColorToModule(ParticleSystem.ColorOverLifetimeModule module, Color colour)
        {
            var existingGradient = module.color.gradient;
            var newGradient = new Gradient();
            
            newGradient.SetKeys(
                new[]
                {
                    new GradientColorKey(colour, 0f), existingGradient.colorKeys[1]
                },
                existingGradient.alphaKeys
            );

            module.color = new ParticleSystem.MinMaxGradient(newGradient);
        }

        private void Update()
        {
            if (animator.enabled) SyncAnimatorColour();
        }
        
        private void EnableAnimator(bool enable)
        {
            animator.enabled = enable;
            enabled = enable;
            animatorToggleButton.interactable = !enable;
        }

        private void SyncAnimatorColour()
        {
            ApplyHueToParticleSystems(animator.GetFloat(propertyValueHash));
        }
    }
}
