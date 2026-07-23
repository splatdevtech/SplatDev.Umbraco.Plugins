
// Type: Umbraco.Forms.Web.Extensions.WorkflowTypeExtensions
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Web.Models.Backoffice;


#nullable enable
namespace Umbraco.Forms.Web.Extensions
{
  internal static class WorkflowTypeExtensions
  {
    public static List<SettingWithValue> GetSettingsWithValues(
      this WorkflowType workflowType,
      IHostingEnvironment hostingEnvironment,
      Dictionary<string, ProviderSettingsCustomizationDetail> settingsCustomization,
      Workflow? workflow = null)
    {
      List<SettingWithValue> source = new List<SettingWithValue>();
      foreach (KeyValuePair<string, SettingAttribute> setting in workflowType.Settings())
      {
        ProviderSettingsCustomizationDetail settingsCustomizationDetail = (ProviderSettingsCustomizationDetail) null;
        settingsCustomization?.TryGetValue(setting.Key, out settingsCustomizationDetail);
        SettingAttribute settingAttribute = setting.Value;
        if ((settingsCustomizationDetail == null || !settingsCustomizationDetail.IsHidden) && !settingAttribute.IsHidden)
        {
          SettingWithValue settingWithValue = new SettingWithValue()
          {
            Name = settingAttribute.Name,
            Alias = settingAttribute.Alias,
            Description = settingAttribute.Description,
            Prevalues = (IEnumerable<string>) settingAttribute.GetPreValues(),
            View = settingAttribute.View,
            Value = WorkflowTypeExtensions.GetSettingValue(settingAttribute, workflow, settingsCustomizationDetail),
            IsReadOnly = settingsCustomizationDetail != null && settingsCustomizationDetail.IsReadOnly,
            DisplayOrder = settingAttribute.DisplayOrder,
            IsMandatory = settingAttribute.IsMandatory
          };
          source.Add(settingWithValue);
        }
      }
      return source.OrderBy<SettingWithValue, int>((Func<SettingWithValue, int>) (x => x.DisplayOrder)).ToList<SettingWithValue>();
    }

    private static string GetSettingValue(
      SettingAttribute settingAttribute,
      Workflow? workflow,
      ProviderSettingsCustomizationDetail? settingsCustomizationDetail)
    {
      return workflow != null ? workflow.Settings.SingleOrDefault<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (x => string.Equals(x.Key, settingAttribute.Alias, StringComparison.OrdinalIgnoreCase))).Value ?? string.Empty : settingsCustomizationDetail?.DefaultValue ?? string.Empty;
    }
  }
}
