using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PartsKit
{
    public class DefaultDialogueShowPanel : MonoBehaviour, IDialogueShowPanel
    {
        [SerializeField] private Text contentText;
        [SerializeField] private List<DefaultDialogShowCharacter> showCharacters;

        private readonly List<DefaultDialogShowCharacter> curShowCharacters = new List<DefaultDialogShowCharacter>();

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetCharacters(List<DialogueCharacterConfig> characters)
        {
            foreach (DefaultDialogShowCharacter showCharacter in curShowCharacters)
            {
                showCharacter.SetHide();
            }

            curShowCharacters.Clear();

            if (characters == null || characters.Count <= 0)
            {
                return;
            }

            foreach (DialogueCharacterConfig characterConfig in characters)
            {
                int showCharacterIndex = showCharacters.FindIndex(item => item.Seat == characterConfig.SeatKey);
                if (showCharacterIndex <= 0)
                {
                    continue;
                }

                DefaultDialogShowCharacter targetShowCharacter = showCharacters[showCharacterIndex];
                curShowCharacters.Add(targetShowCharacter);
                targetShowCharacter.SetShow(characterConfig);
            }
        }

        public void SetContent(string content)
        {
            contentText.text = content;
        }
    }
}