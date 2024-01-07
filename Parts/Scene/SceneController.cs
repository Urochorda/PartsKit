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
        public abstract IEnumerator OnSucceeded();
        public abstract void OnFailed();
    }

    public class SceneController : PartsKitBehaviour
    {
        [Serializable]
        private struct LoadingSceneEffectPool
        {
            [field: SerializeField] public string EffectKey { get; private set; }
            [field: SerializeField] public LoadingSceneEffect EffectValue { get; private set; }
        }

        [SerializeField] private LoadingSceneEffect defaultLoadingEffect;
        [SerializeField] private List<LoadingSceneEffectPool> loadingEffectPool;

        protected override void OnInit()
        {
        }

        protected override void OnDeInit()
        {
        }

        public bool IsInScene(string sceneName)
        {
            Scene currentScene = GetActiveScene();
            return currentScene.name == sceneName;
        }

        public Scene GetActiveScene()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            return currentScene;
        }

        public AsyncOperationHandle<SceneInstance> LoadSceneAsync(object key, string loadingEffectKey = "",
            LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true,
            int priority = 100, Action onEffectEnd = null)
        {
            LoadingSceneEffectPool effectItem = loadingEffectPool.Find(item => item.EffectKey == loadingEffectKey);
            LoadingSceneEffect targetEffect =
                effectItem.EffectValue == null ? defaultLoadingEffect : effectItem.EffectValue;

            targetEffect.OnBegin();
            AsyncOperationHandle<SceneInstance> loadOperation =
                Addressables.LoadSceneAsync(key, loadMode, activateOnLoad, priority);
            StartCoroutine(OnSetLoadingEffect(loadOperation, targetEffect, onEffectEnd));
            return loadOperation;
        }

        private IEnumerator OnSetLoadingEffect(AsyncOperationHandle<SceneInstance> loadOperation,
            LoadingSceneEffect loadingEffect, Action onEffectEnd)
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
                    yield return loadingEffect.OnSucceeded();
                    onEffectEnd?.Invoke();
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