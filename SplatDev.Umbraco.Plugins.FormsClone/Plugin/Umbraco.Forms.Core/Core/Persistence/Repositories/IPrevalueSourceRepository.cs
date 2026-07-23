
// Type: Umbraco.Forms.Core.Persistence.Repositories.IPrevalueSourceRepository
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Persistence;
using Umbraco.Forms.Core.Models;


#nullable enable
namespace Umbraco.Forms.Core.Persistence.Repositories
{
  public interface IPrevalueSourceRepository : 
    IReadWriteQueryRepository<Guid, PrevalueSourceEntity>,
    IReadRepository<Guid, PrevalueSourceEntity>,
    IRepository,
    IWriteRepository<PrevalueSourceEntity>,
    IQueryRepository<PrevalueSourceEntity>
  {
    IEnumerable<PrevalueSourceEntitySlim> GetManySlim(
      params Guid[]? ids);
  }
}
