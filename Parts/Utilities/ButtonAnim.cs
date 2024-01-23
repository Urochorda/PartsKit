using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PartsKit
{
    public class ButtonAnim : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private CheckNullProperty<Transform> rootPoint;
        private Sequence animSequence;
        private Transform SafePoint => rootPoint.GetValue(out Transform rootPointValue) ? rootPointValue : transform;
        public event Action OnDownAnim;
        public event Action OnUpAnim;

        public void OnPointerDown(PointerEventData eventData)
        {
            animSequence?.Kill();
            animSequence = DOTween.Sequence();
            animSequence.Append(SafePoint.DOScale(new Vector3(0.9f, 0.9f, 1), 0.15f));
            animSequence.OnKill(() => { animSequence = null; });
            OnDownAnim?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            animSequence?.Kill();
            animSequence = DOTween.Sequence();
            animSequence.Append(SafePoint.DOScale(Vector3.one, 0.15f));
            animSequence.OnKill(() =>
            {
                SafePoint.localScale = Vector3.one;
                animSequence = null;
            });
            OnUpAnim?.Invoke();
        }

        private void OnDisable()
        {
            animSequence?.Kill();
        }
    }
}