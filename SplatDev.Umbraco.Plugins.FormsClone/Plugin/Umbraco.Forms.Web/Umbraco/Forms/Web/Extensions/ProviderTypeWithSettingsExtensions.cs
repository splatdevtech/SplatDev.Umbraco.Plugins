
// Type: Umbraco.Forms.Web.Extensions.ProviderTypeWithSettingsExtensions
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Web.Models.Backoffice;


#nullable enable
namespace Umbraco.Forms.Web.Extensions
{
  internal static class ProviderTypeWithSettingsExtensions
  {
    public static void ApplySettings(
      this IProviderTypeWithSettings providerType,
      Dictionary<string, SettingAttribute> settingsFromType,
      IHostingEnvironment hostingEnvironment,
      Dictionary<string, ProviderSettingsCustomizationDetail> settingsCustomization)
    {
      List<Setting> source = new List<Setting>();
      foreach (KeyValuePair<string, SettingAttribute> keyValuePair in settingsFromType)
      {
        ProviderSettingsCustomizationDetail customizationDetail = (ProviderSettingsCustomizationDetail) null;
        settingsCustomization?.TryGetValue(keyValuePair.Key, out customizationDetail);
        SettingAttribute settingAttribute = keyValuePair.Value;
        if ((customizationDetail == null || !customizationDetail.IsHidden) && !settingAttribute.IsHidden)
          source.Add(new Setting()
          {
            Alias = keyValuePair.Key,
            Name = settingAttribute.Name,
            View = settingAttribute.View,
            Prevalues = (IEnumerable<string>) settingAttribute.GetPreValues(),
            Description = settingAttribute.Description,
            DisplayOrder = settingAttribute.DisplayOrder,
            DefaultValue = customizationDetail?.DefaultValue ?? string.Empty,
            IsReadOnly = customizationDetail != null && customizationDetail.IsReadOnly,
            IsMandatory = settingAttribute.IsMandatory
          });
      }
      providerType.Settings = (IEnumerable<Setting>) source.OrderBy<Setting, int>((Func<Setting, int>) (x => x.DisplayOrder));
    }
  }
}
