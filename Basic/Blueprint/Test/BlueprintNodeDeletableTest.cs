using System;

namespace PartsKit
{
    [Serializable]
    public class BlueprintNodeDeletableTest : BlueprintNode
    {
        public override bool Deletable => false;
    }
}