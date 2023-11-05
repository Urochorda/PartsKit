using System.Collections.Generic;
using UnityEngine;

namespace PartsKit
{
    public class Blueprint : ScriptableObject
    {
        //所有在Graph中的节点
        [field: SerializeReference] public List<BlueprintNode> Nodes { get; set; } = new List<BlueprintNode>();

        public void AddNode(BlueprintNode treeNode)
        {
            if (treeNode == null || Nodes.Exists(item => item.Guid == treeNode.Guid))
            {
                Debug.LogError("Add Node Err");
                return;
            }

            Nodes.Add(treeNode);
            treeNode.Init();
        }
    }
}