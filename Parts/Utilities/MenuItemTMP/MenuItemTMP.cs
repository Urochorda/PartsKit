using TMPro;
using UnityEditor;
using UnityEngine;

namespace PartsKit
{
    public static class MenuItemTMP
    {
        [MenuItem("Assets/TMP: UpdateAtlasAndMatName")]
        private static void UpdateAtlasAndMatName()
        {
            // 第一阶段：直接选中的字体资源
            var directSelections = Selection.GetFiltered<Object>(SelectionMode.Assets);
            foreach (var obj in directSelections)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                //文件夹
                if (AssetDatabase.IsValidFolder(path))
                {
                    ProcessFolderFonts(path);
                }
                else if (obj is TMP_FontAsset font) //字体
                {
                    ProcessSingleFont(font);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void ProcessFolderFonts(string folderPath)
        {
            // 深度搜索所有子文件夹
            string[] fontGUIDs = AssetDatabase.FindAssets("t:TMP_FontAsset", new[] { folderPath });

            foreach (string guid in fontGUIDs)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(assetPath);
                if (font != null)
                {
                    ProcessSingleFont(font);
                }
            }
        }

        private static void ProcessSingleFont(TMP_FontAsset font)
        {
            Debug.Log($"UpdateAtlasAndMatName: {font.name}");
            if (font.material != null)
            {
                string expectedName = $"{font.name} Material";
                string matPath = AssetDatabase.GetAssetPath(font.material);

                if (AssetDatabase.IsSubAsset(font.material))
                {
                    font.material.name = expectedName;
                }
                else
                {
                    AssetDatabase.RenameAsset(matPath, expectedName);
                }
            }

            if (font.atlasTexture != null)
            {
                string expectedName = $"{font.name} Atlas";
                string atlPath = AssetDatabase.GetAssetPath(font.atlasTexture);

                if (AssetDatabase.IsSubAsset(font.atlasTexture))
                {
                    font.atlasTexture.name = expectedName;
                }
                else
                {
                    AssetDatabase.RenameAsset(atlPath, expectedName);
                }
            }

            EditorUtility.SetDirty(font);
        }
    }
}