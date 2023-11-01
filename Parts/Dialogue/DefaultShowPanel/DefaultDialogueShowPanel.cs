using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PartsKit
{
    public class DefaultDialogueShowPanel : MonoBehaviour, IDialogueShowPanel
    {
        [SerializeField] private Text contentText;
        [SerializeField] private List<DefaultDialogueShowCharacter> showCharacters;

        private readonly List<DefaultDialogueShowCharacter>
            curShowCharacters = new List<DefaultDialogueShowCharacter>();

        public void Show()
        {
            gameObject.SetActive(true);
            contentText.text = string.Empty;
            foreach (DefaultDialogueShowCharacter showCharacter in showCharacters)
            {
                showCharacter.SetHide();
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetCharacters(List<DialogueCharacter> characters)
        {
            foreach (DefaultDialogueShowCharacter showCharacter in curShowCharacters)
            {
                showCharacter.SetHide();
            }

            curShowCharacters.Clear();

            if (characters == null || characters.Count <= 0)
            {
                return;
            }

            foreach (DialogueCharacter character in characters)
            {
                if (character == null || character.Config == null)
                {
                    continue;
                }

                int showCharacterIndex = showCharacters.FindIndex(item => item.Seat == character.SeatKey);
                if (showCharacterIndex < 0)
                {
                    continue;
                }

                DefaultDialogueShowCharacter targetShowCharacter = showCharacters[showCharacterIndex];
                curShowCharacters.Add(targetShowCharacter);
                targetShowCharacter.SetShow(character);
            }
        }

        public void SetContent(string content)
        {
            contentText.text = content;
        }
    }
}