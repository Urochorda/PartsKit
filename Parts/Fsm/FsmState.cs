using System;

namespace PartsKit
{
    public class FsmState<T>
    {
        public T StateId { get; }

        public FsmState(T stateId)
        {
            StateId = stateId;
        }

        public virtual void Entry()
        {
        }

        public virtual void Update(float deltaTime, float unscaledDeltaTime)
        {
        }

        public virtual void FixUpdate(float deltaTime, float unscaledDeltaTime)
        {
        }

        public virtual void LateUpdate(float deltaTime, float unscaledDeltaTime)
        {
        }

        public virtual void Exit()
        {
        }
    }

    public class FsmCallBackState<T> : FsmState<T>
    {
        public event Action onEntry;
        public event Action<float, float> onUpdate;
        public event Action<float, float> onFixUpdate;
        public event Action<float, float> onLateUpdate;
        public event Action onExit;

        public FsmCallBackState(T stateId) : base(stateId)
        {
        }

        public override void Entry()
        {
            onEntry?.Invoke();
        }

        public override void Update(float deltaTime, float unscaledDeltaTime)
        {
            onUpdate?.Invoke(deltaTime, unscaledDeltaTime);
        }

        public override void FixUpdate(float deltaTime, float unscaledDeltaTime)
        {
            onFixUpdate?.Invoke(deltaTime, unscaledDeltaTime);
        }

        public override void LateUpdate(float deltaTime, float unscaledDeltaTime)
        {
            onLateUpdate?.Invoke(deltaTime, unscaledDeltaTime);
        }

        public override void Exit()
        {
            onExit?.Invoke();
        }
    }
}