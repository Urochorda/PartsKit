using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PartsKit
{
    [CustomEditor(typeof(Blueprint), true)]
    public class BlueprintInspectorTest : Editor
    {
        public sealed override VisualElement CreateInspectorGUI()
        {
            base.CreateInspectorGUI();
            VisualElement root = new VisualElement();
            root.Add(new Button(() =>
            {
                BlueprintWindowTest window = EditorWindow.GetWindow<BlueprintWindowTest>();
                window.titleContent = new GUIContent("BlueprintWindowTest");
                window.InitWindow(target as Blueprint);
            })
            {
                text = "OpenTestWindow"
            });
            return root;
        }
    }
}