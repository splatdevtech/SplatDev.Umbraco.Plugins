
// Type: Umbraco.Forms.Core.Persistence.Dtos.RecordField
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using NPoco;

using System.Globalization;

using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;
using Umbraco.Forms.Core.Data.Helpers;
using Umbraco.Forms.Core.Models;


#nullable enable
namespace Umbraco.Forms.Core.Persistence.Dtos
{
    [TableName("UFRecordFields")]
    [PrimaryKey("Key", AutoIncrement = false)]
    [ExplicitColumns]
    public class RecordField
    {
        public RecordField()
        {
            this.Key = Guid.NewGuid();
            this.Values = new List<object>();
        }

        public RecordField(Field field)
        {
            this.Key = Guid.NewGuid();
            this.Values = new List<object>();
            this.Field = field;
            this.FieldId = field.Id;
            this.Alias = field.Alias;
        }

        [PrimaryKeyColumn(AutoIncrement = false)]
        [Column(Name = "Key")]
        public Guid Key { get; set; }

        [Column(Name = "FieldId")]
        public Guid FieldId { get; set; }

        [Index(IndexTypes.NonClustered)]
        [Column(Name = "Record")]
        [ForeignKey(typeof(Umbraco.Forms.Core.Persistence.Dtos.Record))]
        public int Record { get; set; }

        [Column(Name = "Alias")]
        public string Alias { get; set; } = string.Empty;

        [Column("DataType")]
        public string DataTypeAlias { get; set; } = string.Empty;

        [Ignore]
        public List<object> Values { get; set; } = new List<object>();

        [Ignore]
        public Field? Field { get; set; }

        public bool HasValue() => this.Values.Any<object>();

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
