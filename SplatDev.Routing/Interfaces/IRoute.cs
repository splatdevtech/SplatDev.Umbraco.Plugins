namespace SplatDev.Routing.Interfaces
{
    public interface IRoute
    {
        string RouteAlias { get; }

        /// <summary>
        /// Cannot start with /
        /// </summary>
        string Url { get; }
        string Controller { get; }
        string Action { get; }
        object Defaults { get; }
        int? RootId { get; }
        string RootAlias { get; }

        bool IsPluginController { get; }
    }
}
