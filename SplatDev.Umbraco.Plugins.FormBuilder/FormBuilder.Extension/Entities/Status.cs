using Newtonsoft.Json;

using NPoco;

using System.Text.Json.Serialization;

using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace FormBuilder.Extension.Entities
{
    [TableName(TABLE_NAME)]
    [ExplicitColumns]
    [PrimaryKey("Id", AutoIncrement = true)]
    public class Status
    {
        public const string TABLE_NAME = "FormBuilderStatus";

        [Column("id")]
        [JsonPropertyName("id")]
        [JsonProperty("id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        [Column("name")]
        [JsonPropertyName("name")]
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty; // Status name (e.g., "Read Enquiry")

        [Column("active")]
        [JsonPropertyName("active")]
        [JsonProperty("active")]
        public bool IsActive { get; set; } = true;
    }
}
