using UnityEditor;
using UnityEngine;

namespace PartsKit
{
    [CustomEditor(typeof(AddressableGroupTemplateSync))]
    public class AddressableGroupTemplateSyncEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // 绘制默认Inspector
            DrawDefaultInspector();

            // 取到目标对象
            var script = (AddressableGroupTemplateSync)target;

            EditorGUILayout.Space();

            // Sync按钮
            if (GUILayout.Button("SyncTemplate"))
            {
                script.SyncTemplate();
                Debug.Log("Addressable groups synced.");
            }

            // ClearMiss按钮
            if (GUILayout.Button("ClearMiss"))
            {
                script.ClearMiss();
                Debug.Log("Cleared group list.");
            }
        }
    }
}