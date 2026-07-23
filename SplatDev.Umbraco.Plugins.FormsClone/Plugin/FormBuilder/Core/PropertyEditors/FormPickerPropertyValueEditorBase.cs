using FormBuilder.Core.Configuration;

using Microsoft.Extensions.Options;

using Umbraco.Cms.Core;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models.Editors;
using Umbraco.Cms.Core.PropertyEditors;

using Umbraco.Cms.Core.Serialization;

using Umbraco.Cms.Core.Strings;

using Umbraco.Extensions;

namespace FormBuilder.Core.PropertyEditors
{
    internal abstract class FormPickerPropertyValueEditorBase(
      IShortStringHelper shortStringHelper,
      IJsonSerializer jsonSerializer,
      IIOHelper ioHelper,
      DataEditorAttribute attribute,
      IOptions<PackageOptionSettings> packageOptionSettings) : DataValueEditor(shortStringHelper, jsonSerializer, ioHelper, attribute), IDataValueReference
    {
        private readonly PackageOptionSettings _packageOptionSettings = packageOptionSettings.Value;

        public IEnumerable<UmbracoEntityReference> GetReferences(
          object? value)
        {
            string? str = value?.ToString();
            if (!_packageOptionSettings.DisableRelationTracking && !string.IsNullOrWhiteSpace(str) && TryGetFormId(str, out Guid formId))
                yield return new UmbracoEntityReference(Udi.Create("forms-form", formId), "umbForm");
        }

        public IEnumerable<string> GetAutomaticRelationTypesAliases() => "umbForm".Yield();

        protected abstract bool TryGetFormId(string value, out Guid formId);
    }
}