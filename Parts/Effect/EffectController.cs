using UnityEngine;

namespace PartsKit
{
    public abstract class LoadEffectAssetFun : MonoBehaviour
    {
        public abstract EffectConfig LoadEffectConfig(int effectId);
    }

    public class EffectController : PartsKitBehaviour
    {
        [SerializeField] private GameObjectPool gameObjectPool;
        [SerializeField] private LoadEffectAssetFun customLoadEffectAssetFun;

        protected override void OnInit()
        {
        }

        protected override void OnDeInit()
        {
        }

        public EffectItem PlayEffect(int effectId, Vector3 pos)
        {
            EffectItem ge = GetEffect(effectId);
            if (ge == null)
            {
                return null;
            }

            ge.transform.position = pos;
            EffectItem.Play(ge, this);
            return ge;
        }

        public EffectItem PlayEffect(int effectId, Transform parent)
        {
            EffectItem ge = GetEffect(effectId);
            if (ge == null)
            {
                return null;
            }

            ge.ParentPoint = parent;
            EffectItem.Play(ge, this);
            return ge;
        }

        public void Recycle(EffectItem effectItem)
        {
            if (effectItem == null)
            {
                return;
            }

            EffectItem.Recycle(effectItem);
            gameObjectPool.Release(effectItem.gameObject);
        }

        private EffectItem GetEffect(int effectId)
        {
            EffectConfig effectConfig = customLoadEffectAssetFun.LoadEffectConfig(effectId);
            if (effectConfig == null)
            {
                CustomLog.LogError("effectConfig is null");
                return null;
            }

            EffectItem effectItemPrefab = effectConfig.GetEffect();
            EffectItem effectItem = gameObjectPool.Get(effectItemPrefab);
            effectItem.transform.SetParent(transform);
            EffectItem.Init(effectItem);
            return effectItem;
        }
    }
}