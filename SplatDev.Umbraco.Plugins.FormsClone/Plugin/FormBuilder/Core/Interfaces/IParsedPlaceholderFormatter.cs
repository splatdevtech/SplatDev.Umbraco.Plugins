namespace FormBuilder.Core.Interfaces
{
    public interface IParsedPlaceholderFormatter
    {
        string FunctionName { get; }

        string FormatValue(string value, string[] args);
    }
}