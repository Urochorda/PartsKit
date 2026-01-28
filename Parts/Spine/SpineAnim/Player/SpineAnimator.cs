using System;
using System.Collections.Generic;
using UnityEngine;

namespace PartsKit
{
    public class SpineAnimator : MonoBehaviour
    {
        [SerializeField] private SpineAnimMachineData animMachineData;
        [SerializeField] private SpineAnimation animationPlayer;
        [SerializeField] private bool isDisableStop = true; //默认为true，重新开启时会重置状态

        private readonly Dictionary<int, float> floatParameterPool = new Dictionary<int, float>();
        private readonly Dictionary<int, int> intParameterPool = new Dictionary<int, int>();
        private readonly Dictionary<int, bool> boolParameterPool = new Dictionary<int, bool>();
        private readonly Dictionary<int, bool> triggerParameterPool = new Dictionary<int, bool>();
        public IReadOnlyList<SpineAnimMachineParameter> Parameters => animMachineData.Parameters;
        public IReadOnlyList<SpineAnimStateData> States => animMachineData.States;
        public IReadOnlyList<SpineAnimStateData.IClipRef> Clips => animMachineData.Clips;
        public SpineAnimation AnimationPlayer => animationPlayer;

        private SpineAnimStateData curActivateState;
        private SpineAnimStateData curPlayingState;
        private SpineAnimStateData.IClipRef curPlayingAnimClip;
        private float globalSpeed = 1;
        private bool updatePlayerSpeedFag;
        private bool isPlaying;
        private bool updateFlag;

        private void OnEnable()
        {
            Play();
        }

        private void OnDisable()
        {
            if (isDisableStop)
            {
                Stop();
            }
        }

        private void Update()
        {
            UpdateState();
        }

        private void Play()
        {
            if (isPlaying)
            {
                return;
            }

            isPlaying = true;
            animMachineData.InitRuntime();
            InitParameter();
            curActivateState = animMachineData.EnterState;
            curPlayingState = null;
        }

        private void Stop()
        {
            if (!isPlaying)
            {
                return;
            }

            isPlaying = false;
            animationPlayer.StopAnim();
        }

        public bool HasParameterOfType(string key, AnimatorControllerParameterType type)
        {
            return animMachineData.HasParameterOfType(key, type);
        }

        private void InitParameter()
        {
            floatParameterPool.Clear();
            intParameterPool.Clear();
            boolParameterPool.Clear();
            triggerParameterPool.Clear();
            var defaultPar = Parameters;
            foreach (var parameter in defaultPar)
            {
                int parameterId = parameter.ParameterId;
                switch (parameter.ParameterType)
                {
                    case AnimatorControllerParameterType.Float:
                        floatParameterPool[parameterId] = parameter.DefaultValueFloat;
                        break;
                    case AnimatorControllerParameterType.Bool:
                        boolParameterPool[parameterId] = parameter.DefaultValueBool;
                        break;
                    case AnimatorControllerParameterType.Int:
                        intParameterPool[parameterId] = parameter.DefaultValueInteger;
                        break;
                    case AnimatorControllerParameterType.Trigger:
                        triggerParameterPool[parameterId] = parameter.DefaultValueTrigger;
                        break;
                }
            }

            updateFlag = true;
            updatePlayerSpeedFag = false;
        }

        private void UpdateState()
        {
            if (!isPlaying || !updateFlag)
            {
                return;
            }

            UpdateStateLine();
            UpdatePlayingState();
            ResetTriggerAll();
            updateFlag = false;
        }

        private void UpdateStateLine()
        {
            var anyState = animMachineData.AnyState;
            var activateState = curActivateState;

            //从Any状态切换到下一个状态，切换成功则不在同一帧检测下一个状态
            if (TryNext(anyState))
            {
                return;
            }

            //检测当前激活的状态切换
            TryNext(activateState.LinePool);

            bool TryNext(IEnumerable<SpineAnimLineData> lineList)
            {
                bool isSwitch = CheckLine(lineList, out SpineAnimLineData targetLine);
                if (isSwitch)
                {
                    curActivateState = targetLine.NextState;
                    return true;
                }

                return false;
            }
        }

        private void UpdatePlayingState()
        {
            //应该切换播放对象了
            curPlayingState = curActivateState;
            if (curPlayingState != null)
            {
                bool isPlayed = false;
                foreach (var animClip in curPlayingState.ClipPool)
                {
                    if (!CheckCondition(animClip.Conditions))
                    {
                        continue;
                    }

                    if (curPlayingAnimClip == animClip)
                    {
                        break;
                    }

                    curPlayingAnimClip = animClip;
                    PlayAnim(curPlayingAnimClip);
                    isPlayed = true;
                    break;
                }

                if (!isPlayed)
                {
                    var animClip = curPlayingState.DefaultClip;
                    if (curPlayingAnimClip != animClip)
                    {
                        curPlayingAnimClip = animClip;
                        PlayAnim(curPlayingAnimClip);
                    }
                }


                if (!isPlayed && updatePlayerSpeedFag)
                {
                    animationPlayer.UpdateSpeed(GetPlayerSpeed());
                }
            }

            void PlayAnim(SpineAnimStateData.IClipRef clipRef)
            {
                animationPlayer.PlayAnim(clipRef.AnimName, GetPlayerSpeed());
            }
        }

        private bool CheckLine(IEnumerable<SpineAnimLineData> lineList, out SpineAnimLineData targetLine)
        {
            foreach (var line in lineList)
            {
                if (line.NextState == null)
                {
                    break;
                }

                var trigger = CheckCondition(line.Conditions);
                if (trigger)
                {
                    targetLine = line;
                    return true;
                }

                var isEnd = line.HasExitTime && !animationPlayer.IsPlaying();
                if (isEnd)
                {
                    targetLine = line;
                    return true;
                }
            }

            targetLine = null;
            return false;
        }

        private bool CheckCondition(IEnumerable<SpineAnimConditionData> conditionList)
        {
            if (conditionList == null)
            {
                return false;
            }

            foreach (var condition in conditionList)
            {
                var parameterType = condition.ParameterType;
                var parameterName = condition.ParameterName;
                switch (parameterType)
                {
                    case AnimatorControllerParameterType.Float:
                    {
                        var parameterValue = condition.ParameterValueFloat;
                        var curParameterValue = GetFloat(parameterName);
                        switch (condition.FloatConditionMode)
                        {
                            case SpineAnimFloatConditionMode.Greater:
                                return curParameterValue > parameterValue;
                            case SpineAnimFloatConditionMode.Less:
                                return curParameterValue < parameterValue;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    case AnimatorControllerParameterType.Bool:
                    {
                        var curParameterValue = GetBool(parameterName);
                        switch (condition.AnimBoolConditionMode)
                        {
                            case SpineAnimBoolConditionMode.True:
                                return curParameterValue;
                            case SpineAnimBoolConditionMode.False:
                                return !curParameterValue;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    case AnimatorControllerParameterType.Int:
                    {
                        var parameterValue = condition.ParameterValueInteger;
                        var curParameterValue = GetInteger(parameterName);
                        switch (condition.IntConditionMode)
                        {
                            case SpineAnimIntConditionMode.Greater:
                                return curParameterValue > parameterValue;
                            case SpineAnimIntConditionMode.Less:
                                return curParameterValue < parameterValue;
                            case SpineAnimIntConditionMode.Equals:
                                return curParameterValue == parameterValue;
                            case SpineAnimIntConditionMode.NotEqual:
                                return curParameterValue != parameterValue;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    case AnimatorControllerParameterType.Trigger:
                    {
                        return GetTrigger(parameterName);
                    }
                    default:
                        return false;
                }
            }

            return false;
        }

        public float GetSpeed()
        {
            return globalSpeed;
        }

        public void SetSpeed(float speed)
        {
            if (Mathf.Approximately(globalSpeed, speed))
            {
                return;
            }

            globalSpeed = speed;
            updatePlayerSpeedFag = true;
        }

        public AnimatorControllerParameterType GetParameterType(string parameterName)
        {
            int parameterId = Animator.StringToHash(parameterName);
            return GetParameterType(parameterId);
        }

        public AnimatorControllerParameterType GetParameterType(int parameterId)
        {
            foreach (var parameter in Parameters)
            {
                if (parameter.ParameterId == parameterId)
                {
                    return parameter.ParameterType;
                }
            }

            return AnimatorControllerParameterType.Trigger;
        }

        private float GetPlayerSpeed()
        {
            if (curPlayingState == null)
            {
                return 0;
            }

            float speed = curPlayingState.Speed * globalSpeed;
            if (curPlayingState.SpeedParameterActive)
            {
                var parameterType = GetParameterType(curPlayingState.SpeedParameterId);
                float speedMult;
                switch (parameterType)
                {
                    case AnimatorControllerParameterType.Float:
                        speedMult = GetFloat(curPlayingState.SpeedParameterId);
                        break;
                    case AnimatorControllerParameterType.Int:
                        speedMult = GetInteger(curPlayingState.SpeedParameterId);
                        break;
                    default:
                        speedMult = 0;
                        break;
                }

                speed *= speedMult;
            }

            return speed;
        }

        private void CheckPlayerSpeed(int parameter)
        {
            if (curPlayingState != null && curPlayingState.SpeedParameterActive &&
                parameter == curPlayingState.SpeedParameterId)
            {
                updatePlayerSpeedFag = true;
            }
        }

        public float GetFloat(string key)
        {
            int id = Animator.StringToHash(key);
            return GetFloat(id);
        }

        public float GetFloat(int id)
        {
            floatParameterPool.TryGetValue(id, out float value);
            return value; //默认为0
        }

        public void SetFloat(string key, float value)
        {
            int id = Animator.StringToHash(key);
            SetFloat(id, value);
        }

        public void SetFloat(int id, float value)
        {
            floatParameterPool[id] = value;
            updateFlag = true;
            CheckPlayerSpeed(id);
        }

        public bool GetBool(string key)
        {
            int id = Animator.StringToHash(key);
            return GetBool(id);
        }

        public bool GetBool(int id)
        {
            boolParameterPool.TryGetValue(id, out bool value);
            return value; //默认为false
        }

        public void SetBool(string key, bool value)
        {
            int id = Animator.StringToHash(key);
            SetBool(id, value);
        }

        public void SetBool(int id, bool value)
        {
            boolParameterPool[id] = value;
            updateFlag = true;
        }

        public int GetInteger(string key)
        {
            int id = Animator.StringToHash(key);
            return GetInteger(id);
        }

        public int GetInteger(int id)
        {
            intParameterPool.TryGetValue(id, out int value);
            return value; //默认为0
        }

        public void SetInteger(string key, int value)
        {
            int id = Animator.StringToHash(key);
            SetInteger(id, value);
        }

        public void SetInteger(int id, int value)
        {
            intParameterPool[id] = value;
            updateFlag = true;
            CheckPlayerSpeed(id);
        }

        public void SetTrigger(string key)
        {
            int id = Animator.StringToHash(key);
            SetTrigger(id);
        }

        public void SetTrigger(int id)
        {
            triggerParameterPool[id] = true;
            updateFlag = true;
        }

        public bool GetTrigger(string key)
        {
            int id = Animator.StringToHash(key);
            return GetTrigger(id);
        }

        public bool GetTrigger(int id)
        {
            triggerParameterPool.TryGetValue(id, out bool value);
            return value; //默认为false
        }

        public void ResetTrigger(string key)
        {
            int id = Animator.StringToHash(key);
            ResetTrigger(id);
        }

        public void ResetTrigger(int id)
        {
            triggerParameterPool[id] = false;
            updateFlag = true;
        }

        public void ResetTriggerAll()
        {
            triggerParameterPool.Clear(); //清除全部设置为false，不需要设置为default
            updateFlag = true;
        }
    }
}