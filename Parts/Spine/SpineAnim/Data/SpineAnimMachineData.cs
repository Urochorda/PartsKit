using System;
using System.Collections.Generic;
using UnityEngine;

namespace PartsKit
{
    [CreateAssetMenu(menuName = "PartsKit/Spine/SpineMachine", fileName = "SpineMachine_")]
    public class SpineAnimMachineData : ScriptableObject
    {
        [SerializeField] private SpineAnimStateData enterState;
        [SerializeField] private SpineAnimLineData[] anyState;
        [SerializeField] private SpineAnimMachineParameter[] parameterDefault;

        public SpineAnimStateData EnterState => enterState;
        public IReadOnlyList<SpineAnimLineData> AnyState => anyState;


        #region Runtime

        private readonly List<SpineAnimStateData> states = new List<SpineAnimStateData>();
        public IReadOnlyList<SpineAnimStateData> States => states;
        private readonly List<SpineAnimStateData.IClipRef> clips = new List<SpineAnimStateData.IClipRef>();
        public IReadOnlyList<SpineAnimStateData.IClipRef> Clips => clips;
        private readonly List<SpineAnimMachineParameter> parameters = new List<SpineAnimMachineParameter>();
        public IReadOnlyList<SpineAnimMachineParameter> Parameters => parameters;

        public void InitRuntime()
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

            foreach (var state in states)
            {
                state.InitRuntime();
            }

            foreach (var parameter in parameters)
            {
                parameter.InitRuntime();
            }
        }

        public bool HasParameterOfType(string key, AnimatorControllerParameterType type)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            var parameters = Parameters;
            foreach (SpineAnimMachineParameter currParam in parameters)
            {
                if (currParam.ParameterType == type && currParam.ParameterName == key)
                {
                    return true;
                }
            }

            return false;
        }

        private void AddAllParameterByState(SpineAnimStateData stateData)
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
            int hasIndex = parameters.FindIndex(item => item.ParameterName == parameterName);
            if (hasIndex >= 0)
            {
                var hasValue = parameters[hasIndex];
                if (hasValue.ParameterType != parameterType)
                {
                    Debug.LogError($"spineMachineParameter parameterType conflict: {parameterName}");
                }
            }
            else
            {
                int parameterHash = Animator.StringToHash(parameterName);
                SpineAnimMachineParameter addValue = null;
                bool hasParameter = GetParameterDefault(parameterType, parameterHash, out var parameterData);
                switch (parameterType)
                {
                    case AnimatorControllerParameterType.Float:
                        var floatValue = hasParameter ? parameterData.DefaultValueFloat : 0;
                        addValue = SpineAnimMachineParameter.Create(parameterType, parameterName, floatValue, false,
                            0, false);
                        break;
                    case AnimatorControllerParameterType.Bool:
                        var boolValue = hasParameter && parameterData.DefaultValueBool;
                        addValue = SpineAnimMachineParameter.Create(parameterType, parameterName, 0, boolValue,
                            0, false);
                        break;
                    case AnimatorControllerParameterType.Int:
                        var intValue = hasParameter ? parameterData.DefaultValueInteger : 0;
                        addValue = SpineAnimMachineParameter.Create(parameterType, parameterName, 0, false,
                            intValue, false);
                        break;
                    case AnimatorControllerParameterType.Trigger:
                        var triggerValue = hasParameter && parameterData.DefaultValueTrigger;
                        addValue = SpineAnimMachineParameter.Create(parameterType, parameterName, 0, false,
                            0, triggerValue);
                        break;
                }

                if (addValue != null)
                {
                    parameters.Add(addValue);
                }
            }
        }

        private bool GetParameterDefault(AnimatorControllerParameterType parameterType, int parameterHase,
            out SpineAnimMachineParameter parameterData)
        {
            foreach (var parameterItem in parameterDefault)
            {
                var curHashKey = Animator.StringToHash(parameterItem.ParameterName);
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

        private void CollectInfoByLine(IEnumerable<SpineAnimLineData> lineDataList)
        {
            foreach (var line in lineDataList)
            {
                var nextState = line.NextState;
                if (nextState != null)
                {
                    CollectInfoByState(nextState);
                }
            }
        }

        private void CollectInfoByState(SpineAnimStateData stateData)
        {
            if (states.Contains(stateData))
            {
                //已经收集过了
                return;
            }

            states.Add(stateData);
            if (!clips.Contains(stateData.DefaultClip))
            {
                clips.Add(stateData.DefaultClip);
            }

            foreach (var clipData in stateData.ClipPool)
            {
                if (!clips.Contains(clipData))
                {
                    clips.Add(clipData);
                }
            }

            CollectInfoByLine(stateData.LinePool);
        }

        #endregion
    }
}