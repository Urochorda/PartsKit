using System.Collections.Generic;
using UnityEditor.Searcher;

namespace PartsKit
{
    public class FlowTreeView : BlueprintView
    {
        private readonly List<SearcherItem> nodeSearcherItems = new List<SearcherItem>();

        public FlowTreeView()
        {
            CreateNodeSearcherItems();
        }

        protected override List<SearcherItem> GetNodeSearcherItems()
        {
            return nodeSearcherItems;
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
                    return null;
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
    }
}