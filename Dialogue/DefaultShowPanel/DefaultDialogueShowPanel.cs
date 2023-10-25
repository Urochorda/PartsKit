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

        public void SetCharacters(List<DialogueCharacterConfig> characters)
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

            foreach (DialogueCharacterConfig characterConfig in characters)
            {
                int showCharacterIndex = showCharacters.FindIndex(item => item.Seat == characterConfig.SeatKey);
                if (showCharacterIndex < 0)
                {
                    continue;
                }

                DefaultDialogueShowCharacter targetShowCharacter = showCharacters[showCharacterIndex];
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