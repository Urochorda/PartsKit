using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PartsKit
{
    public static class BlueprintPortEditorUtilities
    {
        public static Orientation ToEditorOrientation(this IBlueprintPort.Orientation orientation)
        {
            switch (orientation)
            {
                default:
                case IBlueprintPort.Orientation.Horizontal:
                    return Orientation.Horizontal;
                case IBlueprintPort.Orientation.Vertical:
                    return Orientation.Vertical;
            }
        }

        public static Port.Capacity ToEditorCapacity(this IBlueprintPort.Capacity capacity)
        {
            switch (capacity)
            {
                default:
                case IBlueprintPort.Capacity.Multi:
                    return Port.Capacity.Multi;
                case IBlueprintPort.Capacity.Single:
                    return Port.Capacity.Single;
            }
        }

        public static Direction ToEditorDirection(this IBlueprintPort.Direction direction)
        {
            switch (direction)
            {
                default:
                case IBlueprintPort.Direction.Input:
                    return Direction.Input;
                case IBlueprintPort.Direction.Output:
                    return Direction.Output;
            }
        }
    }

    public class BlueprintEdgeConnectorListener : IEdgeConnectorListener
    {
        private readonly GraphViewChange mGraphViewChange;
        private readonly List<Edge> mEdgesToCreate;
        private readonly List<GraphElement> mEdgesToDelete;

        public BlueprintEdgeConnectorListener()
        {
            mEdgesToCreate = new List<Edge>();
            mEdgesToDelete = new List<GraphElement>();
            mGraphViewChange.edgesToCreate = mEdgesToCreate;
        }

        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
        }

        public void OnDrop(GraphView graphView, Edge edge)
        {
            mEdgesToCreate.Clear();
            mEdgesToCreate.Add(edge);
            mEdgesToDelete.Clear();
            if (edge.input.capacity == Port.Capacity.Single)
            {
                foreach (Edge connection in edge.input.connections)
                {
                    if (connection != edge)
                    {
                        mEdgesToDelete.Add(connection);
                    }
                }
            }

            if (edge.output.capacity == Port.Capacity.Single)
            {
                foreach (Edge connection in edge.output.connections)
                {
                    if (connection != edge)
                    {
                        mEdgesToDelete.Add(connection);
                    }
                }
            }

            if (mEdgesToDelete.Count > 0)
            {
                graphView.DeleteElements(mEdgesToDelete);
            }

            List<Edge> edgesToCreate = mEdgesToCreate;
            if (graphView.graphViewChanged != null)
            {
                edgesToCreate = graphView.graphViewChanged(mGraphViewChange).edgesToCreate;
            }

            foreach (Edge edge1 in edgesToCreate)
            {
                graphView.AddElement(edge1);
                edge.input.Connect(edge1);
                edge.output.Connect(edge1);
            }
        }
    }

    public class BlueprintPortView : Port
    {
        public static BlueprintPortView Create<TEdge>(BlueprintNodeView ownerNodeView, IBlueprintPort blueprintPort)
            where TEdge : Edge, new()
        {
            BlueprintEdgeConnectorListener listener = new BlueprintEdgeConnectorListener();
            BlueprintPortView ele = new BlueprintPortView(blueprintPort.PortOrientation.ToEditorOrientation(),
                blueprintPort.PortDirection.ToEditorDirection(), blueprintPort.PortCapacity.ToEditorCapacity(),
                blueprintPort.PortType)
            {
                m_EdgeConnector = new EdgeConnector<TEdge>(listener)
            };
            ele.AddManipulator(ele.m_EdgeConnector);
            ele.Init(ownerNodeView, blueprintPort);
            return ele;
        }

        public BlueprintNodeView OwnerView { get; private set; }
        public IBlueprintPort BlueprintPort { get; private set; }

        private BlueprintPortStyle? CurPortStyle { get; set; }
        private VisualElement propertyFieldElement;
        private bool execute;

        private BlueprintPortView(Orientation portOrientation, Direction portDirection, Capacity portCapacity,
            Type type) : base(portOrientation, portDirection, portCapacity, type)
        {
        }

        public virtual void Init(BlueprintNodeView ownerNodeView, IBlueprintPort blueprintPort)
        {
            OwnerView = ownerNodeView;
            BlueprintPort = blueprintPort;
            portName = blueprintPort.PortName;

            if (CurPortStyle != null)
            {
                styleSheets.Remove(CurPortStyle.Value.StyleSheet);
            }

            BlueprintPortStyle portStyle = BlueprintPortUtility.GetRegisterPortStyle(portType);
            CurPortStyle = portStyle;
            visualClass = portStyle.VisualClass;
            styleSheets.Add(portStyle.StyleSheet);
            portColor = portStyle.PortColor;
            AddPropertyField();
        }

        public virtual void SetExecuteState(bool isExecute)
        {
            if (execute == isExecute)
            {
                return;
            }

            execute = isExecute;
            if (isExecute)
            {
                portColor = Color.green;
            }
            else
            {
                if (CurPortStyle != null)
                {
                    portColor = CurPortStyle.Value.PortColor;
                }
            }

            //为了触发下portTopLeftRightBottom的刷新
            highlight = !highlight;
            highlight = !highlight;
        }

        public override void Connect(Edge edge)
        {
            base.Connect(edge);
            UpdatePropertyFieldDisplay(DisplayStyle.None);
        }

        public override void Disconnect(Edge edge)
        {
            base.Disconnect(edge);
            UpdatePropertyFieldDisplay(DisplayStyle.Flex);
        }

        private void AddPropertyField()
        {
            SerializedProperty serializedProperty =
                OwnerView.FindNodeProperty(BlueprintPort.PropertyFieldName);
            if (propertyFieldElement != null)
            {
                contentContainer.Remove(propertyFieldElement);
                propertyFieldElement = null;
            }

            if (serializedProperty != null)
            {
                var element = new PropertyField(serializedProperty, string.Empty);
                element.Bind(OwnerView.SerializedObject);
                propertyFieldElement = new VisualElement();
                propertyFieldElement.Add(element);
                contentContainer.Add(propertyFieldElement);
            }
        }

        private void UpdatePropertyFieldDisplay(DisplayStyle displayStyle)
        {
            if (BlueprintPort == null || BlueprintPort.PortDirection == IBlueprintPort.Direction.Output ||
                propertyFieldElement == null)
            {
                return;
            }

            propertyFieldElement.style.display = displayStyle;
        }
    }
}