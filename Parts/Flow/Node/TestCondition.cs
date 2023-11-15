using System;
using PartsKit;

[Serializable]
[FlowCreateNode("Test", "CreateName", "")]
public class TestCondition : BlueprintNode
{
    protected override void RegisterPort()
    {
        base.RegisterPort();

        var boolOutput = BlueprintPortUtility.CreateValuePort<bool>("OutputBool",
            IBlueprintPort.Orientation.Horizontal, IBlueprintPort.Direction.Output, GetConditionValueBool);
        var flowStataOutput = BlueprintPortUtility.CreateValuePort<FlowPointState>("OutputFlowStata",
            IBlueprintPort.Orientation.Horizontal, IBlueprintPort.Direction.Output, GetConditionValuePointState);

        AddPort(boolOutput);
        AddPort(flowStataOutput);
    }

    private bool GetConditionValueBool(BlueprintValuePort<bool> conditionPort)
    {
        bool state = true;
        if (conditionPort.GetPrePortFirst(out BlueprintValuePort<bool> targetPort))
        {
            targetPort.GetValue(out state);
        }

        return state;
    }

    private FlowPointState GetConditionValuePointState(BlueprintValuePort<FlowPointState> conditionPort)
    {
        FlowPointState state = FlowPointState.Running;
        if (conditionPort.GetPrePortFirst(out BlueprintValuePort<FlowPointState> targetPort))
        {
            targetPort.GetValue(out state);
        }

        return state;
    }
}