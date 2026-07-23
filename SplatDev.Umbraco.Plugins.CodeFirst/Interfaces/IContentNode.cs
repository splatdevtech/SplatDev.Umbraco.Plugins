namespace SplatDev.Umbraco.Plugins.CodeFirst.Interfaces
{
    public interface IContentNode
    {
        /// <summary>
        /// The Name for this Content Node
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The Document Type Alias (if not using Document Type)
        /// </summary>
        string DocumentTypeAlias { get; }

        /// <summary>
        /// The Document Type class (if not using Document Type Alias)
        /// </summary>
        IDocumentTypeChildren DocumentType { get; }

        /// <summary>
        /// The Parent Id or Container Id (if not using Parent Node Alias)
        /// </summary>
        int? ParentNodeId { get; }

        /// <summary>
        /// The Parent Node Alias or Content Alias (if not using Parent Node)
        /// </summary>
        string ParentNodeAlias { get; }

        /// <summary>
        /// Gets the name of the parent node.
        /// </summary>
        /// <value>
        /// The name of the parent node.
        /// </value>
        string ParentNodeName { get; }

        /// <summary>
        /// The Order in which to create this content (in case there's a preceeding node to be created first or after)
        /// </summary>
        int? CreateOrder { get; }
    }
}
