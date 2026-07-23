
// Type: Umbraco.Forms.Core.Persistence.Dtos.WorkflowDto
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using NPoco;

using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Umbraco.Forms.Core.Persistence.Dtos
{
    [TableName("UFWorkflows")]
    public class WorkflowDto : BaseDto
    {
        [Column("FormId")]
        [Index(IndexTypes.NonClustered)]
        public Guid FormId { get; set; }
    }
}
