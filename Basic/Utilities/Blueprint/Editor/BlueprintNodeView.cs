using UnityEditor.Experimental.GraphView;

namespace PartsKit
{
    public class BlueprintNodeView : Node
    {
        private BlueprintNode blueprintNode;

        public void Init(BlueprintNode blueprintNodeVal)
        {
            blueprintNode = blueprintNodeVal;
            InitPorts();
        }

        private void InitPorts()
        {
            // todo BlueprintPortView
            foreach (var inputPort in blueprintNode.InputPort)
            {
                BlueprintPortView portView = BlueprintPortView.CreatePortView(inputPort);
                inputContainer.Add(portView);
            }

            foreach (var outputPort in blueprintNode.OutputPort)
            {
                BlueprintPortView portView = BlueprintPortView.CreatePortView(outputPort);
                outputContainer.Add(portView);
            }
        }
    }
}