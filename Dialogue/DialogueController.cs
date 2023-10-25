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
    public class DialogueController : MonoBehaviour
    {
        [SerializeField] private LoadDialogueFun loadDialogueFun;
        [SerializeField] private float charDuration = 0.2f;
        [SerializeField] private float advanceTimeScale = 5f;

        private DialogueNodeConfig curNode;
        private IDialogueShowPanel curShowPanel;
        private int curGroupIndex;
        private Action curOnComplete;
        private Tweener curPlayNodeAnim;
        private bool isPlayingDialogue;
        private bool isPlayingNode;

        /// <summary>
        /// 播放对话
        /// </summary>
        public bool PlayDialogue(int nodeConfigKey, bool isForce, Action onComplete)
        {
            if (isPlayingDialogue && !isForce) //正在播放对话&&非强制更新
            {
                return false;
            }

            StopDialogue(); //结束上次对话
            curNode = loadDialogueFun.LoadNodeConfig(nodeConfigKey);
            curShowPanel = loadDialogueFun.LoadShowPanel();
            curGroupIndex = 0;
            curOnComplete = onComplete;
            curShowPanel.Show();
            isPlayingDialogue = true;
            return DoPlayNode(curGroupIndex);
        }

        /// <summary>
        /// 结束对话
        /// </summary>
        public void StopDialogue()
        {
            if (!isPlayingDialogue)
            {
                return;
            }

            isPlayingDialogue = false;
            StopCurNode();
            curShowPanel.Hide();
            curOnComplete?.Invoke();
            curOnComplete = null;
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
        private void StopCurNode()
        {
            curPlayNodeAnim?.Kill();
        }

        /// <summary>
        /// 尝试加速节点播放
        /// </summary>
        public bool TryAdvancePlayNode()
        {
            if (!isPlayingNode) //没有播放则不加速
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
            if (curShowPanel == null || curNode == null || index >= curNode.NodeGroups.Count)
            {
                StopDialogue(); //结束对话
                return false;
            }

            DialogNodeGroup group = curNode.NodeGroups[index];
            curShowPanel.SetCharacters(group.Characters);

            isPlayingNode = true;
            string targetString = group.ContentKey.GetLocalizedString();
            int endIndex = targetString.Length - 1;
            float duration = targetString.Length * charDuration;
            curPlayNodeAnim?.Kill();
            curPlayNodeAnim = DOTween.To(() => 0, newIndex =>
            {
                string content = targetString.Substring(0, newIndex + 1);
                curShowPanel.SetContent(content);
            }, endIndex, duration).OnKill(() =>
            {
                isPlayingNode = false;
                curPlayNodeAnim = null;
            });
            return true;
        }
    }
}