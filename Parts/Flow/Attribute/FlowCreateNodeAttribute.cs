using System;

namespace PartsKit
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FlowCreateNodeAttribute : Attribute
    {
        public string NodePath { get; }
        public string NodeName { get; }
        public string NodeHelp { get; }

        public FlowCreateNodeAttribute(string nodePath, string nodeName, string nodeHelp)
        {
            NodePath = nodePath;
            NodeName = nodeName;
            NodeHelp = nodeHelp;
        }
    }
}