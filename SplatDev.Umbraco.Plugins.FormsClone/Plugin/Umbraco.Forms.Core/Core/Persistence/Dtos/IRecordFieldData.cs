
// Type: Umbraco.Forms.Core.Persistence.Dtos.IRecordFieldData
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;

namespace Umbraco.Forms.Core.Persistence.Dtos
{
  public interface IRecordFieldData
  {
    int Id { get; set; }

    Guid Key { get; set; }
  }
}
