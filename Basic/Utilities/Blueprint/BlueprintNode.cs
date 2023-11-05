using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace PartsKit
{
    public abstract class BlueprintNode
    {
        public virtual string NodeName => GetType().Name;
        public virtual Color NodeColor => Color.clear;
        public virtual StyleSheet LayoutStyle => null;
        public virtual bool Deletable => true;

        public string Guid { get; private set; }
        public Rect Rect { get; set; }

        //端口是节点实例化时动态注册的，不需要序列化
        [field: NonSerialized] public List<IBlueprintPort> InputPort { get; set; }
        [field: NonSerialized] public List<IBlueprintPort> OutputPort { get; set; }

        public BlueprintNode()
        {
            InputPort = new List<IBlueprintPort>();
            OutputPort = new List<IBlueprintPort>();
        }

        public void OnCreateByView(string guidVal)
        {
            Guid = guidVal;
        }

        public void Init()
        {
            RegisterPort();
        }

        protected abstract void RegisterPort();

        /// <summary>
        /// 添加一个端口
        /// </summary>
        public void AddPort(IBlueprintPort treeNodePort)
        {
            switch (treeNodePort.PortDirection)
            {
                case IBlueprintPort.Direction.Input:
                    InputPort.Add(treeNodePort);
                    break;
                case IBlueprintPort.Direction.Output:
                    OutputPort.Add(treeNodePort);
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
                    InputPort.RemoveAll(item => item.PortName == portName);
                    break;
                case IBlueprintPort.Direction.Output:
                    OutputPort.RemoveAll(item => item.PortName == portName);
                    break;
                default:
                    Debug.LogError("portType错误");
                    break;
            }
        }
    }
}