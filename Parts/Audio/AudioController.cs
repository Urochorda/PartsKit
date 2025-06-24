using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace PartsKit
{
    public abstract class LoadAudioAssetFun : MonoBehaviour
    {
        public abstract void LoadAll();
        public abstract void ReleaseAll();
        public abstract bool GetSoundClipGroup(string audioGroupName, out AudioClipGroup clipGroup);
        public abstract bool GetMusicClipGroup(string audioGroupName, out AudioClipGroup clipGroup);
        public abstract string GetSoundGroupByMaterial(string action, string materialA, string materialB);
        public abstract AudioMixerConfig GetAudioMixer();
    }

    public class PlayMusicKey
    {
        /// <summary>
        /// 管理器调用，不要手动调用
        /// </summary>
        public static void SetStop(PlayMusicKey key, bool isStop)
        {
            key.IsStop = isStop;
            SetPause(key, false);
        }

        /// <summary>
        /// 管理器调用，不要手动调用
        /// </summary>
        public static void SetPause(PlayMusicKey key, bool isPause)
        {
            key.IsPause = isPause;
        }

        /// <summary>
        /// 管理器调用，不要手动调用
        /// </summary>
        public static void SetInsID(PlayMusicKey key, int insId)
        {
            key.InsID = insId;
        }

        public bool IsStop { get; private set; }
        public bool IsPause { get; private set; }
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
        private AudioMixerConfig audioMixerConfig;
        private AudioMixer AudioMixer => audioMixerConfig.AudioMixer;
        private string MasterMixerName => audioMixerConfig.MasterMixerName;
        private string SoundMixerName => audioMixerConfig.SoundMixerName;
        private string MusicMixerName => audioMixerConfig.MusicMixerName;
        private string MasterMixerVolume => audioMixerConfig.MasterMixerVolume;
        private string SoundMixerVolume => audioMixerConfig.SoundMixerVolume;
        private string MusicMixerVolume => audioMixerConfig.MusicMixerVolume;

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
            customLoadAudioAssetFun.LoadAll();
            audioMixerConfig = customLoadAudioAssetFun.GetAudioMixer();
            masterVolume = GetMixerMasterVolume();
            musicVolume = GetMixerMusicVolume();
            soundVolume = GetMixerSoundVolume();
        }

        protected override void OnDeInit()
        {
            customLoadAudioAssetFun.ReleaseAll();
        }

        #region Play/Stop Audio

        public void PlaySound(string audioGroupName)
        {
            if (string.IsNullOrEmpty(audioGroupName))
            {
                return;
            }

            if (!customLoadAudioAssetFun.GetSoundClipGroup(audioGroupName, out AudioClipGroup clipGroup))
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
                objectPool.Get(isOverrideSource ? overrideSourcePrefab : defaultSoundSource, transform);
            float volumeScale = audioClip.GetVolumeScale();
            float pitch = audioClip.GetPitch();
            DoPlaySound(audioClip.AudioClip, source, volumeScale, pitch, source.transform.position);
        }

        public void PlaySoundByMaterial(string action, string materialA, string materialB)
        {
            var audioGroup = customLoadAudioAssetFun.GetSoundGroupByMaterial(action, materialA, materialB);
            PlaySound(audioGroup);
        }

        public void PlaySound3D(string audioGroupName, Vector3 point)
        {
            if (string.IsNullOrEmpty(audioGroupName))
            {
                return;
            }

            if (!customLoadAudioAssetFun.GetSoundClipGroup(audioGroupName, out AudioClipGroup clipGroup))
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
                objectPool.Get(isOverrideSource ? overrideSourcePrefab : defaultSoundSource3D, transform);
            float volumeScale = audioClip.GetVolumeScale();
            float pitch = audioClip.GetPitch();

            DoPlaySound(audioClip.AudioClip, source, volumeScale, pitch, point);
        }

        public void PlaySound3DByMaterial(string action, string materialA, string materialB, Vector3 point)
        {
            var audioGroup = customLoadAudioAssetFun.GetSoundGroupByMaterial(action, materialA, materialB);
            PlaySound3D(audioGroup, point);
        }

        private void DoPlaySound(AudioClip audioClip, AudioSource source, float volumeScale, float pitch, Vector3 point)
        {
            Transform sourceTransform = source.transform;
            sourceTransform.position = point;
            source.clip = audioClip;
            source.outputAudioMixerGroup = SoundMixerGroup;
            source.volume = volumeScale;
            source.pitch = pitch;
            source.loop = false;
            source.Play();
            StartCoroutine(WaitSoundPlayEnd(audioClip.length));

            //音效开始播放后就不可更改了，所以直接等待时间就好了
            IEnumerator WaitSoundPlayEnd(float time)
            {
                yield return new WaitForSeconds(time);
                objectPool.Release(source.gameObject);
            }
        }

        public PlayMusicKey PlayMusic(string audioGroupName, bool isLoop)
        {
            if (string.IsNullOrEmpty(audioGroupName))
            {
                return null;
            }

            if (!customLoadAudioAssetFun.GetMusicClipGroup(audioGroupName, out AudioClipGroup clipGroup))
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
            float volumeScale = audioClip.GetVolumeScale();
            float pitch = audioClip.GetPitch();

            return DoPlayMusic(audioClip.AudioClip, targetMusicSource, isLoop, volumeScale, pitch,
                targetMusicSource.transform.position);
        }

        public PlayMusicKey PlayMusic3D(string audioGroupName, bool isLoop, Vector3 point)
        {
            if (string.IsNullOrEmpty(audioGroupName))
            {
                return null;
            }

            if (!customLoadAudioAssetFun.GetMusicClipGroup(audioGroupName, out AudioClipGroup clipGroup))
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
            float volumeScale = audioClip.GetVolumeScale();
            float pitch = audioClip.GetPitch();

            return DoPlayMusic(audioClip.AudioClip, targetMusicSource, isLoop, volumeScale, pitch, point);
        }

        private PlayMusicKey DoPlayMusic(AudioClip audioClip, AudioSource source, bool isLoop, float volumeScale,
            float pitch, Vector3 point)
        {
            Transform sourceTransform = source.transform;
            sourceTransform.position = point;
            source.clip = audioClip;
            source.volume = volumeScale;
            source.pitch = pitch;
            source.loop = isLoop;
            source.outputAudioMixerGroup = MusicMixerGroup;
            source.Play();

            curMusicSourceList.Add(source);
            PlayMusicKey playMusicKey = new PlayMusicKey();
            PlayMusicKey.SetStop(playMusicKey, false);
            PlayMusicKey.SetInsID(playMusicKey, source.GetInstanceID());

            if (!isLoop) //如果不是loop则定时检测回收
            {
                StartCoroutine(WaitSoundPlayEnd(audioClip.length));
            }

            return playMusicKey;

            IEnumerator WaitSoundPlayEnd(float time)
            {
                yield return new WaitForSeconds(time);
                if (playMusicKey.IsStop) //证明已经被回收掉了
                {
                    yield break;
                }

                if (source.clip != null)
                {
                    float sourceTime = source.time;
                    float clipTime = source.clip.length;
                    //暂停中（这个是防止时间还没有向前走的时候直接暂停）||没有走完时间（时间>0表示已经开始执行了，时间<音乐长度表示没有完成）
                    if (playMusicKey.IsPause || sourceTime > 0 && sourceTime < clipTime)
                    {
                        StartCoroutine(WaitSoundPlayEnd(clipTime - sourceTime));
                    }

                    yield break;
                }

                //到时间了，回收
                curMusicSourceList.Remove(source);
                objectPool.Release(source.gameObject);
                PlayMusicKey.SetStop(playMusicKey, true);
            }
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

            PlayMusicKey.SetPause(playMusicKey, true);
            targetMusicSource.Pause();
        }

        public void UnPauseMusic(PlayMusicKey playMusicKey)
        {
            if (!TryGetMusicAudioSource(playMusicKey, out AudioSource targetMusicSource))
            {
                return;
            }

            PlayMusicKey.SetPause(playMusicKey, false);
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
    }
}