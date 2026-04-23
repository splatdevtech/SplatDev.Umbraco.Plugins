
// Type: Umbraco.Forms.Core.Interfaces.ITypeWithEditorDetails
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;


#nullable enable
namespace Umbraco.Forms.Core.Interfaces
{
  public interface ITypeWithEditorDetails : IType
  {
    int? CreatedBy { get; set; }

    string? CreatedByName { get; set; }

    DateTime Updated { get; set; }

    int? UpdatedBy { get; set; }

    string? UpdatedByName { get; set; }
  }
}
