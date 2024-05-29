using System;
using UnityEngine;

namespace PartsKit
{
    [Serializable]
    [BlueprintParameterCreate(Blueprint.CommonParameterGroup, "Int")]
    public class BlueprintParameterInt : BlueprintParameter<int>
    {
        public override Type SetNodeType => typeof(BlueprintParameterIntSetNode);
    }

    [Serializable]
    public class BlueprintParameterIntSetNode : BlueprintSetParameterNode
    {
        [SerializeField] private int setValue;

        protected override void GetSetValueInfo(out object setValueVal, out string propertyFieldNameVal)
        {
            setValueVal = setValue;
            propertyFieldNameVal = nameof(setValue);
        }
    }
}