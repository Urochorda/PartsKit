using System;
using UnityEngine;

namespace PartsKit
{
    [Serializable]
    public class BlueprintNodeColorTest : BlueprintNode
    {
        public override Color NodeColor => new Color(192 / 255f, 72 / 255f, 81 / 255f, 0.5f);

        protected override void RegisterPort()
        {
            //Horizontal
            AddPort(BlueprintPortUtility.CreateValuePort<int>("Input", IBlueprintPort.Orientation.Horizontal,
                IBlueprintPort.Direction.Input, null));
            AddPort(BlueprintPortUtility.CreateValuePort<bool>("Output", IBlueprintPort.Orientation.Horizontal,
                IBlueprintPort.Direction.Output, null));

            //Vertical
            AddPort(BlueprintPortUtility.CreateValuePort<float>("InputV", IBlueprintPort.Orientation.Vertical,
                IBlueprintPort.Direction.Input, null));
            AddPort(BlueprintPortUtility.CreateValuePort<float>("OutputV", IBlueprintPort.Orientation.Vertical,
                IBlueprintPort.Direction.Output, null));
        }
    }
}