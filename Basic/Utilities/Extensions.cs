using System;
using System.Collections.Generic;

namespace PartsKit
{
    public static class Extensions
    {
        public static void RemoveMatch<T>(this List<T> list, Predicate<T> match)
        {
            for (var i = 0; i < list.Count; i++)
            {
                if (match(list[i]))
                {
                    list.RemoveAt(i);
                    return;
                }
            }
        }

        public static void RemoveAtDisorder<T>(this List<T> self, int index)
        {
            if (index < 0 || index >= self.Count)
            {
                return;
            }

            int lastIndex = self.Count - 1;
            self[index] = self[lastIndex];
            self.RemoveAt(lastIndex);
        }

        public static void RemoveMatchDisorder<T>(this List<T> self, Predicate<T> match)
        {
            int index = self.FindIndex(match);
            if (index < 0)
            {
                return;
            }

            self.RemoveAtDisorder(index);
        }

        public static void RemoveDisorder<T>(this List<T> self, T item)
        {
            int index = self.IndexOf(item);
            self.RemoveAtDisorder(index);
        }
    }
}