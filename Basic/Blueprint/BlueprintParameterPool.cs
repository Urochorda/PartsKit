using System.Collections.Generic;

namespace PartsKit
{
    public class BlueprintParameterPool
    {
        private readonly List<IBlueprintParameter> parameters = new List<IBlueprintParameter>();

        protected virtual void RegisterParameter()
        {
        }

        public void AddParameter(IBlueprintParameter parameter)
        {
            parameters.Add(parameter);
        }

        public void RemoveParameter(IBlueprintParameter parameter)
        {
            parameters.Remove(parameter);
        }
    }
}