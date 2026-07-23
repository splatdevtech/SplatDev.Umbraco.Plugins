
// Type: Umbraco.Forms.Core.Persistence.Factories.IPrevalueSourceFactory
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Collections.Generic;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;


#nullable enable
namespace Umbraco.Forms.Core.Persistence.Factories
{
  public interface IPrevalueSourceFactory
  {
    PrevalueSourceDto BuildDto(PrevalueSourceEntity entity);

    IEnumerable<PrevalueSourceEntity> BuildEntities(
      IEnumerable<PrevalueSourceDto> dtos);

    PrevalueSourceEntity BuildEntity(PrevalueSourceDto dto);

    PrevalueSourceEntitySlim BuildEntitySlim(PrevalueSourceDto dto);
  }
}
