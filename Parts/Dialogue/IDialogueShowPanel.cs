using System;
using System.Collections.Generic;

namespace PartsKit
{
    public interface IDialogueShowPanel
    {
        public void Show();
        public void Hide();
        public void BeginPlay();
        public void EndPlay();
        public void SetCharacters(List<DialogueCharacter> characters);
        public void SetSelects(List<DialogueSelectItemData> selects, Action<DialogueSelectItemData> onSelect);
        public void SetContent(string content);
    }
}