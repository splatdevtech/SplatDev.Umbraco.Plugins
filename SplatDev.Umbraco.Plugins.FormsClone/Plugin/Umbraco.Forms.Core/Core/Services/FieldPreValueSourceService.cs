
// Type: Umbraco.Forms.Core.Services.FieldPreValueSourceService
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;


#nullable enable
namespace Umbraco.Forms.Core.Services
{
  internal sealed class FieldPreValueSourceService : IFieldPreValueSourceService
  {
    public static readonly Guid DefaultFieldPreValueSourceId = new Guid("9FFD217D-C8DF-4a59-9C9A-690A6F389DC4");
    private readonly IPrevalueSourceService _prevalueSourceService;

    public FieldPreValueSourceService(IPrevalueSourceService prevalueSourceService) => this._prevalueSourceService = prevalueSourceService;

    public FieldPreValueSource GetDefaultProvider() => new FieldPreValueSource()
    {
      Name = "Default Provider",
      Id = FieldPreValueSourceService.DefaultFieldPreValueSourceId,
      Settings = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    };

    public FieldPreValueSource? GetById(Guid fieldPreValueSourceId) => fieldPreValueSourceId != FieldPreValueSourceService.DefaultFieldPreValueSourceId && fieldPreValueSourceId != Guid.Empty ? this._prevalueSourceService.Get(fieldPreValueSourceId) : (FieldPreValueSource) null;
  }
}
