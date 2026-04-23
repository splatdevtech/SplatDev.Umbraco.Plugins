using SplatDev.Umbraco.Plugins.MemberGroups.Enums;

namespace SplatDev.Umbraco.Plugins.MemberGroups.Interfaces
{
    public interface IMemberUser
    {
        string Email { get; }
        UserTypes Group { get; }
        string Name { get; }
        string Password { get; }
        string StartContentNodeAlias { get; }
        string StartContentNodeName { get; }
        string StartMediaNodeAlias { get; }
        string StartMediaNodeName { get; }
        string Username { get; }
    }
}
