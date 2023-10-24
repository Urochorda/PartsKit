using UnityEngine;
using UnityEngine.Audio;

namespace PartsKit
{
    [CreateAssetMenu(menuName = "PartsKit/Audio/AudioMixerConfig", fileName = "AudioMixerConfig")]
    public class AudioMixerConfig : ScriptableObject
    {
        [field: SerializeField] public AudioMixer AudioMixer { get; private set; }
        [field: SerializeField] public string MasterMixerName { get; private set; } = "Master";
        [field: SerializeField] public string SoundMixerName { get; private set; } = "Sound";
        [field: SerializeField] public string MusicMixerName { get; private set; } = "Music";
        [field: SerializeField] public string MasterMixerVolume { get; private set; } = "MasterVolume";
        [field: SerializeField] public string SoundMixerVolume { get; private set; } = "SoundVolume";
        [field: SerializeField] public string MusicMixerVolume { get; private set; } = "MusicVolume";
    }
}