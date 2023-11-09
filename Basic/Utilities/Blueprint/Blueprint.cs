using System;
using System.Collections.Generic;
using UnityEngine;

namespace PartsKit
{
    [CreateAssetMenu(menuName = "PartsKit/Blueprint/Blueprint", fileName = "Blueprint_")]
    public class Blueprint : ScriptableObject
    {
        //可能有派生类，所以用SerializeReference
        [field: SerializeReference] public List<BlueprintNode> Nodes { get; private set; } = new List<BlueprintNode>();
        [field: SerializeReference] public List<BlueprintEdge> Edges { get; private set; } = new List<BlueprintEdge>();

        private void OnEnable()
        {
            InitNode();
            InitPort();
        }

        private void InitNode()
        {
            Nodes.RemoveAll(item =>
            {
                if (item == null || string.IsNullOrEmpty(item.Guid))
                {
                    Debug.LogError("Add Node Err");
                    return true;
                }

                return false;
            });

            foreach (BlueprintNode node in Nodes)
            {
                node.Init();
            }
        }

        private void InitPort()
        {
            Edges.RemoveAll(item =>
            {
                if (item == null || string.IsNullOrEmpty(item.Guid))
                {
                    Debug.LogError("Edge Guid Err");
                    return true;
                }

                BlueprintNode inputNode = GetNode(item.InputNodeGuid);
                BlueprintNode outputNode = GetNode(item.OutputNodeGuid);
                if (inputNode == null || outputNode == null)
                {
                    Debug.LogError("Edge Node Err");
                    return true;
                }

                IBlueprintPort inputPort = inputNode.GetPort(IBlueprintPort.Direction.Input, item.InputPortName);
                IBlueprintPort outputPort = outputNode.GetPort(IBlueprintPort.Direction.Output, item.OutputPortName);
                if (inputPort == null || outputPort == null)
                {
                    Debug.LogError("Edge Port Err");
                    return true;
                }

                return false;
            });
        }

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

        public void RemoveNode(BlueprintNode treeNode)
        {
            if (treeNode == null)
            {
                return;
            }

            Nodes.Remove(treeNode);
        }

        public BlueprintNode GetNode(string guid)
        {
            return Nodes.Find(item => item != null && item.Guid == guid);
        }

        public void AddEdge(BlueprintEdge edge)
        {
            //todo 校验
            Edges.Add(edge);
        }

        public void RemoveEdge(BlueprintEdge edge)
        {
            Edges.Remove(edge);
        }
    }
}