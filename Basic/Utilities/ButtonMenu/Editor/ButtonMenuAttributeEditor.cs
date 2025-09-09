using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace PartsKit
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    [CanEditMultipleObjects]
    public class ButtonMenuEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            // 用第一个对象的类型来找方法
            var monoBehaviour = target as MonoBehaviour;
            if (monoBehaviour == null)
            {
                return;
            }

            var methods = monoBehaviour.GetType().GetMethods(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var method in methods)
            {
                var buttonAttr = (ButtonMenuAttribute)Attribute.GetCustomAttribute(method, typeof(ButtonMenuAttribute));
                if (buttonAttr == null)
                {
                    continue;
                }

                if (!GUILayout.Button(buttonAttr.ButtonText))
                {
                    continue;
                }

                foreach (var t in targets) // 多对象一起执行
                {
                    try
                    {
                        method.Invoke(t, null);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"执行{method.Name}失败: {e}");
                    }
                }
            }
        }
    }
}