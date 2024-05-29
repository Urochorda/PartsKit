using System;
using UnityEngine;

namespace PartsKit
{
    public interface IBlueprintParameter
    {
        public static IBlueprintParameter CreateFromType(Type nodeType)
        {
            if (!typeof(IBlueprintParameter).IsAssignableFrom(nodeType))
            {
                return null;
            }

            IBlueprintParameter node = Activator.CreateInstance(nodeType) as IBlueprintParameter;
            if (node != null)
            {
                node.Guid = System.Guid.NewGuid().ToString();
                node.ParameterName = $"New{nodeType.Name}";
                node.Value = default;
            }

            return node;
        }

        public string Guid { get; set; }
        public string ParameterName { get; set; }
        public string ParameterTypeName { get; }
        public object Value { get; set; }
        public Type ParameterType { get; }
        public Type SetNodeType { get; }
    }

    [Serializable]
    public class BlueprintParameter<T> : IBlueprintParameter
    {
        #region 可序列化字段

        [SerializeField] private string guid;
        [SerializeField] private string parameterName;
        [SerializeField] protected T tValue;

        #endregion

        public string Guid
        {
            get => guid;
            set => guid = value;
        }

        public string ParameterName
        {
            get => parameterName;
            set => parameterName = value;
        }

        public string ParameterTypeName => ParameterType.ToString();

        private object oValue;

        public object Value
        {
            get => oValue;
            set
            {
                oValue = value;
                SetTValue();
            }
        }

        public Type ParameterType => typeof(T);
        public virtual Type SetNodeType => typeof(BlueprintSetParameterNode);

        private void SetTValue()
        {
            if (oValue == null)
            {
                tValue = default;
                return;
            }

            Type tType = typeof(T);
            if (oValue.GetType() == tType)
            {
                tValue = (T)oValue;
            }
            else
            {
                if (tType.IsEnum)
                {
                    tValue = (T)Enum.ToObject(typeof(T), oValue);
                }
                else
                {
                    tValue = (T)Convert.ChangeType(oValue, typeof(T));
                }

                //发生类型转化，更新oValue
                oValue = tValue;
            }
        }
    }
}