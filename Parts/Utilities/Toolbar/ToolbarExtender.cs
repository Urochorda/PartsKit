#if UNITY_EDITOR

using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace PartsKit
{
    [InitializeOnLoad]
    public static class ToolbarExtender
    {
        public static event Action OnGuiBodyCallback;
        private static readonly Type KToolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
        private static ScriptableObject sCurrentToolbar;


        static ToolbarExtender()
        {
            EditorApplication.update += OnUpdate;
        }

        private static void OnUpdate()
        {
            if (sCurrentToolbar == null)
            {
                UnityEngine.Object[] toolbars = Resources.FindObjectsOfTypeAll(KToolbarType);
                sCurrentToolbar = toolbars.Length > 0 ? (ScriptableObject)toolbars[0] : null;
                if (sCurrentToolbar == null)
                {
                    return;
                }

                FieldInfo root = sCurrentToolbar.GetType()
                    .GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
                if (root == null)
                {
                    return;
                }

                VisualElement concreteRoot = root.GetValue(sCurrentToolbar) as VisualElement;

                VisualElement toolbarZone = concreteRoot.Q("ToolbarZoneRightAlign");
                VisualElement parent = new VisualElement()
                {
                    style =
                    {
                        flexGrow = 1,
                        flexDirection = FlexDirection.Row,
                    }
                };
                IMGUIContainer container = new IMGUIContainer();
                container.onGUIHandler += OnGuiBody;
                parent.Add(container);
                toolbarZone.Add(parent);
            }
        }

        private static void OnGuiBody()
        {
            //自定义按钮加在此处
            GUILayout.BeginHorizontal();
            OnGuiBodyCallback?.Invoke();
            GUILayout.EndHorizontal();
        }
    }
}

#endif