namespace SplatDev.Routing.Interfaces
{
    public interface IUmbracoPluginRoute : IRoute
    {
        int? RootId { get; }

        string RootAlias { get; }

        bool IsPluginController { get; }
    }
}
