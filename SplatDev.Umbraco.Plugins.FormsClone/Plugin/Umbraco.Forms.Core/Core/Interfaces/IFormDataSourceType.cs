
// Type: Umbraco.Forms.Core.Interfaces.IFormDataSourceType
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;


#nullable enable
namespace Umbraco.Forms.Core.Interfaces
{
  public interface IFormDataSourceType
  {
    Guid Id { get; set; }

    string Alias { get; set; }

    bool SupportsGetRecords { get; set; }

    bool SupportsInsert { get; set; }

    bool SupportsPreValues { get; set; }

    FormDataSource? DataSource { get; }

    Dictionary<object, FormDataSourceField> GetAvailableFields();

    Dictionary<object, string> GetPreValues(Field field, Form form);

    List<Record> GetRecords(
      Form form,
      int page,
      int maxItems,
      object sortByField,
      RecordSorting order);

    Record InsertRecord(Record record);

    Dictionary<string, SettingAttribute> Settings();

    List<Exception> ValidateSettings();
  }
}
