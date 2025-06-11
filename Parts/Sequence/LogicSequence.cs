using System;
using System.Collections.Generic;
using System.Linq;

namespace PartsKit
{
    /// <summary>
    /// 为什么不直接用DoTween的队列：
    /// 1.https://github.com/Demigiant/dotween/issues/40
    /// 2.支持一些其他类型的队列Dynamic、Condition。。。
    /// </summary>
    public class LogicSequence : LogicSequenceBase
    {
        private readonly List<LogicSequenceBase> sequencePool = new List<LogicSequenceBase>();
        public bool IsSequenced { get; private set; }
        public LogicSequenceBase SequenceParent { get; private set; }

        protected override void OnGet()
        {
            //初始化
            IsSequenced = false;
            SequenceParent = null;
            sequencePool.Clear();
        }

        protected override void OnPlay()
        {
            //按顺序开始一个子队列
            foreach (var sequence in sequencePool)
            {
                if (sequence == null)
                {
                    continue;
                }

                sequence.Play();
                if (sequence.IsRunning)
                {
                    break;
                }
            }
        }

        protected override void OnKill()
        {
            foreach (var sequence in sequencePool)
            {
                if (sequence == null)
                {
                    continue;
                }

                sequence.Kill();
            }
        }

        protected override void OnReset()
        {
            foreach (var sequence in sequencePool)
            {
                if (sequence == null)
                {
                    continue;
                }

                sequence.Reset();
            }

            SequenceParent = null;
            IsSequenced = false;
            sequencePool.Clear();
        }

        protected override void OnPause(bool isPause)
        {
            foreach (var sequence in sequencePool)
            {
                sequence.SetPause(false);
            }
        }

        protected override bool OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            for (var i = 0; i < sequencePool.Count; i++)
            {
                var sequence = sequencePool[i];
                if (sequence == null)
                {
                    continue;
                }

                //是否已经播放播放
                bool lastRunning = sequence.IsRunning;

                //更新逻辑
                sequence.Update(deltaTime, unscaledDeltaTime);

                //如果当前还在执行则不继续向下走
                if (sequence.IsRunning)
                {
                    break;
                }

                //尝试播放一个队列
                if (lastRunning)
                {
                    int nextIndex = i + 1;
                    for (int j = nextIndex; j < sequencePool.Count; j++)
                    {
                        var nextSeq = sequencePool[j];
                        if (nextSeq != null)
                        {
                            nextSeq.Play();
                            if (nextSeq.IsRunning)
                            {
                                break;
                            }
                        }
                    }

                    break;
                }
            }

            //全部为空或者不处于运行状态就算完成
            return sequencePool.All(item => item == null || !item.IsRunning);
        }

        private void SetSequenced(LogicSequenceBase sequence)
        {
            if (sequence is not LogicSequence customSequence)
            {
                return;
            }

            customSequence.IsSequenced = true;
            customSequence.SequenceParent = this;
        }

        public void Append(LogicSequenceBase sequence)
        {
            if (sequence == null)
            {
                return;
            }

            SetSequenced(sequence); //设置是否属于队列
            sequencePool.Add(sequence);
        }

        public void AppendDynamic(Func<LogicSequenceBase> callback)
        {
            LogicSequenceDynamic sequenceDynamic = new LogicSequenceDynamic(callback);
            Append(sequenceDynamic);
        }

        public void Join(LogicSequenceBase sequence)
        {
            if (sequence == null)
            {
                return;
            }

            SetSequenced(sequence); //设置是否属于队列
            if (sequencePool.Count <= 0)
            {
                //首个为空，追加一个join
                var joinSequence = new LogicSequenceJoin();
                joinSequence.Add(sequence);
                Append(joinSequence);
            }
            else if (sequencePool[^1] is LogicSequenceJoin joinSequence)
            {
                //最后一个就是join，直接join添加
                joinSequence.Add(sequence);
            }
            else
            {
                //最后一个不是join类型，将最后一个替换为join
                joinSequence = new LogicSequenceJoin();
                joinSequence.Add(sequencePool[^1]);
                joinSequence.Add(sequence);
                SetSequenced(joinSequence); //设置是否属于队列
                sequencePool[^1] = joinSequence;
            }
        }

        public void JoinDynamic(Func<LogicSequenceBase> callback)
        {
            LogicSequenceDynamic sequenceDynamic = new LogicSequenceDynamic(callback);
            Join(sequenceDynamic);
        }

        public void AppendInterval(float interval)
        {
            LogicSequenceInterval sequenceInterval = new LogicSequenceInterval(interval);
            Append(sequenceInterval);
        }

        public void JoinInterval(float interval)
        {
            LogicSequenceInterval sequenceInterval = new LogicSequenceInterval(interval);
            Join(sequenceInterval);
        }

        public void AppendCallback(Action callback)
        {
            LogicSequenceCallback sequenceCallback = new LogicSequenceCallback(callback);
            Append(sequenceCallback);
        }

        public void AppendCondition(Func<bool> condition)
        {
            LogicSequenceCondition sequenceCondition = new LogicSequenceCondition(condition);
            Append(sequenceCondition);
        }

        public void JoinCondition(Func<bool> condition)
        {
            LogicSequenceCondition sequenceCondition = new LogicSequenceCondition(condition);
            Join(sequenceCondition);
        }
    }
}