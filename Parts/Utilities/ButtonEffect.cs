using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PartsKit
{
    public class ButtonEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler,
        IPointerExitHandler, ISelectHandler, IDeselectHandler
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

        public static Action<ButtonEffect> OnDownGlobalEvent { get; set; }
        public static Action<ButtonEffect> OnUpGlobalEvent { get; set; }
        public static Action<ButtonEffect> OnSelectGlobalEvent { get; set; }
        public static Action<ButtonEffect> OnDeselectGlobalEvent { get; set; }
        public static Action<string> OnPlaySound { get; set; }

        [Header("Sound")] [SerializeField] private string pressSoundKey;
        [SerializeField] private string highlightSoundKey;
        [SerializeField] private string selectSoundKey;
        [Header("Anim")] [SerializeField] private bool scaleAnim = true;
        [SerializeField] private CheckNullProperty<Transform> rootPoint;
        [SerializeField] private GameObject[] normalObj;
        [SerializeField] private GameObject[] highlightedObj;
        [SerializeField] private GameObject[] pressedObj;
        [SerializeField] private GameObject[] selectedObj;
        [SerializeField] private GameObject[] disabledObj;

        private Sequence animSequence;
        private Transform SafePoint => rootPoint.GetValue(out Transform rootPointValue) ? rootPointValue : transform;
        public event Action<ButtonEffect> OnDownEvent;
        public event Action<ButtonEffect> OnUpEvent;
        public event Action<ButtonEffect> OnEnterEvent;
        public event Action<ButtonEffect> OnExitEvent;
        public event Action<ButtonEffect> OnSelectEvent;
        public event Action<ButtonEffect> OnDeselectEvent;

        private Vector3 initScale;
        private CheckNullProperty<Button> button;
        private bool isInDown;
        private bool isInEntry;
        private bool isInSelect;
        private bool isDisabled;
        private State curState;

        private void Awake()
        {
            initScale = SafePoint.localScale;
            curState = State.Null;
            button = new CheckNullProperty<Button>(GetComponent<Button>(), false);
            isDisabled = !CanPlayEffect();
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
            bool canPlayerEffect = CanPlayEffect();
            if (isDisabled)
            {
                //不可用时检测切换为可用
                if (canPlayerEffect)
                {
                    isDisabled = false;
                    UpdateState();
                }
            }
            else
            {
                //可用时检测切换为不可用
                if (!canPlayerEffect)
                {
                    OnPointerUp(null);
                    OnPointerExit(null);
                    OnDeselect(null);
                    isDisabled = true;
                    UpdateState();
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

            UpdateState();
            OnDownEvent?.Invoke(this);
            OnDownGlobalEvent?.Invoke(this);
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

            UpdateState();
            OnUpEvent?.Invoke(this);
            OnUpGlobalEvent?.Invoke(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (curState == State.Disabled)
            {
                return;
            }

            isInEntry = true;
            UpdateState();
            OnEnterEvent?.Invoke(this);
            OnPlaySound?.Invoke(highlightSoundKey);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!isInEntry)
            {
                return;
            }

            isInEntry = false;
            UpdateState();
            OnExitEvent?.Invoke(this);
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (curState == State.Disabled)
            {
                return;
            }

            isInSelect = true;
            UpdateState();
            OnSelectEvent?.Invoke(this);
            OnSelectGlobalEvent?.Invoke(this);
            OnPlaySound?.Invoke(selectSoundKey);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (!isInSelect)
            {
                return;
            }

            isInSelect = false;
            UpdateState();
            OnDeselectEvent?.Invoke(this);
            OnDeselectGlobalEvent?.Invoke(this);
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

        private void UpdateState()
        {
            if (isDisabled)
            {
                SetState(State.Disabled);
                return;
            }

            if (isInSelect)
            {
                SetState(State.Selected);
                return;
            }

            if (isInDown)
            {
                SetState(State.Pressed);
                return;
            }

            if (isInEntry)
            {
                SetState(State.Highlighted);
                return;
            }

            SetState(State.Pressed);
        }

        private void SetActiveSelf(GameObject[] objs, bool active)
        {
            if (objs == null)
            {
                return;
            }

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

        #region Editor

        [ButtonMenu("ToNormalEditor")]
        private void ToNormalEditor()
        {
            SetState(State.Normal);
        }

        [ButtonMenu("ToHighlightedEditor")]
        private void ToHighlightedEditor()
        {
            SetState(State.Highlighted);
        }

        [ButtonMenu("ToPressedEditor")]
        private void ToPressedEditor()
        {
            SetState(State.Pressed);
        }

        [ButtonMenu("ToSelectedEditor")]
        private void ToSelectedEditor()
        {
            SetState(State.Selected);
        }

        [ButtonMenu("ToDisabledEditor")]
        private void ToDisabledEditor()
        {
            SetState(State.Disabled);
        }

        #endregion
    }
}