using FormBuilder.Core.Configuration;
using FormBuilder.Core.PropertyEditors.Models;

using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;

using Umbraco.Cms.Core.PropertyEditors;

using Umbraco.Cms.Core.Serialization;

using Umbraco.Cms.Core.Strings;

namespace FormBuilder.Core.PropertyEditors
{
    [DataEditor("FormBuilder.FormDetailsPicker", ValueEditorIsReusable = true, ValueType = "STRING")]
    public class FormDetailsPickerPropertyEditor : DataEditor
    {
        private readonly IIOHelper _ioHelper;

        public FormDetailsPickerPropertyEditor(
          IDataValueEditorFactory dataValueEditorFactory,
          IIOHelper ioHelper)
          : base(dataValueEditorFactory)
        {
            SupportsReadOnly = true;
            _ioHelper = ioHelper;
        }

        protected override IConfigurationEditor CreateConfigurationEditor() => new FormDetailsPickerConfigurationEditor(_ioHelper);

        protected override IDataValueEditor CreateValueEditor() => DataValueEditorFactory.Create<FormDetailsPickerPropertyValueEditor>(Attribute!);

        internal class FormDetailsPickerPropertyValueEditor(
          IShortStringHelper shortStringHelper,
          IJsonSerializer jsonSerializer,
          IIOHelper ioHelper,
          DataEditorAttribute attribute,
          IOptions<PackageOptionSettings> packageOptionSettings) : FormPickerPropertyValueEditorBase(shortStringHelper, jsonSerializer, ioHelper, attribute, packageOptionSettings)
        {
            private readonly IJsonSerializer _jsonSerializer = jsonSerializer;

            protected override bool TryGetFormId(string value, out Guid formId)
            {
                if (!_jsonSerializer.TryDeserialize(value, out FormDetails? formDetails) || !formDetails.FormId.HasValue)
                {
                    formId = Guid.Empty;
                    return false;
                }
                formId = formDetails.FormId.Value;
                return true;
            }
        }
    }
}