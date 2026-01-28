using System;
using UnityEngine;

namespace PartsKit
{
    public enum SpineAnimIntConditionMode
    {
        Greater = 1,
        Less = 2,
        Equals = 3,
        NotEqual = 4,
    }

    public enum SpineAnimFloatConditionMode
    {
        Greater = 1,
        Less = 2,
    }

    public enum SpineAnimBoolConditionMode
    {
        True = 1,
        False = 2,
    }

    [Serializable]
    public class SpineAnimConditionData
    {
        [SerializeField] private AnimatorControllerParameterType parameterType;
        [SerializeField] private string parameterName;
        [Header("Float")] [SerializeField] private SpineAnimFloatConditionMode floatConditionMode;
        [SerializeField] private float parameterValueFloat;
        [Header("Bool")] [SerializeField] private SpineAnimBoolConditionMode boolConditionMode;
        [Header("Integer")] [SerializeField] private SpineAnimIntConditionMode intConditionMode;
        [SerializeField] private int parameterValueInteger;

        public AnimatorControllerParameterType ParameterType => parameterType;
        public string ParameterName => parameterName;
        public SpineAnimFloatConditionMode FloatConditionMode => floatConditionMode;
        public float ParameterValueFloat => parameterValueFloat;
        public SpineAnimBoolConditionMode AnimBoolConditionMode => boolConditionMode;
        public SpineAnimIntConditionMode IntConditionMode => intConditionMode;
        public int ParameterValueInteger => parameterValueInteger;
    }
}