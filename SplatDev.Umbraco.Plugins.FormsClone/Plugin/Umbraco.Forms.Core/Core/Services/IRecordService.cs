
// Type: Umbraco.Forms.Core.Services.IRecordService
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Collections.Generic;
using System.Threading.Tasks;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;


#nullable enable
namespace Umbraco.Forms.Core.Services
{
  public interface IRecordService
  {
    Task SubmitAsync(Record record, Form form);

    Task ApproveAsync(Record record, Form form);

    Task RejectAsync(Record record, Form form);

    Task DeleteAsync(Record record, Form form);

    IReadOnlyList<Record> GetAllRecords(Form form, bool includeFields = true);

    int GetRecordCount(Form form) => this.GetAllRecords(form, false).Count;
  }
}
