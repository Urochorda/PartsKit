using System;
using System.Collections.Generic;

namespace PartsKit
{
    /// <summary>
    /// 数据实例缓存池，每个key只有一个实例
    /// </summary>
    public class DataInstancePool<K, T, TD>
    {
        private readonly Dictionary<K, T> cachePool = new Dictionary<K, T>();
        private Func<K, TD, T> onCreateData;

        public DataInstancePool(Func<K, TD, T> onCreate)
        {
            onCreateData = onCreate;
        }

        public void Add(K key, T value)
        {
            cachePool[key] = value;
        }

        public void Remove(K key)
        {
            cachePool.Remove(key);
        }

        public bool TryGet(K key, out T value)
        {
            return cachePool.TryGetValue(key, out value);
        }

        public T GetOrCreate(K key, TD param)
        {
            if (!cachePool.TryGetValue(key, out var value))
            {
                value = onCreateData(key, param);
                Add(key, value);
            }

            return value;
        }

        public void Clear()
        {
            cachePool.Clear();
        }
    }
}