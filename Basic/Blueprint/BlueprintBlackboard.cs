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

        public Blueprint OwnerBlueprint { get; private set; }

        public void Init(Blueprint blueprintVal)
        {
            OwnerBlueprint = blueprintVal;
        }

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
                CustomLog.LogError("Add Parameter Err");
                return;
            }

            parameter.ParameterName = GetUniqueParameterName(parameter.ParameterName);
            parameters.Add(parameter);
            OwnerBlueprint.SetDirtySelf();
        }

        public virtual void RemoveParameter(IBlueprintParameter parameter)
        {
            parameters.Remove(parameter);
            OwnerBlueprint.SetDirtySelf();
        }

        public IBlueprintParameter GetParameterByGuid(string guid)
        {
            return parameters.Find(item => item.Guid == guid);
        }

        public IBlueprintParameter GetParameterByName(string pName)
        {
            return parameters.Find(item => item.ParameterName == pName);
        }

        public T GetParameterValue<T>(string pName)
        {
            IBlueprintParameter parameter = GetParameterByName(pName);
            if (parameter == null)
            {
                return default;
            }

            return (T)parameter.Value;
        }

        public void SetParameterValue<T>(string pName, T value)
        {
            IBlueprintParameter parameter = GetParameterByName(pName);
            if (parameter == null)
            {
                return;
            }

            parameter.Value = value;
        }

        public void ClearNotValidParameters()
        {
            parameters.RemoveAll(item =>
            {
                if (item == null || string.IsNullOrEmpty(item.Guid))
                {
                    CustomLog.LogError("Parameter NotValid");
                    OwnerBlueprint.SetDirtySelf();
                    return true;
                }

                return false;
            });
        }

        public void OnParameterRename()
        {
            OwnerBlueprint.SetDirtySelf();
        }
    }
}