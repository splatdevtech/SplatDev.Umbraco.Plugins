
// Type: Umbraco.Forms.Core.Models.Mapping.WorkflowMapDefinition
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Umbraco.Cms.Core.Mapping;


#nullable enable
namespace Umbraco.Forms.Core.Models.Mapping
{
    internal sealed class WorkflowMapDefinition : IMapDefinition
    {
        public void DefineMaps(IUmbracoMapper mapper)
        {
            mapper.Define<Workflow, WorkflowEntity>((source, context) => new WorkflowEntity(), Map);
            mapper.Define<WorkflowEntity, Workflow>((source, context) => new Workflow(), Map);
            mapper.Define<WorkflowSlim, WorkflowEntitySlim>((source, context) => new WorkflowEntitySlim(), Map);
            mapper.Define<WorkflowEntitySlim, WorkflowSlim>((source, context) => new WorkflowSlim(), Map);
        }

        private static void Map(Workflow source, WorkflowEntity target, MapperContext context)
        {
            target.Name = source.Name;
            target.CreateDate = source.Created;
            target.FormId = source.Form;
            target.Active = source.Active;
            target.IncludeSensitiveData = source.IncludeSensitiveData;
            target.WorkflowTypeId = source.WorkflowTypeId;
            target.ExecutesOn = source.ExecutesOn;
            target.Settings = source.Settings;
            target.SortOrder = source.SortOrder;
            target.Key = source.Id;
            target.IsMandatory = source.IsMandatory;
            target.Condition = source.Condition;
        }

        private static void Map(WorkflowEntity source, Workflow target, MapperContext context)
        {
            target.Name = source.Name;
            target.Created = source.CreateDate;
            target.Form = source.FormId;
            target.Active = source.Active;
            target.IncludeSensitiveData = source.IncludeSensitiveData;
            target.WorkflowTypeId = source.WorkflowTypeId;
            target.ExecutesOn = source.ExecutesOn;
            target.Settings = source.Settings;
            target.SortOrder = source.SortOrder;
            target.Id = source.Key;
            target.Deleted = source.DeleteDate.HasValue;
            target.IsMandatory = source.IsMandatory;
            target.Condition = source.Condition;
        }

        private static void Map(WorkflowSlim source, WorkflowEntitySlim target, MapperContext context)
        {
            target.Key = source.Id;
            target.Name = source.Name;
            target.CreateDate = source.Created;
            target.FormId = source.Form;
        }

        private static void Map(WorkflowEntitySlim source, WorkflowSlim target, MapperContext context)
        {
            target.Id = source.Key;
            target.Name = source.Name;
            target.Created = source.CreateDate;
            target.Form = source.FormId;
        }
    }
}
