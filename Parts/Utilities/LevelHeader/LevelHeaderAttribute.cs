using UnityEngine;

namespace PartsKit
{
    public class LevelHeaderAttribute : PropertyAttribute
    {
        public string Header { get; private set; }
        public int Level { get; private set; }

        public LevelHeaderAttribute(string header, int level = 0)
        {
            Header = header;
            Level = level;
        }
    }
}