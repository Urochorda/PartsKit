using System;
using System.Collections.Generic;
using UnityEngine;

namespace PartsKit
{
    [Serializable]
    public class AudioMaterialAction
    {
        [field: SerializeField] public string Action { get; private set; }
        [field: SerializeField] public string DefaultAudio { get; private set; }
        [field: SerializeField] public AudioMaterialMatch[] Match { get; private set; }
    }

    [Serializable]
    public class AudioMaterialMatch
    {
        [field: SerializeField] public string MaterialA { get; private set; }
        [field: SerializeField] public string MaterialB { get; private set; }
        [field: SerializeField] public string Audio { get; private set; }
    }

    [CreateAssetMenu(menuName = "PartsKit/Audio/AudioMaterialConfig", fileName = "AudioMaterialConfig")]
    public class AudioMaterialConfig : ScriptableObject
    {
        private class Cache
        {
            public string DefaultAudio { get; set; }
            public Dictionary<(string, string), string> MatchAudio { get; set; }
        }

        [field: SerializeField] public AudioMaterialAction[] ActionMaterials { get; private set; }

        private Dictionary<string, Cache> materialCacheMap;

        private void OnEnable()
        {
            materialCacheMap = new Dictionary<string, Cache>();

            foreach (var actionMaterial in ActionMaterials)
            {
                var action = actionMaterial.Action;
                if (!materialCacheMap.TryGetValue(action, out var cache))
                {
                    cache = new Cache()
                    {
                        DefaultAudio = actionMaterial.DefaultAudio,
                        MatchAudio = new Dictionary<(string, string), string>()
                    };
                    materialCacheMap[action] = cache;
                }

                var matchList = actionMaterial.Match;
                foreach (var match in matchList)
                {
                    var key = GetSortedKey(match.MaterialA, match.MaterialB);
                    cache.MatchAudio[key] = match.Audio;
                }
            }
        }

        private (string, string) GetSortedKey(string a, string b)
        {
            return string.CompareOrdinal(a, b) <= 0 ? (a, b) : (b, a);
        }

        public string GetAudioGroupName(string action, string materialA, string materialB)
        {
            if (!materialCacheMap.TryGetValue(action, out var cache))
            {
                return string.Empty;
            }

            var key = GetSortedKey(materialA, materialB);
            return cache.MatchAudio.TryGetValue(key, out var audio) ? audio : cache.DefaultAudio;
        }
    }
}