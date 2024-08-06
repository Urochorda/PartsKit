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

        [SerializeField] private bool scaleAnim = true;
        [SerializeField] private CheckNullProperty<Transform> rootPoint;
        [SerializeField] private bool selectAnim;
        [SerializeField] private GameObject[] downObj;
        [SerializeField] private GameObject[] upObj;
        private Sequence animSequence;
        private Transform SafePoint => rootPoint.GetValue(out Transform rootPointValue) ? rootPointValue : transform;
        public event Action<ButtonEffect> OnDown;
        public event Action<ButtonEffect> OnUp;

        private Vector3 downScale;

        private void Awake()
        {
            SetSelectObj(false, true);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            animSequence?.Kill(); //先kill，会恢复localScale，使得下次记录的downScale正确
            downScale = SafePoint.localScale;
            if (scaleAnim)
            {
                animSequence = DOTween.Sequence();
                animSequence.Append(SafePoint.DOScale(downScale * 0.9f, 0.15f));
                animSequence.OnKill(() => { animSequence = null; });
            }

            if (selectAnim)
            {
                SetSelectObj(true, false);
            }

            OnDown?.Invoke(this);
            OnDownGlobal?.Invoke(this);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            animSequence?.Kill();
            if (scaleAnim)
            {
                animSequence = DOTween.Sequence();
                animSequence.Append(SafePoint.DOScale(downScale, 0.15f));
                animSequence.OnKill(() =>
                {
                    SafePoint.localScale = downScale;
                    animSequence = null;
                });
            }

            if (selectAnim)
            {
                SetSelectObj(false, true);
            }

            OnUp?.Invoke(this);
            OnUpGlobal?.Invoke(this);
        }

        private void SetSelectObj(bool isDown, bool isUp)
        {
            foreach (var o in downObj)
            {
                if (o == null)
                {
                    continue;
                }

                o.SetActive(isDown);
            }

            foreach (var o in upObj)
            {
                if (o == null)
                {
                    continue;
                }

                o.SetActive(isUp);
            }
        }

        private void OnDisable()
        {
            animSequence?.Kill();
            SetSelectObj(false, true);
        }
    }
}