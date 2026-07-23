using FormBuilder.Core.Configuration;
using FormBuilder.Core.FieldTypes;
using FormBuilder.Core.Models;
using FormBuilder.Core.Providers.Collections;

namespace FormBuilder.Web.Extensions
{
    /// <summary>
    /// Provides extension methods on     /// </summary>
    public static class FieldsetContainerExtensions
    {
        /// <summary>Adds a data consent field to the provided container.</summary>
        public static void AddDataConsentField(
          this FieldsetContainer container,
          FormDesignSettings formDesignSettings,
          FieldCollection fieldCollection)
        {
            Guid guid = Guid.Parse("A72C9DF9-3847-47CF-AFB8-B86773FD12CD");
            FieldType? field = fieldCollection[guid];
            if (field is null)
                return;
            formDesignSettings.SettingsCustomization.FieldTypes.GetValueForProviderType(field).TryGetValue("AcceptCopy", out ProviderSettingsCustomizationDetail? customizationDetail);
            string acceptCopy = customizationDetail?.DefaultValue ?? "Yes, I give permission to store and process my data";
            AddDataConsentField(container, guid, acceptCopy);
        }

        private static void AddDataConsentField(
          FieldsetContainer container,
          Guid fieldId,
          string acceptCopy)
        {
            Field field = new()
            {
                Id = Guid.NewGuid(),
                Caption = "Consent for storing submitted data",
                Alias = "dataConsent",
                FieldTypeId = fieldId,
                Mandatory = true,
                RequiredErrorMessage = "Consent is required to store and process the data in this form.",
                Settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
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