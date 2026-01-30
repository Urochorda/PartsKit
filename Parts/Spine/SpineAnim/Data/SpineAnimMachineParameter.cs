using System;
using UnityEngine;

namespace PartsKit
{
    [Serializable]
    public class SpineAnimMachineParameter
    {
        public static SpineAnimMachineParameter Create(AnimatorControllerParameterType parameterType,
            string parameterName,
            float defaultValueFloat, bool defaultValueBool, int defaultValueInteger, bool defaultValueTrigger)
        {
            var parameter = new SpineAnimMachineParameter()
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

        [SerializeField] private AnimatorControllerParameterType parameterType;
        [SerializeField] private string parameterName;
        [Header("Float")] [SerializeField] private float defaultValueFloat;
        [Header("Bool")] [SerializeField] private bool defaultValueBool;
        [Header("Integer")] [SerializeField] private int defaultValueInteger;
        [Header("Trigger")] [SerializeField] private bool defaultValueTrigger;

        public AnimatorControllerParameterType ParameterType
        {
            get => parameterType;
            set => parameterType = value;
        }
        public string ParameterName
        {
            get => parameterName;
            set => parameterName = value;
        }
        public float DefaultValueFloat
        {
            get => defaultValueFloat;
            set => defaultValueFloat = value;
        }
        public bool DefaultValueBool
        {
            get => defaultValueBool;
            set => defaultValueBool = value;
        }
        public int DefaultValueInteger
        {
            get => defaultValueInteger;
            set => defaultValueInteger = value;
        }
        public bool DefaultValueTrigger
        {
            get => defaultValueTrigger;
            set => defaultValueTrigger = value;
        }

        #region Runtime

        public int ParameterId { get; private set; }

        public void InitRuntime()
        {
            ParameterId = Animator.StringToHash(parameterName);
        }

        #endregion
    }
}