using System;
using PartsKit;
using UnityEngine;

[Serializable]
[FlowCreateNode("Test", "Debug", "")]
public class TestDebug : BlueprintNode
{
    public BlueprintExecutePort InputExePort { get; private set; }

    protected override void RegisterPort()
    {
        base.RegisterPort();

        InputExePort = BlueprintPortUtility.CreateExecutePort("InputExe",
            IBlueprintPort.Orientation.Horizontal, IBlueprintPort.Direction.Input);

        AddPort(InputExePort);
    }

    protected override void OnExecuted(IBlueprintPort port, out BlueprintExecutePort nextPort,
        out BlueprintExecuteState executeState)
    {
        Debug.LogError("测试测试");
        nextPort = null;
        executeState = BlueprintExecuteState.End;
    }
}