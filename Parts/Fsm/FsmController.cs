using System;
using System.Collections.Generic;

namespace PartsKit
{
    public class FsmCallBack
    {
        public Action OnEntry { get; set; }
        public Action<float, float> OnUpdate { get; set; }
        public Action<float, float> OnFixUpdate { get; set; }
        public Action<float, float> OnLateUpdate { get; set; }
        public Action OnExit { get; set; }
    }

    public class FsmController<T>
    {
        private readonly Dictionary<T, FsmState<T>> mFsmPool = new Dictionary<T, FsmState<T>>();
        private FsmState<T> mCurState;
        public event Action<T> onStateChange;
        private bool isSwitchingState;

        public FsmController(T defaultStateId, FsmCallBack callBack)
        {
            isSwitchingState = false;
            AddState(defaultStateId, callBack);
            SetState(defaultStateId, false);
        }

        public FsmController(FsmState<T> defaultState)
        {
            isSwitchingState = false;
            AddState(defaultState);
            SetState(defaultState.StateId, false);
        }

        public void AddState(T stateId, FsmCallBack callBack)
        {
            FsmCallBackState<T> state = new FsmCallBackState<T>(stateId);
            if (callBack != null)
            {
                state.onEntry += callBack.OnEntry;
                state.onUpdate += callBack.OnUpdate;
                state.onFixUpdate += callBack.OnFixUpdate;
                state.onLateUpdate += callBack.OnLateUpdate;
                state.onExit += callBack.OnExit;
            }

            AddState(state);
        }

        public void AddState(FsmState<T> state)
        {
            mFsmPool[state.StateId] = state;
        }

        public void RemoveState(T stateId)
        {
            mFsmPool.Remove(stateId);
        }

        public void SetState(T stateId, bool isReset)
        {
            if (isSwitchingState)
            {
                CustomLog.LogError("设置状态时，正在切换状态，可能导致堆栈溢出或者逻辑混乱，请检车问题");
                return;
            }

            if (!isReset && mCurState != null && stateId.Equals(mCurState.StateId))
            {
                return;
            }

            if (!mFsmPool.TryGetValue(stateId, out FsmState<T> state))
            {
                return;
            }

            isSwitchingState = true;
            mCurState?.Exit();
            mCurState = state;
            state.Entry();
            isSwitchingState = false;

            onStateChange?.Invoke(stateId);
        }

        public void UpdateState(float deltaTime, float unscaledDeltaTime)
        {
            mCurState.Update(deltaTime, unscaledDeltaTime);
        }

        public void FixUpdateState(float deltaTime, float unscaledDeltaTime)
        {
            mCurState.FixUpdate(deltaTime, unscaledDeltaTime);
        }

        public void LateUpdateState(float deltaTime, float unscaledDeltaTime)
        {
            mCurState.LateUpdate(deltaTime, unscaledDeltaTime);
        }

        /// <summary>
        /// 获取当前状态，无状态则获取
        /// </summary>
        /// <returns></returns>
        public T GetCurState()
        {
            return mCurState.StateId;
        }
    }
}