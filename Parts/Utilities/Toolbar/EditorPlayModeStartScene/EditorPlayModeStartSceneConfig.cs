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
        public enum MatchSceneMode
        {
            Include = 1,
            Exclude = 2,
        }

        private const string ConfigPath = "EditorPlayStartSceneConfig";
        [field: SerializeField] public bool IsActive { get; set; }
        [field: SerializeField] public SceneAsset StartSceneAsset { get; set; }
        [field: SerializeField] public MatchSceneMode MatchMode { get; set; } = MatchSceneMode.Include;
        [field: SerializeField] public List<SceneAsset> MatchScenePool { get; set; }

        public static void PlayFromStartScene()
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
                        configList.Find(item =>
                        {
                            switch (item.MatchMode)
                            {
                                case MatchSceneMode.Include:
                                    return item.MatchScenePool.Contains(activeSceneAsset);
                                case MatchSceneMode.Exclude:
                                    return !item.MatchScenePool.Contains(activeSceneAsset);
                                default:
                                    return false;
                            }
                        });

                    if (targetConfig == null)
                    {
                        Debug.LogError(
                            $"\"{activeScene.name}\" failed to match the scene");
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
        }

        public static void StopPlay()
        {
            EditorSceneManager.playModeStartScene = null; //恢复设置;
        }
    }
}