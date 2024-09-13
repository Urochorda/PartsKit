using System;

namespace PartsKit
{
    public class FsmState<T>
    {
        public T StateId { get; }
        public event Action onEntry;
        public event Action<float, float> onUpdate;
        public event Action<float, float> onFixUpdate;
        public event Action<float, float> onLateUpdate;
        public event Action onExit;

        public FsmState(T stateId)
        {
            StateId = stateId;
        }

        public void Entry()
        {
            onEntry?.Invoke();
        }

        public void Update(float deltaTime, float unscaledDeltaTime)
        {
            onUpdate?.Invoke(deltaTime, unscaledDeltaTime);
        }

        public void FixUpdate(float deltaTime, float unscaledDeltaTime)
        {
            onFixUpdate?.Invoke(deltaTime, unscaledDeltaTime);
        }

        public void LateUpdate(float deltaTime, float unscaledDeltaTime)
        {
            onLateUpdate?.Invoke(deltaTime, unscaledDeltaTime);
        }

        public void Exit()
        {
            onExit?.Invoke();
        }
    }
}