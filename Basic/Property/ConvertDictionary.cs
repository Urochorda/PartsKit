using System;
using System.Collections;
using System.Collections.Generic;

namespace PartsKit
{
    public class ConvertDictionary<TK, TVIn, TVOut> : IReadOnlyDictionary<TK, TVOut>
    {
        private readonly Func<TVIn, TVOut> convert;

        private readonly Dictionary<TK, TVIn> mInDict = new Dictionary<TK, TVIn>();
        private readonly Dictionary<TK, TVOut> mOutDict = new Dictionary<TK, TVOut>();

        public ConvertDictionary(Func<TVIn, TVOut> convertVar)
        {
            convert = convertVar;
        }

        public int Count => mOutDict.Count;
        public TVOut this[TK key] => mOutDict[key];
        public IEnumerable<TK> Keys => mOutDict.Keys;
        public IEnumerable<TVOut> Values => mOutDict.Values;

        public IEnumerator<KeyValuePair<TK, TVOut>> GetEnumerator()
        {
            return mOutDict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool ContainsKey(TK key)
        {
            return mOutDict.ContainsKey(key);
        }

        public bool TryGetValue(TK key, out TVOut value)
        {
            return mOutDict.TryGetValue(key, out value);
        }

        public bool TryGetValueIn(TK key, out TVIn value)
        {
            return mInDict.TryGetValue(key, out value);
        }

        public TVIn GetValueIn(TK key)
        {
            return mInDict[key];
        }

        public void SetValue(TK key, TVIn value)
        {
            mInDict[key] = value;

            TVOut newOutValue = convert(value);
            mOutDict[key] = newOutValue;
        }

        public bool Remove(TK key)
        {
            bool existed = mInDict.Remove(key);
            if (!existed)
                return false;

            mOutDict.Remove(key);
            return true;
        }

        public void Clear()
        {
            mInDict.Clear();
            mOutDict.Clear();
        }
    }
}