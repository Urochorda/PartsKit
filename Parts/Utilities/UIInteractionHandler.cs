using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PartsKit
{
    public class UIInteractionHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler,
        IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public event Action<PointerEventData> onPointerClickEvent;
        public event Action<PointerEventData> onPointerDownEvent;
        public event Action<PointerEventData> onPointerUpEvent;
        public event Action<PointerEventData> onPointerEnterEvent;
        public event Action<PointerEventData> onPointerExitEvent;
        public event Action<PointerEventData> onPointerMoveEvent;
        public event Action<PointerEventData> onBeginDragEvent;
        public event Action<PointerEventData> onDragEvent;
        public event Action<PointerEventData> onEndDragEvent;

        public void OnPointerClick(PointerEventData eventData)
        {
            onPointerClickEvent?.Invoke(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            onPointerDownEvent?.Invoke(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            onPointerUpEvent?.Invoke(eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            onPointerEnterEvent?.Invoke(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onPointerExitEvent?.Invoke(eventData);
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            onPointerMoveEvent?.Invoke(eventData);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            onBeginDragEvent?.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            onDragEvent?.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            onEndDragEvent?.Invoke(eventData);
        }
    }
}