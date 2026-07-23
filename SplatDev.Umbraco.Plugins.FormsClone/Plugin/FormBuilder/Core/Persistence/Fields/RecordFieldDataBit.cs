using FormBuilder.Core.Persistence.Interfaces;

using NPoco;

using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace FormBuilder.Core.Persistence.Fields
{
    [TableName("FormBuilderRecordDataBit")]
    [PrimaryKey("Id", AutoIncrement = true)]
    public class RecordFieldDataBit : IRecordFieldData
    {
        [PrimaryKeyColumn(AutoIncrement = true, IdentitySeed = 0)]
        public int Id { get; set; }

        [ForeignKey(typeof(RecordField))]
        [Index(IndexTypes.NonClustered)]
        public Guid Key { get; set; }

        [NullSetting]
        public bool Value { get; set; }
    }
}