
// Type: Umbraco.Forms.Core.Persistence.Dtos.FormDto
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using NPoco;

using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;
using Umbraco.Cms.Infrastructure.Persistence.Dtos;

namespace Umbraco.Forms.Core.Persistence.Dtos
{
    [TableName("UFForms")]
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
