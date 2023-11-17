using System;
using PartsKit;
using UnityEngine;

[Serializable]
[FlowCreateNode("Test", "CreateName", "")]
public class TestCondition : BlueprintNode
{
    [SerializeField] private bool OutputBool = true;
    [SerializeField] private FlowPointState OutputFlowStata = FlowPointState.Running;

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
        bool state = OutputBool;
        if (conditionPort.GetPrePortFirst(out BlueprintValuePort<bool> targetPort))
        {
            targetPort.GetValue(out state);
        }

        return state;
    }

    private FlowPointState GetConditionValuePointState(BlueprintValuePort<FlowPointState> conditionPort)
    {
        FlowPointState state = OutputFlowStata;
        if (conditionPort.GetPrePortFirst(out BlueprintValuePort<FlowPointState> targetPort))
        {
            targetPort.GetValue(out state);
        }

        return state;
    }
}