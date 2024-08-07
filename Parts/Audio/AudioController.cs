using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace PartsKit
{
    public abstract class LoadAudioAssetFun : MonoBehaviour
    {
        public abstract bool LoadSoundClipGroup(string audioGroupName, out AudioClipGroup clipGroup);
        public abstract bool LoadMusicClipGroup(string audioGroupName, out AudioClipGroup clipGroup);
        public abstract AudioMixerConfig LoadAudioMixer();
    }

    public class PlayMusicKey
    {
        /// <summary>
        /// 管理器调用，不要手动调用
        /// </summary>
        public static void SetStop(PlayMusicKey key, bool isStop)
        {
            key.IsStop = isStop;
        }

        /// <summary>
        /// 管理器调用，不要手动调用
        /// </summary>
        public static void SetInsID(PlayMusicKey key, int insId)
        {
            key.InsID = insId;
        }

        public bool IsStop { get; private set; }
        public int InsID { get; private set; }
    }

    public class AudioController : PartsKitBehaviour
    {
        private const int AudioMixerMinVolume = -80;
        private const int AudioMixerMaxVolume = 0;
        [SerializeField] private LoadAudioAssetFun customLoadAudioAssetFun;
        [SerializeField] private int minVolume = 0;
        [SerializeField] private int maxVolume = 100;
        [SerializeField] private AudioSource defaultSoundSource;
        [SerializeField] private AudioSource defaultSoundSource3D;
        [SerializeField] private AudioSource defaultMusicSource;
        [SerializeField] private AudioSource defaultMusicSource3D;
        [SerializeField] private GameObjectPool objectPool;
        private AudioMixer AudioMixer => customLoadAudioAssetFun.LoadAudioMixer().AudioMixer;
        private string MasterMixerName => customLoadAudioAssetFun.LoadAudioMixer().MasterMixerName;
        private string SoundMixerName => customLoadAudioAssetFun.LoadAudioMixer().SoundMixerName;
        private string MusicMixerName => customLoadAudioAssetFun.LoadAudioMixer().MusicMixerName;
        private string MasterMixerVolume => customLoadAudioAssetFun.LoadAudioMixer().MasterMixerVolume;
        private string SoundMixerVolume => customLoadAudioAssetFun.LoadAudioMixer().SoundMixerVolume;
        private string MusicMixerVolume => customLoadAudioAssetFun.LoadAudioMixer().MusicMixerVolume;

        public AudioMixerGroup SoundMixerGroup =>
            AudioMixer.FindMatchingGroups($"{MasterMixerName}/{SoundMixerName}")[0];

        public AudioMixerGroup MusicMixerGroup =>
            AudioMixer.FindMatchingGroups($"{MasterMixerName}/{MusicMixerName}")[0];

        private readonly List<AudioSource> curMusicSourceList = new List<AudioSource>();

        public int MinVolume
        {
            get => minVolume;
            set => minVolume = value;
        }

        public int MaxVolume
        {
            get => maxVolume;
            set => maxVolume = value;
        }

        private int maxLogVolume = 1000;
        private const int MinLogVolume = 1;

        public int MaxLogVolume
        {
            get => maxLogVolume;
            set => maxLogVolume = Mathf.Max(MinLogVolume, value);
        }

        public bool masterOn = true;
        public bool musicOn = true;
        public bool soundOn = true;
        public float masterVolume;
        public float musicVolume;
        public float soundVolume;

        protected override void OnInit()
        {
            masterVolume = GetMixerMasterVolume();
            musicVolume = GetMixerMusicVolume();
            soundVolume = GetMixerSoundVolume();
        }

        protected override void OnDeInit()
        {
        }

        #region Play/Stop Audio

        public void PlaySound(string audioGroupName)
        {
            if (!customLoadAudioAssetFun.LoadSoundClipGroup(audioGroupName, out AudioClipGroup clipGroup))
            {
                CustomLog.LogError($"SoundClipGroup is null: {audioGroupName}");
                return;
            }

            bool hasAudioClipData = clipGroup.GetClip(out AudioClipData audioClip);
            if (!hasAudioClipData)
            {
                CustomLog.LogError($"SoundClipData is null: {audioGroupName}");
                return;
            }

            bool isOverrideSource = clipGroup.SourcePrefab.GetValue(out AudioSource overrideSourcePrefab);
            AudioSource source =
                isOverrideSource ? objectPool.Get(overrideSourcePrefab, transform) : defaultSoundSource;
            DoPlaySound(audioClip.AudioClip, source, isOverrideSource, audioClip.VolumeScale,
                source.transform.position);
        }

        public void PlaySound3D(string audioGroupName, Vector3 point)
        {
            if (!customLoadAudioAssetFun.LoadSoundClipGroup(audioGroupName, out AudioClipGroup clipGroup))
            {
                CustomLog.LogError($"SoundClipGroup is null: {audioGroupName}");
                return;
            }

            bool hasAudioClipData = clipGroup.GetClip(out AudioClipData audioClip);
            if (!hasAudioClipData)
            {
                CustomLog.LogError($"SoundClipData is null: {audioGroupName}");
                return;
            }

            bool isOverrideSource = clipGroup.SourcePrefab3D.GetValue(out AudioSource overrideSourcePrefab);
            AudioSource source =
                isOverrideSource ? objectPool.Get(overrideSourcePrefab, transform) : defaultSoundSource3D;
            DoPlaySound(audioClip.AudioClip, source, isOverrideSource, audioClip.VolumeScale, point);
        }

        private void DoPlaySound(AudioClip audioClip, AudioSource source, bool isRelease, float volumeScale,
            Vector3 point)
        {
            Transform sourceTransform = source.transform;
            sourceTransform.position = point;
            source.clip = null;
            source.outputAudioMixerGroup = SoundMixerGroup;
            source.PlayOneShot(audioClip, volumeScale);
            if (isRelease)
            {
                objectPool.Release(source.gameObject);
            }
        }

        public PlayMusicKey PlayMusic(string audioGroupName, bool isLoop)
        {
            if (!customLoadAudioAssetFun.LoadMusicClipGroup(audioGroupName, out AudioClipGroup clipGroup))
            {
                CustomLog.LogError($"MusicClipGroup is null: {audioGroupName}");
                return null;
            }

            bool hasAudioClipData = clipGroup.GetClip(out AudioClipData audioClip);
            if (!hasAudioClipData)
            {
                CustomLog.LogError($"MusicClipData is null: {audioGroupName}");
                return null;
            }

            AudioSource targetMusicSource =
                objectPool.Get(clipGroup.SourcePrefab.GetValue(out AudioSource overrideSourcePrefab)
                    ? overrideSourcePrefab
                    : defaultMusicSource, transform);

            return DoPlayMusic(audioClip.AudioClip, targetMusicSource, isLoop, audioClip.VolumeScale,
                targetMusicSource.transform.position);
        }

        public PlayMusicKey PlayMusic3D(string audioGroupName, bool isLoop, Vector3 point)
        {
            if (!customLoadAudioAssetFun.LoadMusicClipGroup(audioGroupName, out AudioClipGroup clipGroup))
            {
                CustomLog.LogError($"MusicClipGroup is null: {audioGroupName}");
                return null;
            }

            bool hasAudioClipData = clipGroup.GetClip(out AudioClipData audioClip);
            if (!hasAudioClipData)
            {
                CustomLog.LogError($"MusicClipData is null: {audioGroupName}");
                return null;
            }

            AudioSource targetMusicSource =
                objectPool.Get(clipGroup.SourcePrefab3D.GetValue(out AudioSource overrideSourcePrefab)
                    ? overrideSourcePrefab
                    : defaultMusicSource3D, transform);

            return DoPlayMusic(audioClip.AudioClip, targetMusicSource, isLoop, audioClip.VolumeScale, point);
        }

        private PlayMusicKey DoPlayMusic(AudioClip audioClip, AudioSource source, bool isLoop, float volumeScale,
            Vector3 point)
        {
            Transform sourceTransform = source.transform;
            sourceTransform.position = point;
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

            PlayMusicKey playMusicKey = new PlayMusicKey();
            PlayMusicKey.SetStop(playMusicKey, false);
            PlayMusicKey.SetInsID(playMusicKey, source.GetInstanceID());
            return playMusicKey;
        }

        public void StopMusic(PlayMusicKey playMusicKey)
        {
            if (!TryGetMusicAudioSource(playMusicKey, out AudioSource targetMusicSource))
            {
                return;
            }

            targetMusicSource.Stop();
            curMusicSourceList.Remove(targetMusicSource);
            objectPool.Release(targetMusicSource.gameObject);
            PlayMusicKey.SetStop(playMusicKey, true);
        }

        public void PauseMusic(PlayMusicKey playMusicKey)
        {
            if (!TryGetMusicAudioSource(playMusicKey, out AudioSource targetMusicSource))
            {
                return;
            }

            targetMusicSource.Pause();
        }

        public void UnPauseMusic(PlayMusicKey playMusicKey)
        {
            if (!TryGetMusicAudioSource(playMusicKey, out AudioSource targetMusicSource))
            {
                return;
            }

            targetMusicSource.UnPause();
        }

        private bool TryGetMusicAudioSource(PlayMusicKey playMusicKey, out AudioSource audioSource)
        {
            if (playMusicKey == null || playMusicKey.IsStop) //已经废掉了
            {
                audioSource = null;
                return false;
            }

            audioSource = curMusicSourceList.Find(item => item.GetInstanceID() == playMusicKey.InsID);
            return audioSource != null;
        }

        #endregion

        #region Get/Set Volume

        public void SetMasterOn(bool value)
        {
            masterOn = value;
            SetMixerMasterVolume(GetMasterOn() ? GetMasterVolume() : 0);
        }

        public void SetMusicOn(bool value)
        {
            musicOn = value;
            SetMixerMusicVolume(GetMusicOn() ? GetMusicVolume() : 0);
        }

        public void SetSoundOn(bool value)
        {
            soundOn = value;
            SetMixerSoundVolume(GetSoundOn() ? GetSoundVolume() : 0);
        }

        public void SetMasterVolume(float volume)
        {
            masterVolume = volume;
            if (GetMasterOn())
            {
                SetMixerMasterVolume(GetMasterVolume());
            }
        }

        public void SetSoundVolume(float volume)
        {
            soundVolume = volume;
            if (GetSoundOn())
            {
                SetMixerSoundVolume(GetSoundVolume());
            }
        }

        public void SetMusicVolume(float volume)
        {
            musicVolume = volume;
            if (GetMusicOn())
            {
                SetMixerMusicVolume(GetMusicVolume());
            }
        }

        public bool GetMasterOn()
        {
            return masterOn;
        }

        public bool GetMusicOn()
        {
            return musicOn;
        }

        public bool GetSoundOn()
        {
            return soundOn;
        }

        public float GetMasterVolume()
        {
            return masterVolume;
        }

        public float GetSoundVolume()
        {
            return soundVolume;
        }

        public float GetMusicVolume()
        {
            return musicVolume;
        }

        #endregion

        #region Mixer Set/Get

        private void SetMixerMasterVolume(float volume)
        {
            SetMixerVolume(MasterMixerVolume, volume);
        }

        private void SetMixerSoundVolume(float volume)
        {
            SetMixerVolume(SoundMixerVolume, volume);
        }

        private void SetMixerMusicVolume(float volume)
        {
            SetMixerVolume(MusicMixerVolume, volume);
        }

        private float GetMixerMasterVolume()
        {
            return GetMixerVolume(MasterMixerVolume);
        }

        private float GetMixerSoundVolume()
        {
            return GetMixerVolume(SoundMixerVolume);
        }

        private float GetMixerMusicVolume()
        {
            return GetMixerVolume(MusicMixerVolume);
        }

        private void SetMixerVolume(string nameKey, float volume)
        {
            volume = Mathf.Clamp(volume, minVolume, maxVolume);
            float volumeN = (volume - minVolume) / (maxVolume - minVolume);

            // 将输入范围映射到对数范围
            float maxLogValue = Mathf.Log(MaxLogVolume);
            float minLogValue = Mathf.Log(MinLogVolume);
            float curLogValue =
                Mathf.Log(MinLogVolume + (MaxLogVolume - MinLogVolume) * volumeN);
            float logValueN = (curLogValue - minLogValue) / (maxLogValue - minLogValue);

            float targetVolume = (AudioMixerMaxVolume - AudioMixerMinVolume) * logValueN + AudioMixerMinVolume;
            AudioMixer.SetFloat(nameKey, targetVolume);
        }

        private float GetMixerVolume(string nameKey)
        {
            if (!AudioMixer.GetFloat(nameKey, out float volume))
            {
                return minVolume;
            }

            // 将对数范围映射到线性范围
            float maxLogValue = Mathf.Log(MaxLogVolume);
            float minLogValue = Mathf.Log(MinLogVolume);
            float logValue = (volume - AudioMixerMinVolume) / (AudioMixerMaxVolume - AudioMixerMinVolume) *
                (maxLogValue - minLogValue) + minLogValue;
            float targetVolumeN = (Mathf.Exp(logValue) - minLogValue) / (MaxLogVolume - MinLogVolume);
            float targetVolume = targetVolumeN * (maxVolume - minVolume) + minVolume;
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