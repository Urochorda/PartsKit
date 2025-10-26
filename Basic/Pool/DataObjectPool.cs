using System;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace PartsKit
{
    /// <summary>
    /// 数据对象池（谨慎使用，性能大部分情况不如直接实例化数据）
    /// </summary>
    public class DataObjectPool
    {
        public interface IDataCache
        {
            void Clear();
        }

        public class DataCache<T> : IDataCache where T : class
        {
            public ObjectPool<T> Pool { get; set; }

            public DataCache(Func<T> createFunc, Action<T> actionOnGet, Action<T> actionOnRelease,
                Action<T> actionOnDestroy, bool collectionCheck, int defaultCapacity, int maxSize)
            {
                Pool = new ObjectPool<T>(createFunc, actionOnGet, actionOnRelease, actionOnDestroy, collectionCheck,
                    defaultCapacity, maxSize);
            }

            public void Clear()
            {
                Pool.Clear();
            }
        }

        private readonly Dictionary<Type, IDataCache> itemPoolDic = new Dictionary<Type, IDataCache>();

        private bool mCollectionCheck;
        private int mDefaultCapacity;
        private int mMaxSize;

        public DataObjectPool(bool collectionCheck = true, int defaultCapacity = 10,
            int maxSize = 10000)
        {
            mCollectionCheck = collectionCheck;
            mDefaultCapacity = defaultCapacity;
            mMaxSize = maxSize;
        }

        public T Get<T>() where T : class, new()
        {
            var key = typeof(T);
            if (!itemPoolDic.TryGetValue(key, out IDataCache itemPool))
            {
                itemPool = new DataCache<T>(CreateFunc, null, null, null, mCollectionCheck, mDefaultCapacity, mMaxSize);
                itemPoolDic[key] = itemPool;
            }

            DataCache<T> itemPoolT = (DataCache<T>)itemPool;
            return itemPoolT.Pool.Get();

            T CreateFunc()
            {
                return new T();
            }
        }

        public void Release<T>(T obj) where T : class, new()
        {
            if (obj == null)
            {
                return;
            }

            var key = typeof(T);
            if (!itemPoolDic.TryGetValue(key, out IDataCache objectPool))
            {
                return;
            }

            DataCache<T> objectPoolT = (DataCache<T>)objectPool;
            objectPoolT.Pool.Release(obj);
        }

        public void Release<T>(List<T> objList, Action<T> onRelease) where T : class, new()
        {
            foreach (T obj in objList)
            {
                onRelease?.Invoke(obj);
                Release(obj);
            }

            objList.Clear();
        }

        public void Clear<T>(T objPrefab)
        {
            if (objPrefab == null)
            {
                return;
            }

            var key = typeof(T);
            if (!itemPoolDic.TryGetValue(key, out IDataCache itemPool))
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