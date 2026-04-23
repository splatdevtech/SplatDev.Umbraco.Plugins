using FormBuilder.Core.Helpers;
using FormBuilder.Core.Models;

using NPoco;

using System.Globalization;

using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace FormBuilder.Core.Persistence.Fields
{
    [TableName("FormBuilderRecordFields")]
    [PrimaryKey("Key", AutoIncrement = false)]
    [ExplicitColumns]
    public class RecordField
    {
        public RecordField()
        {
            Key = Guid.NewGuid();
            Values = [];
        }

        public RecordField(Field field)
        {
            Key = Guid.NewGuid();
            Values = [];
            Field = field;
            FieldId = field.Id;
            Alias = field.Alias;
        }

        [PrimaryKeyColumn(AutoIncrement = false)]
        [Column(Name = "Key")]
        public Guid Key { get; set; }

        [Column(Name = "FieldId")]
        public Guid FieldId { get; set; }

        [Index(IndexTypes.NonClustered)]
        [Column(Name = "Record")]
        [ForeignKey(typeof(Record))]
        public int Record { get; set; }

        [Column(Name = "Alias")]
        public string Alias { get; set; } = string.Empty;

        [Column("DataType")]
        public string DataTypeAlias { get; set; } = string.Empty;

        [Ignore]
        public List<object> Values { get; set; } = [];

        [Ignore]
        public Field? Field { get; set; }

        public bool HasValue() => Values.Count != 0;

        public virtual string ValuesAsString(bool escaped = true)
        {
            if (!HasValue())
                return string.Empty;

            // Handle unescaped values
            if (!escaped)
            {
                return string.Join(", ",
                    Values
                        .Select(value => GetValue(value))
                        .ToArray());
            }

            // Handle escaped values
            return string.Join(", ",
                Values
                    .Select(value => JsonHelper.EscapeStringValue(GetValue(value)))
                    .ToArray());
        }

        private static string? GetValue(object input) => input is DateTime dateTime ? dateTime.ToString(CultureInfo.InvariantCulture) : input.ToString();
    }
}