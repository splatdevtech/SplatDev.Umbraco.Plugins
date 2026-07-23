using System;

namespace SplatDev.Umbraco.Plugins.CodeFirst.Interfaces
{
    public interface IMediaTypeProperty
    {
        string HelpText { get; set; }
        Guid DataTypeId { get; set; }
        bool Mandatory { get; set; }
        string PropertyEditorAlias { get; set; }
        int DataTypeDefinitionId { get; set; }
        string Description { get; set; }
        string Alias { get; set; }
        string ValidationRegExp { get; set; }
        int SortOrder { get; set; }
        string Name { get; set; }
    }
}
