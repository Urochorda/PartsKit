namespace PartsKit
{
    public class BlueprintPortCommon
    {
        private static BlueprintExecutePortResult executePortResult;

        public static BlueprintExecutePortResult OnToNextPortEndExecuted(BlueprintExecutePort executePort)
        {
            executePort.GetNextExecute(out BlueprintExecutePort targetPort);
            executePortResult.NextExecute = targetPort;
            executePortResult.ExecuteState = BlueprintExecuteState.End;
            return executePortResult;
        }

        public static BlueprintExecutePortResult OnToTargetPortEndExecuted(BlueprintExecutePort targetPort)
        {
            executePortResult.NextExecute = targetPort;
            executePortResult.ExecuteState = BlueprintExecuteState.End;
            return executePortResult;
        }
    }
}