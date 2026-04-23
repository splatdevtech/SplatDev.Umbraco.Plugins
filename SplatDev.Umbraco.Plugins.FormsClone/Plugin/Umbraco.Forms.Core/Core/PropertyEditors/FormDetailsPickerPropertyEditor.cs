
// Type: Umbraco.Forms.Core.PropertyEditors.FormDetailsPickerPropertyEditor
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
using Umbraco.Forms.Core.PropertyEditors.Models;


#nullable enable
namespace Umbraco.Forms.Core.PropertyEditors
{
  [DataEditor("UmbracoForms.FormDetailsPicker", ValueEditorIsReusable = true, ValueType = "STRING")]
  public class FormDetailsPickerPropertyEditor : DataEditor
  {
    private readonly IIOHelper _ioHelper;

    public FormDetailsPickerPropertyEditor(
      IDataValueEditorFactory dataValueEditorFactory,
      IIOHelper ioHelper)
      : base(dataValueEditorFactory)
    {
      this.SupportsReadOnly = true;
      this._ioHelper = ioHelper;
    }

    protected override IConfigurationEditor CreateConfigurationEditor() => (IConfigurationEditor) new FormDetailsPickerConfigurationEditor(this._ioHelper);

    protected override IDataValueEditor CreateValueEditor() => (IDataValueEditor) this.DataValueEditorFactory.Create<FormDetailsPickerPropertyEditor.FormDetailsPickerPropertyValueEditor>((object) this.Attribute);

    internal class FormDetailsPickerPropertyValueEditor : FormPickerPropertyValueEditorBase
    {
      private readonly IJsonSerializer _jsonSerializer;

      public FormDetailsPickerPropertyValueEditor(
        IShortStringHelper shortStringHelper,
        IJsonSerializer jsonSerializer,
        IIOHelper ioHelper,
        DataEditorAttribute attribute,
        IOptions<PackageOptionSettings> packageOptionSettings)
        : base(shortStringHelper, jsonSerializer, ioHelper, attribute, packageOptionSettings)
      {
        this._jsonSerializer = jsonSerializer;
      }

      protected override bool TryGetFormId(string value, out Guid formId)
      {
        FormDetails formDetails;
        if (!this._jsonSerializer.TryDeserialize<FormDetails>((object) value, out formDetails) || !formDetails.FormId.HasValue)
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
