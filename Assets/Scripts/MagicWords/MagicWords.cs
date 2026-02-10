using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace MagicWords
{
    public class MagicWords : ExerciseController
    {
        public override string Title => "Magic Words";
        
        [Header("Config")]
        [SerializeField]
        private string url = null;
        
        [SerializeField]
        private AsyncJsonFetcher jsonFetcher = null;

        [Header("UI Elements")]
        [SerializeField]
        private DialogueBox dialogueBox = default;

        [SerializeField]
        private GameObject loadingScreen = null;

        [SerializeField]
        private Slider progressBar = null;
        
        [SerializeField]
        private Button quitButton = null;

        [SerializeField]
        private TextMeshProUGUI jsonText = null;

        [Header("Error Handling")]
        [SerializeField]
        private GameObject errorRoot = null;

        [SerializeField]
        private TextMeshProUGUI errorText = null;

        [SerializeField]
        private Button errorRetryButton = null;

        private void Awake()
        {
            if (!jsonFetcher) jsonFetcher = GetComponent<AsyncJsonFetcher>();
            
            if (progressBar) progressBar.value = 0f;
            
            errorRetryButton.onClick.AddListener(Begin);
            quitButton.onClick.AddListener(End);
        }

        public override void Begin()
        {
            base.Begin();

            dialogueBox.Hide();
            errorRoot.SetActive(false);
            quitButton.gameObject.SetActive(false);
            jsonText.text = string.Empty;
            
            FetchDialogueData();
        }

        private void FetchDialogueData()
        {
            loadingScreen.gameObject.SetActive(true);
            
            if (progressBar) progressBar.value = 0f;
            
            jsonFetcher.Fetch<DialogueData>(url, 
                HandleDialogueFetched, 
                HandleDialogueError, 
                HandleDialogueProgress);
        }

        private void HandleDialogueFetched(DialogueData dialogueData)
        {
            loadingScreen.gameObject.SetActive(false);

            jsonText.text = jsonFetcher.FullContent
                .Replace("\n", "")
                .Replace("  ", "");
            
            dialogueBox.Show(dialogueData, () =>
            {
                quitButton.gameObject.SetActive(true);
            });
        }

        private void HandleDialogueError(Exception exception)
        {
            loadingScreen.gameObject.SetActive(false);
            
            errorRoot.SetActive(true);
            errorText.text = exception.Message;
            dialogueBox.Hide();
            
            Debug.LogError(exception);
        }
        
        private void HandleDialogueProgress(float progress)
        {
            if (progressBar) progressBar.value = progress;
        }
    }
}
