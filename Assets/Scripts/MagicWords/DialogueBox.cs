using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace MagicWords
{
    public class DialogueBox : MonoBehaviour
    {
        [SerializeField]
        private TextTypewriterEffect typewriterEffect = null;

        [SerializeField]
        private DialogueAvatar[] avatars = null;
        
        [SerializeField]
        private StringReplacementMap replacementMap = null;
        
        [Header("UI Elements")]
        [SerializeField]
        private Button skipButton = null;
        
        [SerializeField]
        private TextMeshProUGUI nameText = null;
        
        [SerializeField]
        private TextMeshProUGUI mainText = null;

        private DialogueData dialogueData;
        private int lineIndex;
        private Action onLineComplete;
        private Action onDialogueComplete;

        private void Awake()
        {
            if (skipButton) skipButton.onClick.AddListener(() =>
            {
                if (typewriterEffect.enabled)
                {
                    typewriterEffect.SkipToEnd();
                }
                else HandleLineComplete();
            });

            typewriterEffect.OnTypingStateChanged += SyncSkipButton;
        }
        
        public void Show(DialogueData dialogueData,
            Action onDialogueComplete = null, Action onLineComplete = null)
        {
            gameObject.SetActive(true);
            
            this.dialogueData = dialogueData;
            this.onDialogueComplete = onDialogueComplete;
            this.onLineComplete = onLineComplete;
            
            lineIndex = 0;
            skipButton.gameObject.SetActive(true);
            
            HideAllAvatars();

            StartNextLine();
        }
        
        public void Hide() => gameObject.SetActive(false);

        private void StartNextLine()
        {
            if (dialogueData == null || lineIndex >= dialogueData.LineCount)
            {
                HandleDialogueComplete();
                return;
            }

            var line = dialogueData.GetLine(lineIndex);
            if (line != null)
            {
                nameText.text = line.name;
                
                var lineText = replacementMap?.ApplyTo(line.text) ?? line.text;

                if (typewriterEffect.enabled)
                {
                    typewriterEffect.StartTyping(lineText, s => HandleLineComplete());    
                }
                else
                {
                    mainText.text = lineText;
                }
                
                lineIndex++;   
                
                var avatar = dialogueData.GetAvatar(line.name);
                if (avatar != null)
                {
                    ShowAvatar(avatar);
                }
                else HideAllAvatars();
            }
            else HandleDialogueComplete();
        }

        private void HideAllAvatars(Func<DialogueAvatar, bool> filter = null)
        {
            foreach (var avatar in avatars)
            {
                if (filter == null || filter(avatar)) avatar.Hide();
            }
        }

        private void ShowAvatar(DialogueAvatarData avatarData)
        {
            HideAllAvatars(av => av.Position != avatarData.Position);
            
            var avatar = avatars.FirstOrDefault(av => av.Position == avatarData.Position);
            
            if (avatar)
            {
                avatar.Show(avatarData);
            }
        }
        
        private void HandleLineComplete()
        {
            StartNextLine();
            onLineComplete?.Invoke();
        }
        
        private void HandleDialogueComplete()
        {
            skipButton.gameObject.SetActive(false);
            onDialogueComplete?.Invoke();
        }

        private void SyncSkipButton(bool isTyping)
        {
            skipButton.interactable = isTyping;
            
            var icon = skipButton.transform.Find("Icon")?.GetComponent<Image>();
            if (icon) icon.gameObject.SetActive(isTyping);
        }
    }
}
