using System;
using UnityEngine;

namespace PartsKit
{
    [Serializable]
    public class BlueprintNodeColorTest: BlueprintNode
    {
        public override Color NodeColor => Color.yellow;
    }
}