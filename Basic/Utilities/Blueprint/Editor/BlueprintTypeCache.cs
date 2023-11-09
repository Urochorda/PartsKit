using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace PartsKit
{
    public static class BlueprintTypeCache
    {
        private static Dictionary<Type, Type> NodeViewPerType { get; } = new Dictionary<Type, Type>();

        static BlueprintTypeCache()
        {
            BuildScriptCache();
        }

        private static void BuildScriptCache()
        {
            NodeViewPerType.Clear();
            foreach (var nodeViewType in TypeCache.GetTypesDerivedFrom<BlueprintNodeView>())
            {
                if (!nodeViewType.IsAbstract)
                    AddNodeViewScriptAsset(nodeViewType);
            }
        }

        private static void AddNodeViewScriptAsset(Type type)
        {
            if (type.GetCustomAttributes(typeof(BlueprintNodeTypeAttribute), false) is not BlueprintNodeTypeAttribute[]
                attrs)
            {
                return;
            }

            if (attrs.Length <= 0)
            {
                return;
            }

            Type nodeType = attrs.First().NodeType;
            NodeViewPerType[nodeType] = type;
        }

        public static Type GetNodeViewType(Type nodeType)
        {
            if (NodeViewPerType.TryGetValue(nodeType, out Type nodeViewType))
            {
                return nodeViewType;
            }

            return typeof(BlueprintNodeView);
        }
    }
}