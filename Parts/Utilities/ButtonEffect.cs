using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PartsKit
{
    public class ButtonEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public static Action<ButtonEffect> OnDownGlobal { get; set; }
        public static Action<ButtonEffect> OnUpGlobal { get; set; }

        [SerializeField] private CheckNullProperty<Transform> rootPoint;
        private Sequence animSequence;
        private Transform SafePoint => rootPoint.GetValue(out Transform rootPointValue) ? rootPointValue : transform;
        public event Action<ButtonEffect> OnDown;
        public event Action<ButtonEffect> OnUp;

        private Vector3 downScale;

        public void OnPointerDown(PointerEventData eventData)
        {
            animSequence?.Kill(); //先kill，会恢复localScale，使得下次记录的downScale正确
            downScale = SafePoint.localScale;
            animSequence = DOTween.Sequence();
            animSequence.Append(SafePoint.DOScale(downScale * 0.9f, 0.15f));
            animSequence.OnKill(() => { animSequence = null; });
            OnDown?.Invoke(this);
            OnDownGlobal?.Invoke(this);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            animSequence?.Kill();
            animSequence = DOTween.Sequence();
            animSequence.Append(SafePoint.DOScale(downScale, 0.15f));
            animSequence.OnKill(() =>
            {
                SafePoint.localScale = downScale;
                animSequence = null;
            });
            OnUp?.Invoke(this);
            OnUpGlobal?.Invoke(this);
        }

        private void OnDisable()
        {
            animSequence?.Kill();
        }
    }
}