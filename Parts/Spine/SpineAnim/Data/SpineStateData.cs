using System.Collections.Generic;
using UnityEngine;

namespace _Test
{
    [CreateAssetMenu]
    public class SpineStateData : ScriptableObject
    {
        [SerializeField] private SpineClipDataDefault defaultClip;
        [SerializeField] private SpineClipData[] clipPool;
        [SerializeField] private SpineLineData[] linePool;

        public SpineClipDataDefault DefaultClip => defaultClip;
        public IReadOnlyList<SpineClipData> ClipPool => clipPool;
        public IReadOnlyList<SpineLineData> LinePool => linePool;
    }
}