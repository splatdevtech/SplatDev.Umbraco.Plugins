using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Dtos;

namespace FormBuilder.Core.Persistence.Interfaces
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