using System;
using UnityEngine;

namespace PartsKit
{
    [Serializable]
    [BlueprintNodeCreate(Blueprint.CommonNodeGroup, "Sequence", CreateName, "")]
    public class BlueprintReverseBoolNode : BlueprintNode
    {
        private const string CreateName = "!-Bool";
        public override string NodeName => CreateName;
        public BlueprintValuePort<bool> InValuePort { get; private set; }
        public BlueprintValuePort<bool> OutValuePort { get; private set; }
        [SerializeField] private bool inDefaultValue;

        protected override void RegisterPort()
        {
            base.RegisterPort();
            InValuePort = BlueprintPortUtility.CreateValuePort<bool>("In",
                IBlueprintPort.Orientation.Horizontal,
                IBlueprintPort.Direction.Input, nameof(inDefaultValue), OnInGetValue);

            OutValuePort = BlueprintPortUtility.CreateValuePort<bool>("Out",
                IBlueprintPort.Orientation.Horizontal,
                IBlueprintPort.Direction.Output, String.Empty, OnOutGetValue);

            AddPort(InValuePort);
            AddPort(OutValuePort);
        }

        private bool OnInGetValue(BlueprintValuePort<bool> arg)
        {
            if (!arg.GetPrePortValue(out bool inValue))
            {
                inValue = inDefaultValue;
            }

            return inValue;
        }

        private bool OnOutGetValue(BlueprintValuePort<bool> arg)
        {
            InValuePort.GetValue(out bool inValue);
            return !inValue;
        }
    }
}