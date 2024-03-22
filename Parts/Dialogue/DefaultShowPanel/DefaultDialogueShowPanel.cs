using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PartsKit
{
    public class DefaultDialogueShowPanel : MonoBehaviour, IDialogueShowPanel
    {
        [SerializeField] private GameObjectPool gameObjectPool;
        [SerializeField] private Text contentText;
        [SerializeField] private List<DefaultDialogueShowCharacter> showCharacters;

        [SerializeField] private Transform selectItemParent;
        [SerializeField] private DefaultDialogueShowSelectItem selectItemPrefab;

        private readonly List<DefaultDialogueShowCharacter>
            curShowCharacters = new List<DefaultDialogueShowCharacter>();

        private readonly List<DefaultDialogueShowSelectItem> curSelectItems = new List<DefaultDialogueShowSelectItem>();

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void BeginPlay()
        {
            contentText.text = string.Empty;
            foreach (DefaultDialogueShowCharacter showCharacter in showCharacters)
            {
                showCharacter.SetHide();
            }
        }

        public void EndPlay()
        {
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

        public void SetSelects(List<DialogueSelectItemData> selects, Action<DialogueSelectItemData> onSelect)
        {
            foreach (DefaultDialogueShowSelectItem item in curSelectItems)
            {
                gameObjectPool.Release(item.gameObject);
            }

            curSelectItems.Clear();

            foreach (DialogueSelectItemData itemData in selects)
            {
                DefaultDialogueShowSelectItem item = gameObjectPool.Get(selectItemPrefab);
                item.transform.SetParent(selectItemParent);
                item.SetData(itemData, onSelect);
                curSelectItems.Add(item);
            }
        }

        public void SetContent(string content)
        {
            contentText.text = content;
        }
    }
}