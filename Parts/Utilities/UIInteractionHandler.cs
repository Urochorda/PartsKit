using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PartsKit
{
    public class UIInteractionHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler,
        IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler, IBeginDragHandler, IDragHandler,
        IEndDragHandler, ISelectHandler, IDeselectHandler, ISubmitHandler, ICancelHandler
    {
        public event Action<UIInteractionHandler, PointerEventData> onPointerClickEvent;
        public event Action<UIInteractionHandler, PointerEventData> onPointerDownEvent;
        public event Action<UIInteractionHandler, PointerEventData> onPointerUpEvent;
        public event Action<UIInteractionHandler, PointerEventData> onPointerEnterEvent;
        public event Action<UIInteractionHandler, PointerEventData> onPointerExitEvent;
        public event Action<UIInteractionHandler, PointerEventData> onPointerMoveEvent;
        public event Action<UIInteractionHandler, PointerEventData> onBeginDragEvent;
        public event Action<UIInteractionHandler, PointerEventData> onDragEvent;
        public event Action<UIInteractionHandler, PointerEventData> onEndDragEvent;
        public event Action<UIInteractionHandler, BaseEventData> onSelectEvent;
        public event Action<UIInteractionHandler, BaseEventData> onDeselectEvent;
        public event Action<UIInteractionHandler, BaseEventData> onSubmitEvent;
        public event Action<UIInteractionHandler, BaseEventData> onCancelEvent;

        private bool isDragDown;

        public bool IsDragging { get; private set; }
        public bool IsDowning { get; private set; }
        public bool IsEntering { get; private set; }
        public bool IsSelect { get; private set; }

        private void OnDisable()
        {
            var eventSystem = EventSystem.current;
            OnEndDrag(new PointerEventData(eventSystem));
            OnPointerExit(new PointerEventData(eventSystem));
            OnPointerUp(new PointerEventData(eventSystem));
            OnDeselect(new BaseEventData(eventSystem));
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!isDragDown)
            {
                onPointerClickEvent?.Invoke(this, eventData);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            IsDowning = true;
            isDragDown = false;
            onPointerDownEvent?.Invoke(this, eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!IsDowning)
            {
                return;
            }

            IsDowning = false;
            onPointerUpEvent?.Invoke(this, eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            IsEntering = true;
            onPointerEnterEvent?.Invoke(this, eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!IsEntering)
            {
                return;
            }

            IsEntering = false;
            onPointerExitEvent?.Invoke(this, eventData);
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            onPointerMoveEvent?.Invoke(this, eventData);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            IsDragging = true;
            isDragDown = true;
            onBeginDragEvent?.Invoke(this, eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            onDragEvent?.Invoke(this, eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!IsDragging)
            {
                return;
            }

            IsDragging = false;
            onEndDragEvent?.Invoke(this, eventData);
        }

        public void OnSelect(BaseEventData eventData)
        {
            IsSelect = true;
            onSelectEvent?.Invoke(this, eventData);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (!IsSelect)
            {
                return;
            }

            onDeselectEvent?.Invoke(this, eventData);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            onSubmitEvent?.Invoke(this, eventData);
        }

        public void OnCancel(BaseEventData eventData)
        {
            onCancelEvent?.Invoke(this, eventData);
        }
    }
}