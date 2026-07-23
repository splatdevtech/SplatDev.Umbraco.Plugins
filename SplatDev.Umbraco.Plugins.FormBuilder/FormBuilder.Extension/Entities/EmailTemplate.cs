using Newtonsoft.Json;

using NPoco;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace FormBuilder.Extension.Entities
{
    [TableName(TABLE_NAME)]
    [ExplicitColumns]
    [PrimaryKey("Id", AutoIncrement = true)]
    public class EmailTemplate
    {
        public const string TABLE_NAME = "FormBuilderEmailTemplates";

        [Column("id")]
        [JsonPropertyName("id")]
        [JsonProperty("id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        [Column("name")]
        [JsonPropertyName("name")]
        [JsonProperty("name")]
        [Required]
        public string Name { get; set; } = string.Empty;

        [Column("subject")]
        [JsonPropertyName("subject")]
        [JsonProperty("subject")]
        [Required]
        public string Subject { get; set; } = string.Empty;

        [Column("body")]
        [JsonPropertyName("body")]
        [JsonProperty("body")]
        [Required]
        public string Body { get; set; } = string.Empty;

        [Column("recipients")]
        [JsonPropertyName("recipients")]
        [JsonProperty("recipients")]
        [Required]
        public string Recipients { get; set; } = string.Empty;

        [Column("active")]
        [JsonPropertyName("active")]
        [JsonProperty("active")]
        public bool IsActive { get; set; } = true;

        [Column("createdDate")]
        [JsonPropertyName("createdDate")]
        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
