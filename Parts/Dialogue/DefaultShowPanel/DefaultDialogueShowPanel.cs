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

        public List<DefaultDialogueShowCharacter> CurShowCharacters { get; } =
            new List<DefaultDialogueShowCharacter>();

        public List<DefaultDialogueShowSelectItem> CurSelectItems { get; } = new List<DefaultDialogueShowSelectItem>();

        public event Action onSelectItemChange;
        public event Action onCharacterChange;

        public void Show()
        {
            if (selectItemPrefab.transform.parent != null)
            {
                selectItemPrefab.gameObject.SetActive(false);
            }

            gameObject.SetActive(true);
        }

        public void Hide()
        {
            SetCharacters(null);
            SetSelects(null, null);
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
            foreach (DefaultDialogueShowCharacter showCharacter in CurShowCharacters)
            {
                showCharacter.SetHide();
            }

            CurShowCharacters.Clear();

            if (characters == null || characters.Count <= 0)
            {
                onCharacterChange?.Invoke();
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
                CurShowCharacters.Add(targetShowCharacter);
                targetShowCharacter.SetShow(character);
            }

            onCharacterChange?.Invoke();
        }

        public void SetSelects(List<DialogueSelectItemData> selects, Action<DialogueSelectItemData> onSelect)
        {
            foreach (DefaultDialogueShowSelectItem item in CurSelectItems)
            {
                gameObjectPool.Release(item.gameObject);
            }

            CurSelectItems.Clear();

            if (selects == null || selects.Count <= 0)
            {
                onSelectItemChange?.Invoke();
                return;
            }

            foreach (DialogueSelectItemData itemData in selects)
            {
                DefaultDialogueShowSelectItem item = gameObjectPool.Get(selectItemPrefab, selectItemParent);
                var itemTrans = item.transform;
                var prefabTrans = selectItemPrefab.transform;
                itemTrans.localScale = prefabTrans.localScale;
                itemTrans.rotation = prefabTrans.rotation;
                item.SetData(itemData, onSelect);
                CurSelectItems.Add(item);
            }

            onSelectItemChange?.Invoke();
        }

        public void SetContent(string content)
        {
            contentText.text = content;
        }
    }
}