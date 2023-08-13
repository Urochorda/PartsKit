using System;

namespace PartsKit
{
    public class FsmState
    {
        public int StateId { get; }
        public event Action onEntry;
        public event Action<float> onUpdate;
        public event Action<float> onFixUpdate;
        public event Action onExit;

        public FsmState(int stateId)
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

        public void Exit()
        {
            onExit?.Invoke();
        }
    }
}