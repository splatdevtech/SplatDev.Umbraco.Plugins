using UmbracoCms.CodeFirst.Acl;

namespace UmbracoCms.CodeFirst.Interfaces
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
