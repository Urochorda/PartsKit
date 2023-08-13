namespace PartsKit
{
    public interface ICommandItem
    {
        int Id { get; }
        bool ExecuteByString(string factor);
    }
}