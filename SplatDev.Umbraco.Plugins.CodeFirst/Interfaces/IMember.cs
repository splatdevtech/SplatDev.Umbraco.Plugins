namespace SplatDev.Umbraco.Plugins.CodeFirst.Interfaces
{
    public interface IMember
    {
        string Email { get; }
        string GroupName { get; }
        string Name { get; }
        string Password { get; }
        string TypeAlias { get; }
        string Username { get; }
        IMemberType Type { get; }
        IMemberGroup Group { get; }
        IMemberProperties Properties { get; }
    }
}
