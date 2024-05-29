using UnityEngine;

namespace PartsKit
{
    public abstract class BlueprintParameterNodeBase : BlueprintNode
    {
        [field: SerializeField] public string ParameterGuid { get; private set; }
        protected IBlueprintParameter Parameter { get; private set; }

        public override void Init(Blueprint blueprintVal)
        {
            Parameter = blueprintVal.Blackboard.GetParameterByGuid(ParameterGuid);
            base.Init(blueprintVal);
        }

        public override bool IsNotValid()
        {
            return OwnerBlueprint == null || OwnerBlueprint.Blackboard.GetParameterByGuid(ParameterGuid) == null;
        }

        public void OnCreateParameterNode(string pGuid)
        {
            ParameterGuid = pGuid;
        }
    }
}