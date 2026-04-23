
// Type: Umbraco.Forms.Core.Persistence.Factories.IDataSourceFactory
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Collections.Generic;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;


#nullable enable
namespace Umbraco.Forms.Core.Persistence.Factories
{
  public interface IDataSourceFactory
  {
    DataSourceDto BuildDto(DataSourceEntity entity);

    IEnumerable<DataSourceEntity> BuildEntities(
      IEnumerable<DataSourceDto> dtos);

    DataSourceEntity BuildEntity(DataSourceDto dto);

    DataSourceEntitySlim BuildEntitySlim(DataSourceDto dto);
  }
}
