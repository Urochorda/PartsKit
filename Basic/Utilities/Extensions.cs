using System;
using System.Collections.Generic;

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
    }
}