using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace PartsKit
{
    public class SliderFollow : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private RectTransform target;
        [SerializeField] private float speed = 0.2f;
        [SerializeField] private bool addAnim = true;
        [SerializeField] private bool subAnim = true;
        private RectTransform FillRect => slider.fillRect;
        private Tweener followTweener;
        private bool isUpdateFollow;

        private void Awake()
        {
            slider.onValueChanged.AddListener(OnSliderValueChanged);
            Follow(true);
        }

        private void LateUpdate()
        {
            if (isUpdateFollow)
            {
                isUpdateFollow = false;
                Follow(false);
            }
        }

        //同一帧发生的变化收集到帧结束时处理，也可以解决FillRect会延迟更新的问题
        private void OnSliderValueChanged(float arg0)
        {
            isUpdateFollow = true;
        }

        private void Follow(bool isImmediate)
        {
            followTweener?.Kill();
            if (isImmediate || !addAnim && !subAnim)
            {
                target.anchorMax = FillRect.anchorMax;
                return;
            }

            Vector2 curAnchorMax = target.anchorMax;
            Vector2 targetAnchorMax = FillRect.anchorMax;

            float curValue;
            float targetValue;
            switch (slider.direction)
            {
                case Slider.Direction.LeftToRight:
                case Slider.Direction.RightToLeft:
                    curValue = curAnchorMax.x;
                    targetValue = targetAnchorMax.x;
                    break;
                case Slider.Direction.BottomToTop:
                case Slider.Direction.TopToBottom:
                    curValue = curAnchorMax.y;
                    targetValue = targetAnchorMax.y;
                    break;
                default:
                    curValue = 0;
                    targetValue = 0;
                    break;
            }

            float distance = 0;
            if (addAnim && targetValue > curValue)
            {
                distance = targetValue - curValue;
            }

            if (subAnim && curValue > targetValue)
            {
                distance = curValue - targetValue;
            }

            followTweener = target.DOAnchorMax(targetAnchorMax, distance / speed).SetEase(Ease.Linear).OnComplete(() =>
            {
                target.anchorMax = FillRect.anchorMax;
            });
            followTweener.OnKill(() => { followTweener = null; });
        }
    }
}