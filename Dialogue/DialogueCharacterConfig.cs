using UnityEngine;
using UnityEngine.Localization;

namespace PartsKit
{
    [CreateAssetMenu(menuName = "PartsKit/Dialogue/DialogueCharacterConfig", fileName = "DialogueCharacterConfig_")]
    public class DialogueCharacterConfig : ScriptableObject
    {
        [field: SerializeField] public LocalizedString NameKey { get; private set; }
        [field: SerializeField] public Sprite HeadSprite { get; private set; }
    }
}