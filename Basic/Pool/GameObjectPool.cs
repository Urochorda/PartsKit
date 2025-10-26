using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace PartsKit
{
    /// <summary>
    /// 物体对象池
    /// </summary>
    public class GameObjectPool : MonoBehaviour
    {
        public class PoolItemKey : MonoBehaviour
        {
            public int KeyID { get; set; }
        }

        [SerializeField] private bool collectionCheck;
        [SerializeField] private int defaultCapacity = 10;
        [SerializeField] private int maxSize = 10000;
        [SerializeField] private bool autoCancelDontDestroyOnLoad;

        public bool AutoCancelDontDestroyOnLoad
        {
            get => autoCancelDontDestroyOnLoad;
            set => autoCancelDontDestroyOnLoad = value;
        }

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
                    collectionCheck, defaultCapacity, maxSize);
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
                gameObj.transform.SetParent(parent);
                gameObj.SetActive(true);
                if (AutoCancelDontDestroyOnLoad && parent == null)
                {
                    SceneManager.MoveGameObjectToScene(gameObj, SceneManager.GetActiveScene());
                }
            }

            void ActionOnRelease(GameObject gameObj)
            {
                gameObj.transform.SetParent(transform);
                gameObj.transform.localPosition = Vector3.zero;
                gameObj.SetActive(false);
            }

            void ActionOnDestroy(GameObject gameObj)
            {
                Destroy(gameObj);
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

        public void Clear(GameObject objPrefab)
        {
            if (objPrefab == null)
            {
                return;
            }

            int id = objPrefab.GetInstanceID();
            if (!itemPoolDic.TryGetValue(id, out ObjectPool<GameObject> itemPool))
            {
                return;
            }

            itemPool.Clear();
        }

        public void ClearAll()
        {
            foreach (var keyValuePair in itemPoolDic)
            {
                keyValuePair.Value.Clear();
            }

            itemPoolDic.Clear();
        }
    }
}