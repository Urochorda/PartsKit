using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace PartsKit
{
    public abstract class LoadingSceneEffect : MonoBehaviour
    {
        public abstract void OnBegin();
        public abstract void OnProgress(float percentComplete);
        public abstract void OnSucceeded();
        public abstract void OnFailed();
    }

    public class SceneController : MonoBehaviour
    {
        [Serializable]
        private struct LoadingSceneEffectPool
        {
            [field: SerializeField] public string EffectKey { get; private set; }
            [field: SerializeField] public LoadingSceneEffect EffectValue { get; private set; }
        }

        [SerializeField] private LoadingSceneEffect defaultLoadingEffect;
        [SerializeField] private List<LoadingSceneEffectPool> loadingEffectPool;

        public AsyncOperationHandle<SceneInstance> LoadSceneAsync(object key, string loadingEffectKey = "",
            LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true,
            int priority = 100)
        {
            LoadingSceneEffectPool effectItem = loadingEffectPool.Find(item => item.EffectKey == loadingEffectKey);
            LoadingSceneEffect targetEffect =
                effectItem.EffectValue == null ? defaultLoadingEffect : effectItem.EffectValue;

            targetEffect.OnBegin();
            AsyncOperationHandle<SceneInstance> loadOperation =
                Addressables.LoadSceneAsync(key, loadMode, activateOnLoad, priority);
            StartCoroutine(OnSetLoadingEffect(loadOperation, targetEffect));
            return loadOperation;
        }

        private IEnumerator OnSetLoadingEffect(AsyncOperationHandle<SceneInstance> loadOperation,
            LoadingSceneEffect loadingEffect)
        {
            while (!loadOperation.IsDone)
            {
                float percentComplete = Mathf.Clamp01(loadOperation.PercentComplete);
                loadingEffect.OnProgress(percentComplete);
                yield return null;
            }

            switch (loadOperation.Status)
            {
                case AsyncOperationStatus.Succeeded:
                    loadingEffect.OnProgress(1);
                    loadingEffect.OnSucceeded();
                    break;
                default:
                    loadingEffect.OnFailed();
                    break;
            }
        }

        public AsyncOperationHandle<SceneInstance> UnloadSceneAsync(AsyncOperationHandle<SceneInstance> handle,
            bool autoReleaseHandle = true)
        {
            return Addressables.UnloadSceneAsync(handle, autoReleaseHandle);
        }
    }
}