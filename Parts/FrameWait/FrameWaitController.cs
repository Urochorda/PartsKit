using System;
using System.Collections.Generic;
using UnityEngine;

namespace PartsKit
{
    public class FrameWaitController : MonoBehaviour
    {
        private class FrameWaitTask
        {
            private readonly RefInt id;
            public IReadOnlyRefInt Id => id;
            private readonly int startFrame;
            private int remainingFrames;
            private readonly Action callback;

            public bool IsCompleted { get; private set; }
            public bool IsCanceled { get; private set; }

            public FrameWaitTask(int idNumber, int frameCount, int curFrame, Action action)
            {
                id = new RefInt(idNumber);
                startFrame = curFrame;
                remainingFrames = frameCount;
                callback = action;
                IsCompleted = false;
                CheckComplete();
            }

            public bool Update(int curFrame)
            {
                if (IsCompleted || startFrame == curFrame || IsCanceled)
                {
                    return true;
                }

                remainingFrames--;
                CheckComplete();
                return false;
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
                id.Value = NoValid;
            }

            public void OnCancel()
            {
                id.Value = NoValid;
                IsCanceled = true;
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
            int curFrame = Time.frameCount;
            // 迭代任务列表并更新每个任务
            for (int i = tasks.Count - 1; i >= 0; i--)
            {
                var task = tasks[i];
                if (task.Update(curFrame))
                {
                    tasks.RemoveAt(i);
                }
            }
        }

        public IReadOnlyRefInt WaitForFrames(int frameCount, Action action)
        {
            taskId++;
            int curFrame = Time.frameCount;
            var task = new FrameWaitTask(taskId, frameCount, curFrame, action);
            tasks.Add(task);
            return task.Id;
        }

        public void CancelWait(IReadOnlyRefInt id)
        {
            if (id == null || id.Value == NoValid)
            {
                return;
            }

            int idValue = id.Value;
            int index = tasks.FindIndex(item => item.Id.Value == idValue);
            if (index < 0)
            {
                return;
            }

            tasks[index].OnCancel();
        }

        public void ImmediatelyCompleted(IReadOnlyRefInt id)
        {
            int idValue = id.Value;
            int targetIndex = tasks.FindIndex(item => item.Id.Value == idValue);
            if (targetIndex >= 0)
            {
                tasks[targetIndex].ImmediatelyCompleted();
            }
        }

        public void CancelAll()
        {
            foreach (var task in tasks)
            {
                task.OnCancel();
            }
        }
    }
}