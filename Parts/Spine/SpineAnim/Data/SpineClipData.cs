using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Test
{
    [Serializable]
    public class SpineAddAnimData
    {
        [SerializeField] private string animName;
        [SerializeField] private int trackIndex;
        [SerializeField] private bool isLoop;
        [SerializeField] private float delay;

        public string AnimName => animName;
        public int TrackIndex => trackIndex;
        public bool IsLoop => isLoop;
        public float Delay => delay;
    }

    [Serializable]
    public class SpineSetAnimData
    {
        [SerializeField] private string animName;
        [SerializeField] private int trackIndex;
        [SerializeField] private bool isLoop;

        public string AnimName => animName;
        public int TrackIndex => trackIndex;
        public bool IsLoop => isLoop;
    }

    public interface ISpineClipData
    {
        public IReadOnlyList<SpineSetAnimData> SetAnimPool { get; }
        public IReadOnlyList<SpineAddAnimData> AddAnimPool { get; }
    }

    [Serializable]
    public class SpineClipData : ISpineClipData
    {
        [SerializeField] private SpineSetAnimData[] setAnimPool;
        [SerializeField] private SpineAddAnimData[] addAnimPool;
        [SerializeField] private SpineConditionData[] conditions;

        public IReadOnlyList<SpineSetAnimData> SetAnimPool => setAnimPool;
        public IReadOnlyList<SpineAddAnimData> AddAnimPool => addAnimPool;
        public IReadOnlyList<SpineConditionData> Conditions => conditions;
    }

    [Serializable]
    public class SpineClipDataDefault : ISpineClipData
    {
        [SerializeField] private SpineSetAnimData[] setAnimPool;
        [SerializeField] private SpineAddAnimData[] addAnimPool;

        public IReadOnlyList<SpineSetAnimData> SetAnimPool => setAnimPool;
        public IReadOnlyList<SpineAddAnimData> AddAnimPool => addAnimPool;
    }
}