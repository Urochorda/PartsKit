using System;
using UnityEngine;

namespace PartsKit
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ButtonMenuFieldAttribute : PropertyAttribute
    {
        public readonly string MethodName;

        public ButtonMenuFieldAttribute(string methodName)
        {
            MethodName = methodName;
        }
    }
}