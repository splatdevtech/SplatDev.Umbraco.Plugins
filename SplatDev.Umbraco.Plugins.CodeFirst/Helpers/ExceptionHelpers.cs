namespace SplatDev.Umbraco.Plugins.CodeFirst.Helpers
{
    using System.Runtime.CompilerServices;

    public static class ExceptionHelpers
    {
        public static string GenerateCustomStackTrace([CallerLineNumber] int lineNumber = 0, [CallerMemberName] string caller = null, [CallerFilePath] string path = null)
        {
            return $"line {lineNumber} ({caller}) -> {path}";
        }
    }
}
