namespace PartsKit
{
    public interface IReadOnlyRefBool
    {
        public bool Value { get; }
    }

    public class RefBool : IReadOnlyRefBool
    {
        public bool Value { get; set; }

        public RefBool(bool value)
        {
            Value = value;
        }
    }
}