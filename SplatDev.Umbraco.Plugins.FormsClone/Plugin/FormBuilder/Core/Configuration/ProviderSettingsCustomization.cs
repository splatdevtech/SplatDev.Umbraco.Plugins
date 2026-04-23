using FormBuilder.Core.Providers;

using System.Diagnostics.CodeAnalysis;

namespace FormBuilder.Core.Configuration
{
    public class ProviderSettingsCustomization :
        Dictionary<string, Dictionary<string, ProviderSettingsCustomizationDetail>>
    {
        public Dictionary<string, ProviderSettingsCustomizationDetail> GetValueForProviderType(
          ProviderBase providerType)
        {
            return TryGetValue(providerType.Id.ToString(), StringComparison.OrdinalIgnoreCase, out Dictionary<string, ProviderSettingsCustomizationDetail>? dictionary) || TryGetValue(providerType.Alias, StringComparison.OrdinalIgnoreCase, out dictionary) ? dictionary : [];
        }

        private bool TryGetValue(
          string key,
          StringComparison comparisonType,
          [NotNullWhen(true)] out Dictionary<string, ProviderSettingsCustomizationDetail>? value)
        {
            foreach (KeyValuePair<string, Dictionary<string, ProviderSettingsCustomizationDetail>> keyValuePair in (Dictionary<string, Dictionary<string, ProviderSettingsCustomizationDetail>>)this)
            {
                if (string.Equals(keyValuePair.Key, key, comparisonType))
                {
                    value = keyValuePair.Value;
                    return true;
                }
            }
            value = null;
            return false;
        }
    }
}