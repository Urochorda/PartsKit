using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;
using UnityEditor.AnimatedValues;
#endif

namespace PartsKit
{
    public enum CustomImageType
    {
        None = 0,
        Grid = 1,
    }

    [AddComponentMenu("UI/CustomImage")]
    public class CustomImage : Image
    {
        [Serializable]
        public class GridColor
        {
            [SerializeField] private int x;
            [SerializeField] private int y;
            [SerializeField] private Color color;

            public int X
            {
                get => x;
                set => x = value;
            }

            public int Y
            {
                get => y;
                set => y = value;
            }

            public Color Color
            {
                get => color;
                set => color = value;
            }

            public GridColor()
            {
                x = 0;
                Y = 0;
                color = Color.white;
            }

            public GridColor(int xVal, int yVal, Color colorVal)
            {
                x = xVal;
                Y = yVal;
                color = colorVal;
            }
        }

        [SerializeField] private CustomImageType customType;
        [SerializeField] private int tilesX = 1; // X方向的平铺数量
        [SerializeField] private int tilesY = 1; // Y方向的平铺数量
        [SerializeField] private List<GridColor> tileColor; // 平铺项的颜色

        public CustomImageType CustomType
        {
            get => customType;
            set
            {
                customType = value;
                SetVerticesDirty(); // 每次修改后刷新
            }
        }

        public int TilesX
        {
            get => tilesX;
            set
            {
                tilesX = value;
                SetVerticesDirty(); // 每次修改后刷新
            }
        }

        public int TilesY
        {
            get => tilesY;
            set
            {
                tilesY = value;
                SetVerticesDirty(); // 每次修改后刷新
            }
        }

        public IReadOnlyList<GridColor> TileColor => tileColor;

        public void AddTileColor(int x, int y, Color tColor)
        {
            tileColor.Add(new GridColor(x, y, tColor));
            SetVerticesDirty(); // 刷新
        }

        public void AddTileColor(GridColor gColor)
        {
            if (gColor == null)
            {
                return;
            }

            tileColor.Add(gColor);
            SetVerticesDirty(); // 刷新
        }

        public void RemoveTileColor(int x, int y)
        {
            int colorIndex = tileColor.FindIndex(item => item.X == x && item.Y == y);
            if (colorIndex >= 0)
            {
                tileColor.RemoveAt(colorIndex);
                SetVerticesDirty(); // 刷新
            }
        }

        public void ClearTileColors()
        {
            tileColor.Clear();
            SetVerticesDirty(); // 刷新
        }

        public void SetTileColor(List<GridColor> gridColors)
        {
            ClearTileColors();
            if (gridColors == null)
            {
                return;
            }

            foreach (var gridColor in gridColors)
            {
                AddTileColor(gridColor);
            }
        }

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            if (overrideSprite == null)
            {
                base.OnPopulateMesh(toFill);
                return;
            }

            switch (customType)
            {
                default:
                case CustomImageType.None:
                    base.OnPopulateMesh(toFill);
                    break;
                case CustomImageType.Grid:
                    PopulateGrid(toFill);
                    break;
            }
        }

        private void AddQuad(VertexHelper vh, Vector2 posMin, Vector2 posMax, float uvMinX, float uvMinY, float uvMaxX,
            float uvMaxY, Color uvColor)
        {
            int vertIndex = vh.currentVertCount;

            UIVertex vert = UIVertex.simpleVert;
            vert.color = uvColor;

            // 左下角
            vert.position = new Vector3(posMin.x, posMin.y);
            vert.uv0 = new Vector2(uvMinX, uvMinY);
            vh.AddVert(vert);

            // 左上角
            vert.position = new Vector3(posMin.x, posMax.y);
            vert.uv0 = new Vector2(uvMinX, uvMaxY);
            vh.AddVert(vert);

            // 右上角
            vert.position = new Vector3(posMax.x, posMax.y);
            vert.uv0 = new Vector2(uvMaxX, uvMaxY);
            vh.AddVert(vert);

            // 右下角
            vert.position = new Vector3(posMax.x, posMin.y);
            vert.uv0 = new Vector2(uvMaxX, uvMinY);
            vh.AddVert(vert);

            // 创建两个三角形
            vh.AddTriangle(vertIndex, vertIndex + 1, vertIndex + 2);
            vh.AddTriangle(vertIndex + 2, vertIndex + 3, vertIndex);
        }

        private void PopulateGrid(VertexHelper toFill)
        {
            Vector4 outer = UnityEngine.Sprites.DataUtility.GetOuterUV(overrideSprite);
            Vector2 spriteSize = new Vector2(overrideSprite.rect.width, overrideSprite.rect.height);
            Vector4 border = overrideSprite.border; // 获取九宫格边距

            // 获取图片的尺寸和 RectTransform 的尺寸
            var rect = rectTransform.rect;
            Vector2 imageSize = new Vector2(rect.width, rect.height);

            // 清除之前的 Mesh 数据
            toFill.Clear();

            int xTiles;
            int yTiles;
            if (tilesX < 0)
            {
                float scaleX = imageSize.x / Mathf.Ceil(imageSize.x / spriteSize.x);
                xTiles = Mathf.CeilToInt(imageSize.x / scaleX);
            }
            else
            {
                xTiles = tilesX;
            }

            if (tilesY < 0)
            {
                float scaleY = imageSize.y / Mathf.Ceil(imageSize.y / spriteSize.y);
                yTiles = Mathf.CeilToInt(imageSize.y / scaleY);
            }
            else
            {
                yTiles = tilesY;
            }

            float tileWidth = imageSize.x / xTiles;
            float tileHeight = imageSize.y / yTiles;

            // 起始点调整：将坐标从左下角移动到 RectTransform 的左上角
            var pivot = rectTransform.pivot;
            float startX = -pivot.x * imageSize.x;
            float startY = (1 - pivot.y) * imageSize.y;

            // 获取九宫格的边框值
            float left = border.x;
            float right = border.z;
            float top = border.w;
            float bottom = border.y;

            // 在每个平铺单元中应用九宫格处理
            for (int i = 0; i < xTiles; i++)
            {
                for (int j = 0; j < yTiles; j++)
                {
                    // 当前平铺单元的左下角和右上角坐标
                    float xPos = startX + i * tileWidth;
                    float yPos = startY - j * tileHeight;

                    Vector2 outerMin = new Vector2(xPos, yPos - tileHeight);
                    Vector2 outerMax = new Vector2(xPos + tileWidth, yPos);
                    Vector2 innerMin = new Vector2(outerMin.x + left, outerMin.y + bottom);
                    Vector2 innerMax = new Vector2(outerMax.x - right, outerMax.y - top);

                    //计算颜色
                    int overColorIndex = tileColor.FindIndex(item => item.X == i && item.Y == j);
                    Color uvColor = overColorIndex >= 0 ? tileColor[overColorIndex].Color : color;

                    // 对每个平铺单元内应用九宫格
                    // 左下角
                    AddQuad(toFill, new Vector2(outerMin.x, outerMin.y), new Vector2(innerMin.x, innerMin.y),
                        outer.x, outer.y, outer.x + left / spriteSize.x, outer.y + bottom / spriteSize.y, uvColor);

                    // 右下角
                    AddQuad(toFill, new Vector2(innerMax.x, outerMin.y), new Vector2(outerMax.x, innerMin.y),
                        outer.z - right / spriteSize.x, outer.y, outer.z, outer.y + bottom / spriteSize.y, uvColor);

                    // 左上角
                    AddQuad(toFill, new Vector2(outerMin.x, innerMax.y), new Vector2(innerMin.x, outerMax.y),
                        outer.x, outer.w - top / spriteSize.y, outer.x + left / spriteSize.x, outer.w, uvColor);

                    // 右上角
                    AddQuad(toFill, new Vector2(innerMax.x, innerMax.y), new Vector2(outerMax.x, outerMax.y),
                        outer.z - right / spriteSize.x, outer.w - top / spriteSize.y, outer.z, outer.w, uvColor);

                    // 上边
                    AddQuad(toFill, new Vector2(innerMin.x, innerMax.y), new Vector2(innerMax.x, outerMax.y),
                        outer.x + left / spriteSize.x, outer.w - top / spriteSize.y, outer.z - right / spriteSize.x,
                        outer.w, uvColor);

                    // 下边
                    AddQuad(toFill, new Vector2(innerMin.x, outerMin.y), new Vector2(innerMax.x, innerMin.y),
                        outer.x + left / spriteSize.x, outer.y, outer.z - right / spriteSize.x,
                        outer.y + bottom / spriteSize.y, uvColor);

                    // 左边
                    AddQuad(toFill, new Vector2(outerMin.x, innerMin.y), new Vector2(innerMin.x, innerMax.y),
                        outer.x, outer.y + bottom / spriteSize.y, outer.x + left / spriteSize.x,
                        outer.w - top / spriteSize.y, uvColor);

                    // 右边
                    AddQuad(toFill, new Vector2(innerMax.x, innerMin.y), new Vector2(outerMax.x, innerMax.y),
                        outer.z - right / spriteSize.x, outer.y + bottom / spriteSize.y, outer.z,
                        outer.w - top / spriteSize.y, uvColor);

                    // 中间平铺区域
                    AddQuad(toFill, new Vector2(innerMin.x, innerMin.y), new Vector2(innerMax.x, innerMax.y),
                        outer.x + left / spriteSize.x, outer.y + bottom / spriteSize.y, outer.z - right / spriteSize.x,
                        outer.w - top / spriteSize.y, uvColor);
                }
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CustomImage))]
    public class CustomImageEditor : ImageEditor
    {
        SerializedProperty typeProperty;
        SerializedProperty tilesXProperty;
        SerializedProperty tilesYProperty;
        SerializedProperty tileColorProperty;

        private AnimBool showTilesOptions; // 控制 tilesX 和 tilesY 的显示状态

        protected override void OnEnable()
        {
            base.OnEnable();
            // 获取 tilesX、tilesY 和 customType 的序列化属性
            tilesXProperty = serializedObject.FindProperty("tilesX");
            tilesYProperty = serializedObject.FindProperty("tilesY");
            typeProperty = serializedObject.FindProperty("customType");
            tileColorProperty = serializedObject.FindProperty("tileColor");

            // 初始化 AnimBool
            showTilesOptions = new AnimBool(typeProperty.enumValueIndex == (int)CustomImageType.Grid);
            showTilesOptions.valueChanged.AddListener(Repaint); // 监听状态变化以刷新界面
        }

        public override void OnInspectorGUI()
        {
            // 绘制原有的 Inspector 内容
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            // 绘制 CustomImageType 字段
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(typeProperty);

            // 检查 CustomImageType 是否有变化，并更新 AnimBool
            if (EditorGUI.EndChangeCheck())
            {
                showTilesOptions.target = typeProperty.enumValueIndex == (int)CustomImageType.Grid;
            }

            // 显示 tilesX 和 tilesY 的字段
            if (EditorGUILayout.BeginFadeGroup(showTilesOptions.faded))
            {
                EditorGUI.indentLevel++; // 增加缩进级别
                EditorGUILayout.PropertyField(tilesXProperty);
                EditorGUILayout.PropertyField(tilesYProperty);
                EditorGUILayout.PropertyField(tileColorProperty);
                EditorGUI.indentLevel--; // 恢复缩进级别
            }

            EditorGUILayout.EndFadeGroup();

            // 如果参数有改动，标记为修改以更新
            if (serializedObject.ApplyModifiedProperties())
            {
                ((CustomImage)target).SetVerticesDirty(); // 更新网格
            }
        }
    }
#endif
}