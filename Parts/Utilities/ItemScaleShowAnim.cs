using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace PartsKit
{
    public class ItemScaleShowAnim : MonoBehaviour
    {
        [SerializeField] private float delayTime = 0.01f;
        [SerializeField] private float duration = 0.08f;
        [SerializeField] private Ease ease = Ease.Unset;
        [SerializeField] private Vector3 formScale = Vector3.zero;
        [SerializeField] private Vector3 endScale = Vector3.one;
        [SerializeField] private List<Transform> itemList;

        private Sequence sequence;

        public void SetItemList(List<Transform> itemListVal)
        {
            if (itemListVal == null)
            {
                return;
            }

            itemList.Clear();
            foreach (Transform item in itemListVal)
            {
                itemList.Add(item);
            }
        }

        public void SetDelayTime(float delayTimeVal)
        {
            delayTime = delayTimeVal;
        }

        public void SetDuration(float durationVal)
        {
            duration = durationVal;
        }

        public void SetEase(Ease easeVal)
        {
            ease = easeVal;
        }

        public void SetFormEndScale(Vector3 formScaleVal, Vector3 endScaleVal)
        {
            formScale = formScaleVal;
            endScale = endScaleVal;
        }

        [ContextMenu("Play")]
        public void Play()
        {
            Play(0);
        }

        public void Play(float startDelay)
        {
            sequence?.Kill();
            sequence = DOTween.Sequence();
            sequence.OnKill(() => sequence = null);
            sequence.AppendInterval(startDelay);
            for (var i = 0; i < itemList.Count; i++)
            {
                DoScaleAnim(itemList[i], i);
            }
        }

        private void DoScaleAnim(Transform tf, int index)
        {
            if (tf == null)
            {
                return;
            }

            tf.localScale = formScale;
            sequence.AppendInterval(index * delayTime);
            sequence.Append(tf.DOScale(endScale, duration).SetEase(ease));
        }
    }
}