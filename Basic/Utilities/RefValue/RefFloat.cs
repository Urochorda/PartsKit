namespace PartsKit
{
    public interface IReadOnlyRefFloat
    {
        public float Value { get; }
    }

    public class RefFloat : IReadOnlyRefFloat
    {
        public float Value { get; set; }

        public RefFloat(float value)
        {
            Value = value;
        }
    }
}