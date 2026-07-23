using NPoco;

using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace FormBuilder.Core.Persistence.Dtos
{
    [TableName("FormBuilderWorkflows")]
    public class WorkflowDto : BaseDto
    {
        [Column("FormId")]
        [Index(IndexTypes.NonClustered)]
        public Guid FormId { get; set; }
    }
}