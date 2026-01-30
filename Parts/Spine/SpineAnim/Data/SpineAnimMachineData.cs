using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace PartsKit
{
    [CreateAssetMenu(menuName = "PartsKit/Spine/SpineMachine", fileName = "SpineMachine_")]
    public class SpineAnimMachineData : ScriptableObject
    {
        [SerializeField] private SpineAnimStateData defaultState;
        [SerializeField] private SpineAnimLineData[] enterLine;
        [SerializeField] private SpineAnimStateLineData[] anyLine;
        [SerializeField] List<SpineAnimMachineParameter> parameters = new();

        public SpineAnimStateData DefaultState
        {
            get => defaultState;
            set => defaultState = value;
        }

        public SpineAnimLineData[] EnterLine
        {
            get => enterLine;
            set => enterLine = value;
        }

        public SpineAnimStateLineData[] AnyLine
        {
            get => anyLine;
            set => anyLine = value;
        }

        public List<SpineAnimMachineParameter> Parameters
        {
            get => parameters;
            set => parameters = value;
        }

        #region Runtime

        private readonly List<SpineAnimStateData> states = new List<SpineAnimStateData>();
        public IReadOnlyList<SpineAnimStateData> States => states;
        private readonly List<SpineAnimStateData.IClipRef> clips = new List<SpineAnimStateData.IClipRef>();
        public IReadOnlyList<SpineAnimStateData.IClipRef> Clips => clips;

        public void InitRuntime()
        {
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

            foreach (SpineAnimMachineParameter currParam in parameters)
            {
                if (currParam.ParameterType == type && currParam.ParameterName == key)
                {
                    return true;
                }
            }

            return false;
        }

        private void CollectInfoByLine(IEnumerable<SpineAnimLineDataBase> lineDataList)
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