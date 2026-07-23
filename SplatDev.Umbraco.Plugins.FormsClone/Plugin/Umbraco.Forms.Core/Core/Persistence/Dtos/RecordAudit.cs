
// Type: Umbraco.Forms.Core.Persistence.Dtos.RecordAudit
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using NPoco;

using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Umbraco.Forms.Core.Persistence.Dtos
{
    [TableName("UFRecordAudit")]
    [PrimaryKey("Id", AutoIncrement = true)]
    public class RecordAudit
    {
        [Column(Name = "Id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        [Index(IndexTypes.NonClustered)]
        [ForeignKey(typeof(Record))]
        [Column(Name = "Record")]
        public int Record { get; set; }

        [Column(Name = "UpdatedOn")]
        public DateTime UpdatedOn { get; set; }

        [Column(Name = "UpdatedBy")]
        [Index(IndexTypes.NonClustered)]
        [NullSetting]
        public int? UpdatedBy { get; set; }
    }
}
