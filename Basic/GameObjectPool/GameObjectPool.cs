using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

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

        [field: SerializeField] public bool AutoCancelDontDestroyOnLoad { get; set; }

        private readonly Dictionary<int, ObjectPool<GameObject>> itemPoolDic =
            new Dictionary<int, ObjectPool<GameObject>>();

        public GameObject Get(GameObject objPrefab, Transform parent)
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

            return itemPool.Get();

            GameObject CreateFunc()
            {
                GameObject obj = Instantiate(objPrefab.gameObject, parent);
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

                gameObj.transform.SetParent(parent);
                gameObj.SetActive(true);
                if (AutoCancelDontDestroyOnLoad && parent == null)
                {
                    SceneManager.MoveGameObjectToScene(gameObj, SceneManager.GetActiveScene());
                }
            }

            void ActionOnRelease(GameObject gameObj)
            {
                IPoolAction[] poolActions = gameObj.GetComponents<IPoolAction>();
                foreach (IPoolAction poolAction in poolActions)
                {
                    poolAction.PoolOnRelease();
                }

                gameObj.transform.SetParent(transform);
                gameObj.transform.localPosition = Vector3.zero;
                gameObj.SetActive(false);
            }

            void ActionOnDestroy(GameObject bullet)
            {
                Destroy(bullet);
            }
        }

        public GameObject Get(GameObject objPrefab)
        {
            return Get(objPrefab, null);
        }

        public T Get<T>(T objPrefab, Transform parent) where T : Component
        {
            return Get(objPrefab.gameObject, parent).GetComponent<T>();
        }

        public T Get<T>(T objPrefab) where T : Component
        {
            return Get(objPrefab, null);
        }

        public void Release(GameObject obj)
        {
            if (obj == null)
            {
                return;
            }

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

        public void Release<T>(List<T> objList, Action<T> onRelease) where T : Component
        {
            foreach (T obj in objList)
            {
                onRelease?.Invoke(obj);
                Release(obj.gameObject);
            }

            objList.Clear();
        }

        public void Release(List<GameObject> objList, Action<GameObject> onRelease)
        {
            foreach (GameObject obj in objList)
            {
                onRelease?.Invoke(obj);
                Release(obj.gameObject);
            }

            objList.Clear();
        }
    }
}