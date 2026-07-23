using Newtonsoft.Json;

using NPoco;

using System.Text.Json.Serialization;

using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace FormBuilder.Extension.Entities
{
    [TableName(TABLE_NAME)]
    [ExplicitColumns]
    [PrimaryKey("Id", AutoIncrement = true)]
    public class FormField
    {
        public const string TABLE_NAME = "FormBuilderFormFields";

        [Column("id")]
        [JsonPropertyName("id")]
        [JsonProperty("id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        [Column("alias")]
        [JsonPropertyName("alias")]
        [JsonProperty("alias")]
        public string Alias { get; set; } = string.Empty; // Field Alias

        [Column("formId")]
        [JsonPropertyName("formId")]
        [JsonProperty("formId")]
        public int FormId { get; set; } // Foreign key to Form

        [Column("minLength")]
        [JsonPropertyName("minLength")]
        [JsonProperty("minLength")]
        public int MinLength { get; set; } // Field Minimum Length

        [Column("regex")]
        [JsonPropertyName("regex")]
        [JsonProperty("regex")]
        public string? Regex { get; set; } // Field Pattern

        [Column("label")]
        [JsonPropertyName("label")]
        [JsonProperty("label")]
        public string? Label { get; set; } // Field label

        [Column("placeholder")]
        [JsonPropertyName("placeholder")]
        [JsonProperty("placeholder")]
        public string? Placeholder { get; set; } // Placeholder text

        [Column("required")]
        [JsonPropertyName("required")]
        [JsonProperty("required")]
        public bool IsRequired { get; set; } // Required field flag

        [Column("type")]
        [JsonPropertyName("type")]
        [JsonProperty("type")]
        public string? Type { get; set; } // Field type (e.g., TextBox, Dropdown)

        [Column("sortOrder")]
        [JsonPropertyName("sortOrder")]
        [JsonProperty("sortOrder")]
        public int SortOrder { get; set; }

        // For Dropdown/Checkbox/RadioButton lists
        [Ignore]
        [JsonPropertyName("dropdownValues")]
        [JsonProperty("dropdownValues")]
        public ICollection<DropdownValue> DropdownValues { get; set; } = [];

        // Navigation property
        [Ignore]
        [JsonPropertyName("form")]
        [JsonProperty("form")]
        public Form? Form { get; set; }
    }
}
