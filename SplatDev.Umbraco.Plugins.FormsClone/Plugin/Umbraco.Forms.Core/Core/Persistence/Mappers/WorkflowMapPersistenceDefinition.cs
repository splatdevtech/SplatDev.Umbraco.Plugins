
// Type: Umbraco.Forms.Core.Persistence.Mappers.WorkflowMapPersistenceDefinition
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Diagnostics.CodeAnalysis;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Infrastructure.Persistence.Mappers;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;


#nullable enable
namespace Umbraco.Forms.Core.Persistence.Mappers
{
  [MapperFor(typeof (WorkflowDto))]
  [MapperFor(typeof (WorkflowEntity))]
  internal sealed class WorkflowMapPersistenceDefinition : BaseMapper
  {
    public WorkflowMapPersistenceDefinition(
      Lazy<ISqlContext> sqlContext,
      MapperConfigurationStore maps)
      : base(sqlContext, maps)
    {
    }

    [ExcludeFromCodeCoverage]
    protected override void DefineMaps()
    {
      this.DefineMap<WorkflowEntity, WorkflowDto>("CreateDate", "CreateDate");
      this.DefineMap<WorkflowEntity, WorkflowDto>("Id", "Id");
      this.DefineMap<WorkflowEntity, WorkflowDto>("Key", "Key");
      this.DefineMap<WorkflowEntity, WorkflowDto>("Name", "Name");
      this.DefineMap<WorkflowEntity, WorkflowDto>("UpdateDate", "UpdateDate");
    }
  }
}
