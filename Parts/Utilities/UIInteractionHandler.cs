using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PartsKit
{
    public class UIInteractionHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler,
        IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
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

        private bool isDragDown;

        public bool IsDragging;
        public bool IsDowning;
        public bool IsEntering;

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
            IsDragging = false;
            onEndDragEvent?.Invoke(this, eventData);
        }

        private void OnDisable()
        {
            var eventSystem = EventSystem.current;
            if (IsDragging)
            {
                OnEndDrag(new PointerEventData(eventSystem));
            }

            if (IsEntering)
            {
                OnPointerExit(new PointerEventData(eventSystem));
            }

            if (IsDowning)
            {
                OnPointerUp(new PointerEventData(eventSystem));
            }
        }
    }
}