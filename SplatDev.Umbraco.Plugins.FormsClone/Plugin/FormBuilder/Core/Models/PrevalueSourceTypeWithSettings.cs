using System.Runtime.Serialization;

namespace FormBuilder.Core.Models
{
    /// <summary>Defines a prevalue source type representation.</summary>
    [DataContract(Name = "preValueSourceType")]
    [Serializable]
    public class PrevalueSourceTypeWithSettings : IProviderTypeWithSettings
    {
        /// <summary>Gets or sets the prevalue source's Id.</summary>
        [DataMember(Name = "id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets the unique ID for the prevalue source type (with the field name required for front-end rendering).
        /// </summary>
        [DataMember(Name = "unique")]
        public Guid Unique => Id;

        /// <summary>
        /// Gets the prevalue source type entity type (required for front-end rendering).
        /// </summary>
        [DataMember(Name = "entityType")]
        public static string EntityType => "prevaluesource-type";

        /// <summary>Gets or sets the prevalue source type's alias.</summary>
        [DataMember(Name = "alias")]
        public string Alias { get; set; } = string.Empty;

        /// <summary>Gets or sets the prevalue source types's name.</summary>
        [DataMember(Name = "name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the prevalue source types's description.</summary>
        [DataMember(Name = "description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>Gets or sets the prevalue source type's icon.</summary>
        [DataMember(Name = "icon")]
        public string Icon { get; set; } = string.Empty;

        /// <summary>Gets or sets the prevalue source type's settings.</summary>
        [DataMember(Name = "settings")]
        public IEnumerable<Setting> Settings { get; set; } = [];
    }
}