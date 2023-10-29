#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PartsKit
{
    [CreateAssetMenu(menuName = "PartsKit/Scene/EditorPlayModeStartSceneConfig",
        fileName = "EditorPlayModeStartSceneConfig")]
    public class EditorPlayModeStartSceneConfig : ScriptableObject
    {
        private const string ConfigPath = "EditorPlayStartSceneConfig";
        [field: SerializeField] public bool IsActive { get; set; }
        [field: SerializeField] public SceneAsset StartSceneAsset { get; set; }
        [field: SerializeField] public List<SceneAsset> CanJumpScene { get; set; }

        public static void PlayFromStartScent()
        {
            if (!EditorApplication.isPlaying)
            {
                EditorPlayModeStartSceneConfig[]
                    configArray = Resources.LoadAll<EditorPlayModeStartSceneConfig>(ConfigPath);
                if (configArray == null || configArray.Length <= 0)
                {
                    Debug.LogError(
                        $"Please first in the \"Resources/{ConfigPath}/\" create \"{nameof(EditorPlayModeStartSceneConfig)}\"");
                    return;
                }

                List<EditorPlayModeStartSceneConfig> configList = configArray.ToList();
                configList.RemoveAll(item => !item.IsActive);
                if (configList.Count <= 0)
                {
                    Debug.LogError(
                        $"The \"{nameof(IsActive)}\" values of \"{nameof(EditorPlayModeStartSceneConfig)}\" are all \"false\"");
                    return;
                }

                Scene activeScene = SceneManager.GetActiveScene();
                SceneAsset activeSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(activeScene.path);

                SceneAsset scene;
                if (!configList.Exists(item => item.StartSceneAsset == activeSceneAsset))
                {
                    EditorPlayModeStartSceneConfig targetConfig =
                        configList.Find(item => item.CanJumpScene.Contains(activeSceneAsset));
                    if (targetConfig == null)
                    {
                        Debug.LogError(
                            $"\"{activeScene.name}\" is not in the \"{nameof(CanJumpScene)}\" list");
                        return;
                    }

                    if (targetConfig.StartSceneAsset == null)
                    {
                        Debug.LogError(
                            $"The \"{targetConfig.name}\" of \"{nameof(StartSceneAsset)}\" is empty");
                        return;
                    }

                    scene = targetConfig.StartSceneAsset;
                }
                else
                {
                    scene = activeSceneAsset;
                }


                EditorSceneManager.playModeStartScene = scene;
            }

            EditorApplication.isPlaying = !EditorApplication.isPlaying;

            if (EditorApplication.isPlaying)
            {
                EditorSceneManager.playModeStartScene = null; //恢复设置
            }
        }
    }
}

#endif