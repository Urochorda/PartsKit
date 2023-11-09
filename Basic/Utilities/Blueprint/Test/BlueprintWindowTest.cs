using UnityEngine.UIElements;

namespace PartsKit
{
    public class BlueprintWindowTest : BlueprintWindow
    {
        protected override BlueprintView OnCreateBlueprintView()
        {
            BlueprintViewTest bv = new BlueprintViewTest();
            rootVisualElement.Add(bv);
            bv.StretchToParentSize();
            return bv;
        }
    }
}