using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace PartsKit
{
    public class BlueprintNodeView : Node
    {
        public BlueprintNode BlueprintNode { get; private set; }

        public virtual void Init(BlueprintNode blueprintNodeVal)
        {
            BlueprintNode = blueprintNodeVal;
            InitPorts();
            title = blueprintNodeVal.NodeName;
            viewDataKey = blueprintNodeVal.Guid;
            SetPosition(blueprintNodeVal.Rect);
        }

        private void InitPorts()
        {
            foreach (var inputPort in BlueprintNode.InputPort)
            {
                BlueprintPortView portView = InstantiateBlueprintPort(inputPort);
                inputContainer.Add(portView);
            }

            foreach (var outputPort in BlueprintNode.OutputPort)
            {
                BlueprintPortView portView = InstantiateBlueprintPort(outputPort);
                outputContainer.Add(portView);
            }
        }

        public BlueprintPortView GetPortView(Direction portDirection, string portName)
        {
            List<BlueprintPortView> allPort = GetAllPort(portDirection);
            BlueprintPortView portView = allPort.Find(item => item.portName == portName);
            return portView;
        }

        public List<BlueprintPortView> GetAllPort(Direction portDirection)
        {
            switch (portDirection)
            {
                case Direction.Input:
                    return inputContainer.Query<BlueprintPortView>().ToList();
                case Direction.Output:
                    return outputContainer.Query<BlueprintPortView>().ToList();
                default:
                    throw new ArgumentOutOfRangeException(nameof(portDirection), portDirection, null);
            }
        }

        public BlueprintPortView InstantiateBlueprintPort(IBlueprintPort portData)
        {
            BlueprintPortView port = BlueprintPortView.Create<BlueprintEdgeView>(this, portData);
            return port;
        }
    }
}