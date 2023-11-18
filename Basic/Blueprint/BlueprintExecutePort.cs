using System;

namespace PartsKit
{
    public enum BlueprintExecuteState
    {
        Running = 1, //本节点需要继续执行
        End = 2, //本节点执行完毕
        Wait = 3, //本次执行到本节点后暂停
    }

    public struct BlueprintExecutePortData
    {
    }

    public struct BlueprintExecutePortResult
    {
        public BlueprintExecutePort NextExecute { get; set; }
        public BlueprintExecuteState ExecuteState { get; set; }
    }

    public class BlueprintExecutePort : BlueprintPortBase<BlueprintExecutePortData>
    {
        public BlueprintExecuteState ExecuteState { get; private set; }

        private readonly Func<BlueprintExecutePort, BlueprintExecutePortResult> onExecute;

        /// <summary>
        /// 参数为必要数据，必填
        /// </summary>
        public BlueprintExecutePort(string portNameVal, IBlueprintPort.Orientation portOrientationVal,
            IBlueprintPort.Direction portDirectionVal, IBlueprintPort.Capacity portCapacityVal,
            Func<BlueprintExecutePort, BlueprintExecutePortResult> onExecuteVal) : base(
            portNameVal, portOrientationVal, portDirectionVal, portCapacityVal,String.Empty)
        {
            onExecute = onExecuteVal;
        }

        public void Execute(out BlueprintExecutePort nextExecute)
        {
            BlueprintExecuteState executeStateVal;
            if (onExecute == null)
            {
                executeStateVal = BlueprintExecuteState.End;
                nextExecute = null;
            }
            else
            {
                BlueprintExecutePortResult result = onExecute.Invoke(this);
                executeStateVal = result.ExecuteState;
                nextExecute = result.NextExecute;
            }

            ExecuteState = executeStateVal;
        }

        public bool GetNextExecute(out BlueprintExecutePort targetPort)
        {
            if (NextPorts.Count > 0)
            {
                if (NextPorts[0] is BlueprintExecutePort targetPortVal)
                {
                    targetPort = targetPortVal;
                    return true;
                }
            }

            targetPort = null;
            return false;
        }
    }
}