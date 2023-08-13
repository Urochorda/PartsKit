namespace PartsKit
{
    public abstract class CommandItem<T, TD> : ICommandItem where T : CommandItem<T, TD>, new()
    {
        public abstract int Id { get; }
        public abstract bool ExecuteByString(string factor);
        public abstract bool ExecuteByData(TD factor);
    }
}
