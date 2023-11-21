using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace PartsKit
{
    public class BlueprintBlackboardField : BlackboardField
    {
        public BlueprintView OwnerView { get; private set; }
        public IBlueprintParameter Parameter { get; private set; }

        public virtual void Init(BlueprintView graphView, IBlueprintParameter param)
        {
            OwnerView = graphView;
            Parameter = param;
            icon = null;
            text = param.ParameterName;
            typeText = param.ParameterTypeName;

            this.AddManipulator(new ContextualMenuManipulator(BuildContextualMenu));

            (this.Q("textField") as TextField).RegisterValueChangedCallback((e) =>
            {
                graphView.RenameBlackboardParameter(this, e.newValue);
            });
        }

        private void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Rename", (a) => OpenTextEditor(), DropdownMenuAction.AlwaysEnabled);
            evt.menu.AppendAction("Delete", (a) => OwnerView.RemoveBlackboardParameter(this),
                DropdownMenuAction.AlwaysEnabled);
            evt.StopPropagation();
        }
    }
}