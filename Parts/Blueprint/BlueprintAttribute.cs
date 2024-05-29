using System;

namespace PartsKit
{
    [AttributeUsage(AttributeTargets.Class)]
    public class BlueprintNodeCreateAttribute : Attribute
    {
        public string NodeGroup { get; }
        public string NodePath { get; }
        public string NodeName { get; }
        public string NodeHelp { get; }

        public BlueprintNodeCreateAttribute(string nodeGroup, string nodePath, string nodeName, string nodeHelp)
        {
            NodeGroup = nodeGroup;
            NodePath = nodePath;
            NodeName = nodeName;
            NodeHelp = nodeHelp;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class BlueprintParameterCreateAttribute : Attribute
    {
        public string ParameterGroup { get; }
        public string CreateName { get; }

        public BlueprintParameterCreateAttribute(string parameterGroup, string createName)
        {
            ParameterGroup = parameterGroup;
            CreateName = createName;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class BlueprintNodeTypeAttribute : Attribute
    {
        public Type NodeType { get; }

        public BlueprintNodeTypeAttribute(Type nodeType)
        {
            NodeType = nodeType;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ParameterFieldTypeAttribute : Attribute
    {
        public Type ParameterType { get; }

        public ParameterFieldTypeAttribute(Type parameterType)
        {
            ParameterType = parameterType;
        }
    }
}