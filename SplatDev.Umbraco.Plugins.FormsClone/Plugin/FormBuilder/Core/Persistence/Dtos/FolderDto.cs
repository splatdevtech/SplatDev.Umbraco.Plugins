using NPoco;

using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace FormBuilder.Core.Persistence.Dtos
{
    [TableName("FormBuilderFolders")]
    public class FolderDto : BaseDto
    {
        [Column(Name = "ParentKey")]
        [NullSetting]
        [ForeignKey(typeof(FolderDto), Column = "Key")]
        public Guid? ParentKey { get; set; }
    }
}