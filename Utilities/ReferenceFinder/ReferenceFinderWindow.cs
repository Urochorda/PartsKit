#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor.IMGUI.Controls;

namespace PartsKit
{
    public class ReferenceFinderWindow : EditorWindow
    {
        //依赖模式的key
        const string isDependPrefKey = "ReferenceFinderData_IsDepend";

        private static ReferenceFinderData data = new ReferenceFinderData();
        private static bool initializedData = false;

        private bool isDepend = false;

        private bool needUpdateAssetTree = false;

        private bool initializedGUIStyle = false;

        //工具栏按钮样式
        private GUIStyle toolbarButtonGUIStyle;

        //工具栏样式
        private GUIStyle toolbarGUIStyle;

        //选中资源列表
        private List<string> selectedAssetGuid = new List<string>();

        private AssetTreeView m_AssetTreeView;

        [SerializeField] private TreeViewState m_TreeViewState;
        private SearchField m_searchField;

        //查找资源引用信息
        [MenuItem("Assets/Find References In Project %#&f", false, 25)]
        static void FindRef()
        {
            InitDataIfNeeded();
            OpenWindow();
            ReferenceFinderWindow window = GetWindow<ReferenceFinderWindow>();
            window.UpdateSelectedAssets();
        }

        //打开窗口
        [MenuItem("Window/Reference Finder", false, 1000)]
        static void OpenWindow()
        {
            ReferenceFinderWindow window = GetWindow<ReferenceFinderWindow>();
            window.wantsMouseMove = false;
            window.titleContent = new GUIContent("Ref Finder");
            window.Show();
            window.Focus();
        }

        //初始化数据
        static void InitDataIfNeeded()
        {
            if (!initializedData)
            {
                //初始化数据
                if (!data.ReadFromCache())
                {
                    data.CollectDependenciesInfo();
                }

                initializedData = true;
            }
        }

        //初始化GUIStyle
        void InitGUIStyleIfNeeded()
        {
            if (!initializedGUIStyle)
            {
                toolbarButtonGUIStyle = new GUIStyle("ToolbarButton");
                toolbarGUIStyle = new GUIStyle("Toolbar");
                initializedGUIStyle = true;
            }
        }

        //更新选中资源列表
        private void UpdateSelectedAssets()
        {
            selectedAssetGuid.Clear();
            string[] assetGUIDs = Selection.assetGUIDs;
            foreach (var obj in assetGUIDs)
            {
                string path = AssetDatabase.GUIDToAssetPath(obj);
                //如果是文件夹
                if (Directory.Exists(path))
                {
                    string[] folder = new string[] { path };
                    //将文件夹下所有资源作为选择资源
                    string[] guids = AssetDatabase.FindAssets(null, folder);
                    foreach (var guid in guids)
                    {
                        if (!selectedAssetGuid.Contains(guid) &&
                            !Directory.Exists(AssetDatabase.GUIDToAssetPath(guid)))
                        {
                            selectedAssetGuid.Add(guid);
                        }
                    }
                }
                //如果是文件资源
                else
                {
                    string guid = AssetDatabase.AssetPathToGUID(path);
                    selectedAssetGuid.Add(guid);
                }
            }

            needUpdateAssetTree = true;
        }

        private void OnEnable()
        {
            isDepend = PlayerPrefs.GetInt(isDependPrefKey, 0) == 1;
        }

        private void OnGUI()
        {
            InitGUIStyleIfNeeded();
            DrawOptionBar();
            DrawAssetTree();
        }

        //绘制上条
        public void DrawOptionBar()
        {
            EditorGUILayout.BeginHorizontal(toolbarGUIStyle);
            //刷新数据
            if (GUILayout.Button("Refresh Data", toolbarButtonGUIStyle))
            {
                data.CollectDependenciesInfo();
                needUpdateAssetTree = true;
                GUIUtility.ExitGUI();
            }

            //刷新数据
            if (GUILayout.Button("Refresh State", toolbarButtonGUIStyle))
            {
                needUpdateAssetTree = true;
            }

            //修改模式
            bool preIsDepend = isDepend;
            isDepend = GUILayout.Toggle(isDepend, isDepend ? "Model(Depend)" : "Model(Reference)",
                toolbarButtonGUIStyle,
                GUILayout.Width(120));
            if (preIsDepend != isDepend)
            {
                OnModelSelect();
            }

            if (m_searchField == null)
            {
                m_searchField = new SearchField();
            }

            if (m_AssetTreeView != null)
            {
                m_AssetTreeView.searchString = m_searchField.OnGUI(m_AssetTreeView.searchString);
            }
            else
            {
                m_searchField.OnGUI(string.Empty);
            }

            GUILayout.FlexibleSpace();

            //扩展
            if (GUILayout.Button("Expand", toolbarButtonGUIStyle))
            {
                if (m_AssetTreeView != null) m_AssetTreeView.ExpandAll();
            }

            //折叠
            if (GUILayout.Button("Collapse", toolbarButtonGUIStyle))
            {
                if (m_AssetTreeView != null) m_AssetTreeView.CollapseAll();
            }

            EditorGUILayout.EndHorizontal();
        }

        //通过选中资源列表绘制TreeView
        private void DrawAssetTree()
        {
            if (needUpdateAssetTree && selectedAssetGuid.Count != 0)
            {
                var root = SelectedAssetGuidToRootItem(selectedAssetGuid);
                if (m_AssetTreeView == null)
                {
                    //初始化TreeView
                    if (m_TreeViewState == null)
                        m_TreeViewState = new TreeViewState();
                    var headerState = AssetTreeView.CreateDefaultMultiColumnHeaderState(position.width);
                    var multiColumnHeader = new MultiColumnHeader(headerState);
                    m_AssetTreeView = new AssetTreeView(m_TreeViewState, multiColumnHeader);
                }

                m_AssetTreeView.assetRoot = root;
                m_AssetTreeView.Reload();
                needUpdateAssetTree = false;
            }

            if (m_AssetTreeView != null)
            {
                //绘制Treeview
                m_AssetTreeView.OnGUI(new Rect(0, toolbarGUIStyle.fixedHeight, position.width,
                    position.height - toolbarGUIStyle.fixedHeight));
            }
        }

        private void OnModelSelect()
        {
            needUpdateAssetTree = true;
            PlayerPrefs.SetInt(isDependPrefKey, isDepend ? 1 : 0);
        }


        //生成root相关
        private HashSet<string> updatedAssetSet = new HashSet<string>();

        //通过选择资源列表生成TreeView的根节点
        private AssetViewItem SelectedAssetGuidToRootItem(List<string> selectedAssetGuid)
        {
            updatedAssetSet.Clear();
            int elementCount = 0;
            var root = new AssetViewItem { id = elementCount, depth = -1, displayName = "Root", data = null };
            int depth = 0;
            var stack = new Stack<string>();
            foreach (var childGuid in selectedAssetGuid)
            {
                var child = CreateTree(childGuid, ref elementCount, depth, stack);
                if (child != null)
                    root.AddChild(child);
            }

            updatedAssetSet.Clear();
            return root;
        }

        //通过每个节点的数据生成子节点
        private AssetViewItem CreateTree(string guid, ref int elementCount, int _depth, Stack<string> stack)
        {
            if (stack.Contains(guid))
                return null;

            stack.Push(guid);
            if (!updatedAssetSet.Contains(guid))
            {
                data.UpdateAssetState(guid);
                updatedAssetSet.Add(guid);
            }

            ++elementCount;
            var referenceData = data.assetDict[guid];
            var root = new AssetViewItem
                { id = elementCount, displayName = referenceData.path, data = referenceData, depth = _depth };
            var childGuids = isDepend ? referenceData.dependencies : referenceData.references;
            foreach (var childGuid in childGuids)
            {
                var child = CreateTree(childGuid, ref elementCount, _depth + 1, stack);
                if (child != null)
                    root.AddChild(child);
            }

            stack.Pop();
            return root;
        }
    }
}

#endif