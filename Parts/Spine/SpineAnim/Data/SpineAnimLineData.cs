using System;
using System.Collections.Generic;
using UnityEngine;

namespace PartsKit
{
    [Serializable]
    public class SpineAnimLineData
    {
        [SerializeField] private SpineAnimStateData nextState;
        [SerializeField] private bool hasExitTime;
        [SerializeField] private SpineAnimConditionData[] conditions;

        public SpineAnimStateData NextState => nextState;
        public bool HasExitTime => hasExitTime;
        public IReadOnlyList<SpineAnimConditionData> Conditions => conditions;
    }
}