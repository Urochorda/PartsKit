using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PartsKit
{
    [CustomEditor(typeof(TestBlueprint), true)]
    public class BlueprintInspectorTest : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            // 创建根VisualElement
            VisualElement root = new VisualElement();

            // 添加包含原始Inspector的IMGUIContainer
            var defaultInspector = new IMGUIContainer(() => DrawDefaultInspector());
            root.Add(defaultInspector);

            // 添加自定义按钮
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

        // public override void OnInspectorGUI()
        // {
        //     base.OnInspectorGUI();
        //     // 添加自定义按钮
        //     if (GUILayout.Button("OpenTestWindow"))
        //     {
        //         BlueprintWindowTest window = EditorWindow.GetWindow<BlueprintWindowTest>();
        //         window.titleContent = new GUIContent("BlueprintWindowTest");
        //         window.InitWindow(target as Blueprint);
        //     }
        // }
    }
}