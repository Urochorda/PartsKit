using UnityEditor;
using UnityEngine;

namespace PartsKit
{
    [CustomPropertyDrawer(typeof(LevelHeaderAttribute))]
    public class LevelHeaderEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            LevelHeaderAttribute sectionHeader = (LevelHeaderAttribute)attribute;

            // Calculate the height for the header
            float headerHeight = EditorGUIUtility.singleLineHeight;

            // Draw the header
            Rect headerRect = new Rect(position.x, position.y, position.width, headerHeight);
            bool isMax = sectionHeader.Level <= 0;
            string headerText = isMax
                ? sectionHeader.Header
                : new string(' ', (sectionHeader.Level - 1) * 2) + sectionHeader.Header;
            GUIStyle labelStyle = isMax ? EditorStyles.boldLabel : EditorStyles.label;

            EditorGUI.LabelField(headerRect, headerText, labelStyle);

            // Adjust the position for the property field
            Rect propertyRect = new Rect(position.x, position.y + headerHeight, position.width,
                EditorGUI.GetPropertyHeight(property));
            EditorGUI.PropertyField(propertyRect, property, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight + EditorGUI.GetPropertyHeight(property);
        }
    }
}