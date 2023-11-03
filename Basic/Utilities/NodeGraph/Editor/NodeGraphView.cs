using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace PartsKit
{
    public class NodeGraphView : GraphView
    {
        private GraphData graphData;
        public void Init(GraphData graphDataVal)
        {
            graphData = graphDataVal;
        }
        
        public void SaveGraphData()
        {
            if (graphData == null)
            {
                return;
            }
            
            EditorUtility.SetDirty(graphData);
        }
    }
}