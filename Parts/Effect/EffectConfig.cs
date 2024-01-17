using UnityEngine;

namespace PartsKit
{
    [CreateAssetMenu(menuName = "PartsKit/Effect/EffectConfig", fileName = "EffectConfig_")]
    public class EffectConfig : ScriptableObject
    {
        public enum PlayMode
        {
            Random,
            NoRepeatRandom,
            Sequence,
        }

        [SerializeField] private PlayMode playMode = PlayMode.Sequence;
        [SerializeField] private EffectItem[] effectItems;

        int nextIndex = -1;
        int lastIndex = -1;

        public EffectItem GetEffect()
        {
            if (effectItems.Length <= 0)
            {
                CustomLog.LogError("EffectConfigConfig的effectItems为空");
                return null;
            }

            if (effectItems.Length == 1) //如果只有1个则直接返回
            {
                return effectItems[0];
            }

            switch (playMode)
            {
                case PlayMode.Random:
                    nextIndex = Random.Range(0, effectItems.Length);
                    break;
                case PlayMode.NoRepeatRandom:
                    do
                    {
                        nextIndex = Random.Range(0, effectItems.Length);
                    } while (nextIndex == lastIndex);

                    break;
                case PlayMode.Sequence:
                    nextIndex = (int)Mathf.Repeat(++lastIndex, effectItems.Length);
                    break;
            }

            lastIndex = nextIndex;
            return effectItems[nextIndex];
        }
    }
}