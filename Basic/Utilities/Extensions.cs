using System;
using System.Collections.Generic;
using UnityEngine;

namespace PartsKit
{
    public static class Extensions
    {
        public static bool RemoveMatch<T>(this List<T> list, Predicate<T> match)
        {
            for (var i = 0; i < list.Count; i++)
            {
                if (match(list[i]))
                {
                    list.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public static bool RemoveAtDisorder<T>(this List<T> self, int index)
        {
            if (index < 0 || index >= self.Count)
            {
                return false;
            }

            int lastIndex = self.Count - 1;
            self[index] = self[lastIndex];
            self.RemoveAt(lastIndex);
            return true;
        }

        public static bool RemoveMatchDisorder<T>(this List<T> self, Predicate<T> match)
        {
            int index = self.FindIndex(match);
            if (index < 0)
            {
                return false;
            }

            return self.RemoveAtDisorder(index);
        }

        public static bool RemoveDisorder<T>(this List<T> self, T item)
        {
            int index = self.IndexOf(item);
            return self.RemoveAtDisorder(index);
        }

        /// <summary>
        /// 修复到2d，也就是忽略z
        /// </summary>
        public static Bounds To2D(this Bounds bounds)
        {
            Vector3 boundCenter = bounds.center;
            Vector3 sizeCenter = bounds.size;
            boundCenter.z = 0;
            sizeCenter.z = 1;
            return new Bounds(boundCenter, sizeCenter);
        }

        public static void To2D(this List<Bounds> bounds)
        {
            for (var i = 0; i < bounds.Count; i++)
            {
                bounds[i] = bounds[i].To2D();
            }
        }

        public static Circle To2D(this Circle bounds)
        {
            Vector3 boundCenter = bounds.Center;
            float radius = bounds.Radius;
            boundCenter.z = 0;
            return new Circle(boundCenter, radius);
        }

        public static void To2D(this List<Circle> bounds)
        {
            for (var i = 0; i < bounds.Count; i++)
            {
                bounds[i] = bounds[i].To2D();
            }
        }

        /// <summary>
        /// 判断外层包围盒是否完全包含内层包围盒
        /// </summary>
        public static bool Contains(this Bounds outer, Bounds inner)
        {
            return outer.min.x <= inner.min.x &&
                   outer.min.y <= inner.min.y &&
                   outer.min.z <= inner.min.z &&
                   outer.max.x >= inner.max.x &&
                   outer.max.y >= inner.max.y &&
                   outer.max.z >= inner.max.z;
        }

        public static bool Contains(this Circle outer, Bounds inner)
        {
            // 确定每个轴的最远点坐标
            float farthestX = (outer.Center.x > inner.center.x) ? inner.min.x : inner.max.x;
            float farthestY = (outer.Center.y > inner.center.y) ? inner.min.y : inner.max.y;
            float farthestZ = (outer.Center.z > inner.center.z) ? inner.min.z : inner.max.z;
            Vector3 farthestPoint = new Vector3(farthestX, farthestY, farthestZ);

            // 计算距离平方并比较
            float distanceSqr = (farthestPoint - outer.Center).sqrMagnitude;
            return distanceSqr <= outer.Radius * outer.Radius;
        }

        public static bool Contains2D(this Bounds outer, Vector2 point)
        {
            return point.x > outer.min.x && point.x < outer.max.x &&
                   point.y > outer.min.y && point.y < outer.max.y;
        }

        /// <summary>
        /// 合并所有包围盒（提高性能）
        /// </summary>
        public static Bounds Encapsulate(this List<Bounds> boundsList)
        {
            float minX = float.MaxValue;
            float maxX = float.MinValue;

            float minY = float.MaxValue;
            float maxY = float.MinValue;

            float minZ = float.MaxValue;
            float maxZ = float.MinValue;
            foreach (var bonds in boundsList)
            {
                var minPos = bonds.min;
                var maxPos = bonds.max;
                if (minPos.x < minX)
                {
                    minX = minPos.x;
                }

                if (maxPos.x > maxX)
                {
                    maxX = maxPos.x;
                }

                if (minPos.y < minY)
                {
                    minY = minPos.y;
                }

                if (maxPos.y > maxY)
                {
                    maxY = maxPos.y;
                }

                if (minPos.z < minZ)
                {
                    minZ = minPos.z;
                }

                if (maxPos.z > maxZ)
                {
                    maxZ = maxPos.z;
                }
            }

            Vector3 center = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, (minZ + maxZ) / 2);
            Vector3 size = new Vector3(maxX - minX, maxY - minY, maxZ - minZ);
            var bounds = new Bounds(center, size);
            return bounds;
        }

        public static Bounds Encapsulate<T>(this List<T> tList, Func<T, Bounds> getBounds)
        {
            float minX = float.MaxValue;
            float maxX = float.MinValue;

            float minY = float.MaxValue;
            float maxY = float.MinValue;

            float minZ = float.MaxValue;
            float maxZ = float.MinValue;
            foreach (var t in tList)
            {
                var bonds = getBounds.Invoke(t);
                var minPos = bonds.min;
                var maxPos = bonds.max;
                if (minPos.x < minX)
                {
                    minX = minPos.x;
                }

                if (maxPos.x > maxX)
                {
                    maxX = maxPos.x;
                }

                if (minPos.y < minY)
                {
                    minY = minPos.y;
                }

                if (maxPos.y > maxY)
                {
                    maxY = maxPos.y;
                }

                if (minPos.z < minZ)
                {
                    minZ = minPos.z;
                }

                if (maxPos.z > maxZ)
                {
                    maxZ = maxPos.z;
                }
            }

            Vector3 center = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, (minZ + maxZ) / 2);
            Vector3 size = new Vector3(maxX - minX, maxY - minY, maxZ - minZ);
            var bounds = new Bounds(center, size);
            return bounds;
        }

        /// <summary>
        /// 获取相交部分
        /// </summary>
        public static bool Intersection(this Bounds a, Bounds b, out Bounds result)
        {
            // 判断是否相交
            if (!a.Intersects(b))
            {
                result = new Bounds(Vector3.zero, Vector3.zero);
                return false;
            }

            // 计算各轴的重叠区域
            Vector3 min = Vector3.Max(a.min, b.min);
            Vector3 max = Vector3.Min(a.max, b.max);

            // 构造相交后的 Bounds
            Vector3 center = (min + max) * 0.5f;
            Vector3 size = max - min;
            result = new Bounds(center, size);
            return true;
        }

        public static bool Intersection(this List<Bounds> boundsList, out Bounds result)
        {
            if (boundsList == null || boundsList.Count == 0)
            {
                result = new Bounds(Vector3.zero, Vector3.zero);
                return false;
            }

            // 初始化交集为第一个 Bounds
            result = boundsList[0];

            // 遍历剩余 Bounds 并逐步缩小交集
            for (int i = 1; i < boundsList.Count; i++)
            {
                Bounds current = boundsList[i];

                // 若当前无交集，直接返回空
                if (!result.Intersects(current))
                {
                    return false;
                }

                // 计算新的交集范围
                Vector3 newMin = Vector3.Max(result.min, current.min);
                Vector3 newMax = Vector3.Min(result.max, current.max);

                // 更新交集
                result.SetMinMax(newMin, newMax);
            }

            return true;
        }

        public static bool Intersection<T>(this List<T> boundsList, Func<T, Bounds> getBounds, out Bounds result)
        {
            if (boundsList == null || boundsList.Count == 0)
            {
                result = new Bounds(Vector3.zero, Vector3.zero);
                return false;
            }

            // 初始化交集为第一个 Bounds
            result = getBounds.Invoke(boundsList[0]);

            // 遍历剩余 Bounds 并逐步缩小交集
            for (int i = 1; i < boundsList.Count; i++)
            {
                Bounds current = getBounds.Invoke(boundsList[i]);

                // 若当前无交集，直接返回空
                if (!result.Intersects(current))
                {
                    return false;
                }

                // 计算新的交集范围
                Vector3 newMin = Vector3.Max(result.min, current.min);
                Vector3 newMax = Vector3.Min(result.max, current.max);

                // 更新交集
                result.SetMinMax(newMin, newMax);
            }

            return true;
        }
    }
}