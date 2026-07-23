namespace SplatDev.Umbraco.Plugins.CodeFirst.Interfaces
{
    using System.Collections.Generic;

    using SplatDev.Umbraco.Plugins.CodeFirst.Models;
    public interface IAssets
    {
        IEnumerable<Asset> Assets { get; }
    }
}
