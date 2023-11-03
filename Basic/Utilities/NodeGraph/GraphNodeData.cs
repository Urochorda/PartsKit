using UnityEngine;
using UnityEngine.UIElements;

namespace PartsKit
{
    public class GraphNodeData
    {
        public virtual string NodeName => GetType().Name;
        public virtual Color NodeColor => Color.clear;
        public virtual StyleSheet LayoutStyle => null;
        public virtual bool Deletable => true;

        public string Guid { get; set; }
        public Rect Rect { get; set; }
    }
}