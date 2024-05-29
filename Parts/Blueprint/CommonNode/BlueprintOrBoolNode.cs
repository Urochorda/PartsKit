using System;
using UnityEngine;

namespace PartsKit
{
    [Serializable]
    [BlueprintNodeCreate(Blueprint.CommonNodeGroup, "Sequence", CreateName, "")]
    public class BlueprintOrBoolNode : BlueprintNode
    {
        private const string CreateName = "||-Bool";
        public override string NodeName => CreateName;
        public BlueprintValuePort<bool> InValuePort { get; private set; }
        public BlueprintValuePort<bool> InValuePort2 { get; private set; }
        public BlueprintValuePort<bool> OutValuePort { get; private set; }
        [SerializeField] private bool inDefaultValue;
        [SerializeField] private bool inDefaultValue2;

        protected override void RegisterPort()
        {
            base.RegisterPort();
            InValuePort = BlueprintPortUtility.CreateValuePort<bool>("In",
                IBlueprintPort.Orientation.Horizontal,
                IBlueprintPort.Direction.Input, nameof(inDefaultValue), OnInGetValue);

            InValuePort2 = BlueprintPortUtility.CreateValuePort<bool>("In2",
                IBlueprintPort.Orientation.Horizontal,
                IBlueprintPort.Direction.Input, nameof(inDefaultValue2), OnInGetValue2);

            OutValuePort = BlueprintPortUtility.CreateValuePort<bool>("Out",
                IBlueprintPort.Orientation.Horizontal,
                IBlueprintPort.Direction.Output, String.Empty, OnOutGetValue);

            AddPort(InValuePort);
            AddPort(InValuePort2);
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

        private bool OnInGetValue2(BlueprintValuePort<bool> arg)
        {
            if (!arg.GetPrePortValue(out bool inValue))
            {
                inValue = inDefaultValue2;
            }

            return inValue;
        }

        private bool OnOutGetValue(BlueprintValuePort<bool> arg)
        {
            InValuePort.GetValue(out bool inValue);
            InValuePort2.GetValue(out bool inValue2);
            return inValue || inValue2;
        }
    }
}