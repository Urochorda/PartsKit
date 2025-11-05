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
        public static event Action OnGuiBodyRightCallback;
        public static event Action OnGuiBodyRightReverseCallback;
        public static event Action OnGuiBodyLeftCallback;
        public static event Action OnGuiBodyLeftReverseCallback;

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

                SetContainer("ToolbarZoneRightAlign", FlexDirection.RowReverse, OnGuiBodyRightReverse);
                SetContainer("ToolbarZoneRightAlign", FlexDirection.Row, OnGuiBodyRight);
                SetContainer("ToolbarZoneLeftAlign", FlexDirection.Row, OnGuiBodyLeft);
                SetContainer("ToolbarZoneLeftAlign", FlexDirection.RowReverse, OnGuiBodyLeftReverse);

                void SetContainer(string zoneName, FlexDirection flexDirection, Action onGUIHandler)
                {
                    VisualElement toolbarZone = concreteRoot.Q(zoneName);
                    VisualElement parent = new VisualElement()
                    {
                        style =
                        {
                            flexGrow = 1,
                            flexDirection = flexDirection,
                        }
                    };
                    IMGUIContainer container = new IMGUIContainer();
                    container.onGUIHandler += onGUIHandler;
                    parent.Add(container);
                    toolbarZone.Add(parent);
                }
            }
        }

        private static void OnGuiBodyRight()
        {
            GUILayout.BeginHorizontal();
            OnGuiBodyRightCallback?.Invoke();
            GUILayout.EndHorizontal();
        }

        private static void OnGuiBodyRightReverse()
        {
            GUILayout.BeginHorizontal();
            OnGuiBodyRightReverseCallback?.Invoke();
            GUILayout.EndHorizontal();
        }

        private static void OnGuiBodyLeft()
        {
            GUILayout.BeginHorizontal();
            OnGuiBodyLeftCallback?.Invoke();
            GUILayout.EndHorizontal();
        }

        private static void OnGuiBodyLeftReverse()
        {
            GUILayout.BeginHorizontal();
            OnGuiBodyLeftReverseCallback?.Invoke();
            GUILayout.EndHorizontal();
        }
    }
}