
// Type: Umbraco.Forms.Core.Services.IBaseService`2
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Models.Entities;
using Umbraco.Forms.Core.Interfaces;


#nullable enable
namespace Umbraco.Forms.Core.Services
{
  public interface IBaseService<TType, TEntity>
    where TType : IType
    where TEntity : IEntity
  {
    void Delete(Guid id);

    bool Delete(TType item);

    IEnumerable<TType> Get();

    TType? Get(Guid id);

    bool Exists(Guid id);

    IEnumerable<TType> Get(Guid[] ids);

    TType? Insert(TType item);

    IEnumerable<TType> Update(IEnumerable<TType> items);

    TType Update(TType item);

    TType Update(TType item, Dictionary<string, object?> additionalData);
  }
}
