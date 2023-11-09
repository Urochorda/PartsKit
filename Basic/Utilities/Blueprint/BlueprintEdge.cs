using System;
using UnityEngine;

namespace PartsKit
{
    [Serializable]
    public class BlueprintEdge
    {
        public static BlueprintEdge CreateBlueprintEdge(IBlueprintPort inputPort, IBlueprintPort outputPort)
        {
            BlueprintEdge edge = new BlueprintEdge()
            {
                Guid = System.Guid.NewGuid().ToString(),
                InputNodeGuid = inputPort.OwnerNode.Guid,
                InputPortName = inputPort.PortName,
                OutputNodeGuid = outputPort.OwnerNode.Guid,
                OutputPortName = outputPort.PortName
            };
            return edge;
        }

        #region 可序列化的字段

        [field: SerializeField] public string Guid { get; private set; }
        [field: SerializeField] public string InputNodeGuid { get; set; }
        [field: SerializeField] public string InputPortName { get; set; }
        [field: SerializeField] public string OutputNodeGuid { get; set; }
        [field: SerializeField] public string OutputPortName { get; set; }

        #endregion

        private BlueprintEdge()
        {
        }
    }
}