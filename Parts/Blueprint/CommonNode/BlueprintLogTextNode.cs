using System;
using UnityEngine;

namespace PartsKit
{
    [Serializable]
    [BlueprintNodeCreate(Blueprint.CommonNodeGroup, "Debug", CreateName, "")]
    public class BlueprintLogTextNode : BlueprintInOutputExeNodeBase
    {
        public enum LogType
        {
            Log = 1,
            LogError = 2,
            LogWarning = 3,
        }

        private const string CreateName = "LogText";
        public override string NodeName => CreateName;
        public BlueprintValuePort<LogType> LogTypePort { get; private set; }
        public BlueprintValuePort<string> DebugPort { get; private set; }

        [SerializeField] private string info = string.Empty;
        [SerializeField] private LogType logType = LogType.Log;

        protected override void RegisterPort()
        {
            base.RegisterPort();

            DebugPort = BlueprintPortUtility.CreateValuePort<string>("Text", IBlueprintPort.Orientation.Horizontal,
                IBlueprintPort.Direction.Input, nameof(info), GetTextInfoValue);

            LogTypePort = BlueprintPortUtility.CreateValuePort<LogType>("LogType",
                IBlueprintPort.Orientation.Horizontal,
                IBlueprintPort.Direction.Input, nameof(logType), GetLogTypeValue);

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

        protected override BlueprintExecutePortResult OnInputExecuted(BlueprintExecutePort executePort)
        {
            DebugPort.GetValue(out string infoVal);
            LogTypePort.GetValue(out LogType logTypeVal);
            switch (logTypeVal)
            {
                default:
                case LogType.Log:
                    CustomLog.Log(infoVal);
                    break;
                case LogType.LogError:
                    CustomLog.LogError(infoVal);
                    break;
                case LogType.LogWarning:
                    CustomLog.LogWarning(infoVal);
                    break;
            }

            return base.OnInputExecuted(executePort);
        }
    }
}