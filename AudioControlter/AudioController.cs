using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace PartsKit
{
    public class AudioController : MonoBehaviour
    {
        private const int AudioMixerMinVolume = -80;
        private const int AudioMixerMaxVolume = 20;
        [SerializeField] private AudioClipConfig soundClipGroup;
        [SerializeField] private AudioClipConfig musicClipGroup;
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private string masterMixerName = "Master";
        [SerializeField] private string soundMixerName = "Sound";
        [SerializeField] private string musicMixerName = "Music";
        [SerializeField] private string masterMixerVolume = "MasterVolume";
        [SerializeField] private string soundMixerVolume = "SoundVolume";
        [SerializeField] private string musicMixerVolume = "MusicVolume";
        [SerializeField] private int minVolume = 0;
        [SerializeField] private int maxVolume = 100;
        [SerializeField] private AudioSource defaultSoundSource;
        [SerializeField] private AudioSource defaultSoundSource3D;
        [SerializeField] private AudioSource defaultMusicSource;
        [SerializeField] private GameObjectPool objectPool;

        private AudioMixerGroup SoundMixerGroup =>
            audioMixer.FindMatchingGroups($"{masterMixerName}/{soundMixerName}")[0];

        private AudioMixerGroup MusicMixerGroup =>
            audioMixer.FindMatchingGroups($"{masterMixerName}/{musicMixerName}")[0];

        private AudioSource curMusicSource;

        public void PlaySound(string audioGroupName)
        {
            if (!soundClipGroup.GetAudioClipGroup(audioGroupName, out AudioClipGroup clipGroup))
            {
                return;
            }

            AudioClip audioClip = clipGroup.GetClip();

            AudioSource source = null;
            if (clipGroup.SourcePrefab.GetValue(out AudioSource audioSource))
            {
                source = objectPool.Get(audioSource);
                StartCoroutine(FinishPlay(audioClip.length, () => { objectPool.Release(source.gameObject); }));
            }
            else
            {
                source = defaultSoundSource;
            }

            source.clip = null;
            source.outputAudioMixerGroup = SoundMixerGroup;
            source.PlayOneShot(audioClip, clipGroup.VolumeScale);
        }

        public void PlaySound3D(string audioGroupName, Vector3 point)
        {
            if (!soundClipGroup.GetAudioClipGroup(audioGroupName, out AudioClipGroup clipGroup))
            {
                return;
            }

            AudioClip audioClip = clipGroup.GetClip();

            AudioSource source = objectPool.Get(clipGroup.SourcePrefab3D.GetValue(out AudioSource audioSource)
                ? audioSource
                : defaultSoundSource3D);

            StartCoroutine(FinishPlay(audioClip.length, () => { objectPool.Release(source.gameObject); }));

            source.clip = null;
            source.outputAudioMixerGroup = SoundMixerGroup;
            source.PlayOneShot(audioClip, clipGroup.VolumeScale);
            source.transform.position = point;
        }

        public void PlayMusic(string audioGroupName)
        {
            if (!musicClipGroup.GetAudioClipGroup(audioGroupName, out AudioClipGroup clipGroup))
            {
                return;
            }

            StopMusic();
            AudioClip audioClip = clipGroup.GetClip();

            if (clipGroup.SourcePrefab.GetValue(out AudioSource audioSource))
            {
                curMusicSource = objectPool.Get(audioSource);
                StartCoroutine(FinishPlay(audioClip.length, () => { objectPool.Release(curMusicSource.gameObject); }));
            }
            else
            {
                curMusicSource = defaultMusicSource;
            }

            curMusicSource.clip = audioClip;
            curMusicSource.volume = clipGroup.VolumeScale;
            curMusicSource.loop = true;
            curMusicSource.outputAudioMixerGroup = MusicMixerGroup;
            curMusicSource.Play();
        }

        public void StopMusic()
        {
            if (curMusicSource != null)
            {
                curMusicSource.Stop();
            }
        }

        public void SetMasterVolume(float volume)
        {
            SetVolume(masterMixerVolume, volume);
        }

        public void SetSoundVolume(float volume)
        {
            SetVolume(soundMixerVolume, volume);
        }

        public void SetMusicVolume(float volume)
        {
            SetVolume(musicMixerVolume, volume);
        }

        public float GetMasterVolume()
        {
            return GetVolume(masterMixerVolume);
        }

        public float GetSoundVolume()
        {
            return GetVolume(soundMixerVolume);
        }

        public float GetMusicVolume()
        {
            return GetVolume(musicMixerVolume);
        }

        private void SetVolume(string nameKey, float volume)
        {
            float volumeN = volume / (maxVolume - minVolume);
            float targetVolume =
                (AudioMixerMaxVolume - AudioMixerMinVolume) * volumeN +
                AudioMixerMinVolume; //根据audioMixer的volume返回折算，这样写表示计算公式
            targetVolume = Mathf.Clamp(targetVolume, AudioMixerMinVolume, AudioMixerMaxVolume);
            audioMixer.SetFloat(nameKey, targetVolume);
        }

        private float GetVolume(string nameKey)
        {
            if (!audioMixer.GetFloat(nameKey, out float volume))
            {
                return minVolume;
            }

            float volumeN = volume / (AudioMixerMaxVolume - AudioMixerMinVolume);
            float targetVolume = (maxVolume - minVolume) * volumeN + minVolume; //根据audioMixer的volume返回折算，这样写表示计算公式
            return targetVolume;
        }

        private IEnumerator FinishPlay(float time, Action finish)
        {
            yield return new WaitForSeconds(time + 0.5f);
            finish?.Invoke();
        }
    }
}