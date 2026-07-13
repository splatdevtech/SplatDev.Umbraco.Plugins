using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace SplatDev.Umbraco.Workflow.Persistence.Entities;

[TableName("splatWorkflowDefinition")]
[PrimaryKey("id", AutoIncrement = true)]
public class WorkflowDefinitionEntity
{
    [PrimaryKeyColumn(AutoIncrement = true)]
    public int Id { get; set; }

    public string Key { get; set; } = string.Empty;

    public string Label { get; set; } = string.Empty;

    public int Version { get; set; }

    [SpecialDbType(SpecialDbTypes.NVARCHARMAX)]
    public string DefinitionJson { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = string.Empty;
}
