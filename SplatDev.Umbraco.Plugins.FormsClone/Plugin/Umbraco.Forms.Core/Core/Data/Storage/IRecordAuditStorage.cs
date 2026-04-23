
// Type: Umbraco.Forms.Core.Data.Storage.IRecordAuditStorage
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Collections.Generic;
using Umbraco.Forms.Core.Persistence.Dtos;


#nullable enable
namespace Umbraco.Forms.Core.Data.Storage
{
  public interface IRecordAuditStorage
  {
    List<RecordAudit> GetRecordAuditTrail(int recordId);
  }
}
