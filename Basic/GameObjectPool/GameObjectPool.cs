using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace PartsKit
{
    public interface IPoolAction
    {
        void PoolOnGet();

        void PoolOnRelease();
    }

    public class GameObjectPool : MonoBehaviour
    {
        public class PoolItemKey : MonoBehaviour
        {
            public int KeyID { get; set; }
        }

        private readonly Dictionary<int, ObjectPool<GameObject>> itemPoolDic =
            new Dictionary<int, ObjectPool<GameObject>>();

        public T Get<T>(T objPrefab) where T : Component
        {
            if (objPrefab == null)
            {
                return null;
            }

            int id = objPrefab.GetInstanceID();
            if (!itemPoolDic.TryGetValue(id, out ObjectPool<GameObject> itemPool))
            {
                itemPool = new ObjectPool<GameObject>(CreateFunc, ActionOnGet, ActionOnRelease, ActionOnDestroy,
                    true, 10, 100000);
                itemPoolDic[id] = itemPool;
            }

            return itemPool.Get().GetComponent<T>();


            GameObject CreateFunc()
            {
                GameObject obj = Instantiate(objPrefab.gameObject);
                obj.AddComponent<PoolItemKey>().KeyID = id;
                return obj;
            }

            void ActionOnGet(GameObject gameObj)
            {
                IPoolAction[] poolActions = gameObj.GetComponents<IPoolAction>();
                foreach (IPoolAction poolAction in poolActions)
                {
                    poolAction.PoolOnGet();
                }

                gameObj.transform.SetParent(null);
                gameObj.SetActive(true);
            }

            void ActionOnRelease(GameObject gameObj)
            {
                IPoolAction[] poolActions = gameObj.GetComponents<IPoolAction>();
                foreach (IPoolAction poolAction in poolActions)
                {
                    poolAction.PoolOnRelease();
                }

                gameObj.transform.SetParent(transform);
                gameObj.SetActive(false);
            }

            void ActionOnDestroy(GameObject bullet)
            {
                Destroy(bullet);
            }
        }

        public void Release(GameObject obj)
        {
            if (!obj.TryGetComponent(out PoolItemKey itemKey))
            {
                return;
            }

            if (!itemPoolDic.TryGetValue(itemKey.KeyID, out ObjectPool<GameObject> objectPool))
            {
                Destroy(obj.gameObject);
                return;
            }

            objectPool.Release(obj.gameObject);
        }
    }
}