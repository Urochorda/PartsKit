using System;
using PartsKit;

[Serializable]
[FlowCreateNode("Test", "CreateName", "")]
public class TestCondition : BlueprintNode
{
    protected override void RegisterPort()
    {
        base.RegisterPort();

        var boolOutput = BlueprintPortUtility.CreateInOutputPort<bool>("OutputBool",
            IBlueprintPort.Orientation.Horizontal, IBlueprintPort.Direction.Output);
        boolOutput.DefaultValue = true;
        var flowStataOutput = BlueprintPortUtility.CreateInOutputPort<FlowPointState>("OutputFlowStata",
            IBlueprintPort.Orientation.Horizontal, IBlueprintPort.Direction.Output);
        flowStataOutput.DefaultValue = FlowPointState.Running;

        AddPort(boolOutput);
        AddPort(flowStataOutput);
    }
}