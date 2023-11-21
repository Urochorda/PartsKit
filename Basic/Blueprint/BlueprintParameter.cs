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

        #region 可序列化字段

        public string Guid { get; set; }
        public string ParameterName { get; set; }
        public string ParameterTypeName { get; }
        public object Value { get; set; }

        #endregion
    }

    [Serializable]
    public class BlueprintParameter<T> : IBlueprintParameter
    {
        #region 可序列化字段

        [field: SerializeField] private string guid;
        [field: SerializeField] private string parameterName;
        [field: SerializeField] protected T tValue;

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

        public string ParameterTypeName => typeof(T).ToString();

        public object Value
        {
            get => tValue;
            set => tValue = value == default ? default : (T)value;
        }
    }
}