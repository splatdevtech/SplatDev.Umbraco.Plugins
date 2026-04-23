
// Type: Umbraco.Forms.Core.PropertyEditors.FormPickerPropertyValueEditorBase
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models.Editors;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Strings;
using Umbraco.Extensions;
using Umbraco.Forms.Core.Configuration;


#nullable enable
namespace Umbraco.Forms.Core.PropertyEditors
{
  internal abstract class FormPickerPropertyValueEditorBase : DataValueEditor, IDataValueReference
  {
    private readonly PackageOptionSettings _packageOptionSettings;

    public FormPickerPropertyValueEditorBase(
      IShortStringHelper shortStringHelper,
      IJsonSerializer jsonSerializer,
      IIOHelper ioHelper,
      DataEditorAttribute attribute,
      IOptions<PackageOptionSettings> packageOptionSettings)
      : base(shortStringHelper, jsonSerializer, ioHelper, attribute)
    {
      this._packageOptionSettings = packageOptionSettings.Value;
    }

    public IEnumerable<UmbracoEntityReference> GetReferences(
      object? value)
    {
      string str = value?.ToString();
      Guid formId;
      if (!this._packageOptionSettings.DisableRelationTracking && !string.IsNullOrWhiteSpace(str) && this.TryGetFormId(str, out formId))
        yield return new UmbracoEntityReference(Udi.Create("forms-form", formId), "umbForm");
    }

    public IEnumerable<string> GetAutomaticRelationTypesAliases() => "umbForm".Yield<string>();

    protected abstract bool TryGetFormId(string value, out Guid formId);
  }
}
