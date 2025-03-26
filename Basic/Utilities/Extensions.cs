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

        public static Bounds Encapsulate<T>(this IEnumerable<T> tList, Func<T, Bounds> getBounds)
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
    }
}