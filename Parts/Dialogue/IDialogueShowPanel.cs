using System;
using System.Collections.Generic;
using UnityEngine;

namespace PartsKit
{
    public interface IDialogueShowPanel
    {
        public void Show(Transform point);
        public void Hide();
        public void BeginPlay();
        public void EndPlay();
        public void SetCharacters(List<DialogueCharacter> characters);
        public void SetSelects(List<DialogueSelectItemData> selects, Action<DialogueSelectItemData> onSelect);
        public void SetContent(string content);
    }
}