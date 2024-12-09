using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace PartsKit
{
    public abstract class LoadDialogueFun : MonoBehaviour
    {
        public abstract DialogueNodeConfig LoadNodeConfig(int nodeKey);
        public abstract void ReleaseNodeConfig(DialogueNodeConfig nodeConfig);
        public abstract IDialogueShowPanel LoadShowPanel();
        public abstract void ReleaseShowPanel(IDialogueShowPanel showPanel);
    }

    [Serializable]
    public class DialogueSelectItemData
    {
        [field: SerializeField] public string InfoEntryName { get; private set; }

        public DialogueSelectItemData(string infoEntryName)
        {
            InfoEntryName = infoEntryName;
        }
    }

    public struct DialoguePlayData
    {
        public int NodeConfigKey { get; set; }
        public bool IsForce { get; set; }
        public bool HideInEnd { get; set; }
        public List<DialogueSelectItemData> SelectItemList { get; set; }
        public Action<DialogueSelectItemData> OnComplete { get; set; }
        public Action OnBeginPlay { get; set; }

        public DialoguePlayData(int nodeConfigKey, bool isForce)
        {
            NodeConfigKey = nodeConfigKey;
            IsForce = isForce;
            HideInEnd = true;
            SelectItemList = new List<DialogueSelectItemData>();
            OnComplete = null;
            OnBeginPlay = null;
        }
    }

    /// <summary>
    /// 简单的对话控制器
    /// </summary>
    public class DialogueController : PartsKitBehaviour
    {
        [SerializeField] private LoadDialogueFun loadDialogueFun;
        [SerializeField] private float charDuration = 0.05f;
        [SerializeField] private float advanceTimeScale = 5f;
        [SerializeField] private bool defaultHideInEnd = true;

        public event Action OnPlay;
        public event Action OnStop;
        private DialogueNodeConfig curNode;
        private readonly List<DialogueSelectItemData> curSelectItemList = new List<DialogueSelectItemData>();
        public IDialogueShowPanel CurShowPanel { get; private set; }
        private int curGroupIndex;
        private Action<DialogueSelectItemData> curOnComplete;
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
        public bool TryPlayDialogue(DialoguePlayData playData)
        {
            if (IsPlayingDialogue && !playData.IsForce) //正在播放对话&&非强制更新
            {
                return false;
            }

            StopDialogue(); //结束上次对话
            SetTempHideInEnd(playData.HideInEnd);
            if (curNode != null)
            {
                loadDialogueFun.ReleaseNodeConfig(curNode);
            }

            curNode = loadDialogueFun.LoadNodeConfig(playData.NodeConfigKey);
            curSelectItemList.Clear();
            curSelectItemList.AddRange(playData.SelectItemList);
            if (CurShowPanel != null)
            {
                loadDialogueFun.ReleaseShowPanel(CurShowPanel);
            }

            CurShowPanel = loadDialogueFun.LoadShowPanel();
            curGroupIndex = 0;
            curOnComplete = playData.OnComplete;
            CurShowPanel.Show();
            CurShowPanel.BeginPlay();
            IsPlayingDialogue = true;
            playData.OnBeginPlay?.Invoke();
            OnPlay?.Invoke();
            return DoPlayNode(curGroupIndex, null);
        }

        /// <summary>
        /// 结束对话
        /// </summary>
        public void StopDialogue()
        {
            DoStopDialogue(null);
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

            if (DoPlayNode(curGroupIndex + 1, null))
            {
                curGroupIndex++;
                return true;
            }

            return false;
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

        private bool DoPlayNode(int index, DialogueSelectItemData selectItemData)
        {
            if (CurShowPanel == null || curNode == null || index >= curNode.NodeGroups.Count)
            {
                if (selectItemData == null && IsEndShowSelect())
                {
                    return false;
                }

                DoStopDialogue(selectItemData); //结束对话
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
            }, endIndex, duration).SetEase(Ease.Linear).OnKill(() =>
            {
                IsPlayingNode = false;
                curPlayNodeAnim = null;

                if (index == curNode.NodeGroups.Count - 1 && IsEndShowSelect())
                {
                    CurShowPanel.SetSelects(curSelectItemList, (selectItem) =>
                    {
                        DialogueSelectItemData targetSelectItem = curSelectItemList.Find(item =>
                            item.InfoEntryName == selectItem.InfoEntryName);
                        DoPlayNode(curGroupIndex + 1, targetSelectItem);
                    });
                }
            });
            return true;
        }

        private void DoStopDialogue(DialogueSelectItemData selectItemData)
        {
            if (!IsPlayingDialogue)
            {
                return;
            }

            IsPlayingDialogue = false;
            curPlayNodeAnim?.Kill();
            if (curNode != null)
            {
                loadDialogueFun.ReleaseNodeConfig(curNode);
                curNode = null;
            }

            if (CurShowPanel != null)
            {
                CurShowPanel.EndPlay();
                if (CurHideInEnd)
                {
                    CurShowPanel.Hide();
                    loadDialogueFun.ReleaseShowPanel(CurShowPanel);
                    CurShowPanel = null;
                }
            }

            CurHideInEnd = defaultHideInEnd;
            curOnComplete?.Invoke(selectItemData);
            curOnComplete = null;
            OnStop?.Invoke();
        }

        private bool IsEndShowSelect()
        {
            return curSelectItemList.Count > 0;
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