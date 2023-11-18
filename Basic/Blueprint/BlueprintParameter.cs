namespace PartsKit
{
    public class BlueprintParameter<T>
    {
        private T value;
        public string NameKey { get; }

        public BlueprintParameter(string nameKey)
        {
            NameKey = nameKey;
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