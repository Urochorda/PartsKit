using System;
using System.Collections.Generic;
using PartsKit;
using UnityEngine;

namespace PartsKit
{
    public class FrameWaitController : MonoBehaviour
    {
        private class FrameWaitTask
        {
            public int Id { get; }
            private int remainingFrames;
            private readonly Action callback;

            public bool IsCompleted { get; private set; }

            public FrameWaitTask(int id, int frameCount, Action action)
            {
                Id = id;
                remainingFrames = frameCount;
                callback = action;
                IsCompleted = false;
                CheckComplete();
            }

            public void Update()
            {
                if (IsCompleted)
                {
                    return;
                }

                remainingFrames--;
                CheckComplete();
            }

            private void CheckComplete()
            {
                if (remainingFrames <= 0)
                {
                    OnComplete();
                }
            }

            private void OnComplete()
            {
                if (IsCompleted)
                {
                    return;
                }

                callback?.Invoke();
                IsCompleted = true;
            }

            public void ImmediatelyCompleted()
            {
                OnComplete();
            }
        }

        public const int NoValid = -1;

        // 等待任务列表
        private readonly List<FrameWaitTask> tasks = new List<FrameWaitTask>();

        private int taskId = NoValid + 1;

        private void Update()
        {
            // 迭代任务列表并更新每个任务
            for (int i = tasks.Count - 1; i >= 0; i--)
            {
                tasks[i].Update();
                if (tasks[i].IsCompleted)
                {
                    tasks.RemoveAt(i);
                }
            }
        }

        public int WaitForFrames(int frameCount, Action action)
        {
            taskId++;
            var task = new FrameWaitTask(taskId, frameCount, action);
            tasks.Add(task);
            return taskId;
        }

        public void CancelWait(ref int id)
        {
            int idValue = id;
            tasks.RemoveMatchDisorder(item => item.Id == idValue);
            id = -1;
        }

        public void ImmediatelyCompleted(ref int id)
        {
            int idValue = id;
            int targetIndex = tasks.FindIndex(item => item.Id == idValue);
            if (targetIndex >= 0)
            {
                tasks[targetIndex].ImmediatelyCompleted();
            }

            id = -1;
        }

        public void CancelAll()
        {
            tasks.Clear();
        }
    }
}