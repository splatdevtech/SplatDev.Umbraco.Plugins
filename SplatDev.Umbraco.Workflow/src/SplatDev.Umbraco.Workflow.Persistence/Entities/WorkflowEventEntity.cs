using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace SplatDev.Umbraco.Workflow.Persistence.Entities;

[TableName("splatWorkflowEvent")]
[PrimaryKey("id", AutoIncrement = true)]
public class WorkflowEventEntity
{
    [PrimaryKeyColumn(AutoIncrement = true)]
    public long Id { get; set; }

    public long InstanceId { get; set; }

    public byte EventType { get; set; } // 0=Created, 1=Transition, 2=Comment, 3=Assignment, 4=ActionMessage

    [NullSetting(NullSetting = NullSettings.Null)]
    public string? FromStepKey { get; set; }

    [NullSetting(NullSetting = NullSettings.Null)]
    public string? ToStepKey { get; set; }

    [NullSetting(NullSetting = NullSettings.Null)]
    public string? ActionKey { get; set; }

    [SpecialDbType(SpecialDbTypes.NVARCHARMAX)]
    [NullSetting(NullSetting = NullSettings.Null)]
    public string? PayloadJson { get; set; }

    public string ActorUsername { get; set; } = string.Empty;

    public DateTimeOffset OccurredAt { get; set; }
}
