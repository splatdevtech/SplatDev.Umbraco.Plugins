
// Type: Umbraco.Forms.Core.Configuration.ProviderSettingsCustomization
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Umbraco.Forms.Core.Providers;


#nullable enable
namespace Umbraco.Forms.Core.Configuration
{
  public class ProviderSettingsCustomization : 
    Dictionary<string, Dictionary<string, ProviderSettingsCustomizationDetail>>
  {
    public Dictionary<string, ProviderSettingsCustomizationDetail> GetValueForProviderType(
      ProviderBase providerType)
    {
      Dictionary<string, ProviderSettingsCustomizationDetail> dictionary;
      return this.TryGetValue(providerType.Id.ToString(), StringComparison.OrdinalIgnoreCase, out dictionary) || this.TryGetValue(providerType.Alias, StringComparison.OrdinalIgnoreCase, out dictionary) ? dictionary : new Dictionary<string, ProviderSettingsCustomizationDetail>();
    }

    private bool TryGetValue(
      string key,
      StringComparison comparisonType,
      [NotNullWhen(true)] out Dictionary<string, ProviderSettingsCustomizationDetail>? value)
    {
      foreach (KeyValuePair<string, Dictionary<string, ProviderSettingsCustomizationDetail>> keyValuePair in (Dictionary<string, Dictionary<string, ProviderSettingsCustomizationDetail>>) this)
      {
        if (string.Equals(keyValuePair.Key, key, comparisonType))
        {
          value = keyValuePair.Value;
          return true;
        }
      }
      value = (Dictionary<string, ProviderSettingsCustomizationDetail>) null;
      return false;
    }
  }
}
