using System.Collections.Generic;

namespace SplatDev.Umbraco.Plugins.CodeFirst.Interfaces
{
    public interface IDocumentTypeChildren
    {
        /// <summary>
        /// List of Allowed Children for this Document Type
        /// </summary>
        List<IDocumentTypeChildren> Children { get; }
    }
}
