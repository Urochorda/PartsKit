using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace PartsKit
{
    public abstract class LoadAudioAssetFun : MonoBehaviour
    {
        public abstract AudioClipConfig LoadSoundClipGroup();
        public abstract AudioClipConfig LoadMusicClipGroup();
        public abstract AudioMixerConfig LoadAudioMixer();
    }

    public class AudioController : PartsKitBehaviour
    {
        private const int AudioMixerMinVolume = -80;
        private const int AudioMixerMaxVolume = 20;
        [SerializeField] private LoadAudioAssetFun customLoadAudioAssetFun;
        [SerializeField] private int minVolume = 0;
        [SerializeField] private int maxVolume = 100;
        [SerializeField] private AudioSource defaultSoundSource;
        [SerializeField] private AudioSource defaultSoundSource3D;
        [SerializeField] private AudioSource defaultMusicSource;
        [SerializeField] private AudioSource defaultMusicSource3D;
        [SerializeField] private GameObjectPool objectPool;

        private AudioClipConfig SoundClipGroup => customLoadAudioAssetFun.LoadSoundClipGroup();
        private AudioClipConfig MusicClipGroup => customLoadAudioAssetFun.LoadMusicClipGroup();
        private AudioMixer AudioMixer => customLoadAudioAssetFun.LoadAudioMixer().AudioMixer;
        private string MasterMixerName => customLoadAudioAssetFun.LoadAudioMixer().MasterMixerName;
        private string SoundMixerName => customLoadAudioAssetFun.LoadAudioMixer().SoundMixerName;
        private string MusicMixerName => customLoadAudioAssetFun.LoadAudioMixer().MusicMixerName;
        private string MasterMixerVolume => customLoadAudioAssetFun.LoadAudioMixer().MasterMixerVolume;
        private string SoundMixerVolume => customLoadAudioAssetFun.LoadAudioMixer().SoundMixerVolume;
        private string MusicMixerVolume => customLoadAudioAssetFun.LoadAudioMixer().MusicMixerVolume;

        private AudioMixerGroup SoundMixerGroup =>
            AudioMixer.FindMatchingGroups($"{MasterMixerName}/{SoundMixerName}")[0];

        private AudioMixerGroup MusicMixerGroup =>
            AudioMixer.FindMatchingGroups($"{MasterMixerName}/{MusicMixerName}")[0];

        private readonly List<AudioSource> curMusicSourceList = new List<AudioSource>();

        protected override void OnInit()
        {
        }

        protected override void OnDeInit()
        {
        }

        #region Play/Stop Audio

        public void PlaySound(string audioGroupName)
        {
            if (!SoundClipGroup.GetAudioClipGroup(audioGroupName, out AudioClipGroup clipGroup))
            {
                return;
            }

            AudioClip audioClip = clipGroup.GetClip();
            bool isOverrideSource = clipGroup.SourcePrefab.GetValue(out AudioSource overrideSourcePrefab);
            AudioSource source = isOverrideSource ? objectPool.Get(overrideSourcePrefab) : defaultSoundSource;
            DoPlaySound(audioClip, source, isOverrideSource, clipGroup.VolumeScale, source.transform.position);
        }

        public void PlaySound3D(string audioGroupName, Vector3 point)
        {
            if (!SoundClipGroup.GetAudioClipGroup(audioGroupName, out AudioClipGroup clipGroup))
            {
                return;
            }

            AudioClip audioClip = clipGroup.GetClip();
            bool isOverrideSource = clipGroup.SourcePrefab3D.GetValue(out AudioSource overrideSourcePrefab);
            AudioSource source = isOverrideSource ? objectPool.Get(overrideSourcePrefab) : defaultSoundSource3D;
            DoPlaySound(audioClip, source, isOverrideSource, clipGroup.VolumeScale, point);
        }

        private void DoPlaySound(AudioClip audioClip, AudioSource source, bool isRelease, float volumeScale,
            Vector3 point)
        {
            Transform sourceTransform = source.transform;
            sourceTransform.SetParent(transform);
            sourceTransform.position = point;
            source.clip = null;
            source.outputAudioMixerGroup = SoundMixerGroup;
            source.PlayOneShot(audioClip, volumeScale);
            if (isRelease)
            {
                objectPool.Release(source.gameObject);
            }
        }

        public int PlayMusic(string audioGroupName, bool isLoop)
        {
            if (!MusicClipGroup.GetAudioClipGroup(audioGroupName, out AudioClipGroup clipGroup))
            {
                return -1;
            }

            AudioClip audioClip = clipGroup.GetClip();
            AudioSource targetMusicSource =
                objectPool.Get(clipGroup.SourcePrefab.GetValue(out AudioSource overrideSourcePrefab)
                    ? overrideSourcePrefab
                    : defaultMusicSource);

            return DoPlayMusic(audioClip, targetMusicSource, isLoop, clipGroup.VolumeScale,
                targetMusicSource.transform.position);
        }

        public int PlayMusic3D(string audioGroupName, bool isLoop, Vector3 point)
        {
            if (!MusicClipGroup.GetAudioClipGroup(audioGroupName, out AudioClipGroup clipGroup))
            {
                return -1;
            }

            AudioClip audioClip = clipGroup.GetClip();
            AudioSource targetMusicSource =
                objectPool.Get(clipGroup.SourcePrefab3D.GetValue(out AudioSource overrideSourcePrefab)
                    ? overrideSourcePrefab
                    : defaultMusicSource3D);

            return DoPlayMusic(audioClip, targetMusicSource, isLoop, clipGroup.VolumeScale, point);
        }

        private int DoPlayMusic(AudioClip audioClip, AudioSource source, bool isLoop, float volumeScale,
            Vector3 point)
        {
            Transform sourceTransform = source.transform;
            sourceTransform.SetParent(transform);
            sourceTransform.position = point;
            source.transform.SetParent(transform);
            source.clip = audioClip;
            source.volume = volumeScale;
            source.loop = isLoop;
            source.outputAudioMixerGroup = MusicMixerGroup;
            source.Play();

            curMusicSourceList.Add(source);
            if (!isLoop) //如果不是loop则定时回收
            {
                StartCoroutine(FinishPlay(audioClip.length, () =>
                {
                    curMusicSourceList.Remove(source);
                    objectPool.Release(source.gameObject);
                }));
            }

            return source.GetInstanceID();
        }

        public void StopMusic(ref int instanceID)
        {
            int targetInsID = instanceID;
            AudioSource targetMusicSource = curMusicSourceList.Find(item => item.GetInstanceID() == targetInsID);
            if (targetMusicSource != null)
            {
                targetMusicSource.Stop();
                curMusicSourceList.Remove(targetMusicSource);
                objectPool.Release(targetMusicSource.gameObject);
            }

            instanceID = -1;
        }

        #endregion

        #region Get/Set Volume

        public void SetMasterVolume(float volume)
        {
            SetVolume(MasterMixerVolume, volume);
        }

        public void SetSoundVolume(float volume)
        {
            SetVolume(SoundMixerVolume, volume);
        }

        public void SetMusicVolume(float volume)
        {
            SetVolume(MusicMixerVolume, volume);
        }

        public float GetMasterVolume()
        {
            return GetVolume(MasterMixerVolume);
        }

        public float GetSoundVolume()
        {
            return GetVolume(SoundMixerVolume);
        }

        public float GetMusicVolume()
        {
            return GetVolume(MusicMixerVolume);
        }

        private void SetVolume(string nameKey, float volume)
        {
            float volumeN = volume / (maxVolume - minVolume);
            float targetVolume =
                (AudioMixerMaxVolume - AudioMixerMinVolume) * volumeN +
                AudioMixerMinVolume; //根据audioMixer的volume返回折算，这样写表示计算公式
            targetVolume = Mathf.Clamp(targetVolume, AudioMixerMinVolume, AudioMixerMaxVolume);
            AudioMixer.SetFloat(nameKey, targetVolume);
        }

        private float GetVolume(string nameKey)
        {
            if (!AudioMixer.GetFloat(nameKey, out float volume))
            {
                return minVolume;
            }

            float volumeN = volume / (AudioMixerMaxVolume - AudioMixerMinVolume);
            float targetVolume = (maxVolume - minVolume) * volumeN + minVolume; //根据audioMixer的volume返回折算，这样写表示计算公式
            return targetVolume;
        }

        #endregion

        private IEnumerator FinishPlay(float time, Action finish)
        {
            yield return new WaitForSeconds(time + 0.5f);
            finish?.Invoke();
        }
    }
}