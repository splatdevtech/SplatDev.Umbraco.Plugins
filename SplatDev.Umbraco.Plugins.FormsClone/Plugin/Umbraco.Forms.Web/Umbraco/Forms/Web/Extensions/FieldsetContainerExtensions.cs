
// Type: Umbraco.Forms.Web.Extensions.FieldsetContainerExtensions
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using System;
using System.Collections.Generic;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Providers;


#nullable enable
namespace Umbraco.Forms.Web.Extensions
{
  public static class FieldsetContainerExtensions
  {
    public static void AddDataConsentField(
      this FieldsetContainer container,
      FormDesignSettings formDesignSettings,
      FieldCollection fieldCollection)
    {
      Guid guid = Guid.Parse("A72C9DF9-3847-47CF-AFB8-B86773FD12CD");
      FieldType field = fieldCollection[guid];
      if (field == null)
        return;
      ProviderSettingsCustomizationDetail customizationDetail;
      formDesignSettings.SettingsCustomization.FieldTypes.GetValueForProviderType((ProviderBase) field).TryGetValue("AcceptCopy", out customizationDetail);
      string acceptCopy = customizationDetail?.DefaultValue ?? "Yes, I give permission to store and process my data";
      FieldsetContainerExtensions.AddDataConsentField(container, guid, acceptCopy);
    }

    private static void AddDataConsentField(
      FieldsetContainer container,
      Guid fieldId,
      string acceptCopy)
    {
      Field field = new Field()
      {
        Id = Guid.NewGuid(),
        Caption = "Consent for storing submitted data",
        Alias = "dataConsent",
        FieldTypeId = fieldId,
        Mandatory = true,
        RequiredErrorMessage = "Consent is required to store and process the data in this form.",
        Settings = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
        {
          {
            "AcceptCopy",
            acceptCopy
          }
        }
      };
      container.Fields.Add(field);
    }
  }
}
