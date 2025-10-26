using System;
using System.Linq.Expressions;

namespace PartsKit
{
    /// <summary>
    /// 反射生成缓存池
    /// </summary>
    public class ActivatorCachePool<TBase>
    {
        private readonly DataInstancePool<string, Func<TBase>, string> dataPool =
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
            var factory = dataPool.GetOrCreate(className, className);
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
            dataPool.Remove(className);
        }

        public void Clear()
        {
            dataPool.Clear();
        }
    }
}