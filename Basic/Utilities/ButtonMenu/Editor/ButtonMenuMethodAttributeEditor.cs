using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace PartsKit
{
    public static class ButtonMenuMethodAttributeEditor
    {
        public static void DrawButtonMenu(IList<Object> targets)
        {
            if (targets == null || targets.Count <= 0)
            {
                return;
                
            }

            var target = targets[0];
            var methods = target.GetType().GetMethods(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var method in methods)
            {
                var buttonAttr =
                    (ButtonMenuMethodAttribute)Attribute.GetCustomAttribute(method, typeof(ButtonMenuMethodAttribute));
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

                    EditorUtility.SetDirty(t);
                }
            }
        }
    }
}