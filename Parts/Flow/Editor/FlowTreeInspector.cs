using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PartsKit
{
    [CustomEditor(typeof(FlowTree), true)]
    public class FlowTreeInspector : Editor
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
                FlowTreeWindow window = EditorWindow.GetWindow<FlowTreeWindow>();
                window.titleContent = new GUIContent("FlowTreeWindow");
                window.InitWindow(target as Blueprint);
            })
            {
                text = "Editor"
            });

            return root;
        }
    }
}