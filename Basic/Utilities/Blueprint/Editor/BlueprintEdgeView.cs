using UnityEditor.Experimental.GraphView;

namespace PartsKit
{
    public class BlueprintEdgeView : Edge
    {
        public BlueprintEdge BlueprintEdge { get; private set; }

        public virtual void Init(BlueprintEdge blueprintEdgeVal)
        {
            BlueprintEdge = blueprintEdgeVal;
        }
    }
}