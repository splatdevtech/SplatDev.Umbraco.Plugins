#pragma warning disable CS0618
namespace SplatDev.Database.Helpers
{
    [System.Obsolete("Stub for backwards compatibility. Use Yaml2Schema.")]
    public static class DatabaseHelper
    {
        public static void InsertData<T>(NPoco.Database database, System.Collections.Generic.List<T> data, bool skipIfHasData, string table) where T : class
        {
            throw new System.NotSupportedException("CodeFirst is deprecated. Use SplatDev.Umbraco.Plugins.Yaml2Schema instead.");
        }
    }
}
#pragma warning restore CS0618
