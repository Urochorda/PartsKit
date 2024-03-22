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

        private void Awake()
        {
            selectButton.onClick.AddListener(OnSelectClick);
        }

        public void SetData(DialogueSelectItemData itemDataVal, Action<DialogueSelectItemData> onSelectVal)
        {
            itemData = itemDataVal;
            infoStringEvent.SetEntry(itemDataVal.InfoEntryName);
            onSelect = onSelectVal;
        }

        private void OnSelectClick()
        {
            onSelect?.Invoke(itemData);
        }
    }
}