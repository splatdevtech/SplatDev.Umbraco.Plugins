using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace SplatDev.Umbraco.Workflow.Persistence.Entities;

[TableName("splatWorkflowInstance")]
[PrimaryKey("id", AutoIncrement = true)]
public class WorkflowInstanceEntity
{
    [PrimaryKeyColumn(AutoIncrement = true)]
    public long Id { get; set; }

    public string WorkflowKey { get; set; } = string.Empty;

    public int WorkflowVersion { get; set; }

    public string CurrentStepKey { get; set; } = string.Empty;

    public byte Status { get; set; } // 0=Open, 1=Completed, 2=Cancelled

    [SpecialDbType(SpecialDbTypes.NVARCHARMAX)]
    [NullSetting(NullSetting = NullSettings.Null)]
    public string? MetadataJson { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = string.Empty;

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = string.Empty;
}
