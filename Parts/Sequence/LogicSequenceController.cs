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

        private readonly List<LogicSequence> allSequence = new List<LogicSequence>();

        public void Update()
        {
            float deltaTime = Time.deltaTime;
            float unscaledDeltaTime = Time.unscaledDeltaTime;
            UpdateSequence(deltaTime, unscaledDeltaTime);
        }

        public void AddSequence(LogicSequence sequence)
        {
            if (sequence.IsSequenced) //只允许根节点加入
            {
                return;
            }

            if (allSequence.Contains(sequence))
            {
                return;
            }

            allSequence.Add(sequence);

            UpdateSequence(0, 0);
        }

        private void UpdateSequence(float deltaTime, float unscaledDeltaTime)
        {
            foreach (var sequence in allSequence)
            {
                sequence.Update(deltaTime, unscaledDeltaTime);
            }
        }
    }
}