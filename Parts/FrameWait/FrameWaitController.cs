using System;
using System.Collections.Generic;
using PartsKit;
using UnityEngine;

namespace Plugins
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
                if (IsCompleted) return;
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
                callback?.Invoke();
                IsCompleted = true;
            }
        }

        // 等待任务列表
        private readonly List<FrameWaitTask> tasks = new List<FrameWaitTask>();

        private int taskId;

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

        public void CancelWait(int id)
        {
            tasks.RemoveMatchDisorder(item => item.Id == id);
        }

        public void CancelAll()
        {
            tasks.Clear();
        }
    }
}