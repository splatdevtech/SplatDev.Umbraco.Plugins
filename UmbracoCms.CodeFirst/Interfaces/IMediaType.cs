namespace UmbracoCms.CodeFirst.Interfaces
{
    using System.Collections.Generic;

    using Umbraco.Cms.Core.Models;

    public interface IMediaType
    {
        string Alias { get; }
        string Container { get; }
        string Inherits { get; }
        string InheritsCustomDataType { get; }
        string Name { get; }
        List<ContentTypeSort> AllowedContentTypes { get; set; }
        string Description { get; set; }
        string Icon { get; set; }
        List<IMediaTypeProperty> Properties { get; set; }
        string PropertyGroupName { get; set; }
    }
}
