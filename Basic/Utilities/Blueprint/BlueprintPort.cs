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
        public Color PortColor { get; }

        public BlueprintPortStyle(string visualClass, StyleSheet styleSheet, Color portColor)
        {
            VisualClass = visualClass;
            StyleSheet = styleSheet;
            PortColor = portColor;
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
    }

    public enum BlueprintExecuteState
    {
        Running = 1, //本节点需要继续执行
        End = 2, //本节点执行完毕
        Wait = 3, //本次执行到本节点后暂停
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

        public BlueprintExecuteState ExecuteState { get; private set; }

        public BlueprintExecutePort(string portNameVal, IBlueprintPort.Orientation portOrientationVal,
            IBlueprintPort.Direction portDirectionVal, IBlueprintPort.Capacity portCapacityVal,
            bool isExecute) : base(
            portNameVal, portOrientationVal, portDirectionVal, portCapacityVal, isExecute)
        {
        }

        public void Execute(out BlueprintExecutePort nextExecute)
        {
            switch (PortDirection)
            {
                case IBlueprintPort.Direction.Input:
                {
                    BlueprintNode.TryExecuted(OwnerNode, PortName, out nextExecute,
                        out BlueprintExecuteState executeStateVal);
                    ExecuteState = executeStateVal;
                    break;
                }

                case IBlueprintPort.Direction.Output:
                {
                    nextExecute = NextExecute;
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public static class BlueprintPortUtility
    {
        private const string DefaultStylePath = "Styles/BlueprintPortView";
        private static StyleSheet privatePortStyleSheet;

        private static readonly Dictionary<Type, BlueprintPortStyle> TypePortStyleDic =
            new Dictionary<Type, BlueprintPortStyle>();

        static BlueprintPortUtility()
        {
            Type executePortType = typeof(BlueprintExecutePortData);
            RegisterPortStyle(executePortType, CreatePortStyle(executePortType, new Color(0.89f, 0.85f, 0.25f, 1)));

            Type boolPortType = typeof(bool);
            RegisterPortStyle(boolPortType, CreatePortStyle(boolPortType, new Color(0.93f, 0.24f, 0.30f, 1)));
        }

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

        public static string GetDefaultVisualClass(Type type)
        {
            string visualClass = type.ToString();
            visualClass = visualClass.Replace('.', '_');
            return visualClass;
        }

        public static void RegisterPortStyle(Type type, BlueprintPortStyle style)
        {
            TypePortStyleDic[type] = style;
        }

        public static BlueprintPortStyle GetRegisterPortStyle(Type type)
        {
            if (TypePortStyleDic.TryGetValue(type, out BlueprintPortStyle style))
            {
                return style;
            }

            return CreateDefaultPortStyle(type);
        }

        public static BlueprintPortStyle CreatePortStyle(Type type, StyleSheet styleSheet)
        {
            BlueprintPortStyle portStyle =
                new BlueprintPortStyle(GetDefaultVisualClass(type), styleSheet,
                    GizmosUtilities.GenerateColorByType(type));
            return portStyle;
        }

        public static BlueprintPortStyle CreateDefaultPortStyle(Type type)
        {
            BlueprintPortStyle portStyle =
                new BlueprintPortStyle(GetDefaultVisualClass(type), DefaultPortViewStyleSheet,
                    GizmosUtilities.GenerateColorByType(type));
            return portStyle;
        }

        public static BlueprintPortStyle CreatePortStyle(Type type, StyleSheet styleSheet, string visualClass)
        {
            BlueprintPortStyle portStyle =
                new BlueprintPortStyle(visualClass, styleSheet, GizmosUtilities.GenerateColorByType(type));
            return portStyle;
        }

        public static BlueprintPortStyle CreatePortStyle(Type type, StyleSheet styleSheet, string visualClass,
            Color portColor)
        {
            BlueprintPortStyle portStyle =
                new BlueprintPortStyle(visualClass, styleSheet, portColor);
            return portStyle;
        }

        public static BlueprintPortStyle CreatePortStyle(Type type, Color portColor)
        {
            BlueprintPortStyle portStyle =
                new BlueprintPortStyle(GetDefaultVisualClass(type), DefaultPortViewStyleSheet, portColor);
            return portStyle;
        }

        public static BlueprintExecutePort CreateExecutePort(string portNameVal,
            IBlueprintPort.Orientation portOrientationVal, IBlueprintPort.Direction portDirectionVal)
        {
            IBlueprintPort.Capacity portCapacityVal = portDirectionVal == IBlueprintPort.Direction.Input
                ? IBlueprintPort.Capacity.Multi
                : IBlueprintPort.Capacity.Single;

            return new BlueprintExecutePort(portNameVal, portOrientationVal, portDirectionVal, portCapacityVal, true);
        }

        public static BlueprintPort<T> CreateInOutputPort<T>(string portNameVal,
            IBlueprintPort.Orientation portOrientationVal, IBlueprintPort.Direction portDirectionVal)
        {
            IBlueprintPort.Capacity portCapacityVal = portDirectionVal == IBlueprintPort.Direction.Input
                ? IBlueprintPort.Capacity.Single
                : IBlueprintPort.Capacity.Multi;

            return new BlueprintPort<T>(portNameVal, portOrientationVal, portDirectionVal, portCapacityVal, false);
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
        public T DefaultValue { get; set; } //默认数据当PrePort为null时使用默认数据

        /// <summary>
        /// 参数为必要数据，必填
        /// </summary>
        public BlueprintPort(string portNameVal, IBlueprintPort.Orientation portOrientationVal,
            IBlueprintPort.Direction portDirectionVal, IBlueprintPort.Capacity portCapacityVal, bool isExecuteVal)
        {
            PortName = portNameVal;
            PortType = typeof(T);
            PortOrientation = portOrientationVal;
            PortDirection = portDirectionVal;
            PortCapacity = portCapacityVal;
            IsExecute = isExecuteVal;
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