using UnityEngine;

namespace PartsKit
{
    public abstract class LoadEffectAssetFun : MonoBehaviour
    {
        public abstract EffectConfig LoadEffectConfig(string effectId);
        public abstract void Release(EffectConfig effectConfig);
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

        public EffectItem PlayEffect(string effectId, Vector3 pos)
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

        public EffectItem PlayEffect(string effectId, Transform parent)
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

        private EffectItem GetEffect(string effectId)
        {
            EffectConfig effectConfig = customLoadEffectAssetFun.LoadEffectConfig(effectId);
            if (effectConfig == null)
            {
                CustomLog.LogError("effectConfig is null");
                return null;
            }

            EffectItem effectItemPrefab = effectConfig.GetEffect();
            EffectItem effectItem = gameObjectPool.Get(effectItemPrefab, transform);
            var itemTrans = effectItem.transform;
            var prefabTrans = effectItemPrefab.transform;
            itemTrans.localScale = prefabTrans.localScale;
            itemTrans.rotation = prefabTrans.rotation;
            EffectItem.Init(effectItem);
            customLoadEffectAssetFun.Release(effectConfig);
            return effectItem;
        }
    }
}