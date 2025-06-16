using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PartsKit
{
    [Serializable]
    public class AudioClipData
    {
        [field: SerializeField] public AudioClip AudioClip { get; set; }
        [field: SerializeField] public float VolumeScale { get; set; } = 1;
        [field: SerializeField] public float Pitch { get; set; } = 1;
        [field: SerializeField] public bool RandomVolumeScale { get; set; }
        [field: SerializeField] public float VolumeScaleMin { get; set; } = 1;
        [field: SerializeField] public float VolumeScaleMax { get; set; } = 1;
        [field: SerializeField] public bool RandomPitch { get; set; }
        [field: SerializeField] public float PitchMin { get; set; } = 1;
        [field: SerializeField] public float PitchMax { get; set; } = 1;

        public float GetVolumeScale()
        {
            float volumeScale = RandomVolumeScale ? Random.Range(VolumeScaleMin, VolumeScaleMax) : VolumeScale;
            return volumeScale;
        }

        public float GetPitch()
        {
            float pitch = RandomPitch ? Random.Range(PitchMin, PitchMax) : Pitch;
            return pitch;
        }
    }

    [Serializable]
    public class AudioClipGroup
    {
        public enum PlayMode
        {
            Random,
            NoRepeatRandom,
            Sequence,
        }

        [field: SerializeField] public string GroupName { get; private set; }

        [field: SerializeField] public CheckNullProperty<AudioSource> SourcePrefab { get; private set; } //音效source预设

        [field: SerializeField]
        public CheckNullProperty<AudioSource> SourcePrefab3D { get; private set; } //3d音效source预设

        [SerializeField] private PlayMode playMode = PlayMode.Sequence;
        [SerializeField] private AudioClipData[] audioClipData;

        int nextIndex = -1;
        int lastIndex = -1;

        public bool GetClip(out AudioClipData targetData)
        {
            if (audioClipData == null || audioClipData.Length <= 0)
            {
                CustomLog.LogError("AudioClipGroupConfig的" + GroupName + "的AudioClip为空");
                targetData = null;
            }
            else if (audioClipData.Length == 1) //如果只有1个则直接返回
            {
                targetData = audioClipData[0];
            }
            else
            {
                switch (playMode)
                {
                    case PlayMode.Random:
                        nextIndex = Random.Range(0, audioClipData.Length);
                        break;
                    case PlayMode.NoRepeatRandom:
                        do
                        {
                            nextIndex = Random.Range(0, audioClipData.Length);
                        } while (nextIndex == lastIndex);

                        break;
                    case PlayMode.Sequence:
                        nextIndex = (int)Mathf.Repeat(++lastIndex, audioClipData.Length);
                        break;
                }

                lastIndex = nextIndex;
                targetData = audioClipData[nextIndex];
            }

            return targetData != null;
        }
    }

    [CreateAssetMenu(menuName = "PartsKit/Audio/AudioClipConfig", fileName = "AudioClipConfig_")]
    public class AudioClipConfig : ScriptableObject
    {
        [SerializeField] private List<AudioClipGroup> audioClipGroups;

        public bool GetAudioClipGroup(string groupName, out AudioClipGroup clipGroup)
        {
            clipGroup = audioClipGroups.Find(item => item.GroupName == groupName);
            return clipGroup != null;
        }
    }
}