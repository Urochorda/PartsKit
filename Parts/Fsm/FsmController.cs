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

        public FsmController(T defaultStateId, FsmCallBack callBack)
        {
            AddState(defaultStateId, callBack);
            SetState(defaultStateId, false);
        }

        public void AddState(T stateId, FsmCallBack callBack)
        {
            FsmState<T> state = new FsmState<T>(stateId);
            if (callBack != null)
            {
                state.onEntry += callBack.OnEntry;
                state.onUpdate += callBack.OnUpdate;
                state.onFixUpdate += callBack.OnFixUpdate;
                state.onLateUpdate += callBack.OnLateUpdate;
                state.onExit += callBack.OnExit;
            }

            mFsmPool[state.StateId] = state;
        }

        public void RemoveState(T stateId)
        {
            mFsmPool.Remove(stateId);
        }

        public void SetState(T stateId, bool isReset)
        {
            if (!isReset && mCurState != null && stateId.Equals(mCurState.StateId))
            {
                return;
            }

            if (!mFsmPool.TryGetValue(stateId, out FsmState<T> state))
            {
                return;
            }

            mCurState?.Exit();
            mCurState = state;
            state.Entry();
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