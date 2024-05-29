using System;
using UnityEngine;

namespace PartsKit
{
    [Serializable]
    [BlueprintParameterCreate(Blueprint.CommonParameterGroup, "Float")]
    public class BlueprintParameterFloat : BlueprintParameter<float>
    {
        public override Type SetNodeType => typeof(BlueprintParameterFloatSetNode);
    }

    [Serializable]
    public class BlueprintParameterFloatSetNode : BlueprintSetParameterNode
    {
        [SerializeField] private float setValue;

        protected override void GetSetValueInfo(out object setValueVal, out string propertyFieldNameVal)
        {
            setValueVal = setValue;
            propertyFieldNameVal = nameof(setValue);
        }
    }
}