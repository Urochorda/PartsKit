using System;
using System.Collections.Generic;
using UnityEngine;

namespace PartsKit
{
    [CreateAssetMenu]
    public class SpineMachineData : ScriptableObject
    {
        [SerializeField] private SpineStateData enterState;
        [SerializeField] private SpineLineData[] anyState;
        [SerializeField] private SpineMachineParameter[] parameterDefault;

        private readonly List<SpineMachineParameter> parameter = new List<SpineMachineParameter>();

        public SpineStateData EnterState => enterState;
        public IReadOnlyList<SpineLineData> AnyState => anyState;
        public IReadOnlyList<SpineMachineParameter> Parameter => parameter;

        private void Awake()
        {
            InitParameter();
        }

        public bool HasParameterOfType(string key, AnimatorControllerParameterType type)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            var parameters = Parameter;
            foreach (SpineMachineParameter currParam in parameters)
            {
                if (currParam.ParameterType == type && currParam.ParameterName == key)
                {
                    return true;
                }
            }

            return false;
        }

        private void InitParameter()
        {
            var anyStateVal = AnyState;
            foreach (var spineLineData in anyStateVal)
            {
                var conditions = spineLineData.Conditions;
                foreach (var condition in conditions)
                {
                    AddAllParameter(condition.ParameterType, condition.ParameterName);
                }
            }

            var enterStateVal = EnterState;
            AddAllParameterByState(enterStateVal);
        }

        private void AddAllParameterByState(SpineStateData stateData)
        {
            var linePool = stateData.LinePool;
            foreach (var lineData in linePool)
            {
                var conditions = lineData.Conditions;
                foreach (var condition in conditions)
                {
                    AddAllParameter(condition.ParameterType, condition.ParameterName);
                }

                var nextState = lineData.NextState;
                if (nextState != null)
                {
                    AddAllParameterByState(lineData.NextState);
                }
            }
        }

        private void AddAllParameter(AnimatorControllerParameterType parameterType, string parameterName)
        {
            int hasIndex = parameter.FindIndex(item => item.ParameterName == parameterName);
            if (hasIndex >= 0)
            {
                var hasValue = parameter[hasIndex];
                if (hasValue.ParameterType != parameterType)
                {
                    Debug.LogError($"spineMachineParameter parameterType conflict: {parameterName}");
                }
            }
            else
            {
                int parameterHash = SpineMachinePlayer.StringToHash(parameterName);
                SpineMachineParameter addValue = null;
                bool hasParameter = GetParameterDefault(parameterType, parameterHash, out var parameterData);
                switch (parameterType)
                {
                    case AnimatorControllerParameterType.Float:
                        var floatValue = hasParameter ? parameterData.DefaultValueFloat : default;
                        addValue = SpineMachineParameter.Create(parameterType, parameterName, floatValue, default,
                            default, default);
                        break;
                    case AnimatorControllerParameterType.Bool:
                        var boolValue = hasParameter ? parameterData.DefaultValueBool : default;
                        addValue = SpineMachineParameter.Create(parameterType, parameterName, default, boolValue,
                            default, default);
                        break;
                    case AnimatorControllerParameterType.Int:
                        var intValue = hasParameter ? parameterData.DefaultValueInteger : default;
                        addValue = SpineMachineParameter.Create(parameterType, parameterName, default, default,
                            intValue, default);
                        break;
                    case AnimatorControllerParameterType.Trigger:
                        var triggerValue = hasParameter ? parameterData.DefaultValueTrigger : default;
                        addValue = SpineMachineParameter.Create(parameterType, parameterName, default, default,
                            default, triggerValue);
                        break;
                }

                if (addValue != null)
                {
                    parameter.Add(addValue);
                }
            }
        }

        private bool GetParameterDefault(AnimatorControllerParameterType parameterType, int parameterHase,
            out SpineMachineParameter parameterData)
        {
            foreach (var parameterItem in parameterDefault)
            {
                var curHashKey = SpineMachinePlayer.StringToHash(parameterItem.ParameterName);
                var curType = parameterItem.ParameterType;
                if (curType == parameterType && curHashKey == parameterHase)
                {
                    parameterData = parameterItem;
                    return true;
                }
            }

            parameterData = null;
            return false;
        }
    }
}