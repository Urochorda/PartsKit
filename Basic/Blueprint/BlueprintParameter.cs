namespace PartsKit
{
    public interface IBlueprintParameter
    {
        public string NameKey { get; }
    }

    public class BlueprintParameter<T> : IBlueprintParameter
    {
        public string NameKey { get; }
        private T value;

        public BlueprintParameter(string nameKeyVal)
        {
            NameKey = nameKeyVal;
        }

        public T GetValue()
        {
            return value;
        }

        public void SetValue(T newValue)
        {
            value = newValue;
        }
    }
}