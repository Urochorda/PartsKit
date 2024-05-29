using System;
using System.Collections.Generic;

namespace PartsKit
{
    public class RedDot
    {
        public string Key { get; }
        public int SumCount { get; private set; }
        public int SelfCount { get; private set; }
        public RedDot Parent { get; private set; }
        private readonly List<RedDot> children;
        private readonly IRedDotCalculator calculator;
        private bool isRemove;

        public RedDot(string key, IRedDotCalculator cal)
        {
            Key = key;
            calculator = cal;
            children = new List<RedDot>();
        }

        public void AddChild(RedDot child)
        {
            if (!children.Contains(child))
            {
                children.Add(child);
                child.Parent = this;
            }
        }

        public void RemoveChild(RedDot child)
        {
            if (children.Contains(child))
            {
                children.Remove(child);
                child.Parent = this;
            }
        }

        public List<RedDot> GetChildren()
        {
            return new List<RedDot>(children);
        }

        public int GetChildrenCount()
        {
            return children.Count;
        }

        public void UpdateCount()
        {
            if (isRemove)
            {
                SelfCount = 0;
                SumCount = SelfCount;
            }
            else
            {
                SelfCount = calculator?.Calculator(this) ?? 0;
                SumCount = SelfCount;
                foreach (RedDot child in children)
                {
                    SumCount += child.SumCount;
                }
            }
        }

        public void SetRemove()
        {
            isRemove = true;
        }
    }

    public class RedDotController
    {
        public static Func<RedDotController> GetIns { get; private set; }
        private readonly RedDot redDotRoot = new RedDot(String.Empty, null);
        private readonly Dictionary<string, RedDot> redDots = new Dictionary<string, RedDot>();


        public void Init(Func<RedDotController> onGetIns)
        {
            GetIns = onGetIns;
        }

        public void DeInit()
        {
            GetIns = null;
        }

        public void RegisterRedDot(string key, IRedDotCalculator calculator)
        {
            if (redDots.ContainsKey(key))
            {
                UnregisterRedDot(key);
            }

            RedDot newRedDot = new RedDot(key, calculator);
            redDots[key] = newRedDot;

            string parentKey = GetParentKey(key);
            if (!string.IsNullOrEmpty(parentKey))
            {
                //没有父节点则自动注册父节点
                if (!redDots.TryGetValue(parentKey, out RedDot parentRedDot))
                {
                    throw new Exception($"{key} parent is null");
                }

                parentRedDot.AddChild(newRedDot);
            }
            else
            {
                redDotRoot.AddChild(newRedDot);
            }
        }

        public void UnregisterRedDot(string key)
        {
            if (redDots.ContainsKey(key))
            {
                RedDot redDot = redDots[key];

                // 切断与父节点的联系
                if (redDot.Parent != null)
                {
                    redDot.Parent.RemoveChild(redDot);
                }

                // 移除所有子节点
                foreach (var child in redDot.GetChildren())
                {
                    UnregisterRedDot(child.Key);
                }

                redDots.Remove(key);

                //当前的redDot无父无子，只会更新自己，被移除了count为0
                redDot.SetRemove();
                UpdateCounts(redDot);
            }
        }

        public int GetRedDotCount(string key)
        {
            if (redDots.TryGetValue(key, out var redDot))
            {
                return redDot.SumCount;
            }

            return 0;
        }

        public void CalculatorAll()
        {
            foreach (var redDot in redDots)
            {
                if (redDot.Value.GetChildrenCount() <= 0)
                {
                    UpdateCounts(redDot.Value);
                }
            }
        }

        public void Calculator(string key)
        {
            if (redDots.TryGetValue(key, out var redDot))
            {
                UpdateCounts(redDot);
            }
        }

        private void UpdateCounts(RedDot redDot)
        {
            RedDotUpdateEvent eventData = new RedDotUpdateEvent();

            redDot.UpdateCount();
            eventData.RedDot.Add(redDot);

            RedDot parent = redDot.Parent;
            while (parent != null)
            {
                parent.UpdateCount();
                eventData.RedDot.Add(parent);
                parent = parent.Parent;
            }

            TypeEventSystem.Global.Send(eventData);
        }

        private string GetParentKey(string key)
        {
            int lastDotIndex = key.LastIndexOf('.');
            if (lastDotIndex > 0)
            {
                return key.Substring(0, lastDotIndex);
            }

            return null;
        }
    }
}