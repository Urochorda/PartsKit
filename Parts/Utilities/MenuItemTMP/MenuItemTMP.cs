using TMPro;
using UnityEditor;
using UnityEngine;

namespace PartsKit
{
    public static class MenuItemTMP
    {
        [MenuItem("Assets/TMP: UpdateAtlasAndMat", true)]
        private static bool ValidateUpdateAtlasAndMat()
        {
            var selections = Selection.GetFiltered<Object>(SelectionMode.Assets);
            foreach (var obj in selections)
            {
                if (obj is TMP_FontAsset)
                {
                    return true;
                }

                string path = AssetDatabase.GetAssetPath(obj);
                if (AssetDatabase.IsValidFolder(path))
                {
                    string[] guids =
                        AssetDatabase.FindAssets("t:TMP_FontAsset", new[] { AssetDatabase.GetAssetPath(obj) });
                    if (guids.Length > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        [MenuItem("Assets/TMP: UpdateAtlasAndMat")]
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
            Debug.Log($"update atlas and mat: {font.name}");
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

            for (int i = 0; i < font.atlasTextures.Length; i++)
            {
                var atlas = font.atlasTextures[i];
                if (atlas != null)
                {
                    string expectedName = i == 0 ? $"{font.name} Atlas" : $"{font.name} Atlas {i}";
                    string atlPath = AssetDatabase.GetAssetPath(atlas);
                    if (AssetDatabase.IsSubAsset(atlas))
                    {
                        atlas.name = expectedName;
                    }
                    else
                    {
                        AssetDatabase.RenameAsset(atlPath, expectedName);
                    }
                }
            }

            Debug.Log($"clear orphan atlas: {font.name}");
            Object[] subAssets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(font));
            foreach (var sub in subAssets)
            {
                if (sub is Texture2D tex)
                {
                    bool exists = false;
                    foreach (var atlas in font.atlasTextures)
                    {
                        if (atlas == tex)
                        {
                            exists = true;
                            break;
                        }
                    }

                    if (!exists)
                    {
                        Debug.Log($"Deleting orphan atlas: {tex.name} in {font.name}");
                        Object.DestroyImmediate(tex, true);
                    }
                }
            }

            EditorUtility.SetDirty(font);
        }
    }
}