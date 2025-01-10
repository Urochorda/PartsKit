using System.Collections.Generic;
using UnityEngine;

namespace _Test
{
    public enum SpineMachineParameterType
    {
        Float = 1,
        Bool = 2,
        Integer = 3,
        Trigger = 4,
    }

    [CreateAssetMenu]
    public class SpineMachineData : ScriptableObject
    {
        [SerializeField] private SpineStateData enterState;
        [SerializeField] private SpineLineData[] anyState;
        public SpineStateData EnterState => enterState;
        public IReadOnlyList<SpineLineData> AnyState => anyState;
    }
}