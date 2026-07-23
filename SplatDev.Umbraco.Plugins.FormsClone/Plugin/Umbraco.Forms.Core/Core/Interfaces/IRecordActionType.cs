
// Type: Umbraco.Forms.Core.Interfaces.IRecordActionType
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;


#nullable enable
namespace Umbraco.Forms.Core.Interfaces
{
  public interface IRecordActionType
  {
    string Icon { get; set; }

    string JsAction { get; set; }

    RecordActionStatus Execute(Record record, Form form);
  }
}
