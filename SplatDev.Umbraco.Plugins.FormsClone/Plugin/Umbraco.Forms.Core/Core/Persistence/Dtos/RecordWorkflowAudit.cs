
// Type: Umbraco.Forms.Core.Persistence.Dtos.RecordWorkflowAudit
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using NPoco;

using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;


#nullable enable
namespace Umbraco.Forms.Core.Persistence.Dtos
{
    [TableName("UFRecordWorkflowAudit")]
    [PrimaryKey("Id", AutoIncrement = true)]
    public class RecordWorkflowAudit
    {
        [Column(Name = "Id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        [Index(IndexTypes.NonClustered)]
        [Column(Name = "RecordUniqueId")]
        public Guid RecordUniqueId { get; set; }

        [Index(IndexTypes.NonClustered)]
        [Column(Name = "WorkflowKey")]
        public Guid WorkflowKey { get; set; }

        [Column(Name = "WorkflowName")]
        [Length(255)]
        public string WorkflowName { get; set; } = string.Empty;

        [Column(Name = "WorkflowTypeId")]
        public Guid WorkflowTypeId { get; set; }

        [Column(Name = "WorkflowTypeName")]
        [Length(255)]
        public string WorkflowTypeName { get; set; } = string.Empty;

        [Column(Name = "ExecutedOn")]
        public DateTime ExecutedOn { get; set; }

        [Column(Name = "ExecutionStage")]
        [NullSetting]
        public int? ExecutionStage { get; set; }

        [Column(Name = "ExecutionStatus")]
        public int ExecutionStatus { get; set; }
    }
}
