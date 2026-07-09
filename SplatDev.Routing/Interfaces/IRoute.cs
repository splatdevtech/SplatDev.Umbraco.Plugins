namespace SplatDev.Routing.Interfaces
{
    public interface IRoute
    {
        string RouteAlias { get; }

        string Url { get; }

        string Controller { get; }

        string Action { get; }

        object Defaults { get; }
    }
}
