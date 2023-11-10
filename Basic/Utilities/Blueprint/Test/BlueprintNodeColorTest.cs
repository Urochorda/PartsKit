using System;
using UnityEngine;

namespace PartsKit
{
    [Serializable]
    public class BlueprintNodeColorTest : BlueprintNode
    {
        public override Color NodeColor => new Color(192 / 255f, 72 / 255f, 81 / 255f, 0.5f);

        [field: SerializeField] public int Input { get; set; }
        [field: SerializeField] public int Output { get; set; }

        [field: SerializeField] public int InputV { get; set; }
        [field: SerializeField] public int OutputV { get; set; }

        protected override void RegisterPort()
        {
            //Horizontal
            AddPort(new BlueprintPort<int>("Input", IBlueprintPort.Orientation.Horizontal,
                IBlueprintPort.Direction.Input, IBlueprintPort.Capacity.Single,
                (value) => Input = value,
                () => Input, () => Input = 0));
            AddPort(new BlueprintPort<int>("Output", IBlueprintPort.Orientation.Horizontal,
                IBlueprintPort.Direction.Output, IBlueprintPort.Capacity.Single,
                (value) => Output = value,
                () => Output, () => Output = 0));

            //Vertical
            AddPort(new BlueprintPort<int>("InputV", IBlueprintPort.Orientation.Vertical,
                IBlueprintPort.Direction.Input, IBlueprintPort.Capacity.Single,
                (value) => InputV = value,
                () => InputV, () => Input = 0));
            AddPort(new BlueprintPort<int>("OutputV", IBlueprintPort.Orientation.Vertical,
                IBlueprintPort.Direction.Output, IBlueprintPort.Capacity.Single,
                (value) => OutputV = value,
                () => OutputV, () => OutputV = 0));
        }
    }
}