using System;
using UnityEngine;

namespace PartsKit
{
    [Serializable]
    [FlowCreateNode("Debug", CreateName, "")]
    public class FlowLogTextNode : BlueprintNode
    {
        public enum LogType
        {
            Log = 1,
            LogError = 2,
            LogWarning = 3,
        }

        private const string CreateName = "LogText";
        public override string NodeName => CreateName;
        public BlueprintExecutePort InputExePort { get; private set; }
        public BlueprintExecutePort OutputExePort { get; private set; }
        public BlueprintValuePort<LogType> LogTypePort { get; private set; }
        public BlueprintValuePort<string> DebugPort { get; private set; }

        private BlueprintExecutePortResult executePortResult;
        [SerializeField] private string info = string.Empty;
        [SerializeField] private LogType logType = LogType.Log;

        protected override void RegisterPort()
        {
            base.RegisterPort();

            InputExePort = BlueprintPortUtility.CreateExecutePort("InputExe",
                IBlueprintPort.Orientation.Horizontal, IBlueprintPort.Direction.Input, OnExecuted);

            OutputExePort = BlueprintPortUtility.CreateExecutePort("OutputExe",
                IBlueprintPort.Orientation.Horizontal, IBlueprintPort.Direction.Output, OnOutputExecuted);

            DebugPort = BlueprintPortUtility.CreateValuePort<string>("Text", IBlueprintPort.Orientation.Horizontal,
                IBlueprintPort.Direction.Input, nameof(info), GetTextInfoValue);

            LogTypePort = BlueprintPortUtility.CreateValuePort<LogType>("LogType",
                IBlueprintPort.Orientation.Horizontal,
                IBlueprintPort.Direction.Input, nameof(logType), GetLogTypeValue);

            AddPort(InputExePort);
            AddPort(OutputExePort);
            AddPort(LogTypePort);
            AddPort(DebugPort);
        }

        private string GetTextInfoValue(BlueprintValuePort<string> arg)
        {
            string infoVal = info;
            if (DebugPort.GetPrePortValue(out string prePortVal))
            {
                infoVal = prePortVal;
            }

            return infoVal;
        }

        private LogType GetLogTypeValue(BlueprintValuePort<LogType> arg)
        {
            LogType logTypeVal = logType;
            if (LogTypePort.GetPrePortValue(out LogType prePortVal))
            {
                logTypeVal = prePortVal;
            }

            return logTypeVal;
        }

        protected BlueprintExecutePortResult OnExecuted(BlueprintExecutePort executePort)
        {
            DebugPort.GetValue(out string infoVal);
            LogTypePort.GetValue(out LogType logTypeVal);
            switch (logTypeVal)
            {
                default:
                case LogType.Log:
                    Debug.Log(infoVal);
                    break;
                case LogType.LogError:
                    Debug.LogError(infoVal);
                    break;
                case LogType.LogWarning:
                    Debug.LogWarning(infoVal);
                    break;
            }

            executePortResult.NextExecute = OutputExePort;
            executePortResult.ExecuteState = BlueprintExecuteState.End;
            return executePortResult;
        }

        private BlueprintExecutePortResult OnOutputExecuted(BlueprintExecutePort executePort)
        {
            executePort.GetNextExecute(out BlueprintExecutePort targetPort);
            executePortResult.NextExecute = targetPort;
            executePortResult.ExecuteState = BlueprintExecuteState.End;
            return executePortResult;
        }
    }
}