
// Type: Umbraco.Forms.Core.Persistence.Dtos.RecordFieldDataLongString
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using NPoco;

using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;


#nullable enable
namespace Umbraco.Forms.Core.Persistence.Dtos
{
    [TableName("UFRecordDataLongString")]
    [PrimaryKey("Id", AutoIncrement = true)]
    public class RecordFieldDataLongString : IRecordFieldData
    {
        [PrimaryKeyColumn(AutoIncrement = true, IdentitySeed = 0)]
        public int Id { get; set; }

        [ForeignKey(typeof(RecordField))]
        [Index(IndexTypes.NonClustered)]
        public Guid Key { get; set; }

        [SpecialDbType(SpecialDbTypes.NVARCHARMAX)]
        [NullSetting]
        public string Value { get; set; } = string.Empty;
    }
}
