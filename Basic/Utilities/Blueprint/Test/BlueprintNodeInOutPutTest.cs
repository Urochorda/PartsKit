using System;
using UnityEngine;

namespace PartsKit
{
    [Serializable]
    public class BlueprintNodeInOutPutTest : BlueprintNode
    {
        [field: SerializeField] public int Input { get; set; }
        [field: SerializeField] public int Output { get; set; }

        [field: SerializeField] public float InputV { get; set; }
        [field: SerializeField] public float OutputV { get; set; }

        protected override void RegisterPort()
        {
            //Horizontal
            AddPort(BlueprintPortUtility.CreateInOutputPort<GameObject>("Input", IBlueprintPort.Orientation.Horizontal,
                IBlueprintPort.Direction.Input));
            AddPort(BlueprintPortUtility.CreateInOutputPort<GameObject>("Output", IBlueprintPort.Orientation.Horizontal,
                IBlueprintPort.Direction.Output));

            //Vertical
            AddPort(BlueprintPortUtility.CreateInOutputPort<string>("InputV", IBlueprintPort.Orientation.Vertical,
                IBlueprintPort.Direction.Input));
            AddPort(BlueprintPortUtility.CreateInOutputPort<string>("OutputV", IBlueprintPort.Orientation.Vertical,
                IBlueprintPort.Direction.Output));
        }
    }
}