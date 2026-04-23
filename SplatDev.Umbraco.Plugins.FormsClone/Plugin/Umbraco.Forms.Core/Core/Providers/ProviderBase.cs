
// Type: Umbraco.Forms.Core.Providers.ProviderBase
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Runtime.Serialization;


#nullable enable
namespace Umbraco.Forms.Core.Providers
{
  [DataContract(Name = "providerBase")]
  [Serializable]
  public abstract class ProviderBase
  {
    [DataMember(Name = "id")]
    public Guid Id { get; set; }

    [DataMember(Name = "name")]
    public string Name { get; set; } = string.Empty;

    [DataMember(Name = "alias")]
    public string Alias { get; set; } = string.Empty;

    [DataMember(Name = "description")]
    public string Description { get; set; } = string.Empty;

    [DataMember(Name = "icon")]
    public string Icon { get; set; } = string.Empty;

    [DataMember(Name = "group")]
    public string Group { get; set; } = string.Empty;
  }
}
