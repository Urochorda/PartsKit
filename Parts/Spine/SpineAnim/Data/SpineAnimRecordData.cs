using System;
using System.Collections.Generic;
using UnityEngine;

namespace PartsKit
{
    [CreateAssetMenu(menuName = "PartsKit/Spine/AnimRecord", fileName = "AnimRecord_")]
    public class SpineAnimRecordData : ScriptableObject
    {
        [Serializable]
        public class AddAnimData
        {
            [SerializeField] private string animName;
            [SerializeField] private int trackIndex;
            [SerializeField] private float delay;

            public string AnimName => animName;
            public int TrackIndex => trackIndex;
            public float Delay => delay;
        }

        [Serializable]
        public class SetAnimData
        {
            [SerializeField] private string animName;
            [SerializeField] private int trackIndex;
            public string AnimName => animName;
            public int TrackIndex => trackIndex;
        }

        [Serializable]
        public class AnimClipData
        {
            [SerializeField] private string animName;
            [SerializeField] private int animHash;
            [SerializeField] private SetAnimData[] setAnimPool;
            [SerializeField] private AddAnimData[] addAnimPool;
            [SerializeField] private bool isLoop;

            public string AnimName => animName;
            public int AnimHash => animHash;
            public IReadOnlyList<SetAnimData> SetAnimPool => setAnimPool;
            public IReadOnlyList<AddAnimData> AddAnimPool => addAnimPool;
            public bool IsLoop => isLoop;

            public AnimClipData(string animNameVal, SetAnimData[] setAnimPoolVal,
                AddAnimData[] addAnimPoolVal, bool isLoopVal)
            {
                animName = animNameVal;
                animHash = Animator.StringToHash(animNameVal);
                setAnimPool = setAnimPoolVal;
                addAnimPool = addAnimPoolVal;
                isLoop = isLoopVal;
            }
        }

        [SerializeField] private AnimClipData[] clipData = Array.Empty<AnimClipData>();

        public AnimClipData[] ClipData
        {
            get => clipData;
            set => clipData = value;
        }
    }
}