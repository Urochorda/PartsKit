using System;
using System.Collections.Generic;

namespace PartsKit
{
    public class FsmController<T>
    {
        private readonly Dictionary<T, FsmState<T>> mFsmPool = new Dictionary<T, FsmState<T>>();
        private FsmState<T> mCurState;
        public event Action<T> onStateChange;

        public FsmController(T defaultStateId, Action onEntry, Action<float> onUpdate, Action<float> onFixUpdate,
            Action onExit)
        {
            AddState(defaultStateId, onEntry, onUpdate, onFixUpdate, onExit);
            SetState(defaultStateId, false);
        }

        public void AddState(T stateId, Action onEntry, Action<float> onUpdate, Action<float> onFixUpdate,
            Action onExit)
        {
            FsmState<T> state = new FsmState<T>(stateId);
            state.onEntry += onEntry;
            state.onUpdate += onUpdate;
            state.onFixUpdate += onFixUpdate;
            state.onExit += onExit;
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

        public void UpdateState(float deltaTime)
        {
            mCurState.Update(deltaTime);
        }

        public void FixUpdateState(float deltaTime)
        {
            mCurState.FixUpdate(deltaTime);
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