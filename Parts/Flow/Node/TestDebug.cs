using System;
using PartsKit;
using UnityEngine;

[Serializable]
[FlowCreateNode("Test", "Debug", "")]
public class TestDebug : BlueprintNode
{
    public BlueprintExecutePort InputExePort { get; private set; }

    private BlueprintExecutePortResult executePortResult;

    protected override void RegisterPort()
    {
        base.RegisterPort();

        InputExePort = BlueprintPortUtility.CreateExecutePort("InputExe",
            IBlueprintPort.Orientation.Horizontal, IBlueprintPort.Direction.Input, OnExecuted);

        AddPort(InputExePort);
    }

    protected BlueprintExecutePortResult OnExecuted(BlueprintExecutePort executePort)
    {
        Debug.LogError("测试测试");
        executePortResult.NextExecute = null;
        executePortResult.ExecuteState = BlueprintExecuteState.End;
        return executePortResult;
    }
}