using System.Linq;
using System.Threading.Tasks;
using Configuration;
using UnityEngine;
using UnityEngine.UI;

namespace PhoenixFlame
{
    public class PhoenixFlame : ExerciseController<PhoenixFlameConfig>
    {
        private static readonly int propertyValueHash = Animator.StringToHash("Value");

        private static Color HueToColour(float f) => Color.HSVToRGB(f, 1f, 1f);

        [Header("Phoenix Flame")]
        [SerializeField]
        private ParticleSystem[] particleSystems = null;
        
        [SerializeField]
        private Animator animator = null;

        [Header("UI Elements")]
        [SerializeField]
        private PhoenixFlameColourButton colourButtonPrefab = null;

        [SerializeField]
        private GameObject indicator = null;

        [SerializeField]
        private Button animatorToggleButton = null;
        
        [SerializeField]
        private Graphic[] additionalGraphics = null;

        private int numberOfButtons;
        private ParticleSystem.ColorOverLifetimeModule[] colorModules;

        protected override async Task InitialiseAsyncInternal(PhoenixFlameConfig config)
        {
            const float MAX_ANIMATION_SPEED = 0.25f;
            
            colorModules = particleSystems
                .Select(k => k.GetComponent<ParticleSystem>().colorOverLifetime)
                .ToArray();
            
            colourButtonPrefab.gameObject.SetActive(false);
            animatorToggleButton.onClick.AddListener(() => EnableAnimator(!animator.enabled));
            animator.speed = config.GetClampedProperty(config.AnimationSpeed, 
                nameof(config.AnimationSpeed),
                0, MAX_ANIMATION_SPEED);

            GenerateColourButtons();
        }

        public override void Begin()
        {
            base.Begin();
        
            // animator.SetFloat(propertyValueHash, 0f);
            SyncAnimatorColour();

            var hasButtons = (numberOfButtons > 0);
            animatorToggleButton.gameObject.SetActive(hasButtons);
            EnableAnimator(!hasButtons);
            indicator.SetActive(hasButtons);
        }
        
        private void GenerateColourButtons()
        {
            const int MAX_BUTTONS = 124;
            
            numberOfButtons = config.GetClampedProperty(config.NumberOfColourButtons, 
                nameof(config.NumberOfColourButtons),
                0, MAX_BUTTONS);
            
            for (var i = 0; i < numberOfButtons; i++)
            {
                var hue = (float)i / numberOfButtons;
                GenerateColourButton(hue, Quaternion.AngleAxis(hue * 360f, Vector3.forward));
            }
        }

        private void GenerateColourButton(float hue, Quaternion localRotation)
        {
            var newButton = Instantiate(colourButtonPrefab.gameObject, colourButtonPrefab.transform.parent)
                .GetComponentInChildren<PhoenixFlameColourButton>()
                .Initialise(HueToColour(hue), () =>
                {
                    // animator.SetFloat(propertyValueHash, hue);
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
