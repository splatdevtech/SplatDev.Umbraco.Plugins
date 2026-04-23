namespace UmbracoCms.CodeFirst.Interfaces
{
    using System.Collections.Generic;
    public interface IDocumentTypeTemplates
    {
        /// <summary>
        /// First in the list is always the Default Template, other are alternatives
        /// </summary>
        IEnumerable<string> TemplatesAliases { get; }
    }
}
