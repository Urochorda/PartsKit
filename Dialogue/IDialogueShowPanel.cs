using System.Collections.Generic;

namespace PartsKit
{
    public interface IDialogueShowPanel
    {
        public void Show();
        public void Hide();
        public void SetCharacters(List<DialogueCharacterConfig> characters);
        public void SetContent(string content);
    }
}