using UnityEngine;

namespace PartsKit
{
    /// <summary>
    /// 可根据targetRect设置自己的sizeDelta
    /// </summary>
    [ExecuteInEditMode]
    public class RectContainer : MonoBehaviour
    {
        [SerializeField] private RectTransform targetRectTransform;
        [SerializeField] private Vector2 padding;

        private RectTransform selfRectTransform;

        private void Awake()
        {
            selfRectTransform = transform as RectTransform;
        }

        private void Update()
        {
            UpdateSize();
        }

        private void UpdateSize()
        {
            if (targetRectTransform == null)
            {
                return;
            }

            selfRectTransform.sizeDelta = targetRectTransform.sizeDelta + padding;
        }
    }
}