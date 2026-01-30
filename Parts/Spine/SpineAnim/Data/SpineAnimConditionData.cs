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
        [SerializeField] private string parameterName;
        [Header("Float")] [SerializeField] private SpineAnimFloatConditionMode floatConditionMode;
        [SerializeField] private float parameterValueFloat;
        [Header("Bool")] [SerializeField] private SpineAnimBoolConditionMode boolConditionMode;
        [Header("Integer")] [SerializeField] private SpineAnimIntConditionMode intConditionMode;
        [SerializeField] private int parameterValueInteger;

        public string ParameterName
        {
            get => parameterName;
            set => parameterName = value;
        }

        public SpineAnimFloatConditionMode FloatConditionMode
        {
            get => floatConditionMode;
            set => floatConditionMode = value;
        }

        public float ParameterValueFloat
        {
            get => parameterValueFloat;
            set => parameterValueFloat = value;
        }

        public SpineAnimBoolConditionMode AnimBoolConditionMode
        {
            get => boolConditionMode;
            set => boolConditionMode = value;
        }

        public SpineAnimIntConditionMode IntConditionMode
        {
            get => intConditionMode;
            set => intConditionMode = value;
        }

        public int ParameterValueInteger
        {
            get => parameterValueInteger;
            set => parameterValueInteger = value;
        }
    }
}