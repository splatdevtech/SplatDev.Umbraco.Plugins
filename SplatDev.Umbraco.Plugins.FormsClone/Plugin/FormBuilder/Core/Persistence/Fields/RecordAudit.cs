using NPoco;

using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace FormBuilder.Core.Persistence.Fields
{
    [TableName("FormBuilderRecordAudit")]
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