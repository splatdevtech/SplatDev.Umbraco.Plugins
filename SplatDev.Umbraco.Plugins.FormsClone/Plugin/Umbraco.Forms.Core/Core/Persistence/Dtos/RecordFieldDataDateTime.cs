
// Type: Umbraco.Forms.Core.Persistence.Dtos.RecordFieldDataDateTime
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using NPoco;

using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Umbraco.Forms.Core.Persistence.Dtos
{
    [TableName("UFRecordDataDateTime")]
    [PrimaryKey("Id", AutoIncrement = true)]
    public class RecordFieldDataDateTime : IRecordFieldData
    {
        [PrimaryKeyColumn(AutoIncrement = true, IdentitySeed = 0)]
        public int Id { get; set; }

        [ForeignKey(typeof(RecordField))]
        [Index(IndexTypes.NonClustered)]
        public Guid Key { get; set; }

        [NullSetting]
        public DateTime Value { get; set; }
    }
}
