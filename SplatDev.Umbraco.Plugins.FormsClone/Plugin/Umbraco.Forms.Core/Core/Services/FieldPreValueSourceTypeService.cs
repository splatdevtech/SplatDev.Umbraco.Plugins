
// Type: Umbraco.Forms.Core.Services.FieldPreValueSourceTypeService
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using Umbraco.Forms.Core.Providers;


#nullable enable
namespace Umbraco.Forms.Core.Services
{
  internal sealed class FieldPreValueSourceTypeService : IFieldPreValueSourceTypeService
  {
    private readonly FieldPreValueSourceCollectionFactory _fieldPreValueSourceCollectionFactory;

    public FieldPreValueSourceTypeService(
      FieldPreValueSourceCollectionFactory fieldPreValueSourceCollectionFactory)
    {
      this._fieldPreValueSourceCollectionFactory = fieldPreValueSourceCollectionFactory;
    }

    public FieldPreValueSourceType? GetById(Guid fieldPreValueSourceTypeId) => fieldPreValueSourceTypeId == Guid.Empty ? (FieldPreValueSourceType) null : this._fieldPreValueSourceCollectionFactory.GetPreValueSourceCollection()[fieldPreValueSourceTypeId];
  }
}
