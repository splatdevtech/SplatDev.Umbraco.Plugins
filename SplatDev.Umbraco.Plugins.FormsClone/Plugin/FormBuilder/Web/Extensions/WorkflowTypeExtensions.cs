using FormBuilder.Core.Attributes;
using FormBuilder.Core.Configuration;
using FormBuilder.Core.Models;
using FormBuilder.Core.Workflows;

namespace FormBuilder.Web.Extensions
{
    /// <summary>
    /// Provides extensions on     /// </summary>
    internal static class WorkflowTypeExtensions
    {
        private static string GetSettingValue(
          SettingAttribute settingAttribute,
          Workflow? workflow,
          ProviderSettingsCustomizationDetail? settingsCustomizationDetail)
        {
            return workflow != null ? workflow.Settings.SingleOrDefault(x => string.Equals(x.Key, settingAttribute.Alias, StringComparison.OrdinalIgnoreCase)).Value ?? string.Empty : settingsCustomizationDetail?.DefaultValue ?? string.Empty;
        }

        public static List<SettingWithValue> GetSettingsWithValues(
          this WorkflowType workflowType,
          Dictionary<string, ProviderSettingsCustomizationDetail> settingsCustomization,
          Workflow? workflow = null)
        {
            List<SettingWithValue> source = [];
            foreach (KeyValuePair<string, SettingAttribute> setting in workflowType.Settings())
            {
                ProviderSettingsCustomizationDetail? settingsCustomizationDetail = null;
                settingsCustomization?.TryGetValue(setting.Key, out settingsCustomizationDetail);
                SettingAttribute settingAttribute = setting.Value;
                if ((settingsCustomizationDetail is null || !settingsCustomizationDetail.IsHidden) && !settingAttribute.IsHidden)
                {
                    SettingWithValue settingWithValue = new()
                    {
                        Name = settingAttribute.Name,
                        Alias = settingAttribute.Alias,
                        Description = settingAttribute.Description,
                        Prevalues = settingAttribute.GetPreValues(),
                        View = settingAttribute.View,
                        Value = GetSettingValue(settingAttribute, workflow, settingsCustomizationDetail),
                        IsReadOnly = settingsCustomizationDetail is not null && settingsCustomizationDetail.IsReadOnly,
                        DisplayOrder = settingAttribute.DisplayOrder,
                        IsMandatory = settingAttribute.IsMandatory
                    };
                    source.Add(settingWithValue);
                }
            }
            return [.. source.OrderBy(x => x.DisplayOrder)];
        }
    }
}