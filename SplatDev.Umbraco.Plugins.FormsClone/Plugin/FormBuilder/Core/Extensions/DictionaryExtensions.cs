using FormBuilder.Core.Attributes;
using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Fields;
using FormBuilder.Core.Services.Interfaces;

using System.Collections;

namespace FormBuilder.Core.Extensions
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
            return settings.ToDictionary(x => x.Key, x => ParseValue(x, placeholderParsingService, typeSettings, record, form, pageElements, additionalData), StringComparer.OrdinalIgnoreCase);
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
            return !typeSettings.TryGetValue(setting.Key, out SettingAttribute? settingAttribute) || !settingAttribute.SupportsPlaceholders ? setting.Value : placeholderParsingService.ParsePlaceHolders(setting.Value, settingAttribute.HtmlEncodeReplacedPlaceholderValues, record, form, pageElements, additionalData);
        }
    }
}