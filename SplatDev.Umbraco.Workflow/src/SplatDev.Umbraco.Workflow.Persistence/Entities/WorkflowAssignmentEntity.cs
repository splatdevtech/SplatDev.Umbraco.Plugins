using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace SplatDev.Umbraco.Workflow.Persistence.Entities;

[TableName("splatWorkflowAssignment")]
[PrimaryKey("id", AutoIncrement = true)]
public class WorkflowAssignmentEntity
{
    [PrimaryKeyColumn(AutoIncrement = true)]
    public long Id { get; set; }

    public long InstanceId { get; set; }

    [NullSetting(NullSetting = NullSettings.Null)]
    public string? AssignedTo { get; set; }

    [NullSetting(NullSetting = NullSettings.Null)]
    public string? AssignedToGroup { get; set; }

    [NullSetting(NullSetting = NullSettings.Null)]
    public string? Department { get; set; }

    public DateTime AssignedAt { get; set; }

    public bool IsActive { get; set; }
}
