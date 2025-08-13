using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PartsKit
{
    public class ButtonEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler,
        IPointerExitHandler
    {
        enum State
        {
            Null,
            Normal,
            Highlighted,
            Pressed,
            Selected,
            Disabled,
        }

        public static Action<ButtonEffect> OnDownGlobal { get; set; }
        public static Action<ButtonEffect> OnUpGlobal { get; set; }
        public static Action<string> OnPlaySound { get; set; }

        [Header("Sound")] [SerializeField] private string pressSoundKey;
        [SerializeField] private string highlightSoundKey;
        [Header("Anim")] [SerializeField] private bool scaleAnim = true;
        [SerializeField] private CheckNullProperty<Transform> rootPoint;
        [SerializeField] private GameObject[] normalObj;
        [SerializeField] private GameObject[] highlightedObj;
        [SerializeField] private GameObject[] pressedObj;
        [SerializeField] private GameObject[] selectedObj;
        [SerializeField] private GameObject[] disabledObj;

        private Sequence animSequence;
        private Transform SafePoint => rootPoint.GetValue(out Transform rootPointValue) ? rootPointValue : transform;
        public event Action<ButtonEffect> OnDown;
        public event Action<ButtonEffect> OnUp;
        public event Action<ButtonEffect> OnEnter;
        public event Action<ButtonEffect> OnExit;

        private Vector3 initScale;
        private CheckNullProperty<Button> button;
        private bool isInDown;
        private bool isInEntry;
        private State curState;

        private void Awake()
        {
            initScale = SafePoint.localScale;
            curState = State.Null;
            button = new CheckNullProperty<Button>(GetComponent<Button>(), false);
            SetState(CanPlayEffect() ? State.Normal : State.Disabled);
#if UNITY_EDITOR
            CheckNullSelf(normalObj);
            CheckNullSelf(highlightedObj);
            CheckNullSelf(pressedObj);
            CheckNullSelf(selectedObj);
            CheckNullSelf(disabledObj);
#endif
        }

        private void OnDisable()
        {
            OnPointerUp(null);
            OnPointerExit(null);
            SetState(CanPlayEffect() ? State.Normal : State.Disabled);
            animSequence?.Kill();
            SafePoint.localScale = initScale;
        }

        private void Update()
        {
            if (curState != State.Disabled)
            {
                if (!CanPlayEffect())
                {
                    OnPointerUp(null);
                    OnPointerExit(null);
                    SetState(State.Disabled);
                }
            }
            else
            {
                if (CanPlayEffect())
                {
                    SetState(State.Normal);
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (curState == State.Disabled)
            {
                return;
            }

            isInDown = true;
            animSequence?.Kill();
            if (scaleAnim)
            {
                animSequence = DOTween.Sequence();
                animSequence.Append(SafePoint.DOScale(initScale * 0.9f, 0.15f));
                animSequence.OnKill(() => { animSequence = null; });
            }

            SetState(State.Pressed);
            OnDown?.Invoke(this);
            OnDownGlobal?.Invoke(this);
            OnPlaySound?.Invoke(pressSoundKey);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!isInDown)
            {
                return;
            }

            isInDown = false;
            animSequence?.Kill();
            if (scaleAnim)
            {
                animSequence = DOTween.Sequence();
                animSequence.Append(SafePoint.DOScale(initScale, 0.15f));
                animSequence.OnKill(() =>
                {
                    SafePoint.localScale = initScale;
                    animSequence = null;
                });
            }

            SetState(isInEntry ? State.Highlighted : State.Normal);
            OnUp?.Invoke(this);
            OnUpGlobal?.Invoke(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (curState == State.Disabled)
            {
                return;
            }

            isInEntry = true;
            SetState(isInDown ? State.Pressed : State.Highlighted);
            OnEnter?.Invoke(this);
            OnPlaySound?.Invoke(highlightSoundKey);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!isInEntry)
            {
                return;
            }

            isInEntry = false;
            SetState(isInDown ? State.Pressed : State.Normal);
            OnExit?.Invoke(this);
        }

        private bool CanPlayEffect()
        {
            return !button.GetValue(out Button buttonValue) ||
                   buttonValue.interactable && buttonValue.isActiveAndEnabled;
        }

        private void SetState(State state)
        {
            if (curState == state)
            {
                return;
            }

            curState = state;
            switch (state)
            {
                case State.Normal:
                    SetActiveSelf(highlightedObj, false);
                    SetActiveSelf(pressedObj, false);
                    SetActiveSelf(selectedObj, false);
                    SetActiveSelf(disabledObj, false);

                    SetActiveSelf(normalObj, true);
                    break;
                case State.Highlighted:
                    SetActiveSelf(normalObj, false);
                    SetActiveSelf(pressedObj, false);
                    SetActiveSelf(selectedObj, false);
                    SetActiveSelf(disabledObj, false);

                    SetActiveSelf(highlightedObj, true);
                    break;
                case State.Pressed:
                    SetActiveSelf(normalObj, false);
                    SetActiveSelf(highlightedObj, false);
                    SetActiveSelf(selectedObj, false);
                    SetActiveSelf(disabledObj, false);

                    SetActiveSelf(pressedObj, true);
                    break;
                case State.Selected:
                    SetActiveSelf(normalObj, false);
                    SetActiveSelf(highlightedObj, false);
                    SetActiveSelf(pressedObj, false);
                    SetActiveSelf(disabledObj, false);

                    SetActiveSelf(selectedObj, true);
                    break;
                case State.Disabled:
                    SetActiveSelf(normalObj, false);
                    SetActiveSelf(highlightedObj, false);
                    SetActiveSelf(pressedObj, false);
                    SetActiveSelf(selectedObj, false);

                    SetActiveSelf(disabledObj, true);
                    break;
            }
        }

        private void SetActiveSelf(GameObject[] objs, bool active)
        {
            foreach (var obj in objs)
            {
                if (obj)
                {
                    obj.SetActive(active);
                }
            }
        }

        private void CheckNullSelf(GameObject[] objs)
        {
            bool hasNull = false;
            foreach (var obj in objs)
            {
                if (obj == null)
                {
                    hasNull = true;
                    break;
                }
            }

            if (hasNull)
            {
                Debug.LogError(gameObject.name + " ButtonEffect : has null object");
            }
        }
    }
}