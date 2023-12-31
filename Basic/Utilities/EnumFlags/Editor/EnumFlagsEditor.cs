using System;
using UnityEditor;
using UnityEngine;

namespace PartsKit
{
    [CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
    public class EnumFlagsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.PrefixLabel(position, new GUIContent(property.displayName));
            var enumType = (attribute as EnumFlagsAttribute)?.EnumType;
            if (enumType != null)
            {
                var e = EditorGUI.EnumFlagsField(position,
                    (Enum)Enum.ToObject(enumType, property.intValue));
                property.intValue = Convert.ToInt32(e);
            }
        }
    }
}