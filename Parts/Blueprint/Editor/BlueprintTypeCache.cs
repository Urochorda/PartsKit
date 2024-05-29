using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace PartsKit
{
    public class BlueprintNodeCreateInfo
    {
        public Type NodeType { get; }
        public string NodeGroup { get; }
        public string NodePath { get; }
        public string NodeName { get; }
        public string NodeHelp { get; }

        public BlueprintNodeCreateInfo(Type nodeType, string nodeGroup, string nodePath, string nodeName,
            string nodeHelp)
        {
            NodeType = nodeType;
            NodeGroup = nodeGroup;
            NodePath = nodePath;
            NodeName = nodeName;
            NodeHelp = nodeHelp;
        }
    }

    public class BlueprintParameterCreateInfo
    {
        public string ParameterGroup { get; }
        public string CreateName { get; }
        public Type ParameterType { get; }

        public BlueprintParameterCreateInfo(string parameterGroup, string createNameVal, Type parameterTypeVal)
        {
            ParameterGroup = parameterGroup;
            CreateName = createNameVal;
            ParameterType = parameterTypeVal;
        }
    }

    public static class BlueprintTypeCache
    {
        private static Dictionary<Type, Type> NodeViewPerType { get; } = new Dictionary<Type, Type>();
        private static Dictionary<Type, Type> ParameterFieldPerType { get; } = new Dictionary<Type, Type>();

        private static Dictionary<Type, BlueprintNodeCreateInfo> NodeAttributePreType { get; } =
            new Dictionary<Type, BlueprintNodeCreateInfo>();

        private static Dictionary<Type, BlueprintParameterCreateInfo> CreateParameterInfo { get; } =
            new Dictionary<Type, BlueprintParameterCreateInfo>();

        static BlueprintTypeCache()
        {
            BuildScriptCache();
        }

        private static void BuildScriptCache()
        {
            {
                NodeViewPerType.Clear();
                TypeCache.TypeCollection typeCollection = TypeCache.GetTypesDerivedFrom<BlueprintNodeView>();
                AddNodeViewScriptAsset(typeof(BlueprintNodeView));
                foreach (var nodeViewType in typeCollection)
                {
                    AddNodeViewScriptAsset(nodeViewType);
                }
            }

            {
                ParameterFieldPerType.Clear();
                TypeCache.TypeCollection typeCollection = TypeCache.GetTypesDerivedFrom<BlueprintBlackboardField>();
                AddParameterFieldScriptAsset(typeof(BlueprintBlackboardField));
                foreach (var nodeViewType in typeCollection)
                {
                    AddParameterFieldScriptAsset(nodeViewType);
                }
            }

            {
                NodeAttributePreType.Clear();
                TypeCache.TypeCollection typeCache = TypeCache.GetTypesDerivedFrom<BlueprintNode>();
                AddNodeAttributeScriptAsset(typeof(BlueprintNode));
                foreach (var nodeType in typeCache)
                {
                    AddNodeAttributeScriptAsset(nodeType);
                }
            }

            {
                CreateParameterInfo.Clear();
                TypeCache.TypeCollection typeCache = TypeCache.GetTypesDerivedFrom<IBlueprintParameter>();
                foreach (var nodeType in typeCache)
                {
                    AddParameterAttributeScriptAsset(nodeType);
                }
            }
        }

        private static void AddNodeViewScriptAsset(Type type)
        {
            if (type.IsAbstract)
            {
                return;
            }

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

        private static void AddParameterFieldScriptAsset(Type type)
        {
            if (type.IsAbstract)
            {
                return;
            }

            if (type.GetCustomAttributes(typeof(ParameterFieldTypeAttribute), false) is not ParameterFieldTypeAttribute
                []
                attrs)
            {
                return;
            }

            if (attrs.Length <= 0)
            {
                return;
            }

            Type nodeType = attrs.First().ParameterType;
            ParameterFieldPerType[nodeType] = type;
        }

        public static Type GetParameterFieldType(Type nodeType)
        {
            if (ParameterFieldPerType.TryGetValue(nodeType, out Type parameterFieldType))
            {
                return parameterFieldType;
            }

            return typeof(BlueprintBlackboardField);
        }

        private static void AddNodeAttributeScriptAsset(Type type)
        {
            if (type.IsAbstract)
            {
                return;
            }

            if (type.GetCustomAttributes(typeof(BlueprintNodeCreateAttribute), false) is not
                BlueprintNodeCreateAttribute[] attrs)
            {
                return;
            }

            if (attrs.Length <= 0)
            {
                return;
            }

            BlueprintNodeCreateAttribute attribute = attrs.First();
            NodeAttributePreType[type] =
                new BlueprintNodeCreateInfo(type, attribute.NodeGroup, attribute.NodePath, attribute.NodeName,
                    attribute.NodeHelp);
        }

        public static BlueprintNodeCreateInfo GetNodeCreateInfoByType(Type nodeType)
        {
            if (NodeAttributePreType.TryGetValue(nodeType, out BlueprintNodeCreateInfo info))
            {
                return info;
            }

            return null;
        }

        public static List<BlueprintNodeCreateInfo> GetNodeCreateInfoByGroup(List<string> nodeGroup)
        {
            List<BlueprintNodeCreateInfo> nodeInfos = new List<BlueprintNodeCreateInfo>();
            if (nodeGroup == null || nodeGroup.Count <= 0)
            {
                return nodeInfos;
            }

            foreach (KeyValuePair<Type, BlueprintNodeCreateInfo> keyValuePair in NodeAttributePreType)
            {
                if (nodeGroup.Contains(keyValuePair.Value.NodeGroup))
                {
                    nodeInfos.Add(keyValuePair.Value);
                }
            }

            return nodeInfos;
        }

        public static List<BlueprintNodeCreateInfo> GetNodeCreateInfoAll()
        {
            return NodeAttributePreType.Values.ToList();
        }

        private static void AddParameterAttributeScriptAsset(Type type)
        {
            if (type.IsAbstract)
            {
                return;
            }

            if (type.GetCustomAttributes(typeof(BlueprintParameterCreateAttribute), false) is not
                BlueprintParameterCreateAttribute
                [] attrs)
            {
                return;
            }

            if (attrs.Length <= 0)
            {
                return;
            }

            BlueprintParameterCreateAttribute attribute = attrs.First();
            BlueprintParameterCreateInfo parameterInfo =
                new BlueprintParameterCreateInfo(attribute.ParameterGroup, attribute.CreateName, type);
            CreateParameterInfo[type] = parameterInfo;
        }

        public static BlueprintParameterCreateInfo GetParameterCreateInfoByType(Type parameterType)
        {
            if (CreateParameterInfo.TryGetValue(parameterType, out BlueprintParameterCreateInfo info))
            {
                return info;
            }

            return null;
        }

        public static List<BlueprintParameterCreateInfo> GetParameterGroupCreateInfoByGroup(List<string> parameterGroup)
        {
            List<BlueprintParameterCreateInfo> parameterInfos = new List<BlueprintParameterCreateInfo>();
            if (parameterGroup == null || parameterGroup.Count <= 0)
            {
                return parameterInfos;
            }

            foreach (KeyValuePair<Type, BlueprintParameterCreateInfo> keyValuePair in CreateParameterInfo)
            {
                if (parameterGroup.Contains(keyValuePair.Value.ParameterGroup))
                {
                    parameterInfos.Add(keyValuePair.Value);
                }
            }

            return parameterInfos;
        }

        public static List<BlueprintParameterCreateInfo> GetParameterCreateInfoAll()
        {
            return CreateParameterInfo.Values.ToList();
        }
    }
}