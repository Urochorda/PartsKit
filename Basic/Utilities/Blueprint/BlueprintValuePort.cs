using System;

namespace PartsKit
{
    public class BlueprintValuePort<T> : BlueprintPortBase<T>
    {
        private readonly Func<BlueprintValuePort<T>, T> onGetValue;

        /// <summary>
        /// 参数为必要数据，必填
        /// </summary>
        public BlueprintValuePort(string portNameVal, IBlueprintPort.Orientation portOrientationVal,
            IBlueprintPort.Direction portDirectionVal, IBlueprintPort.Capacity portCapacityVal,
            Func<BlueprintValuePort<T>, T> onGetValueVal) : base(portNameVal,
            portOrientationVal, portDirectionVal, portCapacityVal)
        {
            onGetValue = onGetValueVal;
        }

        public void GetValue(out T nextExecute)
        {
            nextExecute = onGetValue == null ? default : onGetValue.Invoke(this);
        }

        public bool GetPrePortFirst(out BlueprintValuePort<T> targetPort)
        {
            if (PrePorts.Count > 0)
            {
                if (PrePorts[0] is BlueprintValuePort<T> targetPortVal)
                {
                    targetPort = targetPortVal;
                    return true;
                }
            }

            targetPort = null;
            return false;
        }
    }
}