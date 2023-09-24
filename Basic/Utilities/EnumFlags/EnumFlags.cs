using System;
using UnityEngine;

namespace PartsKit
{
    public class EnumFlagsAttribute : PropertyAttribute
    {
        public Type EnumType { get; }

        public EnumFlagsAttribute(Type enumType)
        {
            EnumType = enumType;
        }
    }
}