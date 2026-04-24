using SplatDev.Umbraco.Plugins.CodeFirst.Acl;

namespace SplatDev.Umbraco.Plugins.CodeFirst.Interfaces
{
    public interface IDashboard
    {
        string Alias { get; }

        string Caption { get; }

        string[] Sections { get; }

        string View { get; }

        IAccessRule[] AccessRules { get; }
    }
}
