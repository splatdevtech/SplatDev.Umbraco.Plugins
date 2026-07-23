using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Dtos;

using Umbraco.Cms.Infrastructure.Persistence.Dtos;

namespace FormBuilder.Core.Persistence.Interfaces
{
    public interface IFormFactory
    {
        FormDto BuildDto(FormEntity entity);

        NodeDto BuildNodeDto(FormEntity entity);

        IEnumerable<FormEntity> BuildEntities(IEnumerable<FormDto> dtos);

        FormEntity BuildEntity(FormDto dto);

        FormEntitySlim BuildEntitySlim(FormDto dto);
    }
}