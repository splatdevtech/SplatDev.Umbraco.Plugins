using NPoco;

using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

using Umbraco.Cms.Infrastructure.Persistence.Dtos;

namespace FormBuilder.Core.Persistence.Dtos
{
    [TableName("FormBuilderForms")]
    public class FormDto : BaseEntityDto
    {
        [Column(Name = "FolderKey")]
        [NullSetting]
        [ForeignKey(typeof(FolderDto), Column = "Key")]
        [Index(IndexTypes.NonClustered)]
        public Guid? FolderKey { get; set; }

        [Column(Name = "NodeId")]
        [NullSetting]
        [ForeignKey(typeof(NodeDto), Column = "id")]
        [Index(IndexTypes.NonClustered)]
        public int NodeId { get; set; }
    }
}