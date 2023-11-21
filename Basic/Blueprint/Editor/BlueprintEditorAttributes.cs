using System;

namespace PartsKit
{
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