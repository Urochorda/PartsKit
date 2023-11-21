using System.Collections.Generic;
using UnityEditor.Searcher;

namespace PartsKit
{
    public class FlowTreeWindow : BlueprintWindow
    {
        private readonly List<SearcherItem> nodeSearcherItems = new List<SearcherItem>();

        private readonly List<BlueprintCreateParameterInfo> createParameterInfo =
            new List<BlueprintCreateParameterInfo>();

        /// <summary>
        /// 初始化blueprintView
        /// </summary>
        protected override void OnInitBlueprintView(BlueprintView blueprintViewVal, Blueprint blueprintVal)
        {
            CreateNodeSearcherItems();
            CreateParameterInfo();
            blueprintViewVal.Init(this, blueprintVal);
            blueprintViewVal.OnGetNodeSearcherItems = () => nodeSearcherItems;
            blueprintViewVal.BlackboardView.OnGetCreateParameterInfo = () => createParameterInfo;
        }

        private void CreateNodeSearcherItems()
        {
            nodeSearcherItems.Clear();

            List<FlowCreateNodeInfo> createNodeInfos = FlowTypeCache.GetAllNodeAttribute();
            foreach (FlowCreateNodeInfo createNodeInfo in createNodeInfos)
            {
                CreateNodeSearcherItem(createNodeInfo);
            }
        }

        private void CreateNodeSearcherItem(FlowCreateNodeInfo createNodeInfo)
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
            createParameterInfo.AddRange(FlowTypeCache.GetAllCreateParameterInfo());
        }
    }
}