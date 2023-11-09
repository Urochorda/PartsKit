using System.Collections.Generic;
using UnityEditor.Searcher;

namespace PartsKit
{
    public class BlueprintViewTest : BlueprintView
    {
        private readonly List<SearcherItem> nodeSearcherItems = new List<SearcherItem>();

        public BlueprintViewTest()
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
            List<SearcherItem> searcherItems = new List<SearcherItem>
            {
                new SearcherItem("Nodes", "测试用Node列表", new List<SearcherItem>
                {
                    new SearcherItem("InOutPutTest", userData: typeof(BlueprintNodeInOutPutTest)),
                    new SearcherItem("GameObjectTest", userData: typeof(BlueprintNodeGameObjectTest)),
                })
            };

            nodeSearcherItems.AddRange(searcherItems);
        }
    }
}