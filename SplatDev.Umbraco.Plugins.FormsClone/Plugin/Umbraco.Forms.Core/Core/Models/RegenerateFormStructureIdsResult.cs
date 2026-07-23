
// Type: Umbraco.Forms.Core.Models.RegenerateFormStructureIdsResult
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;


#nullable enable
namespace Umbraco.Forms.Core.Models
{
  public class RegenerateFormStructureIdsResult
  {
    public RegenerateFormStructureIdsResult(Dictionary<Guid, Guid> fieldIdMapping) => this.FieldIdMapping = fieldIdMapping;

    public Dictionary<Guid, Guid> FieldIdMapping { get; }
  }
}
