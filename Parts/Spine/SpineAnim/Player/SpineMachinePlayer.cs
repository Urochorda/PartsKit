using System;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Serialization;

namespace PartsKit
{
    public class SpineMachinePlayer : MonoBehaviour
    {
        // 不使用这个了，统一使用Animator.StringToHash，方便业务层无缝切换Animator和SpineMachinePlayer
        // public static int StringToHash(string value)
        // {
        //     return Animator.StringToHash(value);
        // }

        [SerializeField] private SpineMachineData machineData;
        [SerializeField] private SkeletonAnimation skeletonAnimation;
        [SerializeField] private bool isDisableStop = true; //默认为true，重新开启时会重置状态

        private readonly Dictionary<int, float> floatParameterPool = new Dictionary<int, float>();
        private readonly Dictionary<int, int> intParameterPool = new Dictionary<int, int>();
        private readonly Dictionary<int, bool> boolParameterPool = new Dictionary<int, bool>();
        private readonly Dictionary<int, bool> triggerParameterPool = new Dictionary<int, bool>();
        public IReadOnlyList<SpineMachineParameter> Parameter => machineData.Parameter;

        private SpineAnimPlayer spineAnimPlayer;

        private SpineStateData curActivateState;
        private SpineStateData curPlayingState;
        private ISpineClipData curPlayingClip;
        private bool isPlaying;
        private bool updateFag;

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
            InitParameter();
            curActivateState = machineData.EnterState;
            curPlayingState = null;
            spineAnimPlayer ??= new SpineAnimPlayer(skeletonAnimation);
        }

        private void Stop()
        {
            if (!isPlaying)
            {
                return;
            }

            isPlaying = false;
            spineAnimPlayer.StopAnimationAll();
        }

        public bool HasParameterOfType(string key, AnimatorControllerParameterType type)
        {
            return machineData.HasParameterOfType(key, type);
        }

        private void InitParameter()
        {
            floatParameterPool.Clear();
            intParameterPool.Clear();
            boolParameterPool.Clear();
            triggerParameterPool.Clear();
            var defaultPar = Parameter;
            foreach (var parameter in defaultPar)
            {
                int parameterHash = Animator.StringToHash(parameter.ParameterName);
                switch (parameter.ParameterType)
                {
                    case AnimatorControllerParameterType.Float:
                        floatParameterPool[parameterHash] = parameter.DefaultValueFloat;
                        break;
                    case AnimatorControllerParameterType.Bool:
                        boolParameterPool[parameterHash] = parameter.DefaultValueBool;
                        break;
                    case AnimatorControllerParameterType.Int:
                        intParameterPool[parameterHash] = parameter.DefaultValueInteger;
                        break;
                    case AnimatorControllerParameterType.Trigger:
                        triggerParameterPool[parameterHash] = parameter.DefaultValueTrigger;
                        break;
                }
            }

            updateFag = true;
        }

        private void UpdateState()
        {
            if (!isPlaying)
            {
                return;
            }

            UpdateStateLine();
            UpdatePlayingState();
            ResetTriggerAll();
            updateFag = false;
        }

        private void UpdateStateLine()
        {
            var anyState = machineData.AnyState;
            var activateState = curActivateState;

            //从Any状态切换到下一个状态，切换成功则不在同一帧检测下一个状态
            if (TryNext(anyState))
            {
                return;
            }

            //检测当前激活的状态切换
            TryNext(activateState.LinePool);

            bool TryNext(IEnumerable<SpineLineData> lineList)
            {
                bool isSwitch = CheckLine(lineList, out SpineLineData targetLine);
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

                    if (curPlayingClip == animClip)
                    {
                        break;
                    }

                    curPlayingClip = animClip;
                    PlayAnim(animClip.SetAnimPool, animClip.AddAnimPool);
                    isPlayed = true;
                    break;
                }

                if (!isPlayed)
                {
                    var animClip = curPlayingState.DefaultClip;
                    if (curPlayingClip != animClip)
                    {
                        curPlayingClip = animClip;
                        PlayAnim(curPlayingClip.SetAnimPool, curPlayingClip.AddAnimPool);
                    }
                }
            }

            void PlayAnim(IReadOnlyList<SpineSetAnimData> setAnim,
                IReadOnlyList<SpineAddAnimData> addAnim)
            {
                foreach (var setAnimData in setAnim)
                {
                    spineAnimPlayer.PlayAnimation(setAnimData.TrackIndex, setAnimData.AnimName, setAnimData.IsLoop);
                }

                foreach (var addAnimData in addAnim)
                {
                    spineAnimPlayer.AddAnimation(addAnimData.TrackIndex, addAnimData.AnimName, addAnimData.IsLoop,
                        addAnimData.Delay);
                }
            }
        }

        private bool CheckLine(IEnumerable<SpineLineData> lineList, out SpineLineData targetLine)
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

                var isEnd = line.HasExitTime && !spineAnimPlayer.IsPlaying();
                if (isEnd)
                {
                    targetLine = line;
                    return true;
                }
            }

            targetLine = null;
            return false;
        }

        private bool CheckCondition(IEnumerable<SpineConditionData> conditionList)
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
                            case SpineFloatConditionMode.Greater:
                                return curParameterValue > parameterValue;
                            case SpineFloatConditionMode.Less:
                                return curParameterValue < parameterValue;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    case AnimatorControllerParameterType.Bool:
                    {
                        var curParameterValue = GetBool(parameterName);
                        switch (condition.BoolConditionMode)
                        {
                            case SpineBoolConditionMode.True:
                                return curParameterValue;
                            case SpineBoolConditionMode.False:
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
                            case SpineIntConditionMode.Greater:
                                return curParameterValue > parameterValue;
                            case SpineIntConditionMode.Less:
                                return curParameterValue < parameterValue;
                            case SpineIntConditionMode.Equals:
                                return curParameterValue == parameterValue;
                            case SpineIntConditionMode.NotEqual:
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
            return spineAnimPlayer.GetTimeScale();
        }

        public void SetSpeed(float speed)
        {
            spineAnimPlayer.SetTimeScale(speed);
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
            updateFag = true;
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
            updateFag = true;
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
            updateFag = true;
        }

        public void SetTrigger(string key)
        {
            int id = Animator.StringToHash(key);
            SetTrigger(id);
        }

        public void SetTrigger(int id)
        {
            triggerParameterPool[id] = true;
            updateFag = true;
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
            updateFag = true;
        }

        public void ResetTriggerAll()
        {
            triggerParameterPool.Clear(); //清除全部设置为false，不需要设置为default
            updateFag = true;
        }
    }
}