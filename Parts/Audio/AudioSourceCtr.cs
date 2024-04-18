using UnityEngine;

namespace PartsKit
{
    [RequireComponent(typeof(AudioSource))]
    public abstract class AudioSourceCtr : MonoBehaviour
    {
        public enum AudioSourceMixerType
        {
            Sound = 0,
            Music = 1,
        }

        [field: SerializeField] public AudioSourceMixerType MixerType { get; private set; }

        public AudioSource AudioSource { get; private set; }

        private void Awake()
        {
            AudioSource = GetComponent<AudioSource>();
            if (AudioSource == null)
            {
                return;
            }

            AudioController controller = GetController();
            if (controller == null)
            {
                return;
            }

            AudioSource.outputAudioMixerGroup = MixerType == AudioSourceMixerType.Music
                ? controller.MusicMixerGroup
                : controller.SoundMixerGroup;
        }

        protected abstract AudioController GetController();
    }
}