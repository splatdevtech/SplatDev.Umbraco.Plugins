using FormBuilder.Core.Attributes;
using FormBuilder.Core.Configuration;
using FormBuilder.Core.Models;

namespace FormBuilder.Web.Extensions
{
    /// <summary>
    /// Provides extensions on     /// </summary>
    internal static class ProviderTypeWithSettingsExtensions
    {
        public static void ApplySettings(
          this IProviderTypeWithSettings providerType,
          Dictionary<string, SettingAttribute> settingsFromType,
          Dictionary<string, ProviderSettingsCustomizationDetail> settingsCustomization)
        {
            List<Setting> source = [];
            foreach (KeyValuePair<string, SettingAttribute> keyValuePair in settingsFromType)
            {
                ProviderSettingsCustomizationDetail? customizationDetail = null;
                settingsCustomization?.TryGetValue(keyValuePair.Key, out customizationDetail);
                SettingAttribute settingAttribute = keyValuePair.Value;
                if ((customizationDetail is null || !customizationDetail.IsHidden) && !settingAttribute.IsHidden)
                    source.Add(new Setting()
                    {
                        Alias = keyValuePair.Key,
                        Name = settingAttribute.Name,
                        View = settingAttribute.View,
                        Prevalues = settingAttribute.GetPreValues(),
                        Description = settingAttribute.Description,
                        DisplayOrder = settingAttribute.DisplayOrder,
                        DefaultValue = customizationDetail?.DefaultValue ?? string.Empty,
                        IsReadOnly = customizationDetail is not null && customizationDetail.IsReadOnly,
                        IsMandatory = settingAttribute.IsMandatory
                    });
            }
            providerType.Settings = source.OrderBy(x => x.DisplayOrder);
        }
    }
}