using System;

namespace PartsKit
{
    [AttributeUsage(AttributeTargets.Class)]
    public class BlueprintNodeTypeAttribute : Attribute
    {
        public Type NodeType { get; set; }

        public BlueprintNodeTypeAttribute(Type nodeType)
        {
            NodeType = nodeType;
        }
    }
}