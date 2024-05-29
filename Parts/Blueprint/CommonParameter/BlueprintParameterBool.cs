using System;
using UnityEngine;

namespace PartsKit
{
    [Serializable]
    [BlueprintParameterCreate(Blueprint.CommonParameterGroup, "Bool")]
    public class BlueprintParameterBool : BlueprintParameter<bool>
    {
        public override Type SetNodeType => typeof(BlueprintParameterBoolSetNode);
    }

    [Serializable]
    public class BlueprintParameterBoolSetNode : BlueprintSetParameterNode
    {
        [SerializeField] private bool setValue;

        protected override void GetSetValueInfo(out object setValueVal, out string propertyFieldNameVal)
        {
            setValueVal = setValue;
            propertyFieldNameVal = nameof(setValue);
        }
    }
}