using System;
using System.Linq.Expressions;

namespace PartsKit
{
    public class ActivatorCachePool<TBase>
    {
        private readonly DataCachePool<string, Func<TBase>, string> cachePool =
            new((key, info) =>
            {
                Type classType = Type.GetType(info);
                if (classType != null && typeof(TBase).IsAssignableFrom(classType))
                {
                    var ctor = classType.GetConstructor(Type.EmptyTypes);
                    if (ctor != null)
                    {
                        var expression = Expression.New(ctor);
                        var convertExpr = Expression.Convert(expression, typeof(TBase));
                        var lambda = Expression.Lambda<Func<TBase>>(convertExpr);
                        Func<TBase> creator = lambda.Compile();
                        return creator;
                    }
                }

                return null;
            });

        public bool Create(string className, out TBase instance)
        {
            var factory = cachePool.GetOrCreate(className, className);
            if (factory == null)
            {
                instance = default;
                return false;
            }

            instance = factory();
            return true;
        }

        public void Remove(string className)
        {
            cachePool.Remove(className);
        }

        public void Clear()
        {
            cachePool.Clear();
        }
    }
}