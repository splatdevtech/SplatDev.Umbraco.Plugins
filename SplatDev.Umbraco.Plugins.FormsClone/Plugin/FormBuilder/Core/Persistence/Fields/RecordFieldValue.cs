using NPoco;

namespace FormBuilder.Core.Persistence.Fields
{
    [TableName("FormBuilderRecordFieldValues")]
    [PrimaryKey("Id", AutoIncrement = true)]
    public class RecordFieldValue
    {
        public int Id { get; set; }

        public Guid Key { get; set; }

        public string StringValue { get; set; } = string.Empty;

        public bool BooleanValue { get; set; }

        public DateTime DateTimeValue { get; set; }

        public int IntegerValue { get; set; }
    }
}