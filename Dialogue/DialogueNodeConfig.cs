using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace PartsKit
{
    [Serializable]
    public class DialogueCharacter
    {
        [field: SerializeField] public DialogueCharacterConfig Config { get; private set; }
        [field: SerializeField] public int StyleType { get; private set; }
        [field: SerializeField] public int SeatKey { get; private set; }
    }

    [Serializable]
    public class DialogNodeGroup
    {
        [field: SerializeField] public LocalizedString ContentKey { get; private set; }
        [field: SerializeField] public List<DialogueCharacter> Characters { get; private set; }
    }

    [CreateAssetMenu(menuName = "PartsKit/Dialogue/DialogueNodeConfig", fileName = "DialogueNodeConfig_")]
    public class DialogueNodeConfig : ScriptableObject
    {
        [field: SerializeField] public List<DialogNodeGroup> NodeGroups { get; private set; }
    }
}