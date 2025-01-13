using System;
using UnityEngine;

namespace PartsKit
{
    public enum SpineMachineParameterType
    {
        Float = 1,
        Bool = 2,
        Integer = 3,
        Trigger = 4,
    }

    [Serializable]
    public class SpineMachineParameter
    {
        public static SpineMachineParameter Create(SpineMachineParameterType parameterType, string parameterName,
            float defaultValueFloat, bool defaultValueBool, int defaultValueInteger, bool defaultValueTrigger)
        {
            var parameter = new SpineMachineParameter()
            {
                parameterType = parameterType,
                parameterName = parameterName,
                defaultValueFloat = defaultValueFloat,
                defaultValueBool = defaultValueBool,
                defaultValueInteger = defaultValueInteger,
                defaultValueTrigger = defaultValueTrigger,
            };
            return parameter;
        }

        [SerializeField] private SpineMachineParameterType parameterType;
        [SerializeField] private string parameterName;
        [Header("Float")] [SerializeField] private float defaultValueFloat;
        [Header("Bool")] [SerializeField] private bool defaultValueBool;
        [Header("Integer")] [SerializeField] private int defaultValueInteger;
        [Header("Trigger")] [SerializeField] private bool defaultValueTrigger;

        public SpineMachineParameterType ParameterType => parameterType;
        public string ParameterName => parameterName;
        public float DefaultValueFloat => defaultValueFloat;
        public bool DefaultValueBool => defaultValueBool;
        public int DefaultValueInteger => defaultValueInteger;
        public bool DefaultValueTrigger => defaultValueTrigger;
    }
}