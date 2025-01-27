using System;
using UnityEngine;

namespace PartsKit
{
    public enum SpineIntConditionMode
    {
        Greater = 1,
        Less = 2,
        Equals = 3,
        NotEqual = 4,
    }

    public enum SpineFloatConditionMode
    {
        Greater = 1,
        Less = 2,
    }

    public enum SpineBoolConditionMode
    {
        True = 1,
        False = 2,
    }

    [Serializable]
    public class SpineConditionData
    {
        [SerializeField] private AnimatorControllerParameterType parameterType;
        [SerializeField] private string parameterName;
        [Header("Float")] [SerializeField] private SpineFloatConditionMode floatConditionMode;
        [SerializeField] private float parameterValueFloat;
        [Header("Bool")] [SerializeField] private SpineBoolConditionMode boolConditionMode;
        [Header("Integer")] [SerializeField] private SpineIntConditionMode intConditionMode;
        [SerializeField] private int parameterValueInteger;

        public AnimatorControllerParameterType ParameterType => parameterType;
        public string ParameterName => parameterName;
        public SpineFloatConditionMode FloatConditionMode => floatConditionMode;
        public float ParameterValueFloat => parameterValueFloat;
        public SpineBoolConditionMode BoolConditionMode => boolConditionMode;
        public SpineIntConditionMode IntConditionMode => intConditionMode;
        public int ParameterValueInteger => parameterValueInteger;
    }
}