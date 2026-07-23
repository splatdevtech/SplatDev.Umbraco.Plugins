namespace SplatDev.Umbraco.Plugins.CodeFirst.Interfaces
{
    using System.IO;
    public interface IMediaNode
    {
        string CustomMediaTypeAlias { get; }
        int? Level { get; }
        string MediaType { get; }
        string Name { get; }
        int? ParentNode { get; }
        string ParentNodeAlias { get; }
        string ParentNodeName { get; }
        Stream Stream { get; set; }
    }
}
