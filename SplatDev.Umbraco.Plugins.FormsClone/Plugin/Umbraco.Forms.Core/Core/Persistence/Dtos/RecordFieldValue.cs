
// Type: Umbraco.Forms.Core.Persistence.Dtos.RecordFieldValue
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using NPoco;
using System;


#nullable enable
namespace Umbraco.Forms.Core.Persistence.Dtos
{
  [TableName("UFRecordFieldValues")]
  [PrimaryKey("Id", AutoIncrement = true)]
  public class RecordFieldValue
  {
    public int Id { get; set; }

    public Guid Key { get; set; }

    public string StringValue { get; set; } = string.Empty;

    public bool BooleanValue { get; set; }

    public DateTime DateTimeValue { get; set; }

    public int IntegerValue { get; set; }
  }
}
