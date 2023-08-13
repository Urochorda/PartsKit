using System;
using System.Collections.Generic;

namespace PartsKit
{
    public class FsmController
    {
        private readonly Dictionary<int, FsmState> mFsmPool = new Dictionary<int, FsmState>();
        private FsmState mCurState;
        public event Action<int> onStateChange;

        public FsmController(int defaultStateId, Action onEntry, Action<float> onUpdate, Action<float> onFixUpdate,
            Action onExit)
        {
            AddState(defaultStateId, onEntry, onUpdate, onFixUpdate, onExit);
            SetState(defaultStateId, false);
        }

        public void AddState(int stateId, Action onEntry, Action<float> onUpdate, Action<float> onFixUpdate,
            Action onExit)
        {
            FsmState state = new FsmState(stateId);
            state.onEntry += onEntry;
            state.onUpdate += onUpdate;
            state.onFixUpdate += onFixUpdate;
            state.onExit += onExit;
            mFsmPool[state.StateId] = state;
        }

        public void RemoveState(int stateId)
        {
            mFsmPool.Remove(stateId);
        }

        public void SetState(int stateId, bool isReset)
        {
            if (!isReset && mCurState != null && stateId == mCurState.StateId)
            {
                return;
            }

            if (!mFsmPool.TryGetValue(stateId, out FsmState state))
            {
                return;
            }

            mCurState?.Exit();
            state.Entry();
            mCurState = state;
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
        public int GetCurState()
        {
            return mCurState.StateId;
        }
    }
}