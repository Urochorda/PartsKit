using System.Collections.Generic;
using UnityEngine;

namespace PartsKit
{
    public class LogicSequenceController : MonoBehaviour
    {
        public static void Init()
        {
            if (Instance != null)
            {
                return;
            }

            var insObj = new GameObject("LogicSequenceController");
            Instance = insObj.AddComponent<LogicSequenceController>();
            DontDestroyOnLoad(Instance);
        }

        public static LogicSequenceController Instance { get; private set; }

        private readonly List<LogicSequence> validSequencePool = new List<LogicSequence>();
        private readonly Stack<LogicSequence> cacheSequencePool = new Stack<LogicSequence>();

        public void LateUpdate()
        {
            float deltaTime = Time.deltaTime;
            float unscaledDeltaTime = Time.unscaledDeltaTime;
            UpdateSequence(deltaTime, unscaledDeltaTime);
        }

        public LogicSequence GetSequence()
        {
            LogicSequence sequence;
            if (cacheSequencePool.Count > 0)
            {
                sequence = cacheSequencePool.Pop();
            }
            else
            {
                sequence = new LogicSequence();
            }

            sequence.Get();
            validSequencePool.Add(sequence);
            return sequence;
        }

        private void UpdateSequence(float deltaTime, float unscaledDeltaTime)
        {
            for (var i = validSequencePool.Count - 1; i >= 0; i--)
            {
                var sequence = validSequencePool[i];

                if (!sequence.IsValid)
                {
                    sequence.Reset();

                    int lastIndex = validSequencePool.Count - 1;
                    if (i != lastIndex)
                    {
                        validSequencePool[i] = validSequencePool[lastIndex];
                    }

                    validSequencePool.RemoveAt(lastIndex);

                    cacheSequencePool.Push(sequence);
                    continue;
                }

                if (sequence.IsSequenced)
                {
                    continue;
                }

                sequence.Update(deltaTime, unscaledDeltaTime);
            }
        }
    }
}