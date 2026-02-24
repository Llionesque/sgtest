using System;
using System.Threading.Tasks;
using Configuration;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MagicWords
{
    public class MagicWords : ExerciseController<Configuration.MagicWordsConfig>
    {
        [Header("UI Elements")]
        [SerializeField]
        private DialogueBox dialogueBox = default;
        
        [SerializeField]
        private Button quitButton = null;

        [Header("Error Handling")]
        [SerializeField]
        private GameObject errorRoot = null;

        [SerializeField]
        private TextMeshProUGUI errorText = null;

        [SerializeField]
        private Button errorRetryButton = null;
        
        private MagicWordsSession session;

        protected override async Task InitialiseAsyncInternal(Configuration.MagicWordsConfig config)
        {
            dialogueBox.Clear();
            
            errorRetryButton.onClick.AddListener(Begin);
            quitButton.onClick.AddListener(End);
            
            session = new MagicWordsSession(config);
            
            await session.FetchDialogueData(
                HandleDialogueFetched,
                HandleDialogueFetchProgress,
                HandleDialogueFetchError);
        }

        public override void Begin()
        {
            base.Begin();
            
            errorRoot.SetActive(false);
            quitButton.gameObject.SetActive(false);

            if (session?.HasDialogueLines ?? false)
            {
                dialogueBox.Clear();
                
                dialogueBox.Show(session.GetDisplayLine, config.ReplacementMap, () =>
                {
                    quitButton.gameObject.SetActive(true);
                });
            }
            else dialogueBox.Hide();
        }
        
        private void HandleDialogueFetched()
        {
            // Begin() is called automatically by the exercise loader
        }

        private void HandleDialogueFetchError(Exception exception)
        {
            errorRoot.SetActive(true);
            errorText.text = exception.Message;
            dialogueBox.Hide();
            
            Debug.LogException(exception);
        }
        
        private void HandleDialogueFetchProgress(float progress)
        {
            // Progress isn't returned correctly so no sense using it
            // if (progressBar) progressBar.value = progress;
        }
    }
}
