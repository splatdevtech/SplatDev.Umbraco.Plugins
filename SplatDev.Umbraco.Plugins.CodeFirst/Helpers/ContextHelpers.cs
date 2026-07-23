namespace SplatDev.Umbraco.Plugins.CodeFirst.Helpers
{
    public static class ContextHelpers
    {
        public static string Local(string url)
        {
            return !url.Contains("http") ? " (local)" : string.Empty;
        }
    }
}
