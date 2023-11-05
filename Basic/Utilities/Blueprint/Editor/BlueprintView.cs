using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace PartsKit
{
    public class BlueprintView : GraphView
    {
        private Blueprint blueprint;

        public void Init(Blueprint blueprintVal)
        {
            blueprint = blueprintVal;
        }

        public void SaveGraphData()
        {
            if (blueprint == null)
            {
                return;
            }

            EditorUtility.SetDirty(blueprint);
            AssetDatabase.SaveAssets();
        }
    }
}