using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PartsKit
{
    [CustomPropertyDrawer(typeof(ButtonMenuFieldAttribute))]
    public class ButtonMenuFieldAttributeDrawer : PropertyDrawer
    {
        private MethodInfo eventMethodInfo = null;

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            ButtonMenuFieldAttribute inspectorButtonAttribute = (ButtonMenuFieldAttribute)attribute;
            Rect buttonRect = new Rect(position.x, position.y, position.width, position.height);

            if (GUI.Button(buttonRect, inspectorButtonAttribute.MethodName))
            {
                object ownerObject = GetPropertyOwner(prop);
                if (ownerObject == null)
                {
                    Debug.LogWarning("ButtonMenuField: owner object is null");
                    return;
                }

                Type eventOwnerType = ownerObject.GetType();
                string eventName = inspectorButtonAttribute.MethodName;

                if (eventMethodInfo == null || eventMethodInfo.DeclaringType != eventOwnerType)
                {
                    eventMethodInfo = eventOwnerType.GetMethod(eventName,
                        BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                }

                if (eventMethodInfo != null)
                {
                    eventMethodInfo.Invoke(ownerObject, null);
                    EditorUtility.SetDirty(prop.serializedObject.targetObject);
                }
                else
                {
                    Debug.LogWarning(string.Format("InspectorButton: Unable to find method {0} in {1}", eventName,
                        eventOwnerType));
                }
            }
        }

        private object GetPropertyOwner(SerializedProperty prop)
        {
            object obj = prop.serializedObject.targetObject;
            string path = prop.propertyPath.Replace(".Array.data[", "[");

            string[] parts = path.Split('.');
            for (int i = 0; i < parts.Length - 1; i++) //注意 -1
            {
                string part = parts[i];

                if (part.Contains("["))
                {
                    string fieldName = part.Substring(0, part.IndexOf("[", StringComparison.Ordinal));
                    int index = int.Parse(part.Substring(part.IndexOf("[", StringComparison.Ordinal) + 1,
                        part.IndexOf("]", StringComparison.Ordinal) - part.IndexOf("[", StringComparison.Ordinal) - 1));
                    obj = GetFieldValue(obj, fieldName, index);
                }
                else
                {
                    obj = GetFieldValue(obj, part, -1);
                }
            }

            return obj;
        }

        private object GetFieldValue(object source, string name, int index)
        {
            if (source == null)
            {
                return null;
            }

            var type = source.GetType();
            var field = type.GetField(
                name,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (field == null)
            {
                return null;
            }

            var value = field.GetValue(source);

            if (index >= 0 && value is System.Collections.IList list)
            {
                return list[index];
            }

            return value;
        }
    }
}