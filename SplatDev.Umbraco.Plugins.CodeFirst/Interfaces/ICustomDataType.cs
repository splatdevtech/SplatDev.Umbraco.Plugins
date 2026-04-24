using System.Collections.Generic;

namespace SplatDev.Umbraco.Plugins.CodeFirst.Interfaces
{
    public interface ICustomDataType
    {
        string Alias { get; }
        string Container { get; }
        string Name { get; }
        Dictionary<string, object> PreValues { get; }
        string PropertyEditorAlias { get; }
        IAssets AssetList { get; }
    }
}
