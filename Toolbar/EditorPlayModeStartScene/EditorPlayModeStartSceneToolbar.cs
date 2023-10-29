#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace PartsKit
{
    [InitializeOnLoad]
    public static class EditorPlayModeStartSceneToolbar
    {
        static EditorPlayModeStartSceneToolbar()
        {
            CustomToolbar.OnGuiBodyCallback -= OnGuiBody;
            CustomToolbar.OnGuiBodyCallback += OnGuiBody;
        }

        private static void OnGuiBody()
        {
            if (GUILayout.Button(new GUIContent("PlayFromStartScene", EditorGUIUtility.FindTexture("PlayButton"))))
            {
                EditorPlayModeStartSceneConfig.PlayFromStartScent();
            }
        }
    }
}

#endif