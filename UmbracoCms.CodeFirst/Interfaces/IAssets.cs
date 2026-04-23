namespace UmbracoCms.CodeFirst.Interfaces
{
    using System.Collections.Generic;

    using UmbracoCms.CodeFirst.Models;
    public interface IAssets
    {
        IEnumerable<Asset> Assets { get; }
    }
}
