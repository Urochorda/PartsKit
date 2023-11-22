using System;
using System.Collections.Generic;
using UnityEngine;

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
            string propertyFieldNameVal, Func<BlueprintValuePort<T>, T> onGetValueVal) : base(portNameVal,
            portOrientationVal, portDirectionVal, portCapacityVal, propertyFieldNameVal)
        {
            onGetValue = onGetValueVal;
        }

        public void GetValue(out T value)
        {
            value = onGetValue == null ? default : onGetValue.Invoke(this);
        }

        public bool GetPrePortValue(out T value)
        {
            if (PrePorts.Count > 0)
            {
                return GetPortValue(PrePorts[0], out value);
            }

            value = default;
            return false;
        }

        public bool GetAllPrePortValue(out List<T> value)
        {
            value = new List<T>();
            if (PrePorts.Count > 0)
            {
                bool hasSuccess = false;
                foreach (IBlueprintPort prePort in PrePorts)
                {
                    bool isSuccess = GetPortValue(prePort, out T valueItem);
                    if (isSuccess)
                    {
                        hasSuccess = true;
                        value.Add(valueItem);
                    }
                }

                return hasSuccess;
            }

            return false;
        }

        private bool GetPortValue(IBlueprintPort prePort, out T value)
        {
            if (prePort is BlueprintValuePort<T> targetPortVal)
            {
                targetPortVal.GetValue(out value);
                return true;
            }

            if (typeof(T) == typeof(object) && prePort is BlueprintValuePort<dynamic> dyPortVal)
            {
                dyPortVal.GetValue(out dynamic objValue);
                value = objValue;
                return true;
            }

            if (prePort is BlueprintValuePort<object> objPortVal)
            {
                objPortVal.GetValue(out object objValue);
                if (objValue == null)
                {
                    value = default;
                    return true;
                }

                try
                {
                    value = (T)objValue;
                    return true;
                }
                catch (InvalidCastException e)
                {
                    Debug.LogError(e);
                    value = default;
                    return false;
                }
            }

            value = default;
            return false;
        }
    }
}