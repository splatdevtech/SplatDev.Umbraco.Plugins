namespace FormBuilder.Core.Interfaces
{
    public interface ITheme
    {
        string Name { get; }

        IEnumerable<string> Files { get; }
    }
}