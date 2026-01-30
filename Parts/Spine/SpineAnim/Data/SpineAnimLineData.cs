using System;
using System.Collections.Generic;
using UnityEngine;

namespace PartsKit
{
    [Serializable]
    public class SpineAnimLineDataBase
    {
        [SerializeField] private SpineAnimStateData nextState;
        [SerializeField] private SpineAnimConditionData[] conditions;

        public SpineAnimStateData NextState
        {
            get => nextState;
            set => nextState = value;
        }

        public SpineAnimConditionData[] Conditions
        {
            get => conditions;
            set => conditions = value;
        }
    }

    [Serializable]
    public class SpineAnimLineData : SpineAnimLineDataBase
    {
    }

    [Serializable]
    public class SpineAnimStateLineData : SpineAnimLineDataBase
    {
        [SerializeField] private bool hasExitTime;

        public bool HasExitTime
        {
            get => hasExitTime;
            set => hasExitTime = value;
        }
    }
}