using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace PartsKit
{
    public struct BlueprintPortStyle
    {
        public string VisualClass { get; }
        public StyleSheet StyleSheet { get; }

        public BlueprintPortStyle(string visualClass, StyleSheet styleSheet)
        {
            VisualClass = visualClass;
            StyleSheet = styleSheet;
        }
    }

    public interface IBlueprintPort
    {
        public enum Direction
        {
            Input,
            Output,
        }

        public enum Orientation
        {
            Horizontal,
            Vertical,
        }

        public enum Capacity
        {
            Single,
            Multi,
        }

        public string PortName { get; set; } //用作名称展示，也用作唯一标识
        public Type PortType { get; } //端口类型，用作view层显示
        public Orientation PortOrientation { get; set; } //节点方向，用作view层显示
        public Capacity PortCapacity { get; set; } //节点连接类型，用作view层显示
        public Direction PortDirection { get; set; } //节点类型，输入输出
        public BlueprintNode OwnerNode { get; set; } //节点所属Node
        public bool IsExecute { get; set; } //是否是执行节点
        public List<IBlueprintPort> PrePorts { get; set; } //上一个端口
        public List<IBlueprintPort> NextPorts { get; set; } //下一个端口
        public BlueprintPortStyle? PortStyle { get; set; } //端口样式
    }


    public struct BlueprintExecutePortData
    {
    }

    public class BlueprintExecutePort : BlueprintPort<BlueprintExecutePortData>
    {
        public BlueprintExecutePort NextExecute
        {
            get
            {
                if (NextPorts.Count <= 0)
                {
                    return null;
                }

                return NextPorts[0] as BlueprintExecutePort;
            }
        }

        public BlueprintExecutePort(string portNameVal, IBlueprintPort.Orientation portOrientationVal,
            IBlueprintPort.Direction portDirectionVal, IBlueprintPort.Capacity portCapacityVal, bool isExecute,
            BlueprintPortStyle? portStyleVal) : base(
            portNameVal, portOrientationVal, portDirectionVal, portCapacityVal, isExecute, portStyleVal)
        {
        }
    }

    public static class BlueprintPortUtility
    {
        private static StyleSheet privatePortStyleSheet;
        private const string DefaultStylePath = "Styles/BlueprintPortView";

        private static StyleSheet DefaultPortViewStyleSheet
        {
            get
            {
                if (privatePortStyleSheet == null)
                {
                    privatePortStyleSheet = Resources.Load<StyleSheet>(DefaultStylePath);
                }

                return privatePortStyleSheet;
            }
        }

        private static BlueprintPortStyle GetDefaultBlueprintPortStyle(Type type)
        {
            string visualClass = type.ToString();
            visualClass = visualClass.Replace('.', '_');
            BlueprintPortStyle portStyle =
                new BlueprintPortStyle(visualClass, DefaultPortViewStyleSheet);
            return portStyle;
        }

        public static BlueprintExecutePort CreateExecutePort(string portNameVal,
            IBlueprintPort.Orientation portOrientationVal, IBlueprintPort.Direction portDirectionVal)
        {
            IBlueprintPort.Capacity portCapacityVal = portDirectionVal == IBlueprintPort.Direction.Input
                ? IBlueprintPort.Capacity.Multi
                : IBlueprintPort.Capacity.Single;
            BlueprintPortStyle portStyle = GetDefaultBlueprintPortStyle(typeof(BlueprintExecutePortData));

            return new BlueprintExecutePort(portNameVal, portOrientationVal, portDirectionVal, portCapacityVal, true,
                portStyle);
        }

        public static BlueprintPort<T> CreateInOutputPort<T>(string portNameVal,
            IBlueprintPort.Orientation portOrientationVal, IBlueprintPort.Direction portDirectionVal,
            BlueprintPortStyle? portStyleVal = null)
        {
            IBlueprintPort.Capacity portCapacityVal = portDirectionVal == IBlueprintPort.Direction.Input
                ? IBlueprintPort.Capacity.Single
                : IBlueprintPort.Capacity.Multi;
            if (portStyleVal == null)
            {
                portStyleVal = GetDefaultBlueprintPortStyle(typeof(T));
            }

            return new BlueprintPort<T>(portNameVal, portOrientationVal, portDirectionVal, portCapacityVal, false,
                portStyleVal);
        }
    }

    public class BlueprintPort<T> : IBlueprintPort
    {
        public string PortName { get; set; }
        public Type PortType { get; }
        public IBlueprintPort.Orientation PortOrientation { get; set; }
        public IBlueprintPort.Capacity PortCapacity { get; set; }
        public IBlueprintPort.Direction PortDirection { get; set; }
        public BlueprintNode OwnerNode { get; set; }
        public bool IsExecute { get; set; }
        public List<IBlueprintPort> PrePorts { get; set; }
        public List<IBlueprintPort> NextPorts { get; set; }
        public BlueprintPortStyle? PortStyle { get; set; }
        public T DefaultValue { get; set; } //默认数据当LastPort为null时使用默认数据

        /// <summary>
        /// 参数为必要数据，必填
        /// </summary>
        public BlueprintPort(string portNameVal, IBlueprintPort.Orientation portOrientationVal,
            IBlueprintPort.Direction portDirectionVal, IBlueprintPort.Capacity portCapacityVal, bool isExecuteVal,
            BlueprintPortStyle? portStyleVal)
        {
            PortName = portNameVal;
            PortType = typeof(T);
            PortOrientation = portOrientationVal;
            PortDirection = portDirectionVal;
            PortCapacity = portCapacityVal;
            IsExecute = isExecuteVal;
            PortStyle = portStyleVal;
            PrePorts = new List<IBlueprintPort>();
            NextPorts = new List<IBlueprintPort>();
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        public T GetValue()
        {
            foreach (IBlueprintPort port in PrePorts)
            {
                if (port is BlueprintPort<T> targetPort)
                {
                    return targetPort.GetValue();
                }
            }

            return DefaultValue;
        }
    }
}