
// Type: Umbraco.Forms.Core.Interfaces.ITheme
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Collections.Generic;


#nullable enable
namespace Umbraco.Forms.Core.Interfaces
{
  public interface ITheme
  {
    string Name { get; }

    IEnumerable<string> Files { get; }
  }
}
