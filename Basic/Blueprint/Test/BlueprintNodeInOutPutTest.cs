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
            AddPort(BlueprintPortUtility.CreateValuePort<GameObject>("Input", IBlueprintPort.Orientation.Horizontal,
                IBlueprintPort.Direction.Input, string.Empty,null));
            AddPort(BlueprintPortUtility.CreateValuePort<GameObject>("Output", IBlueprintPort.Orientation.Horizontal,
                IBlueprintPort.Direction.Output, string.Empty,null));

            //Vertical
            AddPort(BlueprintPortUtility.CreateValuePort<string>("InputV", IBlueprintPort.Orientation.Vertical,
                IBlueprintPort.Direction.Input, string.Empty,null));
            AddPort(BlueprintPortUtility.CreateValuePort<string>("OutputV", IBlueprintPort.Orientation.Vertical,
                IBlueprintPort.Direction.Output, string.Empty,null));
        }
    }
}