using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Searcher;
using UnityEngine;
using UnityEngine.UIElements;

namespace PartsKit
{
    public class BlueprintWindow : EditorWindow
    {
        private BlueprintView blueprintView;

        private readonly List<SearcherItem> nodeSearcherItems = new List<SearcherItem>();

        private readonly List<BlueprintParameterCreateInfo> createParameterInfo =
            new List<BlueprintParameterCreateInfo>();

        protected virtual void OnDestroy()
        {
            DeInitBlueprintView();
        }

        #region Window

        /// <summary>
        /// 根据数据初始化窗口
        /// </summary>
        public void InitWindow(Blueprint blueprintVal)
        {
            InitBlueprintView(blueprintVal);
        }

        #endregion

        #region BlueprintView

        /// <summary>
        /// 根据数据初始化
        /// </summary>
        private void InitBlueprintView(Blueprint blueprintVal)
        {
            if (blueprintView != null)
            {
                DeInitBlueprintView();
            }

            blueprintView = OnCreateBlueprintView();
            if (blueprintView == null)
            {
                Debug.LogError("GraphView is Null!");
                return;
            }
#if UNITY_EDITOR
            blueprintVal.OnEditorReset += DeInitBlueprintView;
#endif
            OnInitBlueprintView(blueprintView, blueprintVal);
        }

        private void DeInitBlueprintView()
        {
            if (blueprintView == null)
            {
                return;
            }

#if UNITY_EDITOR
            blueprintView.Blueprint.OnEditorReset -= DeInitBlueprintView;
#endif
            blueprintView.DeInit();
            blueprintView.parent.Remove(blueprintView);
            blueprintView = null;
        }

        /// <summary>
        /// 创建BlueprintView，默认创建BlueprintView，如果扩展BlueprintView，可重写生成方法
        /// </summary>
        protected virtual BlueprintView OnCreateBlueprintView()
        {
            BlueprintView bv = new BlueprintView();
            rootVisualElement.Add(bv);
            bv.StretchToParentSize();
            return bv;
        }

        /// <summary>
        /// 初始化blueprintView
        /// </summary>
        protected virtual void OnInitBlueprintView(BlueprintView blueprintViewVal, Blueprint blueprintVal)
        {
            blueprintViewVal.Init(this, blueprintVal);
            CreateNodeSearcherItems();
            CreateParameterInfo();
            blueprintViewVal.OnGetNodeSearcherItems = () => nodeSearcherItems;
            blueprintViewVal.BlackboardView.OnGetCreateParameterInfo = () => createParameterInfo;
        }

        #endregion

        #region Create

        private void CreateNodeSearcherItems()
        {
            nodeSearcherItems.Clear();
            Blueprint blueprint = blueprintView.Blueprint;
            List<BlueprintNodeCreateInfo> createNodeInfos =
                BlueprintTypeCache.GetNodeCreateInfoByGroup(blueprint.NodeGroup);
            foreach (BlueprintNodeCreateInfo createNodeInfo in createNodeInfos)
            {
                CreateNodeSearcherItem(createNodeInfo);
            }
        }

        private void CreateNodeSearcherItem(BlueprintNodeCreateInfo createNodeInfo)
        {
            SearcherItem curSearcherItem;
            SetCurSearcherItem(null);

            string[] pathArray = createNodeInfo.NodePath.Split('/');
            foreach (var curPath in pathArray)
            {
                if (string.IsNullOrWhiteSpace(curPath))
                {
                    continue;
                }

                SearcherItem targetSearcherItem = GetCurSearcherChild(curPath);
                if (targetSearcherItem == null)
                {
                    targetSearcherItem = new SearcherItem(curPath, string.Empty, new List<SearcherItem>());
                    AddCurSearcherChild(targetSearcherItem);
                }

                SetCurSearcherItem(targetSearcherItem);
            }

            SearcherItem nodeSearcherItem = new SearcherItem(createNodeInfo.NodeName, createNodeInfo.NodeHelp,
                new List<SearcherItem>(), createNodeInfo.NodeType);
            AddCurSearcherChild(nodeSearcherItem);

            void SetCurSearcherItem(SearcherItem value)
            {
                curSearcherItem = value;
            }

            SearcherItem GetCurSearcherChild(string cName)
            {
                if (curSearcherItem == null)
                {
                    return nodeSearcherItems.Find(item => item.Name == cName);
                }

                return curSearcherItem.Children.Find(item => item.Name == cName);
            }

            void AddCurSearcherChild(SearcherItem value)
            {
                if (curSearcherItem == null)
                {
                    nodeSearcherItems.Add(value);
                    curSearcherItem = value;
                }
                else
                {
                    curSearcherItem.AddChild(value);
                }
            }
        }

        private void CreateParameterInfo()
        {
            createParameterInfo.Clear();
            Blueprint blueprint = blueprintView.Blueprint;
            List<BlueprintParameterCreateInfo> parameterCreateInfos =
                BlueprintTypeCache.GetParameterGroupCreateInfoByGroup(blueprint.ParameterGroup);
            createParameterInfo.AddRange(parameterCreateInfos);
        }

        #endregion
    }
}