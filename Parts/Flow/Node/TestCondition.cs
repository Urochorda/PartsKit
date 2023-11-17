using System;
using PartsKit;
using UnityEngine;

[Serializable]
[FlowCreateNode("Test", "CreateName", "")]
public class TestCondition : BlueprintNode
{
    [SerializeField] private bool outputBool = true;
    [SerializeField] private FlowPointState outputFlowStata = FlowPointState.Running;

    protected override void RegisterPort()
    {
        base.RegisterPort();

        var boolOutput = BlueprintPortUtility.CreateValuePort<bool>("OutputBool",
            IBlueprintPort.Orientation.Horizontal, IBlueprintPort.Direction.Output, nameof(outputBool),
            GetConditionValueBool);
        var flowStataOutput = BlueprintPortUtility.CreateValuePort<FlowPointState>("OutputFlowStata",
            IBlueprintPort.Orientation.Horizontal, IBlueprintPort.Direction.Output, nameof(outputFlowStata),
            GetConditionValuePointState);

        AddPort(boolOutput);
        AddPort(flowStataOutput);
    }

    private bool GetConditionValueBool(BlueprintValuePort<bool> conditionPort)
    {
        bool state = outputBool;
        if (conditionPort.GetPrePortFirst(out BlueprintValuePort<bool> targetPort))
        {
            targetPort.GetValue(out state);
        }

        return state;
    }

    private FlowPointState GetConditionValuePointState(BlueprintValuePort<FlowPointState> conditionPort)
    {
        FlowPointState state = outputFlowStata;
        if (conditionPort.GetPrePortFirst(out BlueprintValuePort<FlowPointState> targetPort))
        {
            targetPort.GetValue(out state);
        }

        return state;
    }
}