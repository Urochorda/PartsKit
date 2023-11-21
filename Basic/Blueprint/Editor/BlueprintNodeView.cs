using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace PartsKit
{
    public class BlueprintNodeView : Node
    {
        public BlueprintNode BlueprintNode { get; private set; }
        public BlueprintView OwnerView { get; private set; }
        public SerializedObject SerializedObject => OwnerView.SerializedObject;

        public virtual void Init(BlueprintNode blueprintNodeVal, BlueprintView ownerViewVal)
        {
            OwnerView = ownerViewVal;
            BlueprintNode = blueprintNodeVal;
            InitPorts();
            title = blueprintNodeVal.NodeName;
            viewDataKey = blueprintNodeVal.Guid;
            SetPosition(blueprintNodeVal.Rect);
            SetColor(blueprintNodeVal.NodeColor);

            if (!blueprintNodeVal.Deletable)
                capabilities &= ~Capabilities.Deletable;
        }

        private void InitPorts()
        {
            foreach (var inputPort in BlueprintNode.InputPorts)
            {
                BlueprintPortView portView = InstantiateBlueprintPort(inputPort);
                inputContainer.Add(portView);
            }

            foreach (var outputPort in BlueprintNode.OutputPorts)
            {
                BlueprintPortView portView = InstantiateBlueprintPort(outputPort);
                outputContainer.Add(portView);
            }
        }

        public SerializedProperty FindNodeProperty(string relativePropertyPath)
        {
            return OwnerView.FindNodeProperty(BlueprintNode, relativePropertyPath);
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

        protected virtual void SetColor(Color color)
        {
            if (color == Color.clear)
            {
                return;
            }

            StyleColor styleColor = new StyleColor(color);
            titleContainer.style.backgroundColor = styleColor;
        }
    }
}