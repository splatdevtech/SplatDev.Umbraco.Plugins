using FormBuilder.Core.Configuration;

using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;

using Umbraco.Cms.Core.PropertyEditors;

using Umbraco.Cms.Core.Serialization;

using Umbraco.Cms.Core.Strings;

namespace FormBuilder.Core.PropertyEditors
{
    [DataEditor("FormBuilder.FormPicker", ValueEditorIsReusable = true, ValueType = "STRING")]
    public class FormPickerPropertyEditor : DataEditor
    {
        private readonly IIOHelper _ioHelper;

        public FormPickerPropertyEditor(
          IDataValueEditorFactory dataValueEditorFactory,
          IIOHelper ioHelper)
          : base(dataValueEditorFactory)
        {
            SupportsReadOnly = true;
            _ioHelper = ioHelper;
        }

        protected override IConfigurationEditor CreateConfigurationEditor() => new FormPickerConfigurationEditor(_ioHelper);

        protected override IDataValueEditor CreateValueEditor() => DataValueEditorFactory.Create<FormPickerPropertyValueEditor>(Attribute!);

        internal class FormPickerPropertyValueEditor(
          IShortStringHelper shortStringHelper,
          IJsonSerializer jsonSerializer,
          IIOHelper ioHelper,
          DataEditorAttribute attribute,
          IOptions<PackageOptionSettings> packageOptionSettings) : FormPickerPropertyValueEditorBase(shortStringHelper, jsonSerializer, ioHelper, attribute, packageOptionSettings)
        {
            protected override bool TryGetFormId(string value, out Guid formId) => Guid.TryParse(value?.ToString(), out formId);
        }
    }
}