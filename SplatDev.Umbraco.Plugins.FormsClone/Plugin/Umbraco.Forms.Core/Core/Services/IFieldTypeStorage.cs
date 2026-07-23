
// Type: Umbraco.Forms.Core.Services.IFieldTypeStorage
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using Umbraco.Forms.Core.Models;


#nullable enable
namespace Umbraco.Forms.Core.Services
{
  public interface IFieldTypeStorage
  {
    FieldType? GetFieldTypeByField(Field field);

    FieldType? GetFieldTypeByField(Guid fieldTypeId, IDictionary<string, string> settings);
  }
}
