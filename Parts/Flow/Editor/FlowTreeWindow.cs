using UnityEngine.UIElements;

namespace PartsKit
{
    public class FlowTreeWindow : BlueprintWindow
    {
        protected override BlueprintView OnCreateBlueprintView()
        {
            FlowTreeView bv = new FlowTreeView();
            rootVisualElement.Add(bv);
            bv.StretchToParentSize();
            return bv;
        }
    }
}