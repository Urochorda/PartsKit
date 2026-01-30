using System;
using System.Collections.Generic;
using UnityEngine;

namespace PartsKit
{
    [CreateAssetMenu(menuName = "PartsKit/Spine/AnimState", fileName = "AnimState_")]
    public class SpineAnimStateData : ScriptableObject
    {
        public interface IClipRef
        {
            public string AnimName { get; }
        }

        [Serializable]
        public class ClipRefBase : IClipRef
        {
            [SerializeField] private string animName;
            public string AnimName => animName;
        }

        [Serializable]
        public class ClipRef : ClipRefBase
        {
            [SerializeField] private SpineAnimConditionData[] conditions;
            public IReadOnlyList<SpineAnimConditionData> Conditions => conditions;
        }

        [Serializable]
        public class ClipRefDefault : ClipRefBase
        {
        }

        [SerializeField] private ClipRefDefault defaultClip;
        [SerializeField] private ClipRef[] clipPool;
        [SerializeField] private SpineAnimStateLineData[] linePool;
        [SerializeField] private float speed = 1;
        [SerializeField] private string speedParameter;
        [SerializeField] private bool speedParameterActive;

        public ClipRefDefault DefaultClip
        {
            get => defaultClip;
            set => defaultClip = value;
        }

        public ClipRef[] ClipPool
        {
            get => clipPool;
            set => clipPool = value;
        }

        public SpineAnimStateLineData[] LinePool
        {
            get => linePool;
            set => linePool = value;
        }

        public float Speed
        {
            get => speed;
            set => speed = value;
        }

        public string SpeedParameter
        {
            get => speedParameter;
            set => speedParameter = value;
        }

        public bool SpeedParameterActive
        {
            get => speedParameterActive;
            set => speedParameterActive = value;
        }

        #region Runtime

        public int SpeedParameterId { get; private set; }

        public void InitRuntime()
        {
            SpeedParameterId = Animator.StringToHash(SpeedParameter);
        }

        #endregion
    }
}