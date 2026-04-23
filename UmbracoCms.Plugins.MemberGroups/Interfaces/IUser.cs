using UmbracoCms.Plugins.MemberGroups.Enums;

namespace UmbracoCms.Plugins.MemberGroups.Interfaces
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
