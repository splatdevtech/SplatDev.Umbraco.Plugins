using System.Runtime.Serialization;

namespace FormBuilder.Core.Models
{
    /// <summary>Defines a datasource type representation.</summary>
    [DataContract(Name = "dataSourceType")]
    [Serializable]
    public class DataSourceTypeWithSettings : IProviderTypeWithSettings
    {
        /// <summary>Gets or sets the datasource's Id.</summary>
        [DataMember(Name = "id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets the unique ID for the data source type (with the field name required for front-end rendering).
        /// </summary>
        [DataMember(Name = "unique")]
        public Guid Unique => Id;

        /// <summary>
        /// Gets the data source  type entity type (required for front-end rendering).
        /// </summary>
        [DataMember(Name = "entityType")]
        public static string EntityType => "datasource-type";

        /// <summary>Gets or sets the data source type's alias.</summary>
        [DataMember(Name = "alias")]
        public string Alias { get; set; } = string.Empty;

        /// <summary>Gets or sets the datasource's name.</summary>
        [DataMember(Name = "name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the datasource's description.</summary>
        [DataMember(Name = "description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>Gets or sets the prevalue source type's icon.</summary>
        [DataMember(Name = "icon")]
        public string Icon { get; set; } = string.Empty;

        /// <summary>Gets or sets the datasource's settings.</summary>
        [DataMember(Name = "settings")]
        public IEnumerable<Setting> Settings { get; set; } = [];
    }
}