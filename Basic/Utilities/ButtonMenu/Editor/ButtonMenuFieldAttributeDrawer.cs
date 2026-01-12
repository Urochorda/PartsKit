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
                System.Type eventOwnerType = prop.serializedObject.targetObject.GetType();
                string eventName = inspectorButtonAttribute.MethodName;

                if (eventMethodInfo == null)
                {
                    eventMethodInfo = eventOwnerType.GetMethod(eventName,
                        BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                }

                if (eventMethodInfo != null)
                {
                    eventMethodInfo.Invoke(prop.serializedObject.targetObject, null);
                }
                else
                {
                    Debug.LogWarning(string.Format("InspectorButton: Unable to find method {0} in {1}", eventName,
                        eventOwnerType));
                }
            }
        }
    }
}