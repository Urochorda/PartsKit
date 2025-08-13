using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

namespace PartsKit
{
    public class FontReplacerTool : EditorWindow
    {
        private Font targetOldFont;
        private Font targetNewFont;
        private TMP_FontAsset targetOldTMPFont;
        private TMP_FontAsset targetNewTMPFont;
        private string searchPath = "Assets";

        [MenuItem("Tools/Font Replacer")]
        public static void ShowWindow()
        {
            GetWindow<FontReplacerTool>("Font Replacer");
        }

        private void OnGUI()
        {
            GUILayout.Label("Standard Text Font Replacer", EditorStyles.boldLabel);
            targetOldFont = (Font)EditorGUILayout.ObjectField("Old Font", targetOldFont, typeof(Font), false);
            targetNewFont = (Font)EditorGUILayout.ObjectField("New Font", targetNewFont, typeof(Font), false);

            GUILayout.Space(20);
            GUILayout.Label("TextMeshPro Font Replacer", EditorStyles.boldLabel);
            targetOldTMPFont =
                (TMP_FontAsset)EditorGUILayout.ObjectField("Old TMP Font", targetOldTMPFont, typeof(TMP_FontAsset),
                    false);
            targetNewTMPFont =
                (TMP_FontAsset)EditorGUILayout.ObjectField("New TMP Font", targetNewTMPFont, typeof(TMP_FontAsset),
                    false);

            GUILayout.Space(20);
            GUILayout.Label("Search Settings", EditorStyles.boldLabel);
            searchPath = EditorGUILayout.TextField("Search Path", searchPath);

            GUILayout.Space(30);
            if (GUILayout.Button("Replace Fonts in Prefabs"))
            {
                ReplaceFontsInPrefabs();
            }
        }

        private void ReplaceFontsInPrefabs()
        {
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { searchPath });
            int total = prefabGuids.Length;
            int changedCount = 0;

            for (int i = 0; i < prefabGuids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(prefabGuids[i]);
                EditorUtility.DisplayProgressBar("Processing Prefabs",
                    $"Processing {Path.GetFileName(path)} ({i + 1}/{total})",
                    (float)i / total);

                bool modified = ProcessPrefab(path);

                if (modified)
                {
                    changedCount++;
                }
            }

            EditorUtility.ClearProgressBar();
            Debug.Log($"Font Replacer completed. Processed {total} prefabs, modified {changedCount}");
        }

        private bool ProcessPrefab(string prefabPath)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            bool modified = false;

            var prefabAssetType = PrefabUtility.GetPrefabAssetType(prefab);
            bool isVariant = prefabAssetType == PrefabAssetType.Variant;
            // 处理普通Text组件
            if (targetOldFont)
            {
                Text[] textComponents = prefab.GetComponentsInChildren<Text>(true);
                foreach (Text text in textComponents)
                {
                    if (text.font != targetOldFont)
                    {
                        continue;
                    }

                    if (isVariant) //变体
                    {
                        PropertyModification[] modifications = PrefabUtility.GetPropertyModifications(text);
                        PropertyModification textFontPm =
                            Array.Find(modifications, item => item.propertyPath == "m_FontData.m_Font");
                        if (textFontPm == null)
                        {
                            continue;
                        }
                    }

                    text.font = targetNewFont;
                    modified = true;
                }
            }

            // 处理TextMeshPro组件
            if (targetOldTMPFont)
            {
                TMP_Text[] tmpComponents = prefab.GetComponentsInChildren<TMP_Text>(true);
                foreach (TMP_Text tmpText in tmpComponents)
                {
                    if (tmpText.font != targetOldTMPFont)
                    {
                        continue;
                    }

                    if (isVariant) //变体
                    {
                        PropertyModification[] modifications = PrefabUtility.GetPropertyModifications(tmpText);
                        PropertyModification textFontPm =
                            Array.Find(modifications, item => item.propertyPath == "m_fontAsset");
                        if (textFontPm == null)
                        {
                            continue;
                        }
                    }

                    tmpText.font = targetNewTMPFont;
                    modified = true;
                }
            }


            if (modified)
            {
                PrefabUtility.SaveAsPrefabAsset(prefab, prefabPath);
            }

            return modified;
        }
    }
}