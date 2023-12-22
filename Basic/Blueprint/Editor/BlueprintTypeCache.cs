using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace PartsKit
{
    public class BlueprintCreateParameterInfo
    {
        public string CreateName { get; }
        public Type ParameterType { get; }

        public BlueprintCreateParameterInfo(string createNameVal, Type parameterTypeVal)
        {
            CreateName = createNameVal;
            ParameterType = parameterTypeVal;
        }
    }

    public static class BlueprintTypeCache
    {
        private static Dictionary<Type, Type> NodeViewPerType { get; } = new Dictionary<Type, Type>();
        private static Dictionary<Type, Type> ParameterFieldPerType { get; } = new Dictionary<Type, Type>();

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
    }
}