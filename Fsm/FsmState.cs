using System;

namespace PartsKit
{
    public class FsmState<T>
    {
        public T StateId { get; }
        public event Action onEntry;
        public event Action<float> onUpdate;
        public event Action<float> onFixUpdate;
        public event Action<float> onLateUpdate;
        public event Action onExit;

        public FsmState(T stateId)
        {
            StateId = stateId;
        }

        public void Entry()
        {
            onEntry?.Invoke();
        }

        public void Update(float deltaTime)
        {
            onUpdate?.Invoke(deltaTime);
        }

        public void FixUpdate(float deltaTime)
        {
            onFixUpdate?.Invoke(deltaTime);
        }

        public void LateUpdate(float deltaTime)
        {
            onLateUpdate?.Invoke(deltaTime);
        }

        public void Exit()
        {
            onExit?.Invoke();
        }
    }
}