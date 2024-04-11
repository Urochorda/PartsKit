using UnityEngine;
using UnityEditor;

namespace PartsKit
{
    [CustomEditor(typeof(MonoBehaviour))]
    [CanEditMultipleObjects]
    public class ButtonMenuEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var monoBehaviour = target as MonoBehaviour;
            if (monoBehaviour == null)
            {
                return;
            }

            var methods = monoBehaviour.GetType().GetMethods(System.Reflection.BindingFlags.Instance |
                                                             System.Reflection.BindingFlags.Public |
                                                             System.Reflection.BindingFlags.NonPublic);

            foreach (var method in methods)
            {
                var attributes = method.GetCustomAttributes(typeof(ButtonMenuAttribute), true);
                foreach (var attribute in attributes)
                {
                    if (attribute is ButtonMenuAttribute buttonAttribute)
                    {
                        if (GUILayout.Button(buttonAttribute.ButtonText))
                        {
                            method.Invoke(monoBehaviour, null);
                        }
                    }
                }
            }
        }
    }
}