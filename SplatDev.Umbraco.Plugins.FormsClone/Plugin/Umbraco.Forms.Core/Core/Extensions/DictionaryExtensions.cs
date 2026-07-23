
// Type: Umbraco.Forms.Core.Extensions.DictionaryExtensions
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Core.Extensions
{
  public static class DictionaryExtensions
  {
    public static IDictionary<string, string> ParseSettingsPlaceholders(
      this IDictionary<string, string> settings,
      IPlaceholderParsingService placeholderParsingService,
      Dictionary<string, SettingAttribute> typeSettings,
      Record? record = null,
      Form? form = null,
      Hashtable? pageElements = null,
      IDictionary<string, string?>? additionalData = null)
    {
      return (IDictionary<string, string>) settings.ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (x => x.Key), (Func<KeyValuePair<string, string>, string>) (x => DictionaryExtensions.ParseValue(x, placeholderParsingService, typeSettings, record, form, pageElements, additionalData)), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    private static string ParseValue(
      KeyValuePair<string, string> setting,
      IPlaceholderParsingService placeholderParsingService,
      Dictionary<string, SettingAttribute> typeSettings,
      Record? record = null,
      Form? form = null,
      Hashtable? pageElements = null,
      IDictionary<string, string?>? additionalData = null)
    {
      SettingAttribute settingAttribute;
      return !typeSettings.TryGetValue(setting.Key, out settingAttribute) || !settingAttribute.SupportsPlaceholders ? setting.Value : placeholderParsingService.ParsePlaceHolders(setting.Value, settingAttribute.HtmlEncodeReplacedPlaceholderValues, record, form, pageElements, additionalData);
    }
  }
}
