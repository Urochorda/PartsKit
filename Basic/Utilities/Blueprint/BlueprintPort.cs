using System;

namespace PartsKit
{
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
        public void SetValue(IBlueprintPort data);
    }

    public class BlueprintPort<T> : IBlueprintPort
    {
        public string PortName { get; set; }
        public Type PortType { get; }
        public IBlueprintPort.Orientation PortOrientation { get; set; }
        public IBlueprintPort.Capacity PortCapacity { get; set; }
        public IBlueprintPort.Direction PortDirection { get; set; }
        public Action<T> OnSet { get; set; }
        public Func<T> OnGet { get; set; }
        public Action OnSetDefault { get; set; }

        /// <summary>
        /// 参数为必要数据，必填
        /// </summary>
        public BlueprintPort(string portNameVal, IBlueprintPort.Orientation portOrientationVal,
            IBlueprintPort.Direction portDirectionVal,
            IBlueprintPort.Capacity portCapacityVal, Action<T> onSetVal,
            Func<T> onGetVal, Action onSetDefaultVal)
        {
            PortName = portNameVal;
            PortType = typeof(T);
            PortOrientation = portOrientationVal;
            PortDirection = portDirectionVal;
            PortCapacity = portCapacityVal;
            OnSet = onSetVal;
            OnGet = onGetVal;
            OnSetDefault = onSetDefaultVal;
        }

        public void SetValue(IBlueprintPort data)
        {
            if (data is BlueprintPort<T> tData)
            {
                T targetValue = tData.OnGet.Invoke();
                OnSet.Invoke(targetValue);
            }
        }
    }
}