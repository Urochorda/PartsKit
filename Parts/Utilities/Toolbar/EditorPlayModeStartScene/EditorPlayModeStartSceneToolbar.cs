using UnityEditor;
using UnityEngine;

namespace PartsKit
{
    [InitializeOnLoad]
    public static class EditorPlayModeStartSceneToolbar
    {
        static EditorPlayModeStartSceneToolbar()
        {
            ToolbarExtender.OnGuiBodyRightCallback -= OnGuiBody;
            ToolbarExtender.OnGuiBodyRightCallback += OnGuiBody;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnGuiBody()
        {
            if (GUILayout.Button(new GUIContent("PlayFromStartScene", EditorGUIUtility.FindTexture("PlayButton"))))
            {
                EditorPlayModeStartSceneConfig.PlayFromStartScene();
            }
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.ExitingPlayMode)
            {
                EditorPlayModeStartSceneConfig.StopPlay();
            }
        }
    }
}