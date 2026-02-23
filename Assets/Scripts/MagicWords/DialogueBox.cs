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
        
        [Header("UI Elements")]
        [SerializeField]
        private Button skipButton = null;
        
        [SerializeField]
        private TextMeshProUGUI nameText = null;
        
        [SerializeField]
        private TextMeshProUGUI mainText = null;

        private StringReplacementMap replacementMap;
        private int lineIndex;
        private Action onLineComplete;
        private Action onDialogueComplete;

        private Func<int, MagicWordsSession.DisplayLine> getNextLine;

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
        
        public void Show(Func<int, MagicWordsSession.DisplayLine> getNextLine,
            StringReplacementMap replacementMap = null,
            Action onDialogueComplete = null, Action onLineComplete = null)
        {
            gameObject.SetActive(true);
            
            this.getNextLine = getNextLine;
            this.replacementMap = replacementMap;
            this.onDialogueComplete = onDialogueComplete;
            this.onLineComplete = onLineComplete;
            
            lineIndex = 0;
            skipButton.gameObject.SetActive(true);
            
            HideAllAvatars();

            StartNextLine();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            typewriterEffect.Clear();
        }

        public void Clear()
        {
            nameText.text = "";
            HideAllAvatars();
            typewriterEffect.Clear();
            skipButton.gameObject.SetActive(false);
        }

        private void StartNextLine()
        {
            var line = getNextLine.Invoke(lineIndex);
            
            if (!line.IsEnd)
            {
                nameText.text = line.Line.name;
                
                var lineText = replacementMap?.ApplyTo(line.Line.text) ?? line.Line.text;
                if (typewriterEffect.enabled)
                {
                    typewriterEffect.StartTyping(lineText, s => HandleLineComplete());    
                }
                else
                {
                    mainText.text = lineText;
                }
                
                ShowAvatar(line.Avatar);
                
                lineIndex++; 
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
            if (avatarData == null)
            {
                HideAllAvatars();
                return;
            }
            
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
        }
    }
}
