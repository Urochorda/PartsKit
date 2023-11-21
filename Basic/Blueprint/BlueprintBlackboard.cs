using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PartsKit
{
    [Serializable]
    public class BlueprintBlackboard
    {
        [SerializeReference] private List<IBlueprintParameter> parameters = new List<IBlueprintParameter>();

        public IReadOnlyList<IBlueprintParameter> Parameters => parameters;

        public string GetUniqueParameterName(string name)
        {
            string uniqueName = name;
            int i = 0;
            while (Parameters.Any(e => e.ParameterName == name))
            {
                name = uniqueName + i++;
            }

            return name;
        }

        public virtual void AddParameter(IBlueprintParameter parameter)
        {
            if (parameter == null || Parameters.Contains(parameter) ||
                string.IsNullOrWhiteSpace(parameter.ParameterName))
            {
                Debug.LogError("Add Parameter Err");
                return;
            }

            parameter.ParameterName = GetUniqueParameterName(parameter.ParameterName);
            parameters.Add(parameter);
        }

        public virtual void RemoveParameter(IBlueprintParameter parameter)
        {
            parameters.Remove(parameter);
        }
    }
}