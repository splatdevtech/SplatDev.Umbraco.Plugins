
// Type: Umbraco.Forms.Core.PropertyEditors.FormPickerPropertyEditor
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Microsoft.Extensions.Options;
using System;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Strings;
using Umbraco.Forms.Core.Configuration;


#nullable enable
namespace Umbraco.Forms.Core.PropertyEditors
{
  [DataEditor("UmbracoForms.FormPicker", ValueEditorIsReusable = true, ValueType = "STRING")]
  public class FormPickerPropertyEditor : DataEditor
  {
    private readonly IIOHelper _ioHelper;

    public FormPickerPropertyEditor(
      IDataValueEditorFactory dataValueEditorFactory,
      IIOHelper ioHelper)
      : base(dataValueEditorFactory)
    {
      this.SupportsReadOnly = true;
      this._ioHelper = ioHelper;
    }

    protected override IConfigurationEditor CreateConfigurationEditor() => (IConfigurationEditor) new FormPickerConfigurationEditor(this._ioHelper);

    protected override IDataValueEditor CreateValueEditor() => (IDataValueEditor) this.DataValueEditorFactory.Create<FormPickerPropertyEditor.FormPickerPropertyValueEditor>((object) this.Attribute);

    internal class FormPickerPropertyValueEditor : FormPickerPropertyValueEditorBase
    {
      public FormPickerPropertyValueEditor(
        IShortStringHelper shortStringHelper,
        IJsonSerializer jsonSerializer,
        IIOHelper ioHelper,
        DataEditorAttribute attribute,
        IOptions<PackageOptionSettings> packageOptionSettings)
        : base(shortStringHelper, jsonSerializer, ioHelper, attribute, packageOptionSettings)
      {
      }

      protected override bool TryGetFormId(string value, out Guid formId) => Guid.TryParse(value?.ToString(), out formId);
    }
  }
}
