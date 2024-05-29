using System;
using UnityEngine;

namespace PartsKit
{
    [Serializable]
    [BlueprintParameterCreate(Blueprint.CommonParameterGroup, "String")]
    public class BlueprintParameterString : BlueprintParameter<string>
    {
        public override Type SetNodeType => typeof(BlueprintParameterStringSetNode);
    }

    [Serializable]
    public class BlueprintParameterStringSetNode : BlueprintSetParameterNode
    {
        [SerializeField] private string setValue;

        protected override void GetSetValueInfo(out object setValueVal, out string propertyFieldNameVal)
        {
            setValueVal = setValue;
            propertyFieldNameVal = nameof(setValue);
        }
    }
}