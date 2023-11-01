using System;
using UnityEngine;
using UnityEngine.Events;

namespace PartsKit
{
    [Serializable]
    public class FsmStateUnityEvent
    {
        [field: SerializeField] public UnityEvent OnEntry { get; set; }
        [field: SerializeField] public UnityEvent<float> OnUpdate { get; set; }
        [field: SerializeField] public UnityEvent<float> OnFixUpdate { get; set; }
        [field: SerializeField] public UnityEvent OnExit { get; set; }
    }
}