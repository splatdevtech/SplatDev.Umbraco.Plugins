namespace UmbracoCms.CodeFirst.Interfaces
{
    public interface IMemberType
    {
        string Description { get; }
        string Icon { get; }
        string MemberTypeAlias { get; }
        string MemberTypeName { get; }
        string ParentGroupAlias { get; }
        IMemberProperties Properties { get; }
    }
}
