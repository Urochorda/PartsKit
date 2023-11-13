using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace PartsKit
{
    [Serializable]
    public class BlueprintNode
    {
        public static BlueprintNode CreateFromType(Type nodeType)
        {
            if (!nodeType.IsSubclassOf(typeof(BlueprintNode)))
            {
                return null;
            }

            BlueprintNode node = Activator.CreateInstance(nodeType) as BlueprintNode;
            return node;
        }

        /// <summary>
        /// 尝试执行
        /// </summary>
        public static void TryExecuted(BlueprintNode node, string portName, out BlueprintExecutePort nextPort)
        {
            IBlueprintPort inPort = node.GetPort(IBlueprintPort.Direction.Input, portName);
            if (inPort == null)
            {
                nextPort = null;
                return;
            }

            node.OnExecuted(inPort, out nextPort);
        }

        #region 可序列化的字段

        [field: SerializeField] public string Guid { get; private set; }
        [field: SerializeField] public Rect Rect { get; set; }

        #endregion

        public virtual string NodeName => GetType().Name;
        public virtual Color NodeColor => Color.clear;
        public virtual StyleSheet LayoutStyle => null;
        public virtual bool Deletable => true;
        public List<IBlueprintPort> InputPorts { get; }
        public List<IBlueprintPort> OutputPorts { get; }

        public BlueprintNode()
        {
            InputPorts = new List<IBlueprintPort>();
            OutputPorts = new List<IBlueprintPort>();
        }

        public void OnCreate()
        {
            Guid = System.Guid.NewGuid().ToString();
        }

        public void Init()
        {
            RegisterPort();
        }

        protected virtual void RegisterPort()
        {
        }

        /// <summary>
        /// 添加一个端口
        /// </summary>
        public void AddPort(IBlueprintPort treeNodePort)
        {
            treeNodePort.OwnerNode = this;
            switch (treeNodePort.PortDirection)
            {
                case IBlueprintPort.Direction.Input:
                    InputPorts.Add(treeNodePort);
                    break;
                case IBlueprintPort.Direction.Output:
                    OutputPorts.Add(treeNodePort);
                    break;
                default:
                    Debug.LogError("portType错误");
                    break;
            }
        }

        /// <summary>
        /// 移除一个端口
        /// </summary>
        public void RemovePort(IBlueprintPort.Direction portType, string portName)
        {
            switch (portType)
            {
                case IBlueprintPort.Direction.Input:
                    InputPorts.RemoveAll(item => item.PortName == portName);
                    break;
                case IBlueprintPort.Direction.Output:
                    OutputPorts.RemoveAll(item => item.PortName == portName);
                    break;
                default:
                    Debug.LogError("portType错误");
                    break;
            }
        }

        /// <summary>
        /// 获取一个port
        /// </summary>
        public IBlueprintPort GetPort(IBlueprintPort.Direction portType, string portName)
        {
            switch (portType)
            {
                case IBlueprintPort.Direction.Input:
                    return InputPorts.Find(item => item.PortName == portName);
                case IBlueprintPort.Direction.Output:
                    return OutputPorts.Find(item => item.PortName == portName);
                default:
                    Debug.LogError("portType错误");
                    break;
            }

            return null;
        }

        /// <summary>
        /// 执行节点
        /// </summary>
        protected virtual void OnExecuted(IBlueprintPort port, out BlueprintExecutePort nextPort)
        {
            nextPort = null;
        }
    }
}