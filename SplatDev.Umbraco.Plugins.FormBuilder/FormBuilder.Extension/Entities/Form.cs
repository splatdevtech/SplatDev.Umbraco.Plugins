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
    public class Form
    {
        public const string TABLE_NAME = "FormBuilderForms";

        [Column("id")]
        [JsonPropertyName("id")]
        [JsonProperty("id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        [Column("name")]
        [JsonPropertyName("name")]
        [JsonProperty("name")]
        [Required] public string Name { get; set; } = string.Empty;

        [Column("category")]
        [JsonPropertyName("category")]
        [JsonProperty("category")]
        public string Category { get; set; } = string.Empty;

        [Column("createdDate")]
        [JsonPropertyName("createdDate")]
        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        [ResultColumn]
        [Ignore]
        public List<FormField> Fields { get; set; } = [];

        [ResultColumn]
        [Ignore]
        public ICollection<Workflow> Workflows { get; set; } = [];
    }
}
