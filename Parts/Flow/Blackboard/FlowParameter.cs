using System;
using UnityEngine;

namespace PartsKit
{
    #region Int

    [Serializable]
    [FlowCreateParameter("Int")]
    public class FlowParameterInt : BlueprintParameter<int>
    {
        public override Type SetNodeType => typeof(FlowParameterIntSetNode);
    }

    [Serializable]
    public class FlowParameterIntSetNode : BlueprintSetParameterNode
    {
        [SerializeField] private int setValue;

        protected override void GetSetValueInfo(out object setValueVal, out string propertyFieldNameVal)
        {
            setValueVal = setValue;
            propertyFieldNameVal = nameof(setValue);
        }
    }

    #endregion

    #region Float

    [Serializable]
    [FlowCreateParameter("Float")]
    public class FlowParameterFloat : BlueprintParameter<float>
    {
        public override Type SetNodeType => typeof(FlowParameterFloatSetNode);
    }

    [Serializable]
    public class FlowParameterFloatSetNode : BlueprintSetParameterNode
    {
        [SerializeField] private float setValue;

        protected override void GetSetValueInfo(out object setValueVal, out string propertyFieldNameVal)
        {
            setValueVal = setValue;
            propertyFieldNameVal = nameof(setValue);
        }
    }

    #endregion

    #region String

    [Serializable]
    [FlowCreateParameter("String")]
    public class FlowParameterString : BlueprintParameter<string>
    {
        public override Type SetNodeType => typeof(FlowParameterStringSetNode);
    }

    [Serializable]
    public class FlowParameterStringSetNode : BlueprintSetParameterNode
    {
        [SerializeField] private string setValue;

        protected override void GetSetValueInfo(out object setValueVal, out string propertyFieldNameVal)
        {
            setValueVal = setValue;
            propertyFieldNameVal = nameof(setValue);
        }
    }

    #endregion
}