using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace PartsKit
{
    public class FlowCreateNodeInfo : Attribute
    {
        public Type NodeType { get; set; }
        public string NodePath { get; }
        public string NodeName { get; }
        public string NodeHelp { get; }

        public FlowCreateNodeInfo(Type nodeType, string nodePath, string nodeName, string nodeHelp)
        {
            NodeType = nodeType;
            NodePath = nodePath;
            NodeName = nodeName;
            NodeHelp = nodeHelp;
        }
    }

    public class FlowTypeCache
    {
        private static Dictionary<Type, FlowCreateNodeInfo> NodeAttributePreType { get; } =
            new Dictionary<Type, FlowCreateNodeInfo>();

        static FlowTypeCache()
        {
            BuildScriptCache();
        }

        private static void BuildScriptCache()
        {
            NodeAttributePreType.Clear();
            TypeCache.TypeCollection typeCache = TypeCache.GetTypesDerivedFrom<BlueprintNode>();
            AddNodeAttributeScriptAsset(typeof(BlueprintNode));
            foreach (var nodeType in typeCache)
            {
                AddNodeAttributeScriptAsset(nodeType);
            }
        }

        private static void AddNodeAttributeScriptAsset(Type type)
        {
            if (type.IsAbstract)
            {
                return;
            }

            if (type.GetCustomAttributes(typeof(FlowCreateNodeAttribute), false) is not FlowCreateNodeAttribute[] attrs)
            {
                return;
            }

            if (attrs.Length <= 0)
            {
                return;
            }

            FlowCreateNodeAttribute attribute = attrs.First();
            NodeAttributePreType[type] =
                new FlowCreateNodeInfo(type, attribute.NodePath, attribute.NodeName, attribute.NodeHelp);
        }

        public static FlowCreateNodeInfo GetNodeAttribute(Type nodeType)
        {
            if (NodeAttributePreType.TryGetValue(nodeType, out FlowCreateNodeInfo info))
            {
                return info;
            }

            return null;
        }

        public static List<FlowCreateNodeInfo> GetAllNodeAttribute()
        {
            return NodeAttributePreType.Values.ToList();
        }
    }
}