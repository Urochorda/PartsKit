using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace PartsKit
{

    [Serializable]
    public class DialogNodeGroup
    {
        [field: SerializeField] public List<DialogueCharacterConfig> Characters { get; set; }
        [field: SerializeField] public LocalizedString ContentKey { get; private set; }
    }

    [CreateAssetMenu(menuName = "PartsKit/Dialogue/DialogueNodeConfig", fileName = "DialogueNodeConfig_")]
    public class DialogueNodeConfig : ScriptableObject
    {
        [field: SerializeField] public List<DialogNodeGroup> NodeGroups { get; private set; }
    }
}