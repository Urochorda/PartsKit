using System;

namespace PartsKit
{
    [Serializable]
    [FlowCreateParameter("Int")]
    public class FlowParameterInt : BlueprintParameter<int>
    {
    }

    [Serializable]
    [FlowCreateParameter("Float")]
    public class FlowParameterFloat : BlueprintParameter<float>
    {
    }

    [Serializable]
    [FlowCreateParameter("String")]
    public class FlowParameterString : BlueprintParameter<string>
    {
    }
}