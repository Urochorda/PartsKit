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

        public object Value
        {
            get => tValue;
            set => tValue = value == default ? default : (T)value;
        }

        public Type ParameterType => typeof(T);
        public virtual Type SetNodeType => typeof(BlueprintSetParameterNode);
    }
}