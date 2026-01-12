using UnityEditor;

namespace PartsKit
{
    [CustomEditor(typeof(ButtonEffect), true)]
    [CanEditMultipleObjects]
    public class ButtonEffectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            ButtonMenuMethodAttributeEditor.DrawButtonMenu(targets);
        }
    }
}