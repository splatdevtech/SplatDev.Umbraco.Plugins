using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Dtos;

using System.Diagnostics.CodeAnalysis;

using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Infrastructure.Persistence.Mappers;

namespace FormBuilder.Core.Persistence.Mappers
{
    [MapperFor(typeof(WorkflowDto))]
    [MapperFor(typeof(WorkflowEntity))]
    internal sealed class WorkflowMapPersistenceDefinition(
      Lazy<ISqlContext> sqlContext,
      MapperConfigurationStore maps) : BaseMapper(sqlContext, maps)
    {
        [ExcludeFromCodeCoverage]
        protected override void DefineMaps()
        {
            DefineMap<WorkflowEntity, WorkflowDto>("CreateDate", "CreateDate");
            DefineMap<WorkflowEntity, WorkflowDto>("Id", "Id");
            DefineMap<WorkflowEntity, WorkflowDto>("Key", "Key");
            DefineMap<WorkflowEntity, WorkflowDto>("Name", "Name");
            DefineMap<WorkflowEntity, WorkflowDto>("UpdateDate", "UpdateDate");
        }
    }
}