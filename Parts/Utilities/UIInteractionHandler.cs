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

        public void OnPointerClick(PointerEventData eventData)
        {
            onPointerClickEvent?.Invoke(this, eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            onPointerDownEvent?.Invoke(this, eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            onPointerUpEvent?.Invoke(this, eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            onPointerEnterEvent?.Invoke(this, eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onPointerExitEvent?.Invoke(this, eventData);
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            onPointerMoveEvent?.Invoke(this, eventData);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            onBeginDragEvent?.Invoke(this, eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            onDragEvent?.Invoke(this, eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            onEndDragEvent?.Invoke(this, eventData);
        }
    }
}