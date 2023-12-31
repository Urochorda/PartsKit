using System;
using DG.Tweening;
using UnityEngine;

namespace PartsKit
{
    public abstract class LoadDialogueFun : MonoBehaviour
    {
        public abstract DialogueNodeConfig LoadNodeConfig(int nodeKey);
        public abstract IDialogueShowPanel LoadShowPanel();
    }

    /// <summary>
    /// 简单的对话控制器（只有最简单的播放对话功能，不提供选择分支功能）
    /// </summary>
    public class DialogueController : PartsKitBehaviour
    {
        [SerializeField] private LoadDialogueFun loadDialogueFun;
        [SerializeField] private float charDuration = 0.2f;
        [SerializeField] private float advanceTimeScale = 5f;
        [SerializeField] private bool defaultHideInEnd = true;

        public event Action OnPlay;
        public event Action OnStop;
        private DialogueNodeConfig curNode;
        public IDialogueShowPanel CurShowPanel { get; private set; }
        private int curGroupIndex;
        private Action curOnComplete;
        private Tweener curPlayNodeAnim;
        public bool IsPlayingDialogue { get; private set; }
        public bool IsPlayingNode { get; private set; }
        public bool CurHideInEnd { get; private set; }

        protected override void OnInit()
        {
            CurHideInEnd = defaultHideInEnd;
        }

        protected override void OnDeInit()
        {
            StopDialogue();
            OnPlay = null;
            OnStop = null;
        }

        /// <summary>
        /// 播放对话
        /// </summary>
        public bool TryPlayDialogue(int nodeConfigKey, bool isForce, Action onComplete, Action onBeginPlay)
        {
            if (IsPlayingDialogue && !isForce) //正在播放对话&&非强制更新
            {
                return false;
            }

            StopDialogue(); //结束上次对话
            curNode = loadDialogueFun.LoadNodeConfig(nodeConfigKey);
            CurShowPanel = loadDialogueFun.LoadShowPanel();
            curGroupIndex = 0;
            curOnComplete = onComplete;
            CurShowPanel.Show();
            CurShowPanel.BeginPlay();
            IsPlayingDialogue = true;
            onBeginPlay?.Invoke();
            OnPlay?.Invoke();
            return DoPlayNode(curGroupIndex);
        }

        /// <summary>
        /// 结束对话
        /// </summary>
        public void StopDialogue()
        {
            if (!IsPlayingDialogue)
            {
                return;
            }

            IsPlayingDialogue = false;
            StopCurNode();
            CurShowPanel.EndPlay();
            if (CurHideInEnd)
            {
                CurShowPanel.Hide();
            }

            CurHideInEnd = defaultHideInEnd;
            curOnComplete?.Invoke();
            curOnComplete = null;
            OnStop?.Invoke();
        }

        /// <summary>
        /// 尝试播放下一个节点
        /// </summary>
        public bool TryPlayNextNode(bool isAdvance)
        {
            if (isAdvance && TryAdvancePlayNode())
            {
                return true;
            }

            if (DoPlayNode(curGroupIndex + 1))
            {
                curGroupIndex++;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 结束节点播放
        /// </summary>
        public void StopCurNode()
        {
            curPlayNodeAnim?.Kill();
        }

        /// <summary>
        /// 尝试加速节点播放
        /// </summary>
        public bool TryAdvancePlayNode()
        {
            if (!IsPlayingNode) //没有播放则不加速
            {
                return false;
            }

            if (curPlayNodeAnim.IsActive())
            {
                curPlayNodeAnim.timeScale = advanceTimeScale;
            }

            return true;
        }

        private bool DoPlayNode(int index)
        {
            if (CurShowPanel == null || curNode == null || index >= curNode.NodeGroups.Count)
            {
                StopDialogue(); //结束对话
                return false;
            }

            DialogNodeGroup group = curNode.NodeGroups[index];
            CurShowPanel.SetCharacters(group.Characters);

            IsPlayingNode = true;
            string targetString = group.ContentKey.GetLocalizedString();
            int endIndex = targetString.Length - 1;
            float duration = targetString.Length * charDuration;
            curPlayNodeAnim?.Kill();
            curPlayNodeAnim = DOTween.To(() => 0, newIndex =>
            {
                string content = targetString.Substring(0, newIndex + 1);
                CurShowPanel.SetContent(content);
            }, endIndex, duration).OnKill(() =>
            {
                IsPlayingNode = false;
                curPlayNodeAnim = null;
            });
            return true;
        }

        /// <summary>
        /// 临时设置是否在对话结束时隐藏表现（）
        /// </summary>
        public void SetTempHideInEnd(bool hideInEnd)
        {
            if (!IsPlayingDialogue)
            {
                return;
            }

            CurHideInEnd = hideInEnd;
        }

        /// <summary>
        /// 设置是否在对话结束时隐藏表现
        /// </summary>
        public void SetHideInEnd(bool hideInEnd)
        {
            defaultHideInEnd = hideInEnd;
            CurHideInEnd = defaultHideInEnd;
        }
    }
}