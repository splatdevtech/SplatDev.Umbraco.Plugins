using Newtonsoft.Json;

using NPoco;

using System.Text.Json.Serialization;

using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace FormBuilder.Extension.Entities
{
    [TableName(TABLE_NAME)]
    [ExplicitColumns]
    [PrimaryKey("Id", AutoIncrement = true)]
    public class DropdownValue
    {
        public const string TABLE_NAME = "FormBuilderDropdownValues";

        [Column("id")]
        [JsonPropertyName("id")]
        [JsonProperty("id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        [Column("fieldId")]
        [JsonPropertyName("fieldId")]
        [JsonProperty("fieldId")]
        public int FieldId { get; set; } // Foreign key to FormField

        [Column("value")]
        [JsonPropertyName("value")]
        [JsonProperty("value")]
        public string Value { get; set; } = string.Empty;

        // Navigation property
        [Ignore]
        [JsonPropertyName("formField")]
        [JsonProperty("formField")]
        public FormField FormField { get; set; } = new();
    }
}
