
// Type: Umbraco.Forms.Core.Services.IFieldPreValueSourceTypeService
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;


#nullable enable
namespace Umbraco.Forms.Core.Services
{
  public interface IFieldPreValueSourceTypeService
  {
    FieldPreValueSourceType? GetById(Guid fieldPreValueSourceTypeId);
  }
}
