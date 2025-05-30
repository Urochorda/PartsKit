using System;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace PartsKit
{
    public class DefaultDialogueShowSelectItem : MonoBehaviour
    {
        [SerializeField] private LocalizeStringEvent infoStringEvent;
        [SerializeField] private Button selectButton;
        private Action<DialogueSelectItemData> onSelect;
        private DialogueSelectItemData itemData;

        public Button SelectButton => selectButton;

        private void Awake()
        {
            selectButton.onClick.AddListener(OnSelectClick);
        }

        public void SetData(DialogueSelectItemData itemDataVal, Action<DialogueSelectItemData> onSelectVal)
        {
            itemData = itemDataVal;
            infoStringEvent.SetEntry(itemDataVal.InfoEntryName);
            if (string.IsNullOrEmpty(itemDataVal.InfoEntryName))
            {
                infoStringEvent.OnUpdateString.Invoke(string.Empty);
            }

            onSelect = onSelectVal;
        }

        private void OnSelectClick()
        {
            onSelect?.Invoke(itemData);
        }
    }
}