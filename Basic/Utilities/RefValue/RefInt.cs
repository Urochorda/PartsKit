namespace PartsKit
{
    public interface IReadOnlyRefInt
    {
        public int Value { get; }
    }

    public class RefInt : IReadOnlyRefInt
    {
        public int Value { get; set; }

        public RefInt(int value)
        {
            Value = value;
        }
    }
}