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
            AddPort(BlueprintPortUtility.CreateInOutputPort<int>("Input", IBlueprintPort.Orientation.Horizontal,
                IBlueprintPort.Direction.Input));
            AddPort(BlueprintPortUtility.CreateInOutputPort<bool>("Output", IBlueprintPort.Orientation.Horizontal,
                IBlueprintPort.Direction.Output));

            //Vertical
            AddPort(BlueprintPortUtility.CreateInOutputPort<float>("InputV", IBlueprintPort.Orientation.Vertical,
                IBlueprintPort.Direction.Input));
            AddPort(BlueprintPortUtility.CreateInOutputPort<float>("OutputV", IBlueprintPort.Orientation.Vertical,
                IBlueprintPort.Direction.Output));
        }
    }
}