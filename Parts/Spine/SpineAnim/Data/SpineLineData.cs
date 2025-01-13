using System;
using System.Collections.Generic;
using UnityEngine;

namespace PartsKit
{
    [Serializable]
    public class SpineLineData
    {
        [SerializeField] private SpineStateData nextState;
        [SerializeField] private bool hasExitTime;
        [SerializeField] private SpineConditionData[] conditions;

        public SpineStateData NextState => nextState;
        public bool HasExitTime => hasExitTime;
        public IReadOnlyList<SpineConditionData> Conditions => conditions;
    }
}