
// Type: Umbraco.Forms.Core.Services.FieldTypeStorage
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using System.Reflection;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Providers;


#nullable enable
namespace Umbraco.Forms.Core.Services
{
  internal sealed class FieldTypeStorage : IFieldTypeStorage
  {
    private readonly FieldCollection _fieldCollection;

    public FieldTypeStorage(FieldCollection fieldCollection) => this._fieldCollection = fieldCollection;

    public FieldType? GetFieldTypeByField(Field field) => this.GetFieldTypeByField(field.FieldTypeId, field.Settings);

    public FieldType? GetFieldTypeByField(
      Guid fieldTypeId,
      IDictionary<string, string> settings)
    {
      FieldType field = this._fieldCollection[fieldTypeId];
      if (field == null)
        return (FieldType) null;
      foreach (KeyValuePair<string, string> setting in (IEnumerable<KeyValuePair<string, string>>) settings)
      {
        PropertyInfo property = field.GetType().GetProperty(setting.Key, BindingFlags.Instance | BindingFlags.Public);
        if (property != (PropertyInfo) null && property.CanWrite)
          property.SetValue((object) field, (object) setting.Value, (object[]) null);
      }
      return field;
    }
  }
}
