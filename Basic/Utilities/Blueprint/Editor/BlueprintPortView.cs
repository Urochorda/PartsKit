using System;
using UnityEditor.Experimental.GraphView;
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

    public class BlueprintPortView : Port
    {
        protected BlueprintPortView(Orientation portOrientation, Direction portDirection, Capacity portCapacity,
            Type type) : base(portOrientation, portDirection, portCapacity, type)
        {
        }

        public static BlueprintPortView CreatePortView(IBlueprintPort blueprintPort)
        {
            // Port.DefaultEdgeConnectorListener listener = new Port.DefaultEdgeConnectorListener();
            // BlueprintPortView ele = new BlueprintPortView(blueprintPort.PortOrientation.ToEditorOrientation(),
            //     blueprintPort.PortDirection.ToEditorDirection(),
            //     blueprintPort.PortCapacity.ToEditorCapacity(), blueprintPort.PortType)
            // {
            //     m_EdgeConnector = (EdgeConnector)new EdgeConnector<TEdge>(listener)
            // };
            // ele.AddManipulator((IManipulator)ele.m_EdgeConnector);
            return null;
        }
    }
}